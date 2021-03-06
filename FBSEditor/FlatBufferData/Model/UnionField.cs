﻿namespace FlatBufferData.Model
{
    public class UnionField : Node
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
        /// 值
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 类型定义
        /// </summary>
        public object TypeDefined { get; set; }

        /// <summary>
        /// 特性列表
        /// </summary>
        public AttributeTable Attributes { get; } = new AttributeTable();
    }
}
