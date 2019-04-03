using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class Table : Base
    {
        /// <summary>
        /// 字段列表
        /// </summary>
        public List<TableField> Fields { get; set; } = new List<TableField>();

        /// <summary>
        /// 元数据列表
        /// </summary>
        public List<Meta> Metas { get; set; } = new List<Meta>();
    }
}
