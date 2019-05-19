using Antlr4.Runtime;
using FlatBufferData.Model.Attributes;
using FlatBufferData.Model;
using FlatBufferData.Model.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using static FlatbufferParser;

namespace FlatBufferData.Build
{
    public class FBSBuilder
    {
        public delegate void ErrorReport(string projectName, string path, string text, int line, int column, int begin, int count);

        private string projectName;
        private ErrorReport report;
        private Dictionary<string, FBSFile> files = new Dictionary<string, FBSFile>();

        public FBSBuilder(string projectName, ErrorReport report)
        {
            this.projectName = projectName;
            this.report = report;
        }

        public FBSFile Open(string path,string text = null)
        {
            path = Path.GetFullPath(path);
            if (!files.ContainsKey(path) || text != null)
            {
                var fbsFile = new FBSFile(path);

                files.Add(path, fbsFile);

                if (text != null)
                    new FBSFileParser(this, fbsFile, report).Build(text);
                else
                    new FBSFileParser(this, fbsFile, report).Build();

                return fbsFile;
            }
            else
            {
                return files[path];
            }
        }

        public FBSFile FindFile(string currPath, string includePath)
        {
            if (!Path.IsPathRooted(includePath))
            {
                var path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(currPath), includePath));
                if (files.ContainsKey(path))
                    return files[path];

                if (File.Exists(path))
                    return Open(path);
            }
            return null;
        }


        private class FBSFileParser
        {
            private FBSBuilder project;
            private FBSFile file;
            public ErrorReport report;

            private Dictionary<object, object> data2context = new Dictionary<object, object>();

            public FBSFileParser(FBSBuilder project, FBSFile file, ErrorReport report)
            {
                this.project = project;
                this.file = file;
                this.report = report;
            }

            public FBSFile Build()
            {
                return Build(File.ReadAllText(file.Path));
            }

            public FBSFile Build(string text)
            {
                var lexer = new FlatbufferLexer(new AntlrInputStream(text));
                var parser = new FlatbufferParser(new CommonTokenStream(lexer));
                var comments = InitComments(lexer);
                var schema = parser.schema();

                //namespace
                foreach(var context in schema.@namespace())
                {
                    if (file.NameSpace == null)
                    {
                        var idents = context.IDENT();
                        if (idents.Length > 0)
                        {
                            var tokens = new string[idents.Length];
                            for (int j = 0; j < tokens.Length; j++) { tokens[j] = idents[j].GetText(); }
                            file.NameSpace = string.Join(".", tokens);
                        }
                        else
                            ReportError("错误的 namespace 声明!", context);
                    }
                    else
                        ReportError("namespace 重复声明!", context);
                }
                //file_extension
                foreach(var context in schema.fileExtension())
                {
                    if (file.FileExtension == null)
                    {
                        if (context.val == null || context.val.StartIndex == -1)
                            ReportError("错误的file_extension声明。", context);
                        else if (string.IsNullOrEmpty(context.val.Text.Trim('"')))
                            ReportError("file_extension声明的扩展名不能为空。", context.val);
                        else
                            file.FileExtension = context.val.Text.Trim('"');
                    }
                    else
                        ReportError("file_extension 重复声明。", context);
                }
                //file_identifier
                foreach(var context in schema.fileIdentifier())
                {
                    if (file.FileIdentifier == null)
                    {
                        if (context.val == null || context.val.StartIndex == -1)
                            ReportError("错误的file_identifier声明。", context);
                        else if (string.IsNullOrEmpty(context.val.Text.Trim('"')))
                            ReportError("错误的file_identifier声明声明的标识符不能为空。", context.val);
                        else
                            file.FileIdentifier = context.val.Text.Trim('"');
                    }
                    else
                        ReportError("错误的file_identifier声明 重复声明。", context);
                }
                //table
                foreach (var context in schema.table())
                {
                    var data = new Table();
                    data.Comment = GetComment(comments, context, context.name);
                    data.Name = context.name != null ? context.name.Text : null;
                    data.Metas = ParseMetaDatas(context.metaList);
                    data.Fields = new List<TableField>();

                    var fieldNameSet = new HashSet<string>();
                    foreach (var fieldContext in context.tableField())
                    {
                        var field = new TableField();
                        field.Comment = GetComment(comments, fieldContext, fieldContext.fieldName);
                        field.Name = fieldContext.fieldName != null && fieldContext.fieldName.StartIndex != -1 ? fieldContext.fieldName.Text : null;
                        field.Type = fieldContext.fieldType != null ? fieldContext.fieldType.GetText() : (fieldContext.arrayType != null ? fieldContext.arrayType.type.GetText() : null);
                        field.IsArray = fieldContext.arrayType != null && fieldContext.fieldType == null;
                        field.DefaultValue = ParseDefaultValue(field.Type, field.IsArray, fieldContext.fieldValue);
                        field.Metas = ParseMetaDatas(fieldContext.metaList);
                        field.DataField = fieldContext.fieldMap != null && fieldContext.fieldMap.StartIndex != -1 ? fieldContext.fieldMap.Text.Trim('"') : field.Name;

                        if (string.IsNullOrEmpty(field.Name))
                        {
                            ReportError("名称不能为空。", fieldContext);
                            continue;
                        }
                        else if (fieldNameSet.Contains(field.Name))
                        {
                            ReportError("重复的名称。", fieldContext.fieldName);
                            continue;
                        }
                        else if (string.IsNullOrEmpty(field.Type))
                        {
                            ReportError("类型不能为空。", fieldContext);
                            continue;
                        }

                        fieldNameSet.Add(field.Name);

                        data.Fields.Add(field);
                        data2context.Add(field, fieldContext);
                    }

                    file.Tables.Add(data);
                    data2context.Add(data, context);
                }
                //struct
                foreach(var context in schema.@struct())
                {
                    var data = new Struct();
                    data.Comment = GetComment(comments, context, context.name);
                    data.Name = context.name != null ? context.name.Text : null;
                    data.Metas = ParseMetaDatas(context.metaList);
                    data.Fields = new List<StructField>();

                    var fieldNameSet = new HashSet<string>();
                    foreach (var fieldContext in context.structField())
                    {
                        var field = new StructField();
                        field.Comment = GetComment(comments, fieldContext, fieldContext.fieldName);
                        field.Name = fieldContext.fieldName != null && fieldContext.fieldName.StartIndex != -1 ? fieldContext.fieldName.Text : null;
                        field.Type = fieldContext.fieldType != null ? fieldContext.fieldType.GetText() : (fieldContext.arrayType != null ? fieldContext.arrayType.GetText() : null);
                        field.IsArray = fieldContext.arrayType != null && fieldContext.fieldType == null;
                        field.DefaultValue = null;
                        field.Metas = ParseMetaDatas(fieldContext.metaList);
                        field.DataField = fieldContext.fieldMap != null && fieldContext.fieldMap.StartIndex != -1 ? fieldContext.fieldMap.Text.Trim('"') : field.Name;

                        if (string.IsNullOrEmpty(field.Name))
                        {
                            ReportError("名称不能为空。", fieldContext);
                            continue;
                        }
                        else if (fieldNameSet.Contains(field.Name))
                        {
                            ReportError("重复的名称。", fieldContext.fieldName);
                            continue;
                        }
                        else if (string.IsNullOrEmpty(field.Type))
                        {
                            ReportError("类型不能为空", fieldContext);
                            continue;
                        }
                        else if (field.IsArray)
                        {
                            ReportError("struct字段的类型不能是数组。", fieldContext.arrayType != null ? fieldContext.arrayType as ParserRuleContext : fieldContext.fieldType as ParserRuleContext);
                            continue;
                        }
                        
                        if (fieldContext.fieldValue != null)
                            ReportError("struct字段目前不支持默认值。", fieldContext.fieldValue);

                        fieldNameSet.Add(field.Name);

                        data.Fields.Add(field);
                        data2context.Add(field, fieldContext);
                    }

                    file.Structs.Add(data);
                    data2context.Add(data, context);
                }
                //enum
                foreach(var context in schema.@enum())
                {
                    var data = new Enum();
                    data.Comment = GetComment(comments, context, context.name);
                    data.Name = context.name != null ? context.name.Text : null;
                    data.Metas = ParseMetaDatas(context.metaList);
                    data.BaseType = "int";

                    var fieldIDSet = new HashSet<int>();
                    var fieldNameSet = new HashSet<string>();
                    foreach (var fieldContext in context.enumField())
                    {
                        var field = new EnumField();
                        field.Name = fieldContext.fieldName != null && fieldContext.fieldName.StartIndex != -1 ? fieldContext.fieldName.Text : null;
                        field.Value = fieldContext.fieldValue != null && fieldContext.fieldValue.StartIndex != -1 ? fieldContext.fieldValue.Text : null;
                        field.Comment = GetComment(comments, fieldContext, fieldContext.fieldName);

                        var enumIDValidate = false;
                        if (!string.IsNullOrEmpty(field.Value))
                        {
                            var enumID = 0;
                            if (int.TryParse(field.Value, out enumID))
                            {
                                field.ID = enumID;
                                enumIDValidate = true;
                            }
                        }

                        if (string.IsNullOrEmpty(field.Name))
                        {
                            ReportError("名称不能为空。", fieldContext);
                            continue;
                        }
                        else if (fieldIDSet.Contains(field.ID))
                        {
                            ReportError("重复的ID。", fieldContext.fieldValue);
                            continue;
                        }
                        else if (fieldNameSet.Contains(field.Name))
                        {
                            ReportError("重复的名称。", fieldContext.fieldName);
                            continue;
                        }

                        if(enumIDValidate)
                            fieldIDSet.Add(field.ID);

                        fieldNameSet.Add(field.Name);

                        data.Fields.Add(field);
                        data2context.Add(field, fieldContext);
                    }

                    if (context.baseType != null)
                    {
                        var name = context.baseType.GetText();
                        if (IsNativeType(name) && !"float".Equals(name) && !"float32".Equals(name) && !"double".Equals(name) && !"float64".Equals(name) && !"string".Equals(name))
                            data.BaseType = name;
                        else
                            ReportError("enum的基类型必须是整数类型.", context.baseType);
                    }

                    var autoID = 0;
                    var autoedIDs = new HashSet<int>();
                    foreach(var field in data.Fields)
                    {
                        if (fieldIDSet.Contains(field.ID))
                        {
                            autoID = field.ID + 1;
                            continue;
                        }

                        while (fieldIDSet.Contains(autoID) || autoedIDs.Contains(autoID)) { autoID++; }

                        autoedIDs.Add(autoID);

                        field.ID = autoID;
                    }

                    file.Enums.Add(data);
                    data2context.Add(data, context);
                }
                //union
                foreach (var context in schema.union())
                {
                    var data = new Union();
                    data.Comment = GetComment(comments, context, context.name);
                    data.Name = context.name != null ? context.name.Text : null;
                    data.Metas = ParseMetaDatas(context.metaList);

                    var fieldNameSet = new HashSet<string>();
                    foreach (var fieldContext in context.unionField())
                    {
                        var field = new UnionField();
                        field.Name = fieldContext.fieldName != null && fieldContext.fieldName.StartIndex != -1 ? fieldContext.fieldName.Text : null;
                        field.Type = fieldContext.fieldType != null ? fieldContext.fieldType.GetText() : null;
                        field.Comment = GetComment(comments, fieldContext, fieldContext.fieldName);

                        if (string.IsNullOrEmpty(field.Name))
                        {
                            ReportError("名称不能为空。", fieldContext);
                            continue;
                        }
                        else if (fieldNameSet.Contains(field.Name))
                        {
                            ReportError("重复的名称。", fieldContext.fieldName);
                            continue;
                        }
                        else if (string.IsNullOrEmpty(field.Type))
                        {
                            ReportError("类型不能为空", fieldContext);
                            continue;
                        }

                        fieldNameSet.Add(field.Name);

                        data.Fields.Add(field);
                        data2context.Add(field, fieldContext);
                    }

                    file.Unions.Add(data);
                    data2context.Add(data, context);
                }
                //rpc
                foreach (var context in schema.rpc())
                {
                    var data = new Rpc();
                    data.Comment = GetComment(comments, context, context.name);
                    data.Name = context.name != null ? context.name.Text : null;

                    var fieldNameSet = new HashSet<string>();
                    foreach (var fieldContext in context.rpcField())
                    {
                        var field = new RpcMethod();
                        field.Comment = GetComment(comments, fieldContext, fieldContext.fieldName);
                        field.Name = fieldContext.fieldName != null && fieldContext.fieldName.StartIndex != -1 ? fieldContext.fieldName.Text : null;
                        field.Param = fieldContext.fieldParam != null && fieldContext.fieldParam.StartIndex != -1 ? fieldContext.fieldParam.Text : null;
                        field.Return = fieldContext.fieldReturn != null && fieldContext.fieldReturn.StartIndex != -1 ? fieldContext.fieldReturn.Text : null;
                        field.Metas = ParseMetaDatas(fieldContext.metaList);

                        if (string.IsNullOrEmpty(field.Name))
                        {
                            ReportError("名称不能为空。", fieldContext);
                            continue;
                        }
                        else if (fieldNameSet.Contains(field.Name))
                        {
                            ReportError("重复的名称。", fieldContext.fieldName);
                            continue;
                        }

                        fieldNameSet.Add(field.Name);

                        data.Fields.Add(field);
                        data2context.Add(field, fieldContext);
                    }

                    file.Rpcs.Add(data);
                    data2context.Add(data, context);
                }
                //root_type
                foreach(var context in schema.rootType())
                {
                    if(file.RootTable==null)
                    {
                        if (context.val == null || context.val.StartIndex == -1)
                            ReportError("错误的 root_type 声明。", context);
                        else
                        {
                            var tabName = context.val.Text;
                            if (string.IsNullOrEmpty(tabName))
                                ReportError("错误的 root_type 声明。", context.val);
                            else
                            {
                                foreach (var item in file.Tables) { if (item.Name.Equals(tabName)) { file.RootTable = item; break; } }
                                if (file.RootTable == null)
                                    ReportError(string.Format("主表定义 ({0}) 未找到!", context.val.Text), context.val);
                            }
                        }
                    }
                    else
                        ReportError("root_type 重复声明!", context);
                }
                //include
                var includeFBSs = new List<FBSFile>();
                foreach (var context in schema.include())
                {
                    if (context.val == null || context.val.StartIndex == -1)
                    {
                        ReportError("无效的include。", context);
                        continue;
                    }
                    var url = context.val.Text.Trim('"');
                    if (string.IsNullOrEmpty(url))
                    {
                        ReportError("无效的include。", context.val);
                        continue;
                    }
                    var fbs = project.FindFile(file.Path, url);
                    if (fbs == null)
                    {
                        ReportError(string.Format("{0} 未找到。",url), context.val);
                        continue;
                    }

                    includeFBSs.Add(fbs);
                }
                file.Includes = includeFBSs.ToArray();

                //check
                CheckAllDefined();

                //attributes
                ParseAllAttributes();

                //clear
                data2context.Clear();

                return file;
            }

            #region 处理默认值
            private object ParseDefaultValue(string fieldType, bool isArray, ScalarValueContext context)
            {
                if (context == null) { return null; }

                var fieldValue = context.GetText();

                if (string.IsNullOrEmpty(fieldValue)) { return null; }

                if (isArray)
                {
                    ReportError("列表类型不支持默认值。", context);
                    return null;
                }
                else if (fieldType.Equals("bool"))
                {
                    var boolValue = false;
                    if (bool.TryParse(fieldValue, out boolValue))
                        return boolValue;
                    else
                        ReportError(string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
                }
                else if (fieldType.Equals("byte") || fieldType.Equals("int8"))
                {
                    sbyte byteValue = 0;
                    if (sbyte.TryParse(fieldValue, out byteValue))
                        return byteValue;
                    else
                        ReportError(string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
                }
                else if (fieldType.Equals("ubyte") || fieldType.Equals("uint8"))
                {
                    byte byteValue = 0;
                    if (byte.TryParse(fieldValue, out byteValue))
                        return byteValue;
                    else
                        ReportError(string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
                }
                else if (fieldType.Equals("short") || fieldType.Equals("int16"))
                {
                    short byteValue = 0;
                    if (short.TryParse(fieldValue, out byteValue))
                        return byteValue;
                    else
                        ReportError(string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
                }
                else if (fieldType.Equals("ushort") || fieldType.Equals("uint16"))
                {
                    ushort byteValue = 0;
                    if (ushort.TryParse(fieldValue, out byteValue))
                        return byteValue;
                    else
                        ReportError(string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
                }
                else if (fieldType.Equals("int") || fieldType.Equals("int32"))
                {
                    int byteValue = 0;
                    if (int.TryParse(fieldValue, out byteValue))
                        return byteValue;
                    else
                        ReportError(string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
                }
                else if (fieldType.Equals("uint") || fieldType.Equals("uint32"))
                {
                    uint byteValue = 0;
                    if (uint.TryParse(fieldValue, out byteValue))
                        return byteValue;
                    else
                        ReportError(string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
                }
                else if (fieldType.Equals("long") || fieldType.Equals("int64"))
                {
                    long byteValue = 0;
                    if (long.TryParse(fieldValue, out byteValue))
                        return byteValue;
                    else
                        ReportError(string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
                }
                else if (fieldType.Equals("ulong") || fieldType.Equals("uint64"))
                {
                    ulong byteValue = 0;
                    if (ulong.TryParse(fieldValue, out byteValue))
                        return byteValue;
                    else
                        ReportError(string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
                }
                else if (fieldType.Equals("float") || fieldType.Equals("float32"))
                {
                    float byteValue = 0;
                    if (float.TryParse(fieldValue, out byteValue))
                        return byteValue;
                    else
                        ReportError(string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
                }
                else if (fieldType.Equals("double") || fieldType.Equals("float64"))
                {
                    double byteValue = 0;
                    if (double.TryParse(fieldValue, out byteValue))
                        return byteValue;
                    else
                        ReportError(string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
                }
                else
                {
                    ReportError(string.Format("目前 {0} 不支持默认值。", fieldType), context);
                }
                return null;
            }

            #endregion

            #region 处理元数据
            private List<Meta> ParseMetaDatas(FlatbufferParser.MetadataContext metaList)
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
                            ReportError("元数据键不能为空", meta);
                            continue;
                        }
                        fieldMetas.Add(new Meta(metaName, metaValue));
                    }
                }
                return fieldMetas;
            }
            #endregion

            #region 检查所有定义

            private void CheckAllDefined()
            {
                var allDefined = GetAllDefined(file);

                //检查类定义
                var ns = !string.IsNullOrEmpty(file.NameSpace) ? file.NameSpace + "." : "";
                foreach (var data in file.Tables)
                {
                    if (string.IsNullOrEmpty(data.Name)) continue;
                    var key = ns + data.Name;
                    if (allDefined.ContainsKey(key) && allDefined[key].Count > 1)
                        ReportError("名称重复定义。", (data2context[data] as TableContext).name);
                }
                foreach (var data in file.Structs)
                {
                    if (string.IsNullOrEmpty(data.Name)) continue;
                    var key = ns + data.Name;
                    if (allDefined.ContainsKey(key) && allDefined[key].Count > 1)
                        ReportError("名称重复定义。", (data2context[data] as StructContext).name);
                }
                foreach (var data in file.Enums)
                {
                    if (string.IsNullOrEmpty(data.Name)) continue;
                    var key = ns + data.Name;
                    if (allDefined.ContainsKey(key) && allDefined[key].Count > 1)
                        ReportError("名称重复定义。", (data2context[data] as EnumContext).name);
                }
                foreach (var data in file.Unions)
                {
                    if (string.IsNullOrEmpty(data.Name)) continue;
                    var key = ns + data.Name;
                    if (allDefined.ContainsKey(key) && allDefined[key].Count > 1)
                        ReportError("名称重复定义。", (data2context[data] as UnionContext).name);
                }
                foreach (var data in file.Rpcs)
                {
                    if (string.IsNullOrEmpty(data.Name)) continue;
                    var key = ns + data.Name;
                    if (allDefined.ContainsKey(key) && allDefined[key].Count > 1)
                        ReportError("名称重复定义。", (data2context[data] as RpcContext).name);
                }
                //检查字段定义
                foreach (var data in file.Tables)
                {
                    foreach (var field in data.Fields)
                    {
                        if (string.IsNullOrEmpty(field.Type) || IsNativeType(field.Type)) continue;
                        var typeName = field.Type.IndexOf(".") != -1 ? field.Type : ns + field.Type;
                        var context = data2context[field] as TableFieldContext;
                        var locator = context.arrayType != null ? context.arrayType as ParserRuleContext : context.fieldType as ParserRuleContext;
                        if (allDefined.ContainsKey(typeName))
                        {
                            if (allDefined[typeName].Count > 1)
                                ReportError(string.Format("找到多个名称为 {0} 的定义。", field.Type), locator);
                            else if (allDefined[typeName].Count == 1)
                            {
                                var first = allDefined[typeName][0];
                                if (first is Table || first is Struct || first is Enum || first is Union)
                                    field.TypeDefined = first;
                                else
                                    ReportError("table字段的类型不能是rpc。", locator);
                            }
                        }
                        else
                            ReportError(string.Format("没有找到 {0} 的定义。", field.Type), locator);
                    }
                }
                foreach (var data in file.Structs)
                {
                    foreach (var field in data.Fields)
                    {
                        var context = data2context[field] as StructFieldContext;
                        var locator = context.arrayType != null ? context.arrayType as ParserRuleContext : context.fieldType as ParserRuleContext;

                        if ("string".Equals(field.Type))
                            ReportError("struct字段的类型只能是bool, number, enum, struct。", locator);
                        else if (!IsNativeType(field.Type))
                        {
                            var typeName = field.Type.IndexOf(".") != -1 ? field.Type : ns + field.Type;
                            if (allDefined.ContainsKey(typeName))
                            {
                                if (allDefined[typeName].Count > 1)
                                    ReportError(string.Format("找到多个名称为 {0} 的定义。", field.Type), locator);
                                else if (allDefined[typeName].Count == 1)
                                {
                                    var first = allDefined[typeName][0];
                                    if (first is Struct || first is Enum)
                                        field.TypeDefined = first;
                                    else
                                        ReportError("struct字段的类型只能是bool, number, enum, struct。", locator);
                                }
                            }
                            else
                                ReportError(string.Format("没有找到 {0} 的定义。", field.Type), locator);
                        }
                    }
                }
                foreach(var data in file.Unions)
                {
                    foreach(var field in data.Fields)
                    {
                        if ("string".Equals(field.Type))
                            continue;

                        var context = data2context[field] as UnionFieldContext;
                        var locator = context.fieldType as ParserRuleContext;
                        if(IsNativeType(field.Type))
                        {
                            ReportError("union字段的类型只能是string, struct, table。", locator);
                        }
                        else
                        {
                            var typeName = field.Type.IndexOf(".") != -1 ? field.Type : ns + field.Type;
                            if (allDefined.ContainsKey(typeName))
                            {
                                if (allDefined[typeName].Count > 1)
                                    ReportError(string.Format("找到多个名称为 {0} 的定义。", field.Type), locator);
                                else if (allDefined[typeName].Count == 1)
                                {
                                    var first = allDefined[typeName][0];
                                    if (first is Struct || first is Table)
                                        field.TypeDefined = first;
                                    else
                                        ReportError("union字段的类型只能是string,struct,table。", locator);
                                }
                            }
                            else
                                ReportError(string.Format("没有找到 {0} 的定义。", field.Type), locator);
                        }
                    }
                }
                foreach (var data in file.Rpcs)
                {
                    foreach (var field in data.Fields)
                    {
                        if (!string.IsNullOrEmpty(field.Param) && !IsNativeType(field.Param))
                        {
                            var typeName = field.Param.IndexOf(".") != -1 ? field.Param : ns + field.Param;
                            if (allDefined.ContainsKey(typeName))
                            {
                                if (allDefined[typeName].Count > 1)
                                    ReportError(string.Format("找到多个名称为 {0} 的定义。", field.Param), (data2context[field] as RpcFieldContext).fieldParam);
                                else if (allDefined[typeName].Count == 1)
                                {
                                    var first = allDefined[typeName][0];
                                    if (first is Struct || first is Table || first is Enum || first is Union)
                                        field.ParamTypeDefined = first;
                                    else
                                        ReportError("rpc字段的参数类型不能是rpc", (data2context[field] as RpcFieldContext).fieldParam);
                                }
                            }
                            else
                                ReportError(string.Format("没有找到 {0} 的定义。", field.Param), (data2context[field] as RpcFieldContext).fieldParam);
                        }
                        if (!string.IsNullOrEmpty(field.Return) && !IsNativeType(field.Return))
                        {
                            var typeName = field.Return.IndexOf(".") != -1 ? field.Return : ns + field.Return;
                            if (allDefined.ContainsKey(typeName))
                            {
                                if (allDefined[typeName].Count > 1)
                                    ReportError(string.Format("找到多个名称为 {0} 的定义。", field.Return), (data2context[field] as RpcFieldContext).fieldReturn);
                                else if (allDefined[typeName].Count == 1)
                                {
                                    var first = allDefined[typeName][0];
                                    if (first is Struct || first is Table || first is Enum || first is Union)
                                        field.ReturnTypeDefined = first;
                                    else
                                        ReportError("rpc字段的返回类型不能是rpc", (data2context[field] as RpcFieldContext).fieldReturn);
                                }
                            }
                            else
                                ReportError(string.Format("没有找到 {0} 的定义。", field.Return), (data2context[field] as RpcFieldContext).fieldReturn);
                        }
                    }
                }
            }

            private Dictionary<string, List<object>> GetAllDefined(FBSFile file)
            {
                var files = new HashSet<FBSFile>();
                var all = new Dictionary<string, List<object>>();
                GetAllDefined(file, files, all);
                return all;
            }

            private Dictionary<string, List<object>> GetAllDefined(FBSFile file, HashSet<FBSFile> files, Dictionary<string, List<object>> all)
            {
                var ns = !string.IsNullOrEmpty(file.NameSpace) ? file.NameSpace + "." : "";
                foreach(var data in file.Tables)
                {
                    if (string.IsNullOrEmpty(data.Name)) continue;
                    var key = ns + data.Name;
                    if (!all.ContainsKey(key)) all.Add(key, new List<object>());
                    all[key].Add(data);
                }
                foreach (var data in file.Structs)
                {
                    if (string.IsNullOrEmpty(data.Name)) continue;
                    var key = ns + data.Name;
                    if (!all.ContainsKey(key)) all.Add(key, new List<object>());
                    all[key].Add(data);
                }
                foreach (var data in file.Enums)
                {
                    if (string.IsNullOrEmpty(data.Name)) continue;
                    var key = ns + data.Name;
                    if (!all.ContainsKey(key)) all.Add(key, new List<object>());
                    all[key].Add(data);
                }
                foreach (var data in file.Unions)
                {
                    if (string.IsNullOrEmpty(data.Name)) continue;
                    var key = ns + data.Name;
                    if (!all.ContainsKey(key)) all.Add(key, new List<object>());
                    all[key].Add(data);
                }
                foreach (var data in file.Rpcs)
                {
                    if (string.IsNullOrEmpty(data.Name)) continue;
                    var key = ns + data.Name;
                    if (!all.ContainsKey(key)) all.Add(key, new List<object>());
                    all[key].Add(data);
                }

                files.Add(file);

                foreach(var include in file.Includes)
                {
                    if(!files.Contains(include))
                    {
                        GetAllDefined(include, files, all);
                    }
                }
                return all;
            }

            #endregion

            #region 处理附加特性

            private Dictionary<string, System.Type> typeTable;

            private void ParseAllAttributes()
            {
                //处理附加特性
                foreach (var data in file.Tables)
                {
                    ParseAttribute((data2context[data] as TableContext).attr(), data, data.Attributes);
                    foreach (var field in data.Fields)
                        ParseAttribute((data2context[field] as TableFieldContext).attr(), field, field.Attributes);
                }
                foreach (var data in file.Structs)
                {
                    ParseAttribute((data2context[data] as StructContext).attr(), data, data.Attributes);
                    foreach (var field in data.Fields)
                        ParseAttribute((data2context[field] as StructFieldContext).attr(), field, field.Attributes);
                }
                foreach (var data in file.Enums)
                {
                    ParseAttribute((data2context[data] as EnumContext).attr(), data, data.Attributes);
                    foreach (var field in data.Fields)
                        ParseAttribute((data2context[field] as EnumFieldContext).attr(), field, field.Attributes);
                }
                foreach (var data in file.Unions)
                {
                    ParseAttribute((data2context[data] as UnionContext).attr(), data, data.Attributes);
                    foreach (var field in data.Fields)
                        ParseAttribute((data2context[field] as UnionFieldContext).attr(), field, field.Attributes);
                }
                foreach (var data in file.Rpcs)
                {
                    ParseAttribute((data2context[data] as RpcContext).attr(), data, data.Attributes);
                    foreach (var field in data.Fields)
                        ParseAttribute((data2context[field] as RpcFieldContext).attr(), field, field.Attributes);
                }
            }
            private void ParseAttribute(AttrContext[] contexts, object owner,AttributeTable attributes)
            {
                foreach (var context in contexts)
                {
                    var attributeName = context.key != null ? context.key.Text : null;
                    if (string.IsNullOrEmpty(attributeName))
                    {
                        ReportError("特性名称不能为空", context);
                        continue;
                    }

                    var attributeType = GetAttributeSchema(attributeName);
                    if (attributeType == null)
                        return;

                    bool allow = true;
                    foreach (var attributeMeta in attributeType.GetCustomAttributesData())
                    {
                        if (typeof(AllowOwner) == attributeMeta.AttributeType)
                            allow = OnAllowOwner(attributeType, context, owner, attributes, attributeMeta.ConstructorArguments as IList<CustomAttributeTypedArgument>) && allow;
                        else if (typeof(AllowMultiple) == attributeMeta.AttributeType)
                            allow = OnAllowMultiple(attributeType, context, owner, attributes, attributeMeta.ConstructorArguments as IList<CustomAttributeTypedArgument>) && allow;
                        else if (typeof(ConflictType) == attributeMeta.AttributeType)
                            allow = OnConflictType(attributeType, context, owner, attributes, attributeMeta.ConstructorArguments[0].Value as IList<CustomAttributeTypedArgument>) && allow;
                        else if (typeof(RequiredType) == attributeMeta.AttributeType)
                            allow = OnRequiredType(attributeType, context, owner, attributes, attributeMeta.ConstructorArguments[0].Value as IList<CustomAttributeTypedArgument>) && allow;
                        else if (typeof(RequiredArrayField) == attributeMeta.AttributeType)
                            allow = OnRequiredArray(attributeType, context, owner, attributes) && allow;
                    }

                    if (allow)
                        CheckAndCreateAttribute(attributeType, context, attributes);
                }
            }

            private System.Type GetAttributeSchema(string name)
            {
                if(typeTable==null)
                {
                    typeTable = new Dictionary<string, System.Type>();
                    typeTable.Add(typeof(XLS).Name,typeof(XLS));
                    typeTable.Add(typeof(JsonFile).Name, typeof(JsonFile));
                    typeTable.Add(typeof(JsonFileRef).Name, typeof(JsonFileRef));
                    typeTable.Add(typeof(JsonLiteral).Name, typeof(JsonLiteral));
                    typeTable.Add(typeof(JsonPath).Name, typeof(JsonPath));
                    typeTable.Add(typeof(ArrayLiteral).Name, typeof(ArrayLiteral));
                    typeTable.Add(typeof(StructLiteral).Name, typeof(StructLiteral));
                    typeTable.Add(typeof(Index).Name, typeof(Index));
                    typeTable.Add(typeof(Nullable).Name, typeof(Nullable));
                    typeTable.Add(typeof(Unique).Name, typeof(Unique));
                }

                if(typeTable.ContainsKey(name))
                    return typeTable[name];
                else
                    return null;
            }

            private bool OnAllowOwner(System.Type attributeType, AttrContext context, object owner, AttributeTable attributes, IList<CustomAttributeTypedArgument> args)
            {
                if (args.Count > 0)
                {
                    TargetTypeID ownerTypeID = TargetTypeID.All;
                    if (owner is Table)
                        ownerTypeID = TargetTypeID.Table;
                    else if (owner is TableField)
                        ownerTypeID = TargetTypeID.TableField;
                    else if (owner is Struct)
                        ownerTypeID = TargetTypeID.Struct;
                    else if (owner is StructField)
                        ownerTypeID = TargetTypeID.StructField;
                    else if (owner is Enum)
                        ownerTypeID = TargetTypeID.Enum;
                    else if (owner is EnumField)
                        ownerTypeID = TargetTypeID.EnumField;
                    else if (owner is Union)
                        ownerTypeID = TargetTypeID.Union;
                    else if (owner is UnionField)
                        ownerTypeID = TargetTypeID.UnionField;
                    else if (owner is Rpc)
                        ownerTypeID = TargetTypeID.Rpc;
                    else if (owner is RpcMethod)
                        ownerTypeID = TargetTypeID.RpcMethod;

                    var allowOwnerTypes = (TargetTypeID)args[0].Value;
                    var value = ((int)allowOwnerTypes) & ((int)ownerTypeID);
                    if (value == 0)
                    {
                        ReportError(string.Format("{0}不能应用到{1}.", context.key.Text, ownerTypeID), context.key);
                        return false;
                    }
                }

                return true;
            }

            private bool OnAllowMultiple(System.Type attributeType, AttrContext context, object owner, AttributeTable attributes, IList<CustomAttributeTypedArgument> args)
            {
                if (args.Count > 0)
                {
                    var allowMultiple = (bool)args[0].Value;
                    if (allowMultiple == false && attributes.HasAttribute(attributeType))
                    {
                        ReportError(string.Format("同一对象不允许有多个{0}.", attributeType.Name), context.key);
                        return false;
                    }
                }

                return true;
            }

            private bool OnConflictType(System.Type attributeType, AttrContext context, object owner, AttributeTable attributes, IList<CustomAttributeTypedArgument> args)
            {
                foreach(var arg in args)
                {
                    var type = arg.Value as System.Type;
                    if(attributes.HasAttribute(type))
                    {
                        ReportError(string.Format("{0} 不能和 {1} 应用到同一个对象。", attributeType.Name, type.Name), context.key);
                    }
                }
                return true;
            }
            private bool OnRequiredType(System.Type attributeType, AttrContext context, object owner, AttributeTable attributes, IList<CustomAttributeTypedArgument> args)
            {
                bool finded = false;
                foreach (var arg in args)
                {
                    var type = arg.Value as System.Type;
                    if (attributes.HasAttribute(type))
                        finded = true;
                }
                if(!finded)
                {
                    List<string> names = new List<string>();
                    foreach(var arg in args)
                    {
                        names.Add((arg.Value as System.Type).Name);
                    }

                    ReportError(string.Format("{0} 需要和这些标记一起使用（{1}）。", attributeType.Name, string.Join(",", names)), context.key);
                }
                return true;
            }

            private bool OnRequiredArray(System.Type attributeType, AttrContext context, object owner, AttributeTable attributes)
            {
                var field = owner as TableField;
                if (field == null || !field.IsArray)
                {
                    ReportError("只能应用到数组型TableField！", context.key);
                    return false;
                }
                return true;
            }

            private enum ArgumentType { VOID, CONST, BOOL, INT, FLOAT, STRING }

            /// <summary>
            /// 检查并创建Attribute
            /// </summary>
            /// <param name="attributeType"></param>
            /// <param name="context"></param>
            /// <param name="owner"></param>
            /// <param name="attributes"></param>
            private void CheckAndCreateAttribute(System.Type attributeType, AttrContext context, AttributeTable attributes)
            {
                var fields = context.attrField();
                var constructors = attributeType.GetConstructors();

                var constructorIndex = -1;

                for(int i=0;i<constructors.Length;i++)
                {
                    var constructor = constructors[i];
                    var infos = constructor.GetParameters();

                    if (fields.Length != infos.Length)
                        continue;

                    if (constructorIndex == -1)
                        constructorIndex = i;

                    bool matched = true;

                    for (int j = 0; j < infos.Length; j++)
                    {
                        var info = infos[j];
                        var infoType = ArgumentType.VOID;
                        if (info.ParameterType == typeof(bool))
                            infoType = ArgumentType.BOOL;
                        else if (info.ParameterType == typeof(byte) || info.ParameterType == typeof(sbyte))
                            infoType = ArgumentType.INT;
                        else if (info.ParameterType == typeof(short) || info.ParameterType == typeof(int) || info.ParameterType == typeof(long))
                            infoType = ArgumentType.INT;
                        else if (info.ParameterType == typeof(ushort) || info.ParameterType == typeof(uint) || info.ParameterType == typeof(ulong))
                            infoType = ArgumentType.INT;
                        else if (info.ParameterType == typeof(decimal))
                            infoType = ArgumentType.INT;
                        else if (info.ParameterType == typeof(float) || info.ParameterType == typeof(double))
                            infoType = ArgumentType.FLOAT;
                        else if (info.ParameterType == typeof(string))
                            infoType = ArgumentType.STRING;

                        var field = fields[j];
                        var fieldType = ArgumentType.VOID;
                        if (field.attrValue != null)
                        {
                            if (field.attrValue.vbool != null)
                                fieldType = ArgumentType.BOOL;
                            else if (field.attrValue.vint != null)
                                fieldType = ArgumentType.INT;
                            else if (field.attrValue.vfloat != null)
                                fieldType = ArgumentType.FLOAT;
                            else if (field.attrValue.vstr != null)
                                fieldType = ArgumentType.STRING;
                            else if (field.attrValue.vid != null)
                                fieldType = ArgumentType.STRING;
                        }

                        if (infoType == ArgumentType.VOID || fieldType == ArgumentType.VOID || infoType != fieldType)
                        {
                            matched = false;
                            break;
                        }
                    }

                    if(matched)
                    {
                        List<object> values = new List<object>();
                        for (int j = 0; j < fields.Length; j++)
                        {
                            var info = infos[j];
                            var field = fields[j];

                            if (info.ParameterType == typeof(bool))
                                values.Add(bool.Parse(field.attrValue.vbool.Text));
                            else if (info.ParameterType == typeof(byte))
                                values.Add((byte)ulong.Parse(field.attrValue.vint.Text));
                            else if (info.ParameterType == typeof(sbyte))
                                values.Add((sbyte)long.Parse(field.attrValue.vint.Text));
                            else if (info.ParameterType == typeof(short))
                                values.Add((short)long.Parse(field.attrValue.vint.Text));
                            else if (info.ParameterType == typeof(int))
                                values.Add((int)long.Parse(field.attrValue.vint.Text));
                            else if (info.ParameterType == typeof(long))
                                values.Add(long.Parse(field.attrValue.vint.Text));
                            else if (info.ParameterType == typeof(ushort))
                                values.Add((ushort)ulong.Parse(field.attrValue.vint.Text));
                            else if (info.ParameterType == typeof(uint))
                                values.Add((uint)ulong.Parse(field.attrValue.vint.Text));
                            else if (info.ParameterType == typeof(ulong))
                                values.Add(ulong.Parse(field.attrValue.vint.Text));
                            else if (info.ParameterType == typeof(decimal))
                                values.Add(decimal.Parse(field.attrValue.vint.Text));
                            else if (info.ParameterType == typeof(float))
                                values.Add(float.Parse(field.attrValue.vfloat.Text));
                            else if (info.ParameterType == typeof(double))
                                values.Add(double.Parse(field.attrValue.vfloat.Text));
                            else if (info.ParameterType == typeof(string))
                                values.Add(field.attrValue.vstr != null ? field.attrValue.vstr.Text.Trim('"') : field.attrValue.vid.Text);
                        }
                        attributes.Add((Attribute)constructor.Invoke(values.ToArray()));
                        return;
                    }
                }

                //提示错误
                if (constructors.Length > 0)
                {
                    constructorIndex = constructorIndex == -1 ? 0 : constructorIndex;

                    var errorInfo = new List<string>();
                    var constructor = constructors[constructorIndex != -1 ? constructorIndex : 0];
                    foreach(var info in constructor.GetParameters())
                    {
                        errorInfo.Add(info.Name + " : " + info.ParameterType.Name);
                    }
                    ReportError(string.Format("格式错误, 应该是 [ {0}{1} ]。", attributeType.Name, errorInfo.Count > 0 ? " ( " + string.Join(" , ", errorInfo) + " )" : ""), context);
                }
            }




            private Attribute HandleXLS(AttrContext item, object owner, AttributeTable attributes)
            {
                if (!(owner is Table))
                    ReportError("XLS标记只能应用到table", item);
                
                if (attributes.GetAttributes<XLS>().Length > 0)
                    ReportError("XLS不能多次应用到同一对象。", item.key);

                var fields = item.attrField();

                string filePath = null;
                string sheetName = null;
                int titleRow = 0;
                int dataBeginRow = 1;

                //文件路径
                if(fields.Length>0)
                {
                    var field = fields[0];
                    if (field.attrName != null || field.attrValue == null || field.attrValue.vstr == null)
                        ReportError("第一个参数必须是字符串表示的文件路径", field);
                    else
                    {
                        var path = field.attrValue.vstr.Text.Trim('"');
                        if (!File.Exists(path))
                            ReportError(path + "不是一个有效的文件", field.attrValue.vstr);
                        else
                            filePath = path;
                    }
                }
                else
                    ReportError("需要指定文件路径", item);

                //表名
                if (fields.Length > 1)
                {
                    var field = fields[1];
                    if (field.attrName != null || field.attrValue == null || field.attrValue.vstr == null)
                        ReportError("第二个参数必须是字符串表示的表名", field);
                    else if(string.IsNullOrEmpty(field.attrValue.vstr.Text.Trim('"')))
                        ReportError("表名不能为空", field.attrValue.vstr);
                    else
                        sheetName = field.attrValue.vstr.Text.Trim('"');
                }
                else
                    ReportError("需要指定表名", item);

                //标题行
                if(fields.Length>2)
                {
                    var field = fields[2];
                    if (field.attrName != null || field.attrValue == null || field.attrValue.vint == null)
                        ReportError("第二个参数必须是正整数指定的标题行的行号", field);
                    else
                    {
                        var newRow = titleRow;
                        if (int.TryParse(field.attrValue.vint.Text, out newRow))
                        {
                            if (newRow <= 0)
                                ReportError("第二个参数必须是正整数指定的标题行的行号", field.attrValue.vint);
                            else
                                titleRow = newRow;
                        }
                        else
                            ReportError("第二个参数必须是正整数指定的标题行的行号", field.attrValue.vint);
                    }   
                }

                //数据起始行
                if (fields.Length > 3)
                {
                    var field = fields[3];
                    if (field.attrName != null || field.attrValue == null || field.attrValue.vint == null)
                        ReportError("第三个参数必须是正整数指定的数据起始行的行号", field);
                    else
                    {
                        var newRow = dataBeginRow;
                        if (int.TryParse(field.attrValue.vint.Text, out newRow))
                        {
                            if (newRow <= 0)
                                ReportError("第三个参数必须是正整数指定的数据起始行的行号", field);
                            else if (newRow <= titleRow)
                                ReportError("数据起始行必须大于标题行", field.attrValue.vint);
                            else
                                dataBeginRow = newRow;
                        }
                        else
                            ReportError("第三个参数必须是正整数指定的数据起始行的行号", field.attrValue.vint);
                    }
                }

                //后续值
                for (var i = 4; i < fields.Length; i++) { ReportWarning("无效的参数", fields[i]); }

                //返回值
                if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(sheetName))
                    return new XLS(filePath, sheetName, titleRow, dataBeginRow);
                else
                    return null;
            }
            private Attribute HandleIndex(AttrContext item, object owner, AttributeTable attributes)
            {
                if (!(owner is Table))
                    ReportError("此标记只能应用到table", item);

                var fields = item.attrField();

                if (fields.Length <= 0)
                {
                    ReportError("未指定有效的索引名和索引列。", item);
                    return null;
                }

                Dictionary<string, bool> keys = new Dictionary<string, bool>();
                foreach (var key in (owner as Table).Fields) { keys.Add(key.Name, IsNativeType(key.Type)); }

                string indexName = null;
                List<string> indexFields = new List<string>();

                //索引名
                var field = fields[0];
                if(field.attrName!=null || field.attrValue==null || field.attrValue.vstr==null)
                    ReportError("第一个参数必须是字符串指定的索引名称", field);
                else
                {
                    var name = field.attrValue.vstr.Text.Trim('"');

                    var finded = false;
                    var indexList = attributes.GetAttributes<Index>();
                    foreach (var index in indexList)
                    {
                        if (index.IndexName.Equals(name))
                        {
                            finded = true;
                            break;
                        }
                    }

                    if (finded)
                        ReportError("索引名称" + name + "重复定义", field.attrValue.vstr);
                    else
                        indexName = name;
                }

                //索引列
                for (var i = 1; i < fields.Length; i++)
                {
                    field = fields[i];
                    if (field.attrName != null || field.attrValue == null || field.attrValue.vid == null)
                        ReportError("索引字段名必须是不带引号的字符串", field);
                    else
                    {
                        var indexField = field.attrValue.vid.Text;
                        if (!keys.ContainsKey(indexField))
                            ReportError(indexField + "无效，无法为不存在的字段建立索引。", field.attrValue.vid);
                        else if (!keys[indexField])
                            ReportError(indexField + "无效，不支持为此类型的字段建立索引。", field.attrValue.vid);
                        else if (indexFields.Contains(indexField))
                            ReportError(indexField + "已经是存在，重复指定。", field.attrValue.vid);
                        else
                            indexFields.Add(indexField);
                    }
                }

                if (indexFields.Count <= 0)
                    ReportError("有效的索引列数量为0，无法建立索引。", item);
                else if (!string.IsNullOrEmpty(indexName))
                    return new Index(indexName, indexFields.ToArray());

                return null;
            }
            private Attribute HandleNullable(AttrContext item, object owner, AttributeTable attributes)
            {
                if (!(owner is TableField))
                    ReportError("Nullable只能应用到table字段。", item.key);

                if (attributes.GetAttributes<Nullable>().Length > 0)
                    ReportError("Nullable不能多次应用到同一对象。", item.key);

                var fields = item.attrField();
                if (fields.Length > 0)
                {
                    var field = fields[0];
                    if (field.attrName != null || field.attrValue == null || field.attrValue.vbool == null)
                        ReportError("第一个参数必须是一个有效的布尔值。", field);
                    else
                    {
                        var value = true;
                        if (bool.TryParse(field.attrValue.vbool.Text, out value))
                            return new Nullable(value);
                        else
                            ReportError("第一个参数不是一个有效的布尔值。", field.attrValue.vbool);
                    }

                    for(var i=1;i<fields.Length;i++)
                    {
                        ReportError("多余的参数。", fields[i]);
                    }
                    return null;
                }
                else
                    return new Nullable(true);
            }
            private Attribute HandleUnique(AttrContext item, object owner, AttributeTable attributes)
            {
                if (!(owner is TableField))
                    ReportError("Unique只能应用到table字段。", item.key);

                if (attributes.GetAttributes<Unique>().Length > 0)
                    ReportError("Unique不能多次应用到同一对象。", item.key);

                var fields = item.attrField();
                if (fields.Length > 0)
                {
                    for (var i = 1; i < fields.Length; i++)
                    {
                        ReportError("多余的参数。", fields[i]);
                    }
                    return null;
                }
                else
                    return new Unique();
            }

            private Attribute HandleArrayLiteral(AttrContext item, object owner, AttributeTable attributes)
            {
                if (!(owner is TableField))
                    ReportError("ArrayLiteral只能应用到table字段。", item.key);

                if(!(owner as TableField).IsArray)
                    ReportError("ArrayLiteral只能应用到table字段只能应用到数组字段。", item.key);

                if (attributes.GetAttributes<ArrayLiteral>().Length > 0)
                    ReportError("ArrayLiteral只能应用到table字段不能多次应用到同一对象。", item.key);
                if (attributes.GetAttributes<StructLiteral>().Length > 0)
                    ReportError("ArrayLiteral不能和StructLiteral应用到同一个对象。", item.key);
                if (attributes.GetAttributes<JsonLiteral>().Length > 0)
                    ReportError("ArrayLiteral不能和JsonLiteral应用到同一个对象。", item.key);


                var beginning = "";
                var separator = ",";
                var ending = "";

                var fields = item.attrField();
                if (fields.Length > 0)
                {
                    if (fields.Length != 3)
                        ReportError("ArrayLiteral的参数项不对, 格式必须为 [ArrayLiteral(\"beginning\",\"separator\",\"ending\")]", item.key);
                    else
                    {
                        var field = fields[0];
                        if (field.attrName != null || field.attrValue == null || field.attrValue.vstr == null)
                            ReportError("第一个参数必须是一个有效的字符串。", field);
                        else
                            beginning = field.attrValue.vstr.Text.Trim('"');

                        field = fields[1];
                        if (field.attrName != null || field.attrValue == null || field.attrValue.vstr == null)
                            ReportError("第二个参数必须是一个有效的字符串。", field);
                        else
                            separator = field.attrValue.vstr.Text.Trim('"');

                        field = fields[2];
                        if (field.attrName != null || field.attrValue == null || field.attrValue.vstr == null)
                            ReportError("第三个参数必须是一个有效的字符串。", field);
                        else
                            ending = field.attrValue.vstr.Text.Trim('"');

                        for (var i = 3; i < fields.Length; i++) { ReportError("多余的参数。", fields[i]); }
                    }
                }

                return new ArrayLiteral(beginning,separator,ending);
            }
            private Attribute HandleStructLiteral(AttrContext item, object owner, AttributeTable attributes)
            {
                if(!(owner is Struct) && !(owner is StructField) && !(owner is TableField))
                    ReportError("StructLiteral只能应用到 Struct、StructField、TableField。", item.key);

                if ((owner is StructField) && !((owner as StructField).TypeDefined is Struct))
                    ReportError("StructLiteral不能应用到类型不是Struct的StructField", item.key);

                if((owner is TableField) && !((owner as TableField).TypeDefined is Struct))
                    ReportError("StructLiteral不能应用到类型不是Struct的TableField", item.key);

                if (attributes.GetAttributes<StructLiteral>().Length > 0)
                    ReportError("StructLiteral只能应用到table字段不能多次应用到同一对象。", item.key);
                if (attributes.GetAttributes<ArrayLiteral>().Length > 0)
                    ReportError("StructLiteral不能和ArrayLiteral应用到同一个对象。", item.key);
                if (attributes.GetAttributes<JsonLiteral>().Length > 0)
                    ReportError("StructLiteral不能和JsonLiteral应用到同一个对象。", item.key);


                var beginning = "";
                var separator = ",";
                var ending = "";

                var fields = item.attrField();
                if (fields.Length > 0)
                {
                    if (fields.Length < 3)
                        ReportError("StructLiteral的参数项不对, 格式必须为 [StructLiteral(\"beginning\",\"separator\",\"ending\")]", item.key);
                    else
                    {
                        var field = fields[0];
                        if (field.attrName != null || field.attrValue == null || field.attrValue.vstr == null)
                            ReportError("第一个参数必须是一个有效的字符串。", field);
                        else
                            beginning = field.attrValue.vstr.Text.Trim('"');

                        field = fields[1];
                        if (field.attrName != null || field.attrValue == null || field.attrValue.vstr == null)
                            ReportError("第二个参数必须是一个有效的字符串。", field);
                        else
                            separator = field.attrValue.vstr.Text.Trim('"');

                        field = fields[2];
                        if (field.attrName != null || field.attrValue == null || field.attrValue.vstr == null)
                            ReportError("第三个参数必须是一个有效的字符串。", field);
                        else
                            ending = field.attrValue.vstr.Text.Trim('"');

                        for (var i = 3; i < fields.Length; i++) { ReportError("多余的参数。", fields[i]); }
                    }
                }

                return new StructLiteral(beginning, separator, ending);
            }
            private Attribute HandleJsonFile(AttrContext item, object owner, AttributeTable attributes)
            {
                if (!(owner is Table))
                    ReportError("JsonFIle标记只能应用到table", item);

                if (attributes.GetAttributes<JsonFile>().Length > 0)
                    ReportError("JsonFile不能多次应用到同一对象。", item.key);

                var fields = item.attrField();

                string filePath = null;

                //文件路径
                if (fields.Length > 0)
                {
                    var field = fields[0];
                    if (field.attrName != null || field.attrValue == null || field.attrValue.vstr == null)
                        ReportError("第一个参数必须是字符串表示的文件路径", field);
                    else
                    {
                        var path = field.attrValue.vstr.Text.Trim('"');
                        if (!File.Exists(path))
                            ReportError(path + "不是一个有效的文件", field.attrValue.vstr);
                        else
                            filePath = path;
                    }
                }
                else
                    ReportError("需要指定文件路径", item);
                
                if (!string.IsNullOrEmpty(filePath))
                    return new JsonFile(filePath);
                else
                    return null;
            }
            private Attribute HandleJsonPath(AttrContext item, object owner, AttributeTable attributes)
            {
                if (attributes.GetAttributes<JsonPath>().Length > 0)
                    ReportError("JsonPath不能多次应用到同一对象。", item.key);

                var fields = item.attrField();

                string filePath = null;

                //文件路径
                if (fields.Length > 0)
                {
                    var field = fields[0];
                    if (field.attrName != null || field.attrValue == null || field.attrValue.vstr == null)
                        ReportError("第一个参数必须是字符串表示的JsonPath", field);
                    else
                        filePath = field.attrValue.vstr.Text.Trim('"');
                }
                else
                    ReportError("需要指定JsonPath", item);

                if (!string.IsNullOrEmpty(filePath))
                    return new JsonPath(filePath);
                else
                    return null;
            }
            private Attribute HandleJsonLiteral(AttrContext item, object owner, AttributeTable attributes)
            {
                if (!(owner is Table) && !(owner is Struct) && !(owner is TableField) && !(owner is StructField))
                    ReportError("JsonLiteral只能应用到Table,Struct,TableField,StructField。", item.key);
                if (attributes.GetAttributes<JsonLiteral>().Length > 0)
                    ReportError("JsonLiteral不能重复应用到同一个对象。", item.key);
                if (attributes.GetAttributes<ArrayLiteral>().Length > 0)
                    ReportError("JsonLiteral不能和ArrayLiteral应用到同一个对象。", item.key);
                if (attributes.GetAttributes<StructLiteral>().Length > 0)
                    ReportError("JsonLiteral不能和StructLiteral应用到同一个对象。", item.key);

                var jsonPath = "";
                var fields = item.attrField();
                if (fields.Length > 0)
                {
                    var field = fields[0];
                    if (field.attrName != null || field.attrValue == null || field.attrValue.vstr == null)
                        ReportError("第一个参数必须是一个有效的字符串。", field);
                    else
                        jsonPath = field.attrValue.vstr.Text.Trim('"');

                    for (var i = 1; i < fields.Length; i++) { ReportError("多余的参数。", fields[i]); }
                }

                return new JsonLiteral();
            }
            private Attribute HandlJsonFileRef(AttrContext item, object owner, AttributeTable attributes)
            {
                if (!(owner is TableField))
                    ReportError("JsonFile标记只能应用到table", item);

                if (attributes.GetAttributes<JsonFileRef>().Length > 0)
                    ReportError("JsonFileRef不能多次应用到同一对象。", item.key);

                var fields = item.attrField();

                string filePath = null;

                //文件路径
                if (fields.Length > 0)
                {
                    var field = fields[0];
                    if (field.attrName != null || field.attrValue == null || field.attrValue.vstr == null)
                        ReportError("第一个参数必须是字符串表示的文件路径", field);
                    else
                        filePath = field.attrValue.vstr.Text.Trim('"');
                }
                else
                    ReportError("需要指定文件路径", item);

                if (!string.IsNullOrEmpty(filePath))
                    return new JsonFileRef(filePath);
                else
                    return null;
            }

            #endregion

            #region 处理内容注释

            private Dictionary<int, string> InitComments(FlatbufferLexer lexer)
            {
                var commentTable = new Dictionary<int, string>();

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

                return commentTable;
            }

            private string GetComment(Dictionary<int, string> commentTable, ParserRuleContext context, IToken token)
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
            #endregion

            #region 工具函数

            private void ReportError(string text, IToken token)
            {
                ReportError(text, token.Line, token.Column, token.StartIndex, token.StopIndex - token.StartIndex + 1);
            }
            private void ReportError(string text, ParserRuleContext context)
            {
                ReportError(text, context.Start.Line, context.Start.Column, context.Start.StartIndex, context.Stop.StopIndex - context.Start.StartIndex + 1);
            }
            private void ReportError(string text, int line, int column, int begin, int count)
            {
                report?.Invoke(project.projectName, file.Path, text, line, column, begin, count);
            }
            private void ReportWarning(string text, IToken token)
            {
                ReportError(text, token.Line, token.Column, token.StartIndex, token.StopIndex - token.StartIndex + 1);
            }
            private void ReportWarning(string text, ParserRuleContext context)
            {
                ReportError(text, context.Start.Line, context.Start.Column, context.Start.StartIndex, context.Stop.StopIndex - context.Start.StartIndex + 1);
            }
            private void ReportWarning(string text, int line, int column, int begin, int count)
            {
                report?.Invoke(project.projectName, file.Path, text, line, column, begin, count);
            }

            private static bool IsNativeType(string type)
            {
                foreach (var t in new string[] { "bool", "byte", "ubyte", "short", "ushort", "int", "uint", "long", "ulong", "int8", "uint8", "int16", "uint16", "int32", "uint32", "int64", "uint64", "float", "float32", "double", "float64", "string" })
                {
                    if (t.Equals(type))
                        return true;
                }
                return false;
            }

            #endregion
        }
    }
}
