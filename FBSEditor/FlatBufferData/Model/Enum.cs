using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class Enum : Base
    {
        /// <summary>
        /// 基础类型
        /// </summary>
        public string BaseType { get; set; }

        /// <summary>
        /// 元数据列表
        /// </summary>
        public List<Meta> Metas { get; set; } = new List<Meta>();

        /// <summary>
        /// 字段列表
        /// </summary>
        public List<EnumField> Fields { get; } = new List<EnumField>();
    }
}
