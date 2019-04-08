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
        /// 获取枚举值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static object GetEnum(Model.Enum type,string text)
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
            if(double.TryParse(text,out doubleID))
            {
                field = type.FindFieldByID((int)doubleID);
                if (field != null)
                    return field.ID;
            }

            return null;
        }




        public static object GetScalarArray(string type, string[] texts, List<string> errorList)
        {
            var errors = new List<int>();

            if (type.Equals("byte") || type.Equals("int8"))
                return GetNumberArray<sbyte>(type, texts, errorList);
            else if (type.Equals("ubyte") || type.Equals("uint8"))
                return GetNumberArray<byte>(type, texts, errorList);
            else if (type.Equals("short") || type.Equals("int16"))
                return GetNumberArray<short>(type, texts, errorList);
            else if (type.Equals("ushort") || type.Equals("uint16"))
                return GetNumberArray<ushort>(type, texts, errorList);
            else if (type.Equals("int") || type.Equals("int32"))
                return GetNumberArray<int>(type, texts, errorList);
            else if (type.Equals("uint") || type.Equals("uint32"))
                return GetNumberArray<uint>(type, texts, errorList);
            else if (type.Equals("long") || type.Equals("int64"))
                return GetNumberArray<long>(type, texts, errorList);
            else if (type.Equals("ulong") || type.Equals("uint64"))
                return GetNumberArray<ulong>(type, texts, errorList);
            else if (type.Equals("float") || type.Equals("float32"))
                return GetNumberArray<float>(type, texts, errorList);
            else if (type.Equals("double") || type.Equals("double64"))
                return GetNumberArray<double>(type, texts, errorList);

            return null;
        }

        /// <summary>
        /// 获取数字数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="texts"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static T[] GetNumberArray<T>(string type, string[] texts, List<string> errors)
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
    }
}