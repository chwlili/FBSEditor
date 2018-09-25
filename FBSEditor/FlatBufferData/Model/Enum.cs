using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class Enum
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 基础类型
        /// </summary>
        public string BaseType { get; set; }

        /// <summary>
        /// 元数据列表
        /// </summary>
        public List<Meta> Metas { get; set; } = new List<Meta>();

        /// <summary>
        /// 特性列表
        /// </summary>
        public AttributeInfo Attributes { get; set; }

        /// <summary>
        /// 字段列表
        /// </summary>
        public List<EnumField> Fields { get; } = new List<EnumField>();
    }
}
