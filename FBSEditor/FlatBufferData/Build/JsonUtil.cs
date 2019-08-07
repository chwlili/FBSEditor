using Antlr4.Runtime;
using FlatBufferData.Model;
using FlatBufferData.Model.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FlatBufferData.Build
{
    public delegate void ErrorReport(string path, string text, int line, int column, int begin, int count);

    public class JsonUtil
    {
        private class ReportWrap
        {
            public string path;
            public ErrorReport report;

            public ReportWrap(string path,ErrorReport report)
            {
                this.path = path;
                this.report = report;
            }

            public void OnError(string text, int row,int col,int begin,int end)
            {
                report?.Invoke(path, text, row, col, begin, end);
            }
            public void OnError(string text, Location location)
            {
                report?.Invoke(path, text, location.row, location.col, location.begin, location.end);
            }

            public void OnError(string text, ParserRuleContext rule)
            {
                OnError(text, rule.Start.Line, rule.Start.Column, rule.Start.StartIndex, rule.Stop.StopIndex);
            }

            public void OnError(string text,IToken token)
            {
                OnError(text, token.Line, token.Column, token.StartIndex, token.StopIndex);
            }

            public void OnJsonPathError(string jsonPath, JsonParser.JsonValueContext jsonValue)
            {
                OnError(string.Format("\"{0}\" 无法过滤出有效的Json节点 '{1}'", jsonPath, jsonValue.GetText()), jsonValue);
            }
        }

        public static object ParseJsonFile(string filePath, Struct type, AttributeTable attributes, ErrorReport report)
        {
            var jsonLexer = new JsonLexer(new AntlrInputStream(File.ReadAllText(filePath)));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            return ParseSingleValue(type.Name, type, attributes, jsonParser.jsonValue(), new ReportWrap(filePath, report));
        }

        public static object ParseJsonText(string filePath, string text, Struct type, AttributeTable attributes, ErrorReport report)
        {
            var jsonLexer = new JsonLexer(new AntlrInputStream(text));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            return ParseSingleValue(type.Name, type, attributes, jsonParser.jsonValue(), new ReportWrap(filePath, report));
        }

        public static object ParseJsonFile(string filePath, Table type, AttributeTable attributes, ErrorReport report)
        {
            var jsonLexer = new JsonLexer(new AntlrInputStream(File.ReadAllText(filePath)));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            return ParseSingleValue(type.Name, type, attributes, jsonParser.jsonValue(), new ReportWrap(filePath,report));
        }

        public static object ParseJsonText(string filePath, string text, Table type, AttributeTable attributes, ErrorReport report)
        {
            var jsonLexer = new JsonLexer(new AntlrInputStream(text));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            return ParseSingleValue(type.Name, type, attributes, jsonParser.jsonValue(), new ReportWrap(filePath, report));
        }

        /// <summary>
        /// 解析数组值
        /// </summary>
        private static List<object> ParseArrayValue(string type, object typeDefined, AttributeTable attributes, JsonParser.JsonValueContext jsonValue, ReportWrap report)
        {
            var jsonPath = attributes != null ? attributes.GetAttribute<JsonPath>() : null;
            var rootPath = jsonPath != null ? jsonPath.path : "";
            var rootNode = GetRootJsonNode(jsonValue, rootPath);

            if (rootNode == null)
            {
                report.OnJsonPathError(rootPath, jsonValue);
                return null;
            }

            var arrayValue = new List<object>();
            if (rootNode.arraryValue != null)
            {
                foreach (var arrayElementJson in rootNode.arraryValue._arrayElement)
                {
                    var parsedValue = ParseSingleValue(type, typeDefined, attributes, arrayElementJson, report);
                    if (parsedValue != null)
                        arrayValue.Add(parsedValue);
                }
            }
            else
            {
                var parsedValue = ParseSingleValue(type, typeDefined, attributes, rootNode, report);
                if (parsedValue != null)
                    arrayValue.Add(parsedValue);
            }

            return arrayValue;
        }

        /// <summary>
        /// 解析单个值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeDefined"></param>
        /// <param name="attributes"></param>
        /// <param name="jsonValue"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        private static object ParseSingleValue(string type, object typeDefined, AttributeTable attributes, JsonParser.JsonValueContext jsonValue, ReportWrap report)
        {
            var jsonPath = attributes != null ? attributes.GetAttribute<JsonPath>() : null;
            var rootPath = jsonPath != null ? jsonPath.path : "";

            var rootNode = GetRootJsonNode(jsonValue, rootPath);
            if (rootNode == null)
                report.OnJsonPathError(rootPath, jsonValue);
            else
            {
                if (typeDefined is Enum)
                    return ParseEnum(typeDefined as Enum, rootNode, report);
                else if (typeDefined is Struct)
                    return ParseStruct(typeDefined as Struct, rootNode, report);
                else if (typeDefined is Table)
                    return ParseTable(typeDefined as Table, rootNode, report);
                else if (BaseUtil.IsBaseType(type))
                    return ParseScalar(type, rootNode, report);
            }

            return null;
        }

        /// <summary>
        /// 解析枚举
        /// </summary>
        /// <param name="type"></param>
        /// <param name="jsonValue"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        private static object ParseEnum(Enum type, JsonParser.JsonValueContext jsonValue, ReportWrap report)
        {
            IToken token = jsonValue.strValue ?? jsonValue.intValue ?? jsonValue.floatValue;
            if (token != null)
            {
                object result = BaseUtil.GetEnum(type, token.Text.Trim('"'));
                if (result != null)
                    return result;
                else
                    report.OnError(string.Format("值\"{0}\"无法转换成枚举{1}.", token.Text.Trim('"'), type), token);
            }
            else
                report.OnError(string.Format("\"{0}\"无法转换成枚举{1}.", jsonValue.GetText(), type), jsonValue);

            return null;
        }

        /// <summary>
        /// 解析结构
        /// </summary>
        /// <param name="type"></param>
        /// <param name="jsonValue"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        private static object ParseStruct(Struct type, JsonParser.JsonValueContext jsonValue, ReportWrap report)
        {
            if (jsonValue.objectValue != null)
            {
                var name2jsonNode = new Dictionary<string, JsonParser.JsonValueContext>();
                foreach (var jsonNode in jsonValue.objectValue._props)
                {
                    var name = jsonNode.propName.Text.Trim('"');
                    if (name2jsonNode.ContainsKey(name))
                        report.OnError(string.Format("key为{0}的Json属性已存在.", name), jsonNode.propName);
                    else
                        name2jsonNode.Add(name, jsonNode.propValue);
                }

                var valid = true;
                var values = new Dictionary<string, object>();
                foreach (var field in type.Fields)
                {
                    var jsonKey = field.DataField;

                    object value = null;
                    if (name2jsonNode.ContainsKey(jsonKey))
                    {
                        if (field.IsArray)
                            value = ParseArrayValue(field.Type, field.TypeDefined, field.Attributes, name2jsonNode[jsonKey], report);
                        else
                            value = ParseSingleValue(field.Type, field.TypeDefined, field.Attributes, name2jsonNode[jsonKey], report);
                    }

                    values.Add(field.Name, value);
                    
                    if (field.Attributes.HasAttribute<JsonFileRef>())
                        continue;

                    if (value == null)
                    {
                        valid = false;
                        report.OnError(string.Format("找不到名为{0}的属性. '{1}'", jsonKey, jsonValue.GetText()), jsonValue);
                    }
                }

                if (!valid) return null;

                //JsonFileRef
                foreach (var field in type.Fields)
                {
                    var jsonFile = field.Attributes.GetAttribute<JsonFileRef>();
                    if (jsonFile == null) continue;

                    var filePath = Regex.Replace(jsonFile.filePath, "\\{.*?\\}", (match) =>
                    {
                        var key = match.Value.TrimStart('{').TrimEnd('}');
                        return values.ContainsKey(key) ? values[key].ToString() : "???";
                    });

                    if (!File.Exists(filePath))
                    {
                        valid = false;
                        report.OnError(string.Format("\"{0}\" 文件未找到.", filePath), field.Location);
                        continue;
                    }

                    var jsonLexer = new JsonLexer(new AntlrFileStream(filePath));
                    var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));
                    var fileNode = jsonParser.jsonValue();
                    if (fileNode != null)
                    {
                        object value = null;
                        if (field.IsArray)
                            value = ParseArrayValue(field.Type, field.TypeDefined, field.Attributes, fileNode, new ReportWrap(filePath, report.report));
                        else
                            value = ParseSingleValue(field.Type, field.TypeDefined, field.Attributes, fileNode, new ReportWrap(filePath, report.report));

                        if (value != null)
                            values[field.Name] = value;
                        else
                            valid = false;
                    }
                    else
                    {
                        valid = false;
                        report.OnError(string.Format("\"{0}\" 无法解析成一个Json.", filePath), field.Location);
                    }
                }

                if (!valid) return null;

                return values;
            }
            else
            {
                report.OnError(string.Format("\"{0}\" 无法转换成结构{1}.", jsonValue.GetText(), type), jsonValue);
            }

            return null;
        }

        /// <summary>
        /// 解析表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="jsonValue"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        private static object ParseTable(Table type, JsonParser.JsonValueContext jsonValue, ReportWrap report)
        {
            if(jsonValue.arraryValue==null && jsonValue.objectValue==null)
            {
                report.OnError(string.Format("\"{0}\"无法转换成表{1}.", jsonValue.GetText(), type.Name), jsonValue);
                return null;
            }

            List<JsonParser.JsonObjectContext> nodes = new List<JsonParser.JsonObjectContext>();
            if (jsonValue.arraryValue != null)
            {
                foreach (var node in jsonValue.arraryValue._arrayElement)
                {
                    if (node.objectValue == null)
                        report.OnError(string.Format("\"{0}\"无法转换成表{1}.", node.GetText(), type.Name), node);
                    else
                        nodes.Add(node.objectValue);
                }
            }
            else if (jsonValue.objectValue != null)
            {
                nodes.Add(jsonValue.objectValue);
            }

            //Unique
            var uniqueFields = new Dictionary<string, List<object>>();
            //Nullable
            var nullableFields = new HashSet<string>();
            foreach (var field in type.Fields)
            {
                if (field.Attributes.HasAttribute<Unique>())
                    uniqueFields.Add(field.Name, new List<object>());
                if (field.Attributes.HasAttribute<Nullable>())
                    nullableFields.Add(field.Name);
            }

            //表行
            var tableRows = new List<object>();
            var name2jsonNode = new Dictionary<string, JsonParser.JsonValueContext>();
            foreach (var childNode in nodes)
            {
                name2jsonNode.Clear();
                foreach (var node in childNode._props)
                {
                    var name = node.propName.Text.Trim('"');
                    if (name2jsonNode.ContainsKey(name))
                        report.OnError(string.Format("key为{0}的Json属性已存在.", name), node.propName);
                    else
                        name2jsonNode.Add(name, node.propValue);
                }

                var valid = true;
                var values = new Dictionary<string, object>();
                foreach (var field in type.Fields)
                {
                    var jsonKey = field.DataField;
                    var jsonNode = name2jsonNode.ContainsKey(jsonKey) ? name2jsonNode[jsonKey] : null;

                    object value = null;
                    if (jsonNode!=null)
                    {
                        if (field.IsArray)
                            value = ParseArrayValue(field.Type, field.TypeDefined, field.Attributes, jsonNode, report);
                        else
                            value = ParseSingleValue(field.Type, field.TypeDefined, field.Attributes, jsonNode, report);
                    }

                    values.Add(field.Name, value);

                    if (field.Attributes.HasAttribute<JsonFileRef>())
                        continue;

                    if (value == null)
                    {
                        if (uniqueFields.ContainsKey(field.Name) || !nullableFields.Contains(field.Name))
                        {
                            valid = false;
                            if (jsonNode == null)
                                report.OnError(string.Format("找不到名为{0}的属性. '{1}'", jsonKey, childNode.GetText()), childNode);
                        }
                        else
                        {
                            //默认值
                        }
                    }
                    else
                    {
                        if (uniqueFields.ContainsKey(field.Name))
                        {
                            if(uniqueFields[field.Name].Contains(value))
                            {
                                valid = false;
                                report.OnError(string.Format("字段{0}被标记为Unique,内容不允许重复。", field.Name), jsonNode);
                            }
                            else
                            {
                                uniqueFields[field.Name].Add(value);
                            }
                        }
                    }
                }

                if (!valid) continue;

                //JsonFileRef
                foreach (var field in type.Fields)
                {
                    var jsonFile = field.Attributes.GetAttribute<JsonFileRef>();
                    if (jsonFile == null) continue;

                    var filePath = Regex.Replace(jsonFile.filePath, "\\{.*?\\}", (match) =>
                    {
                        var key = match.Value.TrimStart('{').TrimEnd('}');
                        return values.ContainsKey(key) ? values[key].ToString() : "???";
                    });

                    if (!File.Exists(filePath))
                    {
                        valid = false;
                        report.OnError(string.Format("\"{0}\" 文件未找到.", filePath), field.Location);
                        break;
                    }

                    var jsonLexer = new JsonLexer(new AntlrFileStream(filePath));
                    var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));
                    var fileNode = jsonParser.jsonValue();
                    if (fileNode != null)
                    {
                        object value = null;

                        if (field.IsArray)
                            value = ParseArrayValue(field.Type, field.TypeDefined, field.Attributes, fileNode, new ReportWrap(filePath, report.report));
                        else
                            value = ParseSingleValue(field.Type, field.TypeDefined, field.Attributes, fileNode, new ReportWrap(filePath, report.report));

                        values[field.Name] = value;

                        if (value == null)
                        {
                            if (uniqueFields.ContainsKey(field.Name) || !nullableFields.Contains(field.Name))
                            {
                                valid = false;
                                //ParseValue里已经报过错。
                            }
                        }
                        else
                        {
                            if(uniqueFields.ContainsKey(field.Name))
                            {
                                if (uniqueFields[field.Name].Contains(value))
                                {
                                    valid = false;
                                    report.OnError(string.Format("字段{0}被标记为Unique,内容不允许重复。", field.Name), fileNode);
                                }
                                else
                                {
                                    uniqueFields[field.Name].Add(value);
                                }
                            }
                        }
                    }
                    else
                    {
                        valid = false;
                        report.OnError(string.Format("\"{0}\" 无法解析成一个Json.", filePath), field.Location);
                    }
                }

                if (!valid) continue;

                tableRows.Add(values);
            }

            return tableRows;
        }

        /// <summary>
        /// 解析标量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="jsonValue"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        private static object ParseScalar(string type, JsonParser.JsonValueContext jsonValue, ReportWrap report)
        {
            IToken token = jsonValue.boolValue ?? jsonValue.intValue ?? jsonValue.floatValue ?? jsonValue.strValue ?? null;
            if (token != null)
            {
                object result = BaseUtil.GetScalar(type, token.Text.Trim('"'));
                if (result != null)
                    return result;
                else
                {
                    if (BaseUtil.IsInteger(type) || BaseUtil.IsFloat(type))
                        report.OnError(string.Format("值\"{0}\"无法转换成{1},或超出{1}的精度范围.", token.Text.Trim('"'), type), token);
                    else
                        report.OnError(string.Format("值\"{0}\"无法转换成{1}.", token.Text.Trim('"'), type), token);
                }
            }
            else
                report.OnError(string.Format("\"{0}\"无法转换成{1}.", jsonValue.GetText(), type), jsonValue);

            return null;
        }

        /// <summary>
        /// 按路径查找Json节点
        /// </summary>
        /// <param name="json"></param>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        private static JsonParser.JsonValueContext GetRootJsonNode(JsonParser.JsonValueContext json, string jsonPath)
        {
            if (string.IsNullOrEmpty(jsonPath))
            {
                return json;
            }

            jsonPath = Regex.Replace(jsonPath, @"\[(.*)\]", @".$1");

            var jsonPathParts = jsonPath.Trim().Trim('.').Split('.');
            for (var i = 0; i < jsonPathParts.Length; i++)
            {
                var partText = jsonPathParts[i].Trim();
                if (json.objectValue != null)
                {
                    JsonParser.JsonValueContext findedNode = null;
                    foreach (var prop in json.objectValue._props)
                    {
                        if (partText.Equals(prop.propName.Text.Trim('"')))
                        {
                            findedNode = prop.propValue;
                            break;
                        }
                    }
                    if (findedNode != null)
                    {
                        json = findedNode;
                    }
                    else
                    {
                        json = null;
                        break;
                    }
                }
                else if (json.arraryValue != null)
                {
                    int index = -1;
                    if (int.TryParse(partText, out index))
                    {
                        if (index >= 0 && index < json.arraryValue._arrayElement.Count)
                        {
                            json = json.arraryValue._arrayElement[index];
                        }
                        else
                        {
                            json = null;
                            break;
                        }
                    }
                }
                else
                {
                    json = null;
                    break;
                }
            }
            return json;
        }
    }
}