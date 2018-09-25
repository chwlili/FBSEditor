using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using FlatBufferData.Editor;
using FlatBufferData.Model;
using System.Collections.Generic;
using System.IO;
using static FlatbufferParser;

namespace FlatBufferData.Build
{
    public class FBSBuilder
    {
        public delegate void ErrorReport(string projectName,string path,string text,int line,int column);


        public static FBSFile[] Build(string projectName, string[] paths, ErrorReport report)
        {
            var files = new List<FBSFile>();

            foreach (var path in paths)
            {
                if (path.ToLower().EndsWith(Constants.ExtName))
                {
                    var builder = new FBSFileBuild(path, report);
                    files.Add(builder.Build());
                }
            }

            return files.ToArray();
        }

        private class FBSFileBuild
        {
            private string path;
            public ErrorReport report;
            private Dictionary<int, string> commentTable;
            private Dictionary<string, int> typeName2start = new Dictionary<string, int>();

            private FBSFile file;

            public FBSFileBuild(string path, ErrorReport report)
            {
                this.path = path;
                this.report = report;
            }

            public FBSFile Build()
            {
                var text = File.ReadAllText(path);

                var lexer = new FlatbufferLexer(new AntlrInputStream(text));
                var parser = new FlatbufferParser(new CommonTokenStream(lexer));

                InitComments(lexer);

                file = new FBSFile();

                var schema = parser.schema();
                var namespaces = schema.@namespace();
                var tables = schema.table();
                var structs = schema.@struct();
                var enums = schema.@enum();
                var unions = schema.union();
                var rpcs = schema.rpc();
                var rootTypes = schema.rootType();

                typeName2start.Clear();
                foreach (var item in tables) { if (item.name != null && !string.IsNullOrEmpty(item.name.Text) && !typeName2start.ContainsKey(item.name.Text)) { typeName2start[item.name.Text] = item.name.StartIndex; } }
                foreach (var item in structs) { if (item.name != null && !string.IsNullOrEmpty(item.name.Text) && !typeName2start.ContainsKey(item.name.Text)) { typeName2start[item.name.Text] = item.name.StartIndex; } }
                foreach (var item in enums) { if (item.name != null && !string.IsNullOrEmpty(item.name.Text) && !typeName2start.ContainsKey(item.name.Text)) { typeName2start[item.name.Text] = item.name.StartIndex; } }
                foreach (var item in unions) { if (item.name != null && !string.IsNullOrEmpty(item.name.Text) && !typeName2start.ContainsKey(item.name.Text)) { typeName2start[item.name.Text] = item.name.StartIndex; } }
                foreach (var item in rpcs) { if (item.name != null && !string.IsNullOrEmpty(item.name.Text) && !typeName2start.ContainsKey(item.name.Text)) { typeName2start[item.name.Text] = item.name.StartIndex; } }

                HandleNamespace(namespaces);
                HandleTable(tables);
                HandleStruct(structs);
                HandleEnum(enums);
                HandleUnion(unions);
                HandleRpc(rpcs);
                HandleRootType(rootTypes);
                HandleFileExtension(schema.fileExtension());
                HandleFileIdentifier(schema.fileIdentifier());

                return file;
            }

            private void ReportError(string text, int line, int column)
            {
                report?.Invoke("", path, text, line, column);
            }

            private void HandleNamespace(NamespaceContext[] contexts)
            {
                for (var i = 0; i < contexts.Length; i++)
                {
                    var context = contexts[i];
                    if (i == 0)
                    {
                        var idents = context.IDENT();
                        var tokens = new string[idents.Length];
                        for (int j = 0; j < tokens.Length; j++) { tokens[j] = idents[j].GetText(); }
                        file.NameSpace = string.Join(".", tokens);
                    }
                    else
                    {
                        ReportError("namespace 重复声明!", context.Start.Line, context.Start.Column);
                    }
                }
            }

            private void HandleTable(TableContext[] contexts)
            {
                foreach (var context in contexts)
                {
                    var fieldList = new List<TableField>();
                    var fieldNameList = new List<string>();

                    var fieldsContext = context.tableField();
                    if (fieldsContext != null)
                    {
                        var nameTable = new HashSet<string>();
                        foreach (var fieldContext in fieldsContext)
                        {
                            var fieldComm = GetComment(fieldContext, fieldContext.fieldName);
                            var fieldName = fieldContext.fieldName != null ? fieldContext.fieldName.Text : "";
                            var fieldType = fieldContext.fieldType != null ? fieldContext.fieldType.GetText() : "";
                            var arrayType = fieldContext.arrayType != null && fieldContext.arrayType.type != null ? fieldContext.arrayType.type.GetText() : "";
                            var fieldValue = fieldContext.fieldValue != null ? fieldContext.fieldValue.GetText() : "";
                            var fieldLink = fieldContext.fieldMap != null ? fieldContext.fieldMap.Text : "";
                            var fieldMetas = GetMetaDatas(fieldContext.metaList);
                            var fieldAttrs = GetFieldAttributes(fieldContext.attr());

                            if (string.IsNullOrEmpty(fieldName))
                            {
                                ReportError("字段名不能为空", fieldContext.Start.Line, fieldContext.Start.Column);
                                continue;
                            }
                            if (string.IsNullOrEmpty(fieldType) && string.IsNullOrEmpty(arrayType))
                            {
                                ReportError("字段类型不能为空", fieldContext.fieldName.Line, fieldContext.fieldName.Column);
                                continue;
                            }
                            if (nameTable.Contains(fieldName))
                            {
                                ReportError(string.Format("字段名称重复定义:\"{0}\"", fieldName), fieldContext.fieldName.Line, fieldContext.fieldName.Column);
                                continue;
                            }

                            var field = new TableField();
                            field.Comment = fieldComm;
                            field.Name = fieldName;
                            field.Type = !string.IsNullOrEmpty(fieldType) ? fieldType : arrayType;
                            field.IsArray = !string.IsNullOrEmpty(arrayType);
                            field.DefaultValue = fieldValue;
                            field.LinkField = !string.IsNullOrEmpty(fieldLink) ? fieldLink : fieldName;
                            field.Metas = fieldMetas;
                            field.Attributes = fieldAttrs;

                            fieldList.Add(field);
                            fieldNameList.Add(field.Name);

                            nameTable.Add(fieldName);
                        }
                    }

                    var tableName = context.name.Text;
                    if (typeName2start.ContainsKey(tableName) && typeName2start[tableName] !=context.name.StartIndex)
                    {
                        ReportError(string.Format("表名重复定义:\"{0}\"", tableName), context.name.Line, context.name.Column);
                    }
                    else
                    {
                        var data = new Table();
                        data.Name = tableName;
                        data.Comment = GetComment(context, context.name);
                        data.Metas = GetMetaDatas(context.metaList);
                        data.AttributeInfo = GetTableAttributes(context.attr(),fieldNameList.ToArray());

                        file.Tables.Add(data);
                    }
                }
            }

            private void HandleStruct(StructContext[] contexts)
            {
                foreach(var context in contexts)
                {
                    var fieldList = new List<StructField>();
                    var fieldNameList = new List<string>();

                    var fieldsContext = context.structField();
                    if (fieldsContext != null)
                    {
                        var nameTable = new HashSet<string>();
                        foreach (var fieldContext in fieldsContext)
                        {
                            var fieldComm = GetComment(fieldContext, fieldContext.fieldName);
                            var fieldName = fieldContext.fieldName != null ? fieldContext.fieldName.Text : "";
                            var fieldType = fieldContext.fieldType != null ? fieldContext.fieldType.GetText() : "";
                            var arrayType = fieldContext.arrayType != null && fieldContext.arrayType.type != null ? fieldContext.arrayType.type.GetText() : "";
                            var fieldValue = fieldContext.fieldValue != null ? fieldContext.fieldValue.GetText() : "";
                            var fieldLink = fieldContext.fieldMap != null ? fieldContext.fieldMap.Text : "";
                            var fieldMetas = GetMetaDatas(fieldContext.metaList);
                            var fieldAttrs = GetFieldAttributes(fieldContext.attr());

                            if (string.IsNullOrEmpty(fieldName))
                            {
                                ReportError("字段名不能为空", fieldContext.Start.Line, fieldContext.Start.Column);
                                continue;
                            }
                            if (string.IsNullOrEmpty(fieldType) && string.IsNullOrEmpty(arrayType))
                            {
                                ReportError("字段类型不能为空", fieldContext.fieldName.Line, fieldContext.fieldName.Column);
                                continue;
                            }
                            if (nameTable.Contains(fieldName))
                            {
                                ReportError(string.Format("字段名称重复定义:\"{0}\"", fieldName), fieldContext.fieldName.Line, fieldContext.fieldName.Column);
                                continue;
                            }

                            var field = new StructField();
                            field.Comment = fieldComm;
                            field.Name = fieldName;
                            field.Type = !string.IsNullOrEmpty(fieldType) ? fieldType : arrayType;
                            field.IsArray = !string.IsNullOrEmpty(arrayType);
                            field.DefaultValue = fieldValue;
                            field.Metas = fieldMetas;
                            field.LinkField = !string.IsNullOrEmpty(fieldLink) ? fieldLink : fieldName;
                            field.Attributes = fieldAttrs;

                            fieldList.Add(field);
                            fieldNameList.Add(field.Name);

                            nameTable.Add(fieldName);
                        }
                    }

                    var tableName = context.name.Text;
                    if (typeName2start.ContainsKey(tableName) && typeName2start[tableName] != context.name.StartIndex)
                    {
                        ReportError(string.Format("结构名称重复定义:\"{0}\"", tableName), context.name.Line, context.name.Column);
                    }
                    else
                    {
                        var data = new Struct();
                        data.Name = context.name.Text;
                        data.Comment = GetComment(context, context.name);
                        data.Metas = GetMetaDatas(context.metaList);
                        data.Attributes = GetTableAttributes(context.attr(),fieldNameList.ToArray());

                        file.Structs.Add(data);
                    }
                }
            }

            private void HandleEnum(EnumContext[] contexts)
            {
                var EnumBaseTypeNames = new List<string>() { "bool", "byte", "ubyte", "short", "ushort", "int", "uint", "long", "ulong", "int8", "uint8", "int16", "uint16", "int32", "uint32", "int64", "uint64" };

                foreach (var context in contexts)
                {
                    var data = new Enum();
                    data.Name = context.name.Text;
                    data.BaseType = "int";
                    data.Metas = GetMetaDatas(context.metaList);
                    data.Attributes = GetAttributes(context.attr());
                    data.Comment = GetComment(context, context.name);

                    if (context.baseType != null)
                    {
                        var name = context.baseType.GetText();
                        if (EnumBaseTypeNames.Contains(name))
                        {
                            data.BaseType = name;
                        }
                        else
                        {
                            ReportError("enum的基类型必须是整数类型.", context.baseType.Start.Line, context.baseType.Start.Column);
                        }
                    }

                    var fieldsContext = context.enumField();
                    if (fieldsContext != null)
                    {
                        var nameTable = new HashSet<string>();
                        foreach (var fieldContext in fieldsContext)
                        {
                            var fieldName = fieldContext.fieldName != null ? fieldContext.fieldName.Text : "";
                            var fieldValue = fieldContext.fieldValue != null ? fieldContext.fieldValue.Text : "";
                            var fieldComm = GetComment(fieldContext, fieldContext.fieldName);
                            var fieldAttrs = GetAttributes(fieldContext.attr());

                            if (string.IsNullOrEmpty(fieldName))
                            {
                                ReportError("字段名不能为空", fieldContext.Start.Line, fieldContext.Start.Column);
                                continue;
                            }
                            if (nameTable.Contains(fieldName))
                            {
                                ReportError(string.Format("字段名称重复定义:\"{0}\"", fieldName), fieldContext.fieldName.Line, fieldContext.fieldName.Column);
                                continue;
                            }

                            var field = new EnumField();
                            field.Name = fieldName;
                            field.Value = fieldValue;
                            field.Comment = fieldComm;
                            field.Attributes = fieldAttrs;

                            data.Fields.Add(field);
                            nameTable.Add(fieldName);
                        }
                    }

                    if (typeName2start.ContainsKey(data.Name) && typeName2start[data.Name] != context.name.StartIndex)
                    {
                        ReportError(string.Format("枚举名称重复定义:\"{0}\"", data.Name), context.name.Line, context.name.Column);
                    }
                    else
                    {
                        file.Enums.Add(data);
                    }
                }
            }

            private void HandleUnion(UnionContext[] contexts)
            {
                foreach(var context in contexts)
                {
                    var data = new Union();
                    data.Name = context.name.Text;
                    data.Metas = GetMetaDatas(context.metaList);
                    data.Attributes = GetAttributes(context.attr());
                    data.Comment = GetComment(context, context.name);

                    var fieldsContext = context.unionField();
                    if (fieldsContext != null)
                    {
                        var nameTable = new HashSet<string>();
                        foreach (var fieldContext in fieldsContext)
                        {
                            var fieldName = fieldContext.fieldName != null ? fieldContext.fieldName.Text : "";
                            var fieldValue = fieldContext.fieldValue != null ? fieldContext.fieldValue.Text : "";
                            var fieldComm = GetComment(fieldContext, fieldContext.fieldName);
                            var fieldAttrs = GetAttributes(fieldContext.attr());

                            if (string.IsNullOrEmpty(fieldName))
                            {
                                ReportError("字段名不能为空", fieldContext.Start.Line, fieldContext.Start.Column);
                                continue;
                            }
                            if (nameTable.Contains(fieldName))
                            {
                                ReportError(string.Format("字段名称重复定义:\"{0}\"", fieldName), fieldContext.fieldName.Line, fieldContext.fieldName.Column);
                                continue;
                            }

                            var field = new UnionField();
                            field.Name = fieldName;
                            field.Value = fieldValue;
                            field.Comment = fieldComm;
                            field.Attributes = fieldAttrs;

                            data.Fields.Add(field);
                            nameTable.Add(fieldName);
                        }
                    }

                    if (typeName2start.ContainsKey(data.Name) && typeName2start[data.Name] != context.name.StartIndex)
                    {
                        ReportError(string.Format("联合名称重复定义:\"{0}\"", data.Name), context.name.Line, context.name.Column);
                    }
                    else
                    {
                        file.Unions.Add(data);
                    }
                }
            }

            private void HandleRpc(RpcContext[] contexts)
            {
                foreach(var context in contexts)
                {
                    var data = new Rpc();
                    data.Name = context.name.Text;
                    data.Attributes = GetAttributes(context.attr());
                    data.Comment = GetComment(context, context.name);

                    var fieldsContext = context.rpcField();
                    if (fieldsContext != null)
                    {
                        var nameTable = new HashSet<string>();
                        foreach (var fieldContext in fieldsContext)
                        {
                            var fieldName = fieldContext.fieldName != null ? fieldContext.fieldName.Text : "";
                            var fieldParam = fieldContext.fieldParam != null ? fieldContext.fieldParam.Text : "";
                            var fieldReturn = fieldContext.fieldReturn != null ? fieldContext.fieldReturn.Text : "";
                            var fieldComm = GetComment(fieldContext, fieldContext.fieldName);
                            var fieldMetas = GetMetaDatas(fieldContext.metaList);
                            var fieldAttrs = GetAttributes(fieldContext.attr());

                            if (string.IsNullOrEmpty(fieldName))
                            {
                                ReportError("方法名不能为空", fieldContext.Start.Line, fieldContext.Start.Column);
                                continue;
                            }
                            if (nameTable.Contains(fieldName))
                            {
                                ReportError(string.Format("方法名称重复定义:\"{0}\"", fieldName), fieldContext.fieldName.Line, fieldContext.fieldName.Column);
                                continue;
                            }

                            var field = new RpcMethod();
                            field.Name = fieldName;
                            field.Param = fieldParam;
                            field.Return = fieldReturn;
                            field.Comment = fieldComm;
                            field.Metas = fieldMetas;
                            field.Attributes = fieldAttrs;

                            data.Fields.Add(field);
                            nameTable.Add(fieldName);
                        }
                    }

                    if (typeName2start.ContainsKey(data.Name) && typeName2start[data.Name] != context.name.StartIndex)
                    {
                        ReportError(string.Format("RPC名称重复定义:\"{0}\"", data.Name), context.name.Line, context.name.Column);
                    }
                    else
                    {
                        file.Rpcs.Add(data);
                    }
                }
            }

            private void HandleRootType(RootTypeContext[] contexts)
            {
                for (var i = 0; i < contexts.Length; i++)
                {
                    var context = contexts[i];
                    if (i == 0)
                    {
                        if (context.val != null && !string.IsNullOrEmpty(context.val.Text))
                        {
                            foreach (var item in file.Tables) { if (item.Name.Equals(context.val.Text)) { file.RootTable = item; break; } }
                            if (file.RootTable == null)
                            {
                                foreach (var item in file.Structs) { if (item.Name.Equals(context.val.Text)) { file.RootStruct = item; break; } }
                            }

                            if (file.RootTable == null && file.RootStruct == null)
                            {
                                ReportError(context.val.Text + "主类型未找到!", context.Start.Line, context.Start.Column);
                            }
                        }
                        else
                        {
                            ReportError("root_type 不能声明空类型!", context.Start.Line, context.Start.Column);
                        }
                    }
                    else
                    {
                        ReportError("root_type 重复声明!", context.Start.Line, context.Start.Column);
                    }
                }
            }

            private void HandleFileExtension(FileExtensionContext[] contexts)
            {
                for (var i = 0; i < contexts.Length; i++)
                {
                    var context = contexts[i];
                    if (i == 0)
                    {
                        file.FileExtension = context.key.Text.Trim('"');
                    }
                    else
                    {
                        ReportError("file_extension 重复声明!", context.Start.Line, context.Start.Column);
                    }
                }
            }

            private void HandleFileIdentifier(FileIdentifierContext[] contexts)
            {
                for (var i = 0; i < contexts.Length; i++)
                {
                    var context = contexts[i];
                    if (i == 0)
                    {
                        file.FileIdentifier = context.key.Text;
                    }
                    else
                    {
                        ReportError("file_identifier 重复声明!", context.Start.Line, context.Start.Column);
                    }
                }
            }

            private void InitComments(FlatbufferLexer lexer)
            {
                commentTable = new Dictionary<int, string>();

                foreach (var token in lexer.GetAllTokens())
                {
                    if (token.Type != FlatbufferLexer.COMMENT) { continue; }
                    var txt = token.Text;
                    if (txt.StartsWith("//"))
                    {
                        txt = txt.Substring(2).Trim();
                    }
                    else if (txt.StartsWith("/*"))
                    {
                        txt = txt.Substring(2, txt.Length - 4).Trim().Trim('*').Trim();
                    }
                    var lines = txt.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        commentTable.Add(token.Line + i, txt);
                    }
                }
                lexer.Reset();
            }

            private string GetComment(ParserRuleContext context, IToken token)
            {
                if (token == null) { return null; }

                var startLine = token.Line;
                var stopLine = context.Start.Line - 1;

                while (startLine > 0)
                {
                    if (commentTable.ContainsKey(startLine))
                    {
                        return commentTable[startLine];
                    }
                    startLine--;
                    if (startLine < stopLine) { break; }
                }
                return null;
            }

            private List<Meta> GetMetaDatas(FlatbufferParser.MetadataContext metaList)
            {
                var fieldMetas = new List<Meta>();
                if (metaList != null)
                {
                    foreach (var meta in metaList.metadataField())
                    {
                        var metaName = meta.metaName != null ? meta.metaName.Text : "";
                        var metaValue = meta.metaValue != null ? meta.metaValue.GetText() : "";
                        if (string.IsNullOrEmpty(metaName))
                        {
                            ReportError("元数据键不能为空", meta.Start.Line, meta.Start.Column);
                            continue;
                        }
                        fieldMetas.Add(new Meta(metaName, metaValue));
                    }
                }
                return fieldMetas;
            }

            private AttributeInfo GetAttributes(FlatbufferParser.AttrContext[] context)
            {
                var info = new AttributeInfo();
                foreach (var item in context)
                {
                    var name = item.key != null ? item.key.Text : null;
                    if (string.IsNullOrEmpty(name))
                    {
                        ReportError("特性名称不能为空", item.Start.Line, item.Start.Column);
                        continue;
                    }

                    var values = new List<string>();
                    foreach (var value in item.attrField()) { values.Add(value.GetText()); }

                    var attribute = new Attribute();
                    attribute.Name = name;
                    attribute.Values = values;

                    info.Attributes.Add(attribute);
                }
                return info;
            }

            private AttributeInfo GetTableAttributes(FlatbufferParser.AttrContext[] context,string[] fieldNames)
            {
                var info = new AttributeInfo();

                foreach (var item in context)
                {
                    var name = item.key != null ? item.key.Text : null;
                    if (string.IsNullOrEmpty(name))
                    {
                        ReportError("特性名称不能为空", item.Start.Line, item.Start.Column);
                        continue;
                    }

                    var attrFields = item.attrField();
                    if ("Bind".Equals(name))
                    {
                        if(string.IsNullOrEmpty(info.BindInfo))
                        {
                            if (attrFields.Length != 1 || attrFields[0].vstr == null)
                                ReportError("[Bind(\"xx/xx/xx.xx\")]", attrFields[0].Start.Line, attrFields[0].Start.Column);
                            else
                                info.BindInfo = attrFields[0].vstr.Text;
                        }
                        else
                        {
                            ReportError("Bind 重复指定!", item.Start.Line, item.Start.Column);
                        }
                    }
                    else if("Index".Equals(name))
                    {
                        var indexNames = new List<string>();
                        var attrFieldCounter = new Dictionary<string, int>();
                        foreach (var fieldName in fieldNames) { attrFieldCounter.Add(fieldName, 0); }
                        foreach (var attrField in attrFields)
                        {
                            if (attrField.vid != null && attrFieldCounter.ContainsKey(attrField.vid.Text))
                            {
                                var attrFieldName = attrField.vid.Text;
                                if (attrFieldCounter[attrFieldName] > 0)
                                {
                                    ReportError("[" + attrFieldName + "]不能重复存在", attrField.Start.Line, attrField.Start.Column);
                                }
                                else
                                { 
                                    indexNames.Add(attrFieldName);
                                }
                                attrFieldCounter[attrFieldName] += 1;
                            }
                            else
                            {
                                ReportError("[" + attrField.GetText() + "]不是一个字段名", attrField.Start.Line, attrField.Start.Column);
                            }
                        }
                        info.IndexTable.Add(indexNames.ToArray());
                    }
                    else if("Nullable".Equals(name) || "Reference".Equals(name))
                    {
                        ReportError("["+name+"] 只能应用到table字段或struct字段", item.Start.Line, item.Start.Column);
                    }
                    else
                    {
                        var values = new List<string>();
                        foreach (var value in attrFields) { values.Add(value.GetText()); }

                        var attribute = new Attribute();
                        attribute.Name = name;
                        attribute.Values = values;
                        info.Attributes.Add(attribute);
                    }
                }

                return info;
            }

            private AttributeInfo GetFieldAttributes(AttrContext[] context)
            {
                var info = new AttributeInfo();
                var nullableCount = 0;
                var referenceCount = 0;
                foreach (var item in context)
                {
                    var name = item.key != null ? item.key.Text : null;
                    if (string.IsNullOrEmpty(name))
                    {
                        ReportError("特性名称不能为空", item.Start.Line, item.Start.Column);
                        continue;
                    }

                    var attrFields = item.attrField();
                    if ("Bind".Equals(name) || "Index".Equals(name))
                    {
                        ReportError("[" + name + "] 只能应用到table或struct", item.Start.Line, item.Start.Column);
                    }
                    else if ("Nullable".Equals(name))
                    {
                        if (nullableCount > 0)
                            ReportError("Nullable 重复声明", attrFields[0].Start.Line, attrFields[0].Start.Column);
                        else
                        {
                            if (attrFields.Length == 0)
                                info.Nullable = true;
                            else if (attrFields.Length == 1)
                            {
                                if (attrFields[0].vbool == null)
                                    ReportError("Nullable 值只能是true/false", attrFields[0].Start.Line, attrFields[0].Start.Column);
                                else
                                    info.Nullable = attrFields[0].vbool.Text.Equals("true");
                            }
                        }
                        nullableCount++;
                    }
                    else if("Reference".Equals(name))
                    {
                        if(referenceCount>0)
                            ReportError("Nullable 重复声明", attrFields[0].Start.Line, attrFields[0].Start.Column);
                        else
                        {
                            //...
                        }
                        referenceCount++;
                    }
                    else
                    {
                        var values = new List<string>();
                        foreach (var value in attrFields) { values.Add(value.GetText()); }

                        var attribute = new Attribute();
                        attribute.Name = name;
                        attribute.Values = values;
                        info.Attributes.Add(attribute);
                    }
                }

                return info;
            }
        }
    }
}
