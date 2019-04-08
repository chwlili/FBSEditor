using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class Rpc
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
        public List<RpcMethod> Fields { get; set; } = new List<RpcMethod>();

        /// <summary>
        /// 特性列表
        /// </summary>
        public AttributeTable Attributes { get; } = new AttributeTable();

    }
}
