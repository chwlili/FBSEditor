using FlatBufferData.Model;
using FlatBufferData.Model.Attributes;
using System;
using System.Collections.Generic;

namespace FlatBufferData.Build
{
    public class BaseUtil : DataReader
    {
        private static List<string> integerTypes = new List<string>() { "byte", "int8", "ubyte", "uint8", "short", "int16", "ushort", "uint16", "int", "int32", "uint", "uint32", "long", "int64", "ulong", "uint64" };
        private static List<string> floatTypes = new List<string>() { "float", "float32", "double", "double64" };
        private static List<string> boolTypes = new List<string>() { "bool" };
        private static List<string> stringTypes = new List<string>() { "string" };

        /// <summary>
        /// 是否是整数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsInteger(string Type)
        {
            return integerTypes.Contains(Type);
        }

        /// <summary>
        /// 是否是浮点数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsFloat(string Type)
        {
            return floatTypes.Contains(Type);
        }

        /// <summary>
        /// 是否是布尔
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBool(string Type)
        {
            return boolTypes.Contains(Type);
        }

        /// <summary>
        /// 是否是字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsString(string Type)
        {
            return stringTypes.Contains(Type);
        }

        /// <summary>
        /// 是否是基础类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBaseType(string type)
        {
            return IsInteger(type) || IsFloat(type) || IsBool(type) || IsString(type);
        }



        /// <summary>
        /// 标量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static object GetScalar(string type, string text)
        {
            if (type.Equals("byte") || type.Equals("int8"))
            {
                sbyte value = 0;
                if (sbyte.TryParse(text, out value)) return value;
            }
            else if (type.Equals("ubyte") || type.Equals("uint8"))
            {
                byte value = 0;
                if (byte.TryParse(text, out value)) return value;
            }
            else if (type.Equals("short") || type.Equals("int16"))
            {
                short value = 0;
                if (short.TryParse(text, out value)) return value;
            }
            else if (type.Equals("ushort") || type.Equals("uint16"))
            {
                ushort value = 0;
                if (ushort.TryParse(text, out value)) return value;
            }
            else if (type.Equals("int") || type.Equals("int32"))
            {
                int value = 0;
                if (int.TryParse(text, out value)) return value;
            }
            else if (type.Equals("uint") || type.Equals("uint32"))
            {
                uint value = 0;
                if (uint.TryParse(text, out value)) return value;
            }
            else if (type.Equals("long") || type.Equals("int64"))
            {
                long value = 0;
                if (long.TryParse(text, out value)) return value;
            }
            else if (type.Equals("ulong") || type.Equals("uint64"))
            {
                ulong value = 0;
                if (ulong.TryParse(text, out value)) return value;
            }
            else if (type.Equals("float") || type.Equals("float32"))
            {
                float value = 0;
                if (float.TryParse(text, out value)) return value;
            }
            else if (type.Equals("double") || type.Equals("double64"))
            {
                double value = 0;
                if (double.TryParse(text, out value)) return value;
            }
            else if (type.Equals("bool"))
            {
                bool value1 = false;
                if (bool.TryParse(text, out value1))
                    return value1;

                double value2 = 0;
                if (double.TryParse(text, out value2))
                    return value2 != 0;
            }
            else if (type.Equals("string"))
            {
                return text;
            }

            return null;
        }


        /// <summary>
        /// 获取标量数组
        /// </summary>
        /// <param name="type"></param>
        /// <param name="texts"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static object GetScalarArray(string type, string[] texts, List<string> errors)
        {
            if (type.Equals("byte") || type.Equals("int8"))
                return GetScalarArray<sbyte>(type, texts, errors);
            else if (type.Equals("ubyte") || type.Equals("uint8"))
                return GetScalarArray<byte>(type, texts, errors);
            else if (type.Equals("short") || type.Equals("int16"))
                return GetScalarArray<short>(type, texts, errors);
            else if (type.Equals("ushort") || type.Equals("uint16"))
                return GetScalarArray<ushort>(type, texts, errors);
            else if (type.Equals("int") || type.Equals("int32"))
                return GetScalarArray<int>(type, texts, errors);
            else if (type.Equals("uint") || type.Equals("uint32"))
                return GetScalarArray<uint>(type, texts, errors);
            else if (type.Equals("long") || type.Equals("int64"))
                return GetScalarArray<long>(type, texts, errors);
            else if (type.Equals("ulong") || type.Equals("uint64"))
                return GetScalarArray<ulong>(type, texts, errors);
            else if (type.Equals("float") || type.Equals("float32"))
                return GetScalarArray<float>(type, texts, errors);
            else if (type.Equals("double") || type.Equals("double64"))
                return GetScalarArray<double>(type, texts, errors);
            else if (type.Equals("bool"))
                return GetScalarArray<bool>(type, texts, errors);
            else if (type.Equals("string"))
                return GetScalarArray<string>(type, texts, errors);

            return null;
        }

        /// <summary>
        /// 获取标量数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="texts"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static T[] GetScalarArray<T>(string type, string[] texts, List<string> errors)
        {
            var list = new List<T>();

            var errorIndexList = new List<int>();
            for (var i = 0; i < texts.Length; i++)
            {
                var value = GetScalar(type, texts[i]);
                if (value != null)
                    list.Add((T)value);
                else
                    errorIndexList.Add(i);
            }

            if (errorIndexList.Count > 0)
                errors.Add(string.Format("列表的第{0}个元素不合法,或不在{1}的数值范围内.", string.Join(",", errorIndexList), type));

            return list.ToArray();
        }



        /// <summary>
        /// 枚举值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static object GetEnum(Model.Enum type, string text)
        {
            var field = type.FindFieldByName(text);
            if (field != null)
                return field.ID;

            int intID = 0;
            if (int.TryParse(text, out intID))
            {
                field = type.FindFieldByID(intID);
                if (field != null)
                    return field.ID;
            }

            double doubleID = 0;
            if (double.TryParse(text, out doubleID))
            {
                field = type.FindFieldByID((int)doubleID);
                if (field != null)
                    return field.ID;
            }

            return null;
        }

        /// <summary>
        /// 枚举值列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="texts"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static object GetEnumArray(Model.Enum type,string[] texts,List<string> errors)
        {
            var list = new List<bool>();

            var errorIndexList = new List<int>();
            for (var i = 0; i < texts.Length; i++)
            {
                var value = GetEnum(type, texts[i]);
                if (value != null)
                    list.Add((bool)value);
                else
                    errorIndexList.Add(i);
            }

            if (errorIndexList.Count > 0)
                errors.Add(string.Format("列表的第{0}个元素不合法,无法解析成枚举:{1}.", string.Join(",", errorIndexList), type.Name));

            return list.ToArray();
        }


        /// <summary>
        /// 获取结构
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static object GetStruct(AttributeTable attributes, Struct type, string text, List<string> errors)
        {
            var format = type.Attributes.GetAttribute<JsonLiteral, StructLiteral>();
            if (format == null)
                format = type.Attributes.GetAttribute<JsonLiteral, StructLiteral>();
            if (format == null)
                format = JsonLiteral.NORMAL;

            if (format is JsonLiteral)
                return GetStruct(format as JsonLiteral, type, text, errors);
            else if (format is StructLiteral)
                return GetStruct(format as StructLiteral, type, text, errors);
            else
                return null;
        }

        /// <summary>
        /// 获取列表格式的结构
        /// </summary>
        /// <param name="literal"></param>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static object GetStruct(StructLiteral literal, Model.Struct type, string text, List<string> errors)
        {
            text = text.Trim();

            //查找自定义的分割规则
            var separator = ",";
            if (literal != null)
            {
                separator = literal.separator;
                if (text.StartsWith(literal.beginning))
                    text = text.Substring(literal.beginning.Length);
                if (text.EndsWith(literal.ending))
                    text = text.Substring(0, text.Length - literal.ending.Length);
            }

            //分割字符串
            var texts = new string[] { };
            if (!string.IsNullOrEmpty(text))
                texts = text.Split(new string[] { separator }, StringSplitOptions.None);

            //解析字符串
            var values = new Dictionary<string, object>();
            for (int i = 0; i < texts.Length; i++)
            {
                var txt = texts[i].Trim();
                if (i < type.Fields.Count)
                {
                    var field = type.Fields[i];
                    var fieldType = field.Type;
                    if (BaseUtil.IsInteger(fieldType) || BaseUtil.IsFloat(fieldType) || BaseUtil.IsBool(fieldType))
                    {
                        object value = BaseUtil.GetScalar(fieldType, txt);
                        if (value != null)
                            values.Add(field.Name, value);
                        else
                            errors.Add(string.Format("第{0}个元素{1}无法解析成{2}。", i, txt, fieldType));
                    }
                    else if (BaseUtil.IsString(fieldType))
                    {
                        values.Add(field.Name, txt);
                    }
                    else if (field.TypeDefined is Model.Enum)
                    {
                        var value = BaseUtil.GetEnum(field.TypeDefined as Model.Enum, txt);
                        if (value != null)
                            values.Add(field.Name, value);
                        else
                            errors.Add(string.Format("第{0}个元素{1}无法解析成{2}。", i, txt, fieldType));
                    }
                    else if (field.TypeDefined is Model.Struct)
                    {
                        var value = GetStruct(field.Attributes, field.TypeDefined as Model.Struct, txt, errors);
                        if (value != null)
                            values.Add(field.Name, value);
                    }
                }
                else
                    errors.Add(string.Format("第{0}个元素将被忽略，因为{1}的字段数量只有{2}个。", i, type.Name, type.Fields.Count));
            }

            for (int i = texts.Length; i < type.Fields.Count; i++)
                errors.Add(String.Format("字段{0}没有对应的数据项，因为数据只有{1}个元素。", type.Fields[i].Name, texts.Length));

            return values;
        }

        /// <summary>
        /// 获取Json格式的结构
        /// </summary>
        /// <param name="literal"></param>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static object GetStruct(JsonLiteral literal, Struct type, string text, List<string> errors)
        {
            return JsonUtil.ParseJsonText(text, literal.path, type, errors);
        }

        /// <summary>
        /// 获取结构数组
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="type"></param>
        /// <param name="texts"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static object GetStructArray(AttributeTable attributes, Model.Struct type, string[] texts, List<string> errors)
        {
            var list = new List<Dictionary<string, object>>();

            for (var i = 0; i < texts.Length; i++)
            {
                var value = GetStruct(attributes, type, texts[i], errors);
                if (value != null)
                    list.Add((Dictionary<string, object>)value);
            }

            return list;
        }
    }
}