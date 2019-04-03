using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class Rpc : Base
    {
        /// <summary>
        /// 字段列表
        /// </summary>
        public List<RpcMethod> Fields { get; set; } = new List<RpcMethod>();
    }
}
