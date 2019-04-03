using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class Union : Base
    {
        /// <summary>
        /// 字段列表
        /// </summary>
        public List<UnionField> Fields { get; } = new List<UnionField>();

        /// <summary>
        /// 元数据列表
        /// </summary>
        public List<Meta> Metas { get; set; } = new List<Meta>();
    }
}
