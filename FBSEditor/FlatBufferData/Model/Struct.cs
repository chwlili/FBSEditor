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
        /// 特性列表
        /// </summary>
        public List<Attribute> Attributes { get; set; } = new List<Attribute>();

        /// <summary>
        /// 字段列表
        /// </summary>
        public List<StructField> Fields { get; } = new List<StructField>();
    }
}
