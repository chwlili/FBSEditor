using Antlr4.Runtime;
using FlatBufferData.Model;
using System;
using System.Collections.Generic;

namespace FlatBufferData.Build
{
    public class JsonUtil
    {
        public static object ParseJsonFile(string filePath, string rootPath, Table type, List<string> errors)
        {
            var jsonLexer = new JsonLexer(new AntlrFileStream(filePath));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            var jsonValue = jsonParser.jsonValue();

            var jsonPath = "";
            if(!string.IsNullOrEmpty(rootPath))
                jsonPath = rootPath;

            var error = "";
            var rootNode = GetRootJsonNode(jsonValue, jsonPath, out error);
            if (rootNode == null)
            {
                errors.Add("无效的jsonPath:" + jsonPath);
                return null;
            }

            return ParseJsonNode(type.Name, type, rootNode, errors);
        }

        public static object ParseJsonFile(string filePath, string rootPath, Struct type, List<string> errors)
        {
            var jsonLexer = new JsonLexer(new AntlrFileStream(filePath));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            var jsonValue = jsonParser.jsonValue();

            var jsonPath = "";
            if (!string.IsNullOrEmpty(rootPath))
                jsonPath = rootPath;

            var error = "";
            var rootNode = GetRootJsonNode(jsonValue, jsonPath, out error);
            if (rootNode == null)
            {
                errors.Add("无效的jsonPath:" + jsonPath);
                return null;
            }

            return ParseJsonNode(type.Name, type, rootNode, errors);
        }

        public static object ParseJsonText(string text, string rootPath, Table type, List<string> errors)
        {
            var jsonLexer = new JsonLexer(new AntlrInputStream(text));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            var jsonValue = jsonParser.jsonValue();

            var jsonPath = "";
            if (!string.IsNullOrEmpty(rootPath))
                jsonPath = rootPath;

            var error = "";
            var rootNode = GetRootJsonNode(jsonValue, jsonPath, out error);
            if (rootNode == null)
            {
                errors.Add("无效的jsonPath:" + jsonPath);
                return null;
            }

            return ParseJsonNode(type.Name, type, rootNode, errors);
        }

        public static object ParseJsonText(string text, string rootPath, Struct type, List<string> errors)
        {
            var jsonLexer = new JsonLexer(new AntlrInputStream(text));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            var jsonValue = jsonParser.jsonValue();

            var jsonPath = "";
            if (!string.IsNullOrEmpty(rootPath))
                jsonPath = rootPath;

            var error = "";
            var rootNode = GetRootJsonNode(jsonValue, jsonPath, out error);
            if (rootNode == null)
            {
                errors.Add("无效的jsonPath:" + jsonPath);
                return null;
            }

            return ParseJsonNode(type.Name, type, rootNode, errors);
        }


        /// <summary>
        /// 尝试解析JsonValue
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeDefined"></param>
        /// <param name="jsonValue"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static object ParseJsonNode(string type, object typeDefined, JsonParser.JsonValueContext jsonValue, List<string> errors)
        {
            var isInteger = BaseUtil.IsInteger(type);
            var isFloat = BaseUtil.IsFloat(type);
            var isBool = BaseUtil.IsBool(type);
            var isString = BaseUtil.IsString(type);
            var isBase = isInteger || isFloat || isBool || isString;

            var isEnum = typeDefined is Model.Enum;
            var isTable = typeDefined is Model.Table;
            var isStruct = typeDefined is Model.Struct;

            if (jsonValue.nullValue != null)
            {
                errors.Add(string.Format("null无法转换成{1}.({2}:{3})", jsonValue.nullValue.Text, type, jsonValue.nullValue.Line, jsonValue.nullValue.Column));
                return null;
            }
            if (jsonValue.arraryValue != null && (isBase || isEnum))
            {
                errors.Add(string.Format("数组无法转换成{0}.({1}:{2})", type, jsonValue.arraryValue.Start.Line, jsonValue.arraryValue.Start.Column));
                return null;
            }
            if (jsonValue.objectValue != null && (isBase || isEnum))
            {
                errors.Add(string.Format("对象无法转换成{0}.({1}:{2}).", type, jsonValue.objectValue.Start.Line, jsonValue.objectValue.Start.Column));
                return null;
            }

            if (isBase)
            {
                var error = isInteger || isFloat ? "值\"{0}\"无法转换成{1},或超出{1}的精度范围.({2}:{3})" : "值\"{0}\"无法转换成{1}.({2}:{3})";

                if (jsonValue.boolValue != null && (isInteger || isFloat))
                {
                    errors.Add(string.Format(error, jsonValue.boolValue.Text.Trim('"'), type, jsonValue.boolValue.Line, jsonValue.boolValue.Column));
                    return null;
                }

                if (jsonValue.strValue != null && isBool)
                {
                    errors.Add(string.Format(error, jsonValue.strValue.Text.Trim('"'), type, jsonValue.strValue.Line, jsonValue.strValue.Column));
                    return null;
                }

                IToken token = null;
                if (jsonValue.boolValue != null)
                    token = jsonValue.boolValue;
                else if (jsonValue.intValue != null)
                    token = jsonValue.intValue;
                else if (jsonValue.floatValue != null)
                    token = jsonValue.floatValue;
                else if (jsonValue.strValue != null)
                    token = jsonValue.strValue;

                object result = BaseUtil.GetScalar(type, token.Text.Trim('"'));
                if (result != null)
                    return result;
                else
                    errors.Add(string.Format(error, token.Text.Trim('"'), type, token.Line, token.Column));
            }
            else if (isEnum)
            {
                IToken token = null;
                if (jsonValue.strValue != null)
                    token = jsonValue.strValue;
                else if (jsonValue.intValue != null)
                    token = jsonValue.intValue;
                else if (jsonValue.floatValue != null)
                    token = jsonValue.floatValue;

                var text = token.Text.Trim('"');
                object result = BaseUtil.GetEnum(typeDefined as Model.Enum, text);
                if (result != null)
                    return result;

                errors.Add(string.Format("值\"{0}\"无法转换成枚举{1}.({2}:{3})", text, type, token.Line, token.Column));
                return null;
            }
            else if (isStruct)
            {
                if (jsonValue.nullValue != null || jsonValue.boolValue != null || jsonValue.intValue != null || jsonValue.floatValue != null)
                {
                    errors.Add(string.Format("值\"{0}\"无法转换成结构{1}.({2},{3})", jsonValue.GetText(), type, jsonValue.Start.Line, jsonValue.Start.Column));
                    return null;
                }
                else if (jsonValue.arraryValue != null)
                {
                    errors.Add(string.Format("数组无法转换成结构{0}.({1}:{2})", type, jsonValue.Start.Line, jsonValue.Start.Column));
                    return null;
                }
                else if (jsonValue.objectValue != null)
                {
                    var name2jsonNode = new Dictionary<string, JsonParser.JsonValueContext>();
                    foreach (var jsonNode in jsonValue.objectValue._props)
                    {
                        var name = jsonNode.propName.Text.Trim('"');
                        if (name2jsonNode.ContainsKey(name))
                            errors.Add(string.Format("key为{0}的Json属性已存在.({1}:{2})", name, jsonNode.propName.Line, jsonNode.propName.Column));
                        else
                            name2jsonNode.Add(jsonNode.propName.Text.Trim('"'), jsonNode.propValue);
                    }

                    var values = new Dictionary<string, object>();
                    var structType = typeDefined as Model.Struct;
                    foreach (var structField in structType.Fields)
                    {
                        var jsonKey = structField.DataField;
                        if (name2jsonNode.ContainsKey(jsonKey))
                        {
                            var value = ParseJsonNode(structField.Type, structField.TypeDefined, name2jsonNode[jsonKey], errors);
                            if (value != null)
                                values.Add(structField.Name, value);
                        }
                        else
                        {
                            errors.Add(String.Format("找不到名为{0}的json字段。", jsonKey));
                        }
                    }
                    return values;
                }
            }
            else if (isTable)
            {
                if (jsonValue.nullValue != null || jsonValue.boolValue != null || jsonValue.intValue != null || jsonValue.floatValue != null)
                {
                    errors.Add(string.Format("值\"{0}\"无法转换成{1}结构.({2},{3})", jsonValue.GetText(), type, jsonValue.Start.Line, jsonValue.Start.Column));
                    return null;
                }
                else if (jsonValue.objectValue != null)
                {
                    errors.Add(string.Format("结构无法转换成{0}数组.({1}:{2})", type, jsonValue.Start.Line, jsonValue.Start.Column));
                    return null;
                }

                var values = new List<object>();
                foreach (var jsonNode in jsonValue.arraryValue._arrayElement)
                {
                    var value = ParseJsonNode(type, typeDefined, jsonNode, errors);
                    if (value != null)
                        values.Add(value);
                }
                return values;
            }

            return null;
        }


        private static JsonParser.JsonValueContext GetRootJsonNode(JsonParser.JsonValueContext json, string jsonPath, out string error)
        {
            if (string.IsNullOrEmpty(jsonPath))
            {
                error = null;
                return json;
            }

            var jsonPathParts = jsonPath.Trim().Trim('.').Split('.');
            for (var i = 0; i < jsonPathParts.Length; i++)
            {
                var partText = jsonPathParts[i].Trim();
                var partInt = -1;
                var partStr = "";
                if (partText.EndsWith("]"))
                {
                    var index = partText.IndexOf("[");
                    if (index != -1)
                    {
                        int parsedInt = 0;
                        partStr = partText.Substring(index + 1, partText.Length - 1 - index - 1);
                        if (int.TryParse(partStr, out parsedInt))
                            partInt = parsedInt;
                        partText = partText.Substring(0, index);
                    }
                }

                if (json.objectValue!=null)
                {
                    JsonParser.JsonValueContext nextNode = null;
                    foreach(var prop in json.objectValue._props)
                    {
                        if(partText.Equals(prop.propName.Text.Trim('"')))
                        {
                            nextNode = prop.propValue;
                            break;
                        }
                    }

                    if (nextNode != null)
                    {
                        json = nextNode;

                        if (partInt != -1)
                        {
                            if (json.arraryValue!=null)
                            {
                                if (partInt >= 0 && partInt < json.arraryValue._arrayElement.Count)
                                {
                                    json = json.arraryValue._arrayElement[partInt];
                                    break;
                                }
                                else
                                {
                                    error = "索引partInt越界";
                                    return null;
                                }
                            }
                            else
                            {
                                error = "无效的路径";
                                return null;
                            }
                        }
                        else if (!string.IsNullOrEmpty(partStr))
                        {
                            if (json.objectValue!=null)
                            {
                                JsonParser.JsonValueContext selectedNode = null;
                                foreach (var prop in json.objectValue._props)
                                {
                                    if (partStr.Equals(prop.propName.Text.Trim('"')))
                                    {
                                        selectedNode = prop.propValue;
                                        break;
                                    }
                                }

                                if (selectedNode != null)
                                    json = selectedNode;
                                else
                                {
                                    error = "无效的路径";
                                    return null;
                                }
                            }
                            else
                            {
                                error = "无效的路径";
                                return null;
                            }
                        }
                    }
                    else
                    {
                        error = "无效的路径";
                        return null;
                    }
                }
                else
                {
                    error = "无效的路径";
                    return null;
                }
            }

            error = null;

            return json;
        }


    }
}