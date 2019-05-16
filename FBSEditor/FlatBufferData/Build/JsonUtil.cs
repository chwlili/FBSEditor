using Antlr4.Runtime;
using FlatBufferData.Model;
using System;
using System.Collections.Generic;

namespace FlatBufferData.Build
{
    public class JsonUtil
    {
        /// <summary>
        /// 解析Json文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="rootPath"></param>
        /// <param name="type"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static object ParseJsonFile(string filePath, string rootPath, Table type, List<string> errors)
        {
            var jsonLexer = new JsonLexer(new AntlrFileStream(filePath));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            return Parse(type.Name, type, jsonParser.jsonValue(), rootPath, errors);
        }

        /// <summary>
        /// 解析Json文本
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rootPath"></param>
        /// <param name="type"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static object ParseJsonText(string text, string rootPath, Table type, List<string> errors)
        {
            var jsonLexer = new JsonLexer(new AntlrInputStream(text));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            return Parse(type.Name, type, jsonParser.jsonValue(), rootPath, errors);
        }

        /// <summary>
        /// 解析Json文本
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rootPath"></param>
        /// <param name="type"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static object ParseJsonText(string text, string rootPath, Struct type, List<string> errors)
        {
            var jsonLexer = new JsonLexer(new AntlrInputStream(text));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            return Parse(type.Name, type, jsonParser.jsonValue(), rootPath, errors);
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="type"></param>
        /// <param name="jsonValue"></param>
        /// <param name="rootPath"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static object Parse(string typeName, object type, JsonParser.JsonValueContext jsonValue, string rootPath, List<string> errors)
        {
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

            return ParseJson(typeName, type, false, rootNode, errors);
        }

        /// <summary>
        /// 解析Json
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeDefined"></param>
        /// <param name="isArray"></param>
        /// <param name="jsonValue"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static object ParseJson(string type, object typeDefined, bool isArray, JsonParser.JsonValueContext jsonValue, List<string> errors)
        {
            if (isArray)
            {
                var arrayValue = new List<object>();
                if (jsonValue.arraryValue == null)
                {
                    errors.Add(string.Format("{0}无法转换成{1}数组.({2}:{3})", jsonValue.GetText(), type, jsonValue.Start.Line, jsonValue.Start.Column));
                }
                else
                {
                    foreach (var arrayElementJson in jsonValue.arraryValue._arrayElement)
                    {
                        var parsedValue = ParseJson(type, typeDefined, false, arrayElementJson, errors);
                        if (parsedValue != null)
                            arrayValue.Add(parsedValue);
                    }
                }
                return arrayValue;
            }
            else
            {
                if (BaseUtil.IsInteger(type) || BaseUtil.IsFloat(type) || BaseUtil.IsBool(type) || BaseUtil.IsString(type))
                {
                    return ParseScalar(type, jsonValue, errors);
                }
                else if (typeDefined is Model.Enum)
                {
                    return ParseEnum(typeDefined as Model.Enum, jsonValue, errors);
                }
                else if (typeDefined is Model.Struct)
                {
                    return ParseStruct(typeDefined as Model.Struct, jsonValue, errors);
                }
                else if (typeDefined is Model.Table)
                {
                    return ParseTable(typeDefined as Model.Table, jsonValue, errors);
                }
            }
            return null;
        }

        /// <summary>
        /// 解析标量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="jsonValue"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static object ParseScalar(string type, JsonParser.JsonValueContext jsonValue, List<string> errors)
        {
            IToken token = jsonValue.boolValue ?? jsonValue.intValue ?? jsonValue.floatValue ?? jsonValue.strValue ?? null;
            if (token != null)
            {
                object result = BaseUtil.GetScalar(type, token.Text.Trim('"'));
                if (result != null)
                    return result;
                else
                {
                    if(BaseUtil.IsInteger(type) || BaseUtil.IsFloat(type))
                        errors.Add(string.Format("值\"{0}\"无法转换成{1},或超出{1}的精度范围.({2}:{3})", token.Text.Trim('"'), type, token.Line, token.Column));
                    else
                        errors.Add(string.Format("值\"{0}\"无法转换成{1}.({2}:{3})", token.Text.Trim('"'), type, token.Line, token.Column));
                }
            }
            else
                errors.Add(string.Format("\"{0}\"无法转换成{1}.({2}:{3})", jsonValue.GetText(), type, token.Line, token.Column));

            return null;
        }

        /// <summary>
        /// 解析枚举
        /// </summary>
        /// <param name="type"></param>
        /// <param name="jsonValue"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static object ParseEnum(Model.Enum type, JsonParser.JsonValueContext jsonValue, List<string> errors)
        {
            IToken token = jsonValue.strValue ?? jsonValue.intValue ?? jsonValue.floatValue;
            if(token!=null)
            {
                object result = BaseUtil.GetEnum(type, token.Text.Trim('"'));
                if (result != null)
                    return result;
                else
                    errors.Add(string.Format("值\"{0}\"无法转换成枚举{1}.({2}:{3})", token.Text.Trim('"'), type, token.Line, token.Column));
            }
            else
                errors.Add(string.Format("\"{0}\"无法转换成枚举{1}.({2}:{3})", jsonValue.GetText(), type, token.Line, token.Column));

            return null;
        }

        /// <summary>
        /// 解析结构
        /// </summary>
        /// <param name="type"></param>
        /// <param name="jsonValue"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static object ParseStruct(Struct type, JsonParser.JsonValueContext jsonValue, List<string> errors)
        {
            if (jsonValue.objectValue != null)
            {
                var name2jsonNode = new Dictionary<string, JsonParser.JsonValueContext>();
                foreach (var jsonNode in jsonValue.objectValue._props)
                {
                    var name = jsonNode.propName.Text.Trim('"');
                    if (name2jsonNode.ContainsKey(name))
                        errors.Add(string.Format("key为{0}的Json属性已存在.({1}:{2})", name, jsonNode.propName.Line, jsonNode.propName.Column));
                    else
                        name2jsonNode.Add(name, jsonNode.propValue);
                }

                var values = new Dictionary<string, object>();
                foreach (var field in type.Fields)
                {
                    var jsonKey = field.DataField;
                    if(name2jsonNode.ContainsKey(jsonKey))
                    {
                        values.Add(field.Name, ParseJson(field.Type, field.TypeDefined, field.IsArray, name2jsonNode[jsonKey], errors));
                    }
                    else
                    {
                        values.Add(field.Name, null);
                        errors.Add(string.Format("\"{0}\"找不到名为{1}的属性.({2}:{3})", jsonValue.GetText(), jsonKey, jsonValue.Start.Line, jsonValue.Start.Column));
                    }
                }
                return values;
            }
            else
            {
                errors.Add(string.Format("\"{0}\"无法转换成结构{1}.({2}:{3})", jsonValue.GetText(), type, jsonValue.Start.Line, jsonValue.Start.Column));
            }

            return null;
        }

        /// <summary>
        /// 解析表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="jsonValue"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static object ParseTable(Table type, JsonParser.JsonValueContext jsonValue, List<string> errors)
        {
            if (jsonValue.arraryValue == null)
            {
                errors.Add(string.Format("\"{0}\"无法转换成表{1}.({2}:{3})", jsonValue.GetText(), type.Name, jsonValue.Start.Line, jsonValue.Start.Column));
                return null;
            }

            var tableRows = new List<object>();
            foreach (var childNode in jsonValue.arraryValue._arrayElement)
            {
                if (childNode.objectValue == null)
                {
                    errors.Add(string.Format("\"{0}\"无法转换成表{1}.({2}:{3})", jsonValue.GetText(), type.Name, jsonValue.Start.Line, jsonValue.Start.Column));
                    continue;
                }

                var name2jsonNode = new Dictionary<string, JsonParser.JsonValueContext>();
                foreach (var jsonNode in childNode.objectValue._props)
                {
                    var name = jsonNode.propName.Text.Trim('"');
                    if (name2jsonNode.ContainsKey(name))
                        errors.Add(string.Format("key为{0}的Json属性已存在.({1}:{2})", name, jsonNode.propName.Line, jsonNode.propName.Column));
                    else
                        name2jsonNode.Add(name, jsonNode.propValue);
                }

                var singleRow = new Dictionary<string, object>();
                foreach (var field in type.Fields)
                {
                    var jsonKey = field.DataField;
                    if (name2jsonNode.ContainsKey(jsonKey))
                    {
                        singleRow.Add(field.Name, ParseJson(field.Type, field.TypeDefined, field.IsArray, name2jsonNode[jsonKey], errors));
                    }
                    else
                    {
                        singleRow.Add(field.Name, null);
                        errors.Add(string.Format("\"{0}\"找不到名为{1}的属性.({2}:{3})", childNode.GetText(), jsonKey, childNode.Start.Line, childNode.Start.Column));
                    }
                }
                tableRows.Add(singleRow);
            }
            return tableRows;
        }

        /// <summary>
        /// 按路径查找Json节点
        /// </summary>
        /// <param name="json"></param>
        /// <param name="jsonPath"></param>
        /// <param name="error"></param>
        /// <returns></returns>
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