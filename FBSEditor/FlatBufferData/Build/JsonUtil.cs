﻿using Antlr4.Runtime;
using FlatBufferData.Model;
using FlatBufferData.Model.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FlatBufferData.Build
{
    public class JsonUtil
    {
        //UNDONE ？？？？
        /// <summary>
        /// 解析Json文本
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rootPath"></param>
        /// <param name="type"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static object ParseJsonText(string text, Struct type, AttributeTable attributes, List<string> errors)
        {
            var jsonLexer = new JsonLexer(new AntlrInputStream(text));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            return ParseJson(type.Name, type, false, attributes, jsonParser.jsonValue(), errors);
        }

        /// <summary>
        /// 解析Json文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="rootPath"></param>
        /// <param name="type"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static object ParseJsonFile(string filePath, Table type, AttributeTable attributes, List<string> errors)
        {
            var jsonLexer = new JsonLexer(new AntlrFileStream(filePath));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            return ParseJson(type.Name, type, false, attributes, jsonParser.jsonValue(), errors);
        }

        /// <summary>
        /// 解析Json文本
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rootPath"></param>
        /// <param name="type"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static object ParseJsonText(string text, Table type, AttributeTable attributes, List<string> errors)
        {
            var jsonLexer = new JsonLexer(new AntlrInputStream(text));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            return ParseJson(type.Name, type, false, attributes, jsonParser.jsonValue(), errors);
        }

        private static JsonParser.JsonValueContext GetJsonFile(string filePath)
        {
            if(!File.Exists(filePath))
            {
                return null;
            }

            var jsonLexer = new JsonLexer(new AntlrFileStream(filePath));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));
            return jsonParser.jsonValue();
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
        private static object ParseJson(string type, object typeDefined, bool isArray, AttributeTable attributes, JsonParser.JsonValueContext jsonValue, List<string> errors)
        {
            var jsonPath = attributes != null ? attributes.GetAttribute<JsonPath>() : null;
            var rootPath = jsonPath != null ? jsonPath.path : "";

            var rootNode = GetRootJsonNode(jsonValue, rootPath);
            if (rootNode == null)
            {
                errors.Add("无效的jsonPath:" + jsonPath);
                return null;
            }

            if (isArray)
            {
                var arrayValue = new List<object>();
                if (rootNode.arraryValue != null)
                {
                    foreach (var arrayElementJson in rootNode.arraryValue._arrayElement)
                    {
                        var parsedValue = ParseJson(type, typeDefined, false, null, arrayElementJson, errors);
                        if (parsedValue != null)
                            arrayValue.Add(parsedValue);
                    }
                }
                else
                {
                    var parsedValue = ParseJson(type, typeDefined, false, null, rootNode, errors);
                    if (parsedValue != null)
                        arrayValue.Add(parsedValue);
                }
                return arrayValue;
            }
            else
            {
                if (BaseUtil.IsInteger(type) || BaseUtil.IsFloat(type) || BaseUtil.IsBool(type) || BaseUtil.IsString(type))
                {
                    return ParseScalar(type, rootNode, errors);
                }
                else if (typeDefined is Model.Enum)
                {
                    return ParseEnum(typeDefined as Model.Enum, rootNode, errors);
                }
                else if (typeDefined is Model.Struct)
                {
                    return ParseStruct(typeDefined as Model.Struct, rootNode, errors);
                }
                else if (typeDefined is Model.Table)
                {
                    return ParseTable(typeDefined as Model.Table, rootNode, errors);
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
                errors.Add(string.Format("\"{0}\"无法转换成{1}.({2}:{3})", jsonValue.GetText(), type, jsonValue.Start.Line, jsonValue.Start.Column));

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
                errors.Add(string.Format("\"{0}\"无法转换成枚举{1}.({2}:{3})", jsonValue.GetText(), type, jsonValue.Start.Line, jsonValue.Start.Column));

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
                        values.Add(field.Name, ParseJson(field.Type, field.TypeDefined, field.IsArray, field.Attributes, name2jsonNode[jsonKey], errors));
                    else if (field.Attributes.GetAttribute<JsonFileRef>() != null)
                        values.Add(field.Name, null);
                    else
                    {
                        values.Add(field.Name, null);
                        errors.Add(string.Format("\"{0}\"找不到名为{1}的属性.({2}:{3})", jsonValue.GetText(), jsonKey, jsonValue.Start.Line, jsonValue.Start.Column));
                    }
                }
                foreach (var field in type.Fields)
                {
                    ReplaceFromRefFile(field.Name, field.Type, field.TypeDefined, field.IsArray, field.Attributes, values, errors);
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
                        singleRow.Add(field.Name, ParseJson(field.Type, field.TypeDefined, field.IsArray, field.Attributes, name2jsonNode[jsonKey], errors));
                    else if (field.Attributes.GetAttribute<JsonFileRef>() != null)
                        singleRow.Add(field.Name, null);
                    else
                    {
                        singleRow.Add(field.Name, null);
                        errors.Add(string.Format("\"{0}\"找不到名为{1}的属性.({2}:{3})", childNode.GetText(), jsonKey, childNode.Start.Line, childNode.Start.Column));
                    }
                }
                foreach (var field in type.Fields)
                {
                    ReplaceFromRefFile(field.Name, field.Type, field.TypeDefined, field.IsArray, field.Attributes, singleRow, errors);
                }
                tableRows.Add(singleRow);
            }
            return tableRows;
        }

        /// <summary>
        /// 从引用文件替换
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="typeDefined"></param>
        /// <param name="isArray"></param>
        /// <param name="attributes"></param>
        /// <param name="values"></param>
        /// <param name="errors"></param>
        private static void ReplaceFromRefFile(string name, string type, object typeDefined, bool isArray, AttributeTable attributes, Dictionary<string, object> values, List<string> errors)
        {
            var jsonFile = attributes.GetAttribute<JsonFileRef>();
            if (jsonFile != null)
            {
                var filePath = Regex.Replace(jsonFile.filePath, "\\{.*?\\}", (match) =>
                {
                    var key = match.Value.TrimStart('{').TrimEnd('}');
                    if (values.ContainsKey(key))
                        return values[key] + "";
                    else
                        return "null";
                });

                values[name] = null;

                if (!File.Exists(filePath))
                    errors.Add(string.Format("[JsonFileRef(\"{0}\")] 文件未找到.", filePath));
                else
                {
                    var fileNode = GetJsonFile(filePath);
                    if (fileNode != null)
                        values[name] = ParseJson(type, typeDefined, isArray, attributes, fileNode, errors);
                }
            }
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