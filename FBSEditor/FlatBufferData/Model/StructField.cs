using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class StructField : Base
    {
        private static List<string> integerTypes = new List<string>() { "byte", "int8", "ubyte", "uint8", "short", "int16", "ushort", "uint16", "int", "int32", "uint", "uint32", "long", "int64", "ulong", "uint64" };
        private static List<string> floatTypes = new List<string>() { "float", "float32", "double", "double64" };
        private static List<string> boolTypes = new List<string>() { "bool" };
        private static List<string> stringTypes = new List<string>() { "string" };

        /// <summary>
        /// 元数据列表
        /// </summary>
        public List<Meta> Metas { get; set; } = new List<Meta>();

        /// <summary>
        /// 类型名称
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 类型定义
        /// </summary>
        public object TypeDefined { get; set; }

        //是否为数组
        public bool IsArray { get; set; }

        /// <summary>
        /// 是否可以为空
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; set; }



        public string DataField { get; set; }

        /// <summary>
        /// 是否是整数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsInteger()
        {
            return integerTypes.Contains(Type);
        }

        /// <summary>
        /// 是否是浮点数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsFloat()
        {
            return floatTypes.Contains(Type);
        }

        /// <summary>
        /// 是否是布尔
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsBool()
        {
            return boolTypes.Contains(Type);
        }

        /// <summary>
        /// 是否是字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsString()
        {
            return stringTypes.Contains(Type);
        }

        /// <summary>
        /// 是否是枚举
        /// </summary>
        /// <returns></returns>
        public bool IsEnum()
        {
            return TypeDefined is Enum;
        }

        /// <summary>
        /// 是否是结构
        /// </summary>
        /// <returns></returns>
        public bool IsStruct()
        {
            return TypeDefined is Struct;
        }
    }
}
