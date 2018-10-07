using System.Collections.Generic;

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
        /// 参数的类型定义
        /// </summary>
        public object ParamTypeDefined { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        public string Return { get; set; }

        /// <summary>
        /// 返回值的类型定义
        /// </summary>
        public object ReturnTypeDefined { get; set; }

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
        public AttributeInfo Attributes { get; set; }
    }
}
