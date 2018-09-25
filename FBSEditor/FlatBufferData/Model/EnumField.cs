using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class EnumField
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 特性列表
        /// </summary>
        public AttributeInfo Attributes { get; set; }
    }
}
