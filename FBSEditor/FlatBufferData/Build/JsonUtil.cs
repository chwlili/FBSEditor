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

            public ReportWrap()
            {

            }
        }

        public static object ParseJsonFile(string filePath, Struct type, AttributeTable attributes, ErrorReport report)
        {
            return ParseJsonText(File.ReadAllText(filePath), type, attributes, report);
        }

        public static object ParseJsonText(string text, Struct type, AttributeTable attributes, ErrorReport report)
        {
            var jsonLexer = new JsonLexer(new AntlrInputStream(text));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            return ParseSingleValue(type.Name, type, attributes, jsonParser.jsonValue(), report);
        }

        public static object ParseJsonFile(string filePath, Table type, AttributeTable attributes, ErrorReport report)
        {
            return ParseJsonText(File.ReadAllText(filePath), type, attributes, report);
        }

        public static object ParseJsonText(string text, Table type, AttributeTable attributes, ErrorReport report)
        {
            var jsonLexer = new JsonLexer(new AntlrInputStream(text));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            return ParseSingleValue(type.Name, type, attributes, jsonParser.jsonValue(), report);
        }

        /// <summary>
        /// 解析数组值
        /// </summary>
        private static List<object> ParseArrayValue(string type, object typeDefined, AttributeTable attributes, JsonParser.JsonValueContext jsonValue, ErrorReport report)
        {
            var jsonPath = attributes != null ? attributes.GetAttribute<JsonPath>() : null;
            var rootPath = jsonPath != null ? jsonPath.path : "";
            var rootNode = GetRootJsonNode(jsonValue, rootPath);

            if (rootNode == null)
            {
                report.Add("无效的jsonPath:" + jsonPath);
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
        private static object ParseSingleValue(string type, object typeDefined, AttributeTable attributes, JsonParser.JsonValueContext jsonValue, ErrorReport report)
        {
            var jsonPath = attributes != null ? attributes.GetAttribute<JsonPath>() : null;
            var rootPath = jsonPath != null ? jsonPath.path : "";

            var rootNode = GetRootJsonNode(jsonValue, rootPath);
            if (rootNode == null)
                report.Add("无效的jsonPath : " + rootPath);
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
        private static object ParseEnum(Enum type, JsonParser.JsonValueContext jsonValue, ErrorReport report)
        {
            IToken token = jsonValue.strValue ?? jsonValue.intValue ?? jsonValue.floatValue;
            if(token!=null)
            {
                object result = BaseUtil.GetEnum(type, token.Text.Trim('"'));
                if (result != null)
                    return result;
                else
                    report.Add(string.Format("值\"{0}\"无法转换成枚举{1}.({2}:{3})", token.Text.Trim('"'), type, token.Line, token.Column));
            }
            else
                report.Add(string.Format("\"{0}\"无法转换成枚举{1}.({2}:{3})", jsonValue.GetText(), type, jsonValue.Start.Line, jsonValue.Start.Column));

            return null;
        }

        /// <summary>
        /// 解析结构
        /// </summary>
        /// <param name="type"></param>
        /// <param name="jsonValue"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        private static object ParseStruct(Struct type, JsonParser.JsonValueContext jsonValue, ErrorReport report)
        {
            if (jsonValue.objectValue != null)
            {
                var name2jsonNode = new Dictionary<string, JsonParser.JsonValueContext>();
                foreach (var jsonNode in jsonValue.objectValue._props)
                {
                    var name = jsonNode.propName.Text.Trim('"');
                    if (name2jsonNode.ContainsKey(name))
                        report.Add(string.Format("key为{0}的Json属性已存在.({1}:{2})", name, jsonNode.propName.Line, jsonNode.propName.Column));
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
                        report.Add(string.Format("\"{0}\"找不到名为{1}的属性.({2}:{3})", jsonValue.GetText(), jsonKey, jsonValue.Start.Line, jsonValue.Start.Column));
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
                        report.Add(string.Format("\"{0}\" 文件未找到.", filePath));
                        continue;
                    }

                    var jsonLexer = new JsonLexer(new AntlrFileStream(filePath));
                    var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));
                    var fileNode = jsonParser.jsonValue();
                    if (fileNode != null)
                    {
                        object value = null;
                        if (field.IsArray)
                            value = ParseArrayValue(field.Type, field.TypeDefined, field.Attributes, fileNode, report);
                        else
                            value = ParseSingleValue(field.Type, field.TypeDefined, field.Attributes, fileNode, report);

                        if (value != null)
                            values[field.Name] = value;
                        else
                            valid = false;
                    }
                    else
                    {
                        valid = false;
                        report.Add(string.Format("\"{0}\" 无法解析成一个Json.", filePath));
                    }
                }

                if (!valid) return null;

                return values;
            }
            else
            {
                report.Add(string.Format("\"{0}\"无法转换成结构{1}.({2}:{3})", jsonValue.GetText(), type, jsonValue.Start.Line, jsonValue.Start.Column));
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
        private static object ParseTable(Table type, JsonParser.JsonValueContext jsonValue, ErrorReport report)
        {
            if(jsonValue.arraryValue==null && jsonValue.objectValue==null)
            {
                report.Add(string.Format("\"{0}\"无法转换成表{1}.({2}:{3})", jsonValue.GetText(), type.Name, jsonValue.Start.Line, jsonValue.Start.Column));
                return null;
            }

            List<JsonParser.JsonObjectContext> nodes = new List<JsonParser.JsonObjectContext>();
            if (jsonValue.arraryValue != null)
            {
                foreach (var node in jsonValue.arraryValue._arrayElement)
                {
                    if (node.objectValue == null)
                        report.Add(string.Format("\"{0}\"无法转换成表{1}.({2}:{3})", node.GetText(), type.Name, node.Start.Line, node.Start.Column));
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
                        report.Add(string.Format("key为{0}的Json属性已存在.({1}:{2})", name, node.propName.Line, node.propName.Column));
                    else
                        name2jsonNode.Add(name, node.propValue);
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
                        if (uniqueFields.ContainsKey(field.Name))
                        {
                            valid = false;
                            report.Add("unique can't contains null.");
                        }
                        else if (!nullableFields.Contains(field.Name))
                        {
                            valid = false;
                            if (!name2jsonNode.ContainsKey(jsonKey))
                                report.Add(string.Format("\"{0}\"找不到名为{1}的属性.({2}:{3})", childNode.GetText(), jsonKey, childNode.Start.Line, childNode.Start.Column));
                        }
                    }
                    else
                    {
                        if (uniqueFields.ContainsKey(field.Name))
                        {
                            if(uniqueFields[field.Name].Contains(value))
                            {
                                valid = false;
                                report.Add("unique can't contains same value.");
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
                        report.Add(string.Format("\"{0}\" 文件未找到.", filePath));
                        break;
                    }

                    var jsonLexer = new JsonLexer(new AntlrFileStream(filePath));
                    var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));
                    var fileNode = jsonParser.jsonValue();
                    if (fileNode != null)
                    {
                        object value = null;

                        if (field.IsArray)
                            value = ParseArrayValue(field.Type, field.TypeDefined, field.Attributes, fileNode, report);
                        else
                            value = ParseSingleValue(field.Type, field.TypeDefined, field.Attributes, fileNode, report);

                        values[field.Name] = value;

                        if (value == null)
                        {
                            if (uniqueFields.ContainsKey(field.Name))
                            {
                                valid = false;
                                report.Add("unique can't contains null.");
                            }
                            else if (!nullableFields.Contains(field.Name))
                            {
                                valid = false;
                                //..
                            }
                        }
                        else
                        {
                            if(uniqueFields.ContainsKey(field.Name))
                            {
                                if (uniqueFields[field.Name].Contains(value))
                                {
                                    valid = false;
                                    report.Add("unique can't contains same value.");
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
                        report.Add(string.Format("\"{0}\" 无法解析成一个Json.", filePath));
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
        private static object ParseScalar(string type, JsonParser.JsonValueContext jsonValue, ErrorReport report)
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
                        report.Add(string.Format("值\"{0}\"无法转换成{1},或超出{1}的精度范围.({2}:{3})", token.Text.Trim('"'), type, token.Line, token.Column));
                    else
                        report.Add(string.Format("值\"{0}\"无法转换成{1}.({2}:{3})", token.Text.Trim('"'), type, token.Line, token.Column));
                }
            }
            else
                report.Add(string.Format("\"{0}\"无法转换成{1}.({2}:{3})", jsonValue.GetText(), type, jsonValue.Start.Line, jsonValue.Start.Column));

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