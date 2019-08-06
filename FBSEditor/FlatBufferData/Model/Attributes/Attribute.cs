using System;
using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class Attribute : Node
    {
        public static List<Type> AvailableTypes = new List<Type>();

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 值列表
        /// </summary>
        public List<string> Values { get; set; } = new List<string>();
    }
}
