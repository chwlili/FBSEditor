using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class StructField
    {
        /// <summary>
        /// 元数据列表
        /// </summary>
        public List<Meta> Metas { get; set; } = new List<Meta>();

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 类型定义
        /// </summary>
        public object TypeDefined { get; set; }

        //是否为数组
        public bool IsArray { get; set; }

        /// <summary>
        /// 是否可以为空
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; set; }



        public string DataField { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 特性列表
        /// </summary>
        public AttributeInfo Attributes { get; set; }
    }
}
