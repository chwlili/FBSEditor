using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class Struct : Base
    {
        /// <summary>
        /// 元数据列表
        /// </summary>
        public List<Meta> Metas { get; set; } = new List<Meta>();

        /// <summary>
        /// 字段列表
        /// </summary>
        public List<StructField> Fields { get; set; } = new List<StructField>();
    }
}
