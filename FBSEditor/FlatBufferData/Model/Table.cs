using System.Collections.Generic;
using static FlatbufferParser;

namespace FlatBufferData.Model
{
    public class Table
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
        /// 字段列表
        /// </summary>
        public List<TableField> Fields { get; set; } = new List<TableField>();

        /// <summary>
        /// 元数据列表
        /// </summary>
        public List<Meta> Metas { get; set; } = new List<Meta>();

        /// <summary>
        /// 特性列表
        /// </summary>
        public AttributeInfo Attributes { get; set; }
    }
}
