using Antlr4.Runtime;
using FlatBufferData.Model;
using FlatBufferData.Model.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using static FlatbufferParser;

namespace FlatBufferData.Build
{
    public class FBSProject
    {
        public delegate void ErrorReport(string projectName, string path, string text, int line, int column, int begin, int count);

        /// <summary>
        /// 项目名称
        /// </summary>
        private string projectName;
        /// <summary>
        /// 错误处理器
        /// </summary>
        private ErrorReport report;
        /// <summary>
        /// 文件列表
        /// </summary>
        private List<string> filePaths = new List<string>();
        /// <summary>
        /// 文件表
        /// </summary>
        private Dictionary<string, FBSFile> path2fbs = new Dictionary<string, FBSFile>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="fileList"></param>
        /// <param name="report"></param>
        public FBSProject(string projectName, string[] fileList, ErrorReport report)
        {
            this.projectName = projectName;
            this.filePaths = new List<string>(fileList);
            this.report = report;
        }

        /// <summary>
        /// 构建
        /// </summary>
        public FBSFile Build(string path)
        {
            return ParseDocument(Path.GetFullPath(path), File.ReadAllText(path));
        }

        /// <summary>
        /// 构建
        /// </summary>
        public FBSFile Build(string path, string text)
        {
            return ParseDocument(Path.GetFullPath(path), text);
        }

        /// <summary>
        /// 构建所有
        /// </summary>
        public void BuildAll()
        {
            foreach(var file in filePaths)
            {
                Build(file);
            }
        }

        /// <summary>
        /// 按当前文件查找包含文件
        /// </summary>
        /// <param name="currPath"></param>
        /// <param name="includePath"></param>
        /// <returns></returns>
        public FBSFile FindDocument(string currPath, string includePath)
        {
            if (!Path.IsPathRooted(includePath))
            {
                var path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(currPath), includePath));
                if (path2fbs.ContainsKey(path))
                    return path2fbs[path];

                if (File.Exists(path))
                    return Build(path);
            }
            return null;
        }

        public FBSFile ParseDocument(string path, string text)
        {
            if (path2fbs.ContainsKey(path))
                path2fbs.Remove(path);

            var fbsFile = new FBSFile(path);

            path2fbs.Add(path, fbsFile);

            var lexer = new FlatbufferLexer(new AntlrInputStream(text));
            var parser = new FlatbufferParser(new CommonTokenStream(lexer));
            var comments = InitComments(lexer);
            var data2context = new Dictionary<object, object>();
            var schema = parser.schema();

            //namespace
            foreach (var context in schema.@namespace())
            {
                if (fbsFile.NameSpace == null)
                {
                    var idents = context.IDENT();
                    if (idents.Length > 0)
                    {
                        var tokens = new string[idents.Length];
                        for (int j = 0; j < tokens.Length; j++) { tokens[j] = idents[j].GetText(); }
                        fbsFile.NameSpace = string.Join(".", tokens);
                    }
                    else
                        ReportError(fbsFile, "错误的 namespace 声明!", context);
                }
                else
                    ReportError(fbsFile, "namespace 重复声明!", context);
            }
            //file_extension
            foreach (var context in schema.fileExtension())
            {
                if (fbsFile.FileExtension == null)
                {
                    if (context.val == null || context.val.StartIndex == -1)
                        ReportError(fbsFile, "错误的file_extension声明。", context);
                    else if (string.IsNullOrEmpty(context.val.Text.Trim('"')))
                        ReportError(fbsFile, "file_extension声明的扩展名不能为空。", context.val);
                    else
                        fbsFile.FileExtension = context.val.Text.Trim('"');
                }
                else
                    ReportError(fbsFile, "file_extension 重复声明。", context);
            }
            //file_identifier
            foreach (var context in schema.fileIdentifier())
            {
                if (fbsFile.FileIdentifier == null)
                {
                    if (context.val == null || context.val.StartIndex == -1)
                        ReportError(fbsFile, "错误的file_identifier声明。", context);
                    else if (string.IsNullOrEmpty(context.val.Text.Trim('"')))
                        ReportError(fbsFile, "错误的file_identifier声明声明的标识符不能为空。", context.val);
                    else
                        fbsFile.FileIdentifier = context.val.Text.Trim('"');
                }
                else
                    ReportError(fbsFile, "错误的file_identifier声明 重复声明。", context);
            }
            //table
            foreach (var context in schema.table())
            {
                var data = new Table();
                data.Comment = GetComment(comments, context, context.name);
                data.Name = context.name != null ? context.name.Text : null;
                data.Metas = ParseMetaDatas(fbsFile, context.metaList);
                data.Fields = new List<TableField>();

                var fieldNameSet = new HashSet<string>();
                foreach (var fieldContext in context.tableField())
                {
                    var field = new TableField();
                    field.Comment = GetComment(comments, fieldContext, fieldContext.fieldName);
                    field.Name = fieldContext.fieldName != null && fieldContext.fieldName.StartIndex != -1 ? fieldContext.fieldName.Text : null;
                    field.Type = fieldContext.fieldType != null ? fieldContext.fieldType.GetText() : (fieldContext.arrayType != null ? fieldContext.arrayType.type.GetText() : null);
                    field.IsArray = fieldContext.arrayType != null && fieldContext.fieldType == null;
                    field.DefaultValue = ParseDefaultValue(fbsFile, field.Type, field.IsArray, fieldContext.fieldValue);
                    field.Metas = ParseMetaDatas(fbsFile, fieldContext.metaList);
                    field.DataField = fieldContext.fieldMap != null && fieldContext.fieldMap.StartIndex != -1 ? fieldContext.fieldMap.Text.Trim('"') : field.Name;

                    if (string.IsNullOrEmpty(field.Name))
                    {
                        ReportError(fbsFile, "名称不能为空。", fieldContext);
                        continue;
                    }
                    else if (fieldNameSet.Contains(field.Name))
                    {
                        ReportError(fbsFile, "重复的名称。", fieldContext.fieldName);
                        continue;
                    }
                    else if (string.IsNullOrEmpty(field.Type))
                    {
                        ReportError(fbsFile, "类型不能为空。", fieldContext);
                        continue;
                    }

                    fieldNameSet.Add(field.Name);

                    data.Fields.Add(field);
                    data2context.Add(field, fieldContext);
                }

                fbsFile.Tables.Add(data);
                data2context.Add(data, context);
            }
            //struct
            foreach (var context in schema.@struct())
            {
                var data = new Struct();
                data.Comment = GetComment(comments, context, context.name);
                data.Name = context.name != null ? context.name.Text : null;
                data.Metas = ParseMetaDatas(fbsFile, context.metaList);
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
                    field.Metas = ParseMetaDatas(fbsFile, fieldContext.metaList);
                    field.DataField = fieldContext.fieldMap != null && fieldContext.fieldMap.StartIndex != -1 ? fieldContext.fieldMap.Text.Trim('"') : field.Name;

                    if (string.IsNullOrEmpty(field.Name))
                    {
                        ReportError(fbsFile, "名称不能为空。", fieldContext);
                        continue;
                    }
                    else if (fieldNameSet.Contains(field.Name))
                    {
                        ReportError(fbsFile, "重复的名称。", fieldContext.fieldName);
                        continue;
                    }
                    else if (string.IsNullOrEmpty(field.Type))
                    {
                        ReportError(fbsFile, "类型不能为空", fieldContext);
                        continue;
                    }
                    else if (field.IsArray)
                    {
                        ReportError(fbsFile, "struct字段的类型不能是数组。", fieldContext.arrayType != null ? fieldContext.arrayType as ParserRuleContext : fieldContext.fieldType as ParserRuleContext);
                        continue;
                    }

                    if (fieldContext.fieldValue != null)
                        ReportError(fbsFile, "struct字段目前不支持默认值。", fieldContext.fieldValue);

                    fieldNameSet.Add(field.Name);

                    data.Fields.Add(field);
                    data2context.Add(field, fieldContext);
                }

                fbsFile.Structs.Add(data);
                data2context.Add(data, context);
            }
            //enum
            foreach (var context in schema.@enum())
            {
                var data = new Enum();
                data.Comment = GetComment(comments, context, context.name);
                data.Name = context.name != null ? context.name.Text : null;
                data.Metas = ParseMetaDatas(fbsFile, context.metaList);
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
                        ReportError(fbsFile, "名称不能为空。", fieldContext);
                        continue;
                    }
                    else if (fieldIDSet.Contains(field.ID))
                    {
                        ReportError(fbsFile, "重复的ID。", fieldContext.fieldValue);
                        continue;
                    }
                    else if (fieldNameSet.Contains(field.Name))
                    {
                        ReportError(fbsFile, "重复的名称。", fieldContext.fieldName);
                        continue;
                    }

                    if (enumIDValidate)
                        fieldIDSet.Add(field.ID);

                    fieldNameSet.Add(field.Name);

                    data.Fields.Add(field);
                    data2context.Add(field, fieldContext);
                }

                if (context.baseType != null)
                {
                    var name = context.baseType.GetText();
                    if(BaseUtil.IsInteger(name))
                        data.BaseType = name;
                    else
                        ReportError(fbsFile, "enum的基类型必须是整数类型.", context.baseType);
                }

                var autoID = 0;
                var autoedIDs = new HashSet<int>();
                foreach (var field in data.Fields)
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

                fbsFile.Enums.Add(data);
                data2context.Add(data, context);
            }
            //union
            foreach (var context in schema.union())
            {
                var data = new Union();
                data.Comment = GetComment(comments, context, context.name);
                data.Name = context.name != null ? context.name.Text : null;
                data.Metas = ParseMetaDatas(fbsFile, context.metaList);

                var fieldNameSet = new HashSet<string>();
                foreach (var fieldContext in context.unionField())
                {
                    var field = new UnionField();
                    field.Name = fieldContext.fieldName != null && fieldContext.fieldName.StartIndex != -1 ? fieldContext.fieldName.Text : null;
                    field.Type = fieldContext.fieldType != null ? fieldContext.fieldType.GetText() : null;
                    field.Comment = GetComment(comments, fieldContext, fieldContext.fieldName);

                    if (string.IsNullOrEmpty(field.Name))
                    {
                        ReportError(fbsFile, "名称不能为空。", fieldContext);
                        continue;
                    }
                    else if (fieldNameSet.Contains(field.Name))
                    {
                        ReportError(fbsFile, "重复的名称。", fieldContext.fieldName);
                        continue;
                    }
                    else if (string.IsNullOrEmpty(field.Type))
                    {
                        ReportError(fbsFile, "类型不能为空", fieldContext);
                        continue;
                    }

                    fieldNameSet.Add(field.Name);

                    data.Fields.Add(field);
                    data2context.Add(field, fieldContext);
                }

                fbsFile.Unions.Add(data);
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
                    field.Metas = ParseMetaDatas(fbsFile, fieldContext.metaList);

                    if (string.IsNullOrEmpty(field.Name))
                    {
                        ReportError(fbsFile, "名称不能为空。", fieldContext);
                        continue;
                    }
                    else if (fieldNameSet.Contains(field.Name))
                    {
                        ReportError(fbsFile, "重复的名称。", fieldContext.fieldName);
                        continue;
                    }

                    fieldNameSet.Add(field.Name);

                    data.Fields.Add(field);
                    data2context.Add(field, fieldContext);
                }

                fbsFile.Rpcs.Add(data);
                data2context.Add(data, context);
            }
            //root_type
            foreach (var context in schema.rootType())
            {
                if (fbsFile.RootTable == null)
                {
                    if (context.val == null || context.val.StartIndex == -1)
                        ReportError(fbsFile, "错误的 root_type 声明。", context);
                    else
                    {
                        var tabName = context.val.Text;
                        if (string.IsNullOrEmpty(tabName))
                            ReportError(fbsFile, "错误的 root_type 声明。", context.val);
                        else
                        {
                            foreach (var item in fbsFile.Tables) { if (item.Name.Equals(tabName)) { fbsFile.RootTable = item; break; } }
                            if (fbsFile.RootTable == null)
                                ReportError(fbsFile, string.Format("主表定义 ({0}) 未找到!", context.val.Text), context.val);
                        }
                    }
                }
                else
                    ReportError(fbsFile, "root_type 重复声明!", context);
            }
            //include
            var includeFBSs = new List<FBSFile>();
            foreach (var context in schema.include())
            {
                if (context.val == null || context.val.StartIndex == -1)
                {
                    ReportError(fbsFile, "无效的include。", context);
                    continue;
                }
                var url = context.val.Text.Trim('"');
                if (string.IsNullOrEmpty(url))
                {
                    ReportError(fbsFile, "无效的include。", context.val);
                    continue;
                }
                var fbs = FindDocument(fbsFile.Path, url);
                if (fbs == null)
                {
                    ReportError(fbsFile, string.Format("{0} 未找到。", url), context.val);
                    continue;
                }

                includeFBSs.Add(fbs);
            }
            fbsFile.Includes = includeFBSs.ToArray();

            //check
            CheckDefined(fbsFile, data2context);

            //attributes
            ParseAttributes(fbsFile, data2context);

            //clear
            data2context.Clear();

            return fbsFile;
        }

        /// <summary>
        /// 解析默认值
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="isArray"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private object ParseDefaultValue(FBSFile fbs, string fieldType, bool isArray, ScalarValueContext context)
        {
            if (context == null) { return null; }

            var fieldValue = context.GetText();

            if (string.IsNullOrEmpty(fieldValue)) { return null; }

            if (isArray)
            {
                ReportError(fbs, "列表类型不支持默认值。", context);
                return null;
            }
            else if (fieldType.Equals("bool"))
            {
                var boolValue = false;
                if (bool.TryParse(fieldValue, out boolValue))
                    return boolValue;
                else
                    ReportError(fbs, string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
            }
            else if (fieldType.Equals("byte") || fieldType.Equals("int8"))
            {
                sbyte byteValue = 0;
                if (sbyte.TryParse(fieldValue, out byteValue))
                    return byteValue;
                else
                    ReportError(fbs, string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
            }
            else if (fieldType.Equals("ubyte") || fieldType.Equals("uint8"))
            {
                byte byteValue = 0;
                if (byte.TryParse(fieldValue, out byteValue))
                    return byteValue;
                else
                    ReportError(fbs, string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
            }
            else if (fieldType.Equals("short") || fieldType.Equals("int16"))
            {
                short byteValue = 0;
                if (short.TryParse(fieldValue, out byteValue))
                    return byteValue;
                else
                    ReportError(fbs, string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
            }
            else if (fieldType.Equals("ushort") || fieldType.Equals("uint16"))
            {
                ushort byteValue = 0;
                if (ushort.TryParse(fieldValue, out byteValue))
                    return byteValue;
                else
                    ReportError(fbs, string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
            }
            else if (fieldType.Equals("int") || fieldType.Equals("int32"))
            {
                int byteValue = 0;
                if (int.TryParse(fieldValue, out byteValue))
                    return byteValue;
                else
                    ReportError(fbs, string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
            }
            else if (fieldType.Equals("uint") || fieldType.Equals("uint32"))
            {
                uint byteValue = 0;
                if (uint.TryParse(fieldValue, out byteValue))
                    return byteValue;
                else
                    ReportError(fbs, string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
            }
            else if (fieldType.Equals("long") || fieldType.Equals("int64"))
            {
                long byteValue = 0;
                if (long.TryParse(fieldValue, out byteValue))
                    return byteValue;
                else
                    ReportError(fbs, string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
            }
            else if (fieldType.Equals("ulong") || fieldType.Equals("uint64"))
            {
                ulong byteValue = 0;
                if (ulong.TryParse(fieldValue, out byteValue))
                    return byteValue;
                else
                    ReportError(fbs, string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
            }
            else if (fieldType.Equals("float") || fieldType.Equals("float32"))
            {
                float byteValue = 0;
                if (float.TryParse(fieldValue, out byteValue))
                    return byteValue;
                else
                    ReportError(fbs, string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
            }
            else if (fieldType.Equals("double") || fieldType.Equals("float64"))
            {
                double byteValue = 0;
                if (double.TryParse(fieldValue, out byteValue))
                    return byteValue;
                else
                    ReportError(fbs, string.Format("默认值 {0} 不是一个有效的 {1}。", fieldValue, fieldType), context);
            }
            else
            {
                ReportError(fbs, string.Format("目前 {0} 不支持默认值。", fieldType), context);
            }
            return null;
        }

        /// <summary>
        /// 解析元数据
        /// </summary>
        /// <param name="metaList"></param>
        /// <returns></returns>
        private List<Meta> ParseMetaDatas(FBSFile fbs, FlatbufferParser.MetadataContext metaList)
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
                        ReportError(fbs, "元数据键不能为空", meta);
                        continue;
                    }
                    fieldMetas.Add(new Meta(metaName, metaValue));
                }
            }
            return fieldMetas;
        }


        #region 检查所有定义

        private void CheckDefined(FBSFile fbs, Dictionary<object, object> data2context)
        {
            var allDefined = new Dictionary<string, List<object>>();

            //查找能访问到的的所有定义
            var flaged = new HashSet<FBSFile>();
            var queue = new List<FBSFile>();
            queue.Add(fbs);
            while (queue.Count > 0)
            {
                FBSFile doc = queue[0];

                queue.RemoveAt(0);

                var docNS = !string.IsNullOrEmpty(doc.NameSpace) ? doc.NameSpace + "." : "";
                foreach (var data in doc.Tables)
                {
                    if (string.IsNullOrEmpty(data.Name)) continue;
                    var key = docNS + data.Name;
                    if (!allDefined.ContainsKey(key)) allDefined.Add(key, new List<object>());
                    allDefined[key].Add(data);
                }
                foreach (var data in doc.Structs)
                {
                    if (string.IsNullOrEmpty(data.Name)) continue;
                    var key = docNS + data.Name;
                    if (!allDefined.ContainsKey(key)) allDefined.Add(key, new List<object>());
                    allDefined[key].Add(data);
                }
                foreach (var data in doc.Enums)
                {
                    if (string.IsNullOrEmpty(data.Name)) continue;
                    var key = docNS + data.Name;
                    if (!allDefined.ContainsKey(key)) allDefined.Add(key, new List<object>());
                    allDefined[key].Add(data);
                }
                foreach (var data in doc.Unions)
                {
                    if (string.IsNullOrEmpty(data.Name)) continue;
                    var key = docNS + data.Name;
                    if (!allDefined.ContainsKey(key)) allDefined.Add(key, new List<object>());
                    allDefined[key].Add(data);
                }
                foreach (var data in doc.Rpcs)
                {
                    if (string.IsNullOrEmpty(data.Name)) continue;
                    var key = docNS + data.Name;
                    if (!allDefined.ContainsKey(key)) allDefined.Add(key, new List<object>());
                    allDefined[key].Add(data);
                }

                flaged.Add(doc);

                foreach (var include in doc.Includes)
                {
                    if (!flaged.Contains(include))
                        queue.Add(include);
                }
            }

            //确定名称空间
            var ns = !string.IsNullOrEmpty(fbs.NameSpace) ? fbs.NameSpace + "." : "";

            //检查 Table
            foreach (var data in fbs.Tables)
            {
                //table name
                if (string.IsNullOrEmpty(data.Name)) continue;
                var key = ns + data.Name;
                if (allDefined.ContainsKey(key) && allDefined[key].Count > 1)
                    ReportError(fbs, "名称重复定义。", (data2context[data] as TableContext).name);

                //table fields
                foreach (var field in data.Fields)
                {
                    if (string.IsNullOrEmpty(field.Type) || BaseUtil.IsBaseType(field.Type)) continue;
                    var typeName = field.Type.IndexOf(".") != -1 ? field.Type : ns + field.Type;
                    var context = data2context[field] as TableFieldContext;
                    var locator = context.arrayType != null ? context.arrayType as ParserRuleContext : context.fieldType as ParserRuleContext;
                    if (allDefined.ContainsKey(typeName))
                    {
                        if (allDefined[typeName].Count > 1)
                            ReportError(fbs, string.Format("找到多个名称为 {0} 的定义。", field.Type), locator);
                        else if (allDefined[typeName].Count == 1)
                        {
                            var first = allDefined[typeName][0];
                            if (first is Table || first is Struct || first is Enum || first is Union)
                                field.TypeDefined = first;
                            else
                                ReportError(fbs, "table字段的类型不能是rpc。", locator);
                        }
                    }
                    else
                        ReportError(fbs, string.Format("没有找到 {0} 的定义。", field.Type), locator);
                }
            }
            //检查 Struct
            foreach (var data in fbs.Structs)
            {
                //Struct 名称
                if (string.IsNullOrEmpty(data.Name)) continue;
                var key = ns + data.Name;
                if (allDefined.ContainsKey(key) && allDefined[key].Count > 1)
                    ReportError(fbs, "名称重复定义。", (data2context[data] as StructContext).name);

                //Struct 字段
                foreach (var field in data.Fields)
                {
                    var context = data2context[field] as StructFieldContext;
                    var locator = context.arrayType != null ? context.arrayType as ParserRuleContext : context.fieldType as ParserRuleContext;

                    if ("string".Equals(field.Type))
                        ReportError(fbs, "struct字段的类型只能是bool, number, enum, struct。", locator);
                    else if (!BaseUtil.IsBaseType(field.Type))
                    {
                        var typeName = field.Type.IndexOf(".") != -1 ? field.Type : ns + field.Type;
                        if (allDefined.ContainsKey(typeName))
                        {
                            if (allDefined[typeName].Count > 1)
                                ReportError(fbs, string.Format("找到多个名称为 {0} 的定义。", field.Type), locator);
                            else if (allDefined[typeName].Count == 1)
                            {
                                var first = allDefined[typeName][0];
                                if (first is Struct || first is Enum)
                                    field.TypeDefined = first;
                                else
                                    ReportError(fbs, "struct字段的类型只能是bool, number, enum, struct。", locator);
                            }
                        }
                        else
                            ReportError(fbs, string.Format("没有找到 {0} 的定义。", field.Type), locator);
                    }
                }
            }
            //检查 Enum
            foreach (var data in fbs.Enums)
            {
                //Enum 名称
                if (string.IsNullOrEmpty(data.Name)) continue;
                var key = ns + data.Name;
                if (allDefined.ContainsKey(key) && allDefined[key].Count > 1)
                    ReportError(fbs, "名称重复定义。", (data2context[data] as EnumContext).name);

                //Enum 字段
                //null
            }
            //检查 Union
            foreach (var data in fbs.Unions)
            {
                //Union 名称
                if (string.IsNullOrEmpty(data.Name)) continue;
                var key = ns + data.Name;
                if (allDefined.ContainsKey(key) && allDefined[key].Count > 1)
                    ReportError(fbs, "名称重复定义。", (data2context[data] as UnionContext).name);

                //Union 字段
                foreach (var field in data.Fields)
                {
                    if ("string".Equals(field.Type))
                        continue;

                    var context = data2context[field] as UnionFieldContext;
                    var locator = context.fieldType as ParserRuleContext;
                    if (BaseUtil.IsBaseType(field.Type))
                    {
                        ReportError(fbs, "union字段的类型只能是string, struct, table。", locator);
                    }
                    else
                    {
                        var typeName = field.Type.IndexOf(".") != -1 ? field.Type : ns + field.Type;
                        if (allDefined.ContainsKey(typeName))
                        {
                            if (allDefined[typeName].Count > 1)
                                ReportError(fbs, string.Format("找到多个名称为 {0} 的定义。", field.Type), locator);
                            else if (allDefined[typeName].Count == 1)
                            {
                                var first = allDefined[typeName][0];
                                if (first is Struct || first is Table)
                                    field.TypeDefined = first;
                                else
                                    ReportError(fbs, "union字段的类型只能是string,struct,table。", locator);
                            }
                        }
                        else
                            ReportError(fbs, string.Format("没有找到 {0} 的定义。", field.Type), locator);
                    }
                }
            }
            //检查 Rpc
            foreach (var data in fbs.Rpcs)
            {
                //Rpc 名称
                if (string.IsNullOrEmpty(data.Name)) continue;
                var key = ns + data.Name;
                if (allDefined.ContainsKey(key) && allDefined[key].Count > 1)
                    ReportError(fbs, "名称重复定义。", (data2context[data] as RpcContext).name);

                //Rpc 字段
                foreach (var field in data.Fields)
                {
                    if (!string.IsNullOrEmpty(field.Param) && !BaseUtil.IsBaseType(field.Param))
                    {
                        var typeName = field.Param.IndexOf(".") != -1 ? field.Param : ns + field.Param;
                        if (allDefined.ContainsKey(typeName))
                        {
                            if (allDefined[typeName].Count > 1)
                                ReportError(fbs, string.Format("找到多个名称为 {0} 的定义。", field.Param), (data2context[field] as RpcFieldContext).fieldParam);
                            else if (allDefined[typeName].Count == 1)
                            {
                                var first = allDefined[typeName][0];
                                if (first is Struct || first is Table || first is Enum || first is Union)
                                    field.ParamTypeDefined = first;
                                else
                                    ReportError(fbs, "rpc字段的参数类型不能是rpc", (data2context[field] as RpcFieldContext).fieldParam);
                            }
                        }
                        else
                            ReportError(fbs, string.Format("没有找到 {0} 的定义。", field.Param), (data2context[field] as RpcFieldContext).fieldParam);
                    }
                    if (!string.IsNullOrEmpty(field.Return) && !BaseUtil.IsBaseType(field.Return))
                    {
                        var typeName = field.Return.IndexOf(".") != -1 ? field.Return : ns + field.Return;
                        if (allDefined.ContainsKey(typeName))
                        {
                            if (allDefined[typeName].Count > 1)
                                ReportError(fbs, string.Format("找到多个名称为 {0} 的定义。", field.Return), (data2context[field] as RpcFieldContext).fieldReturn);
                            else if (allDefined[typeName].Count == 1)
                            {
                                var first = allDefined[typeName][0];
                                if (first is Struct || first is Table || first is Enum || first is Union)
                                    field.ReturnTypeDefined = first;
                                else
                                    ReportError(fbs, "rpc字段的返回类型不能是rpc", (data2context[field] as RpcFieldContext).fieldReturn);
                            }
                        }
                        else
                            ReportError(fbs, string.Format("没有找到 {0} 的定义。", field.Return), (data2context[field] as RpcFieldContext).fieldReturn);
                    }
                }
            }
        }

        #endregion

        #region 处理所有特性

        /// <summary>
        /// 解析特性
        /// </summary>
        private void ParseAttributes(FBSFile fbs, Dictionary<object, object> data2context)
        {
            foreach (var data in fbs.Tables)
            {
                ParseAttribute(fbs, (data2context[data] as TableContext).attr(), data, data.Attributes);
                foreach (var field in data.Fields)
                    ParseAttribute(fbs, (data2context[field] as TableFieldContext).attr(), field, field.Attributes);
            }
            foreach (var data in fbs.Structs)
            {
                ParseAttribute(fbs, (data2context[data] as StructContext).attr(), data, data.Attributes);
                foreach (var field in data.Fields)
                    ParseAttribute(fbs, (data2context[field] as StructFieldContext).attr(), field, field.Attributes);
            }
            foreach (var data in fbs.Enums)
            {
                ParseAttribute(fbs, (data2context[data] as EnumContext).attr(), data, data.Attributes);
                foreach (var field in data.Fields)
                    ParseAttribute(fbs, (data2context[field] as EnumFieldContext).attr(), field, field.Attributes);
            }
            foreach (var data in fbs.Unions)
            {
                ParseAttribute(fbs, (data2context[data] as UnionContext).attr(), data, data.Attributes);
                foreach (var field in data.Fields)
                    ParseAttribute(fbs, (data2context[field] as UnionFieldContext).attr(), field, field.Attributes);
            }
            foreach (var data in fbs.Rpcs)
            {
                ParseAttribute(fbs, (data2context[data] as RpcContext).attr(), data, data.Attributes);
                foreach (var field in data.Fields)
                    ParseAttribute(fbs, (data2context[field] as RpcFieldContext).attr(), field, field.Attributes);
            }
        }

        /// <summary>
        /// 解析特性
        /// </summary>
        private void ParseAttribute(FBSFile fbs, AttrContext[] contexts, object owner, AttributeTable attributes)
        {
            foreach (var context in contexts)
            {
                var attributeName = context.key != null ? context.key.Text : null;
                if (string.IsNullOrEmpty(attributeName))
                {
                    ReportError(fbs, "特性名称不能为空", context);
                    continue;
                }

                var attributeType = AttributeFactory.GetAttributeSchema(attributeName);
                if (attributeType == null)
                {
                    ReportError(fbs, "不支持的特性", context.key);
                    return;
                }

                bool allow = true;
                foreach (var attributeMeta in attributeType.GetCustomAttributes())
                {
                    if (attributeMeta is AllowTarget)
                        allow = OnAllowTarget(fbs, attributeType, context, owner, attributes, attributeMeta as AllowTarget) && allow;
                    else if (attributeMeta is AllowMultiple)
                        allow = OnAllowMultiple(fbs, attributeType, context, owner, attributes, attributeMeta as AllowMultiple) && allow;
                    else if (attributeMeta is ConflictFlag)
                        allow = OnConflictFlag(fbs, attributeType, context, owner, attributes, attributeMeta as ConflictFlag) && allow;
                    else if (attributeMeta is RequiredFlag)
                        allow = OnRequiredFlag(fbs, attributeType, context, owner, attributes, attributeMeta as RequiredFlag) && allow;
                    else if (attributeMeta is RequiredArrayField)
                        allow = OnRequiredArrayField(fbs, attributeType, context, owner, attributes, attributeMeta as RequiredArrayField) && allow;
                    else if (attributeMeta is RequiredFieldType)
                        allow = OnRequiredFieldType(fbs, attributeType, context, owner, attributes, attributeMeta as RequiredFieldType) && allow;
                }

                if (allow)
                    CreateAttribute(fbs, attributeType, context, attributes);
            }
        }

        /// <summary>
        /// 处理 AllowTarget
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="context"></param>
        /// <param name="owner"></param>
        /// <param name="attributes"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool OnAllowTarget(FBSFile fbs, System.Type attributeType, AttrContext context, object owner, AttributeTable attributes, AllowTarget arg)
        {
            if (arg.AllowTargets != TargetTypeID.ANY)
            {
                TargetTypeID ownerTypeID = TargetTypeID.ANY;
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

                var value = ((int)arg.AllowTargets) & ((int)ownerTypeID);
                if (value == 0)
                {
                    var infos = new List<string>();
                    foreach (TargetTypeID item in System.Enum.GetValues(typeof(TargetTypeID)))
                    {
                        if (((int)arg.AllowTargets & (int)item) > 0)
                            infos.Add(item.ToString());
                    }

                    ReportError(fbs, string.Format("{0} 只能应用到 {1}.", attributeType.Name, string.Join(",", infos)), context.key);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 处理 AllowMultiple
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="context"></param>
        /// <param name="owner"></param>
        /// <param name="attributes"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool OnAllowMultiple(FBSFile fbs, System.Type attributeType, AttrContext context, object owner, AttributeTable attributes, AllowMultiple arg)
        {
            if (arg.Allow == false && attributes.HasAttribute(attributeType))
            {
                ReportError(fbs, string.Format("{0}不能多次应用到同一个对象.", attributeType.Name), context.key);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 处理 ConflictFlag
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="context"></param>
        /// <param name="owner"></param>
        /// <param name="attributes"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool OnConflictFlag(FBSFile fbs, System.Type attributeType, AttrContext context, object owner, AttributeTable attributes, ConflictFlag arg)
        {
            foreach (var type in arg.ConflictTypes)
            {
                if (attributes.HasAttribute(type))
                {
                    ReportError(fbs, string.Format("{0} 不能和 {1} 应用到同一个对象。", attributeType.Name, type.Name), context.key);
                }
            }
            return true;
        }

        /// <summary>
        /// 处理 RequiredFlag
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="context"></param>
        /// <param name="owner"></param>
        /// <param name="attributes"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool OnRequiredFlag(FBSFile fbs, System.Type attributeType, AttrContext context, object owner, AttributeTable attributes, RequiredFlag arg)
        {
            bool finded = false;
            foreach (var type in arg.RequiredTypes)
            {
                if (attributes.HasAttribute(type))
                    finded = true;
            }
            if (!finded)
            {
                List<string> names = new List<string>();
                foreach (var type in arg.RequiredTypes)
                {
                    names.Add(type.Name);
                }

                ReportError(fbs, string.Format("{0} 需要和这些标记一起使用（{1}）。", attributeType.Name, string.Join(",", names)), context.key);
            }
            return true;
        }

        /// <summary>
        /// 处理 RequiredArrayField
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="context"></param>
        /// <param name="owner"></param>
        /// <param name="attributes"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool OnRequiredArrayField(FBSFile fbs, System.Type attributeType, AttrContext context, object owner, AttributeTable attributes, RequiredArrayField arg)
        {
            var field = owner as TableField;
            if (field == null || !field.IsArray)
            {
                ReportError(fbs, string.Format("{0} 只能应用到数组型 TableField！", attributeType.Name), context.key);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 处理 RequiredFieldType
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="context"></param>
        /// <param name="owner"></param>
        /// <param name="attributes"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool OnRequiredFieldType(FBSFile fbs, System.Type attributeType, AttrContext context, object owner, AttributeTable attributes, RequiredFieldType arg)
        {
            if (arg.FieldType != FieldTypeID.ANY)
            {
                string type = null;
                object typeDefined = null;
                var typeID = FieldTypeID.ANY;

                if (owner is TableField)
                {
                    var field = owner as TableField;
                    type = field.Type;
                    typeDefined = field.TypeDefined;
                }
                else if (owner is StructField)
                {
                    var field = owner as StructField;
                    type = field.Type;
                    typeDefined = field.TypeDefined;
                }

                if (BaseUtil.IsBool(type))
                    typeID = FieldTypeID.BOOL;
                else if (BaseUtil.IsInteger(type))
                    typeID = FieldTypeID.INT;
                else if (BaseUtil.IsFloat(type))
                    typeID = FieldTypeID.FLOAT;
                else if (BaseUtil.IsString(type))
                    typeID = FieldTypeID.STRING;
                else if (typeDefined is Model.Enum)
                    typeID = FieldTypeID.ENUM;
                else if (typeDefined is Model.Union)
                    typeID = FieldTypeID.UNION;
                else if (typeDefined is Model.Struct)
                    typeID = FieldTypeID.STRUCT;
                else if (typeDefined is Model.Table)
                    typeID = FieldTypeID.TABLE;

                if ((arg.FieldType & typeID) == 0)
                {
                    var infos = new List<string>();
                    foreach (FieldTypeID item in System.Enum.GetValues(typeof(FieldTypeID)))
                    {
                        if (((int)arg.FieldType & (int)item) > 0)
                        {
                            infos.Add(item.ToString());
                        }
                    }

                    ReportError(fbs, string.Format("{0} 只能应用到类型为 {1} 的 TableField 和 StructField ！", attributeType.Name, string.Join(",", infos)), context.key);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 创建Attribute
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="context"></param>
        /// <param name="owner"></param>
        /// <param name="attributes"></param>
        private void CreateAttribute(FBSFile fbs, System.Type attributeType, AttrContext context, AttributeTable attributes)
        {
            var fields = context.attrField();
            var constructors = attributeType.GetConstructors();

            var constructorIndex = -1;

            for (int i = 0; i < constructors.Length; i++)
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

                if (matched)
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

                    Attribute attribute = (Attribute)constructor.Invoke(values.ToArray());
                    attribute.Location = new Location() { file=fbs.Path };
                    if (context.key != null)
                    {
                        attribute.Location.row = context.key.Line;
                        attribute.Location.col = context.key.Column;
                        attribute.Location.begin = context.key.StartIndex;
                        attribute.Location.end = context.key.StopIndex;
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
                foreach (var info in constructor.GetParameters())
                {
                    errorInfo.Add(info.Name + " : " + info.ParameterType.Name);
                }
                ReportError(fbs, string.Format("格式错误, 应该是 [ {0}{1} ]。", attributeType.Name, errorInfo.Count > 0 ? " ( " + string.Join(" , ", errorInfo) + " )" : ""), context);
            }
        }

        /// <summary>
        /// 参数类型
        /// </summary>
        private enum ArgumentType { VOID, CONST, BOOL, INT, FLOAT, STRING }

        #endregion

        #region 处理内容注释

        /// <summary>
        /// 初始化注释信息
        /// </summary>
        /// <param name="lexer"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取注释
        /// </summary>
        /// <param name="commentTable"></param>
        /// <param name="context"></param>
        /// <param name="token"></param>
        /// <returns></returns>
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

        private void ReportError(FBSFile fbs, string text, IToken token)
        {
            ReportError(fbs, text, token.Line, token.Column, token.StartIndex, token.StopIndex - token.StartIndex + 1);
        }
        private void ReportError(FBSFile fbs, string text, ParserRuleContext context)
        {
            ReportError(fbs, text, context.Start.Line, context.Start.Column, context.Start.StartIndex, context.Stop.StopIndex - context.Start.StartIndex + 1);
        }
        private void ReportError(FBSFile fbs, string text, int line, int column, int begin, int count)
        {
            report?.Invoke(projectName, fbs.Path, text, line, column, begin, count);
        }
        private void ReportWarning(FBSFile fbs, string text, IToken token)
        {
            ReportError(fbs, text, token.Line, token.Column, token.StartIndex, token.StopIndex - token.StartIndex + 1);
        }
        private void ReportWarning(FBSFile fbs, string text, ParserRuleContext context)
        {
            ReportError(fbs, text, context.Start.Line, context.Start.Column, context.Start.StartIndex, context.Stop.StopIndex - context.Start.StartIndex + 1);
        }
        private void ReportWarning(FBSFile fbs, string text, int line, int column, int begin, int count)
        {
            report?.Invoke(projectName, fbs.Path, text, line, column, begin, count);
        }

        #endregion
    }
}
