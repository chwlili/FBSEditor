﻿using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class RpcMethod
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string Param { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        public string Return { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get; set; }


        public List<Meta> Metas { get; set; } = new List<Meta>();
    }
}
