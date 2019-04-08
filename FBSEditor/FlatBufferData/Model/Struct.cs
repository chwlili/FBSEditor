using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class Struct
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
        /// 元数据列表
        /// </summary>
        public List<Meta> Metas { get; set; } = new List<Meta>();

        /// <summary>
        /// 字段列表
        /// </summary>
        public List<StructField> Fields { get; set; } = new List<StructField>();

        /// <summary>
        /// 特性列表
        /// </summary>
        public AttributeTable Attributes { get; } = new AttributeTable();
    }
}
