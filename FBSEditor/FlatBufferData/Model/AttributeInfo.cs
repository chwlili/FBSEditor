using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatBufferData.Model
{
    public class AttributeInfo
    {
        /// <summary>
        /// 特性列表
        /// </summary>
        public List<Attribute> Attributes { get; set; } = new List<Attribute>();

        /// <summary>
        /// 绑定信息
        /// </summary>
        public string BindInfo;

        /// <summary>
        /// 索引表
        /// </summary>
        public List<string[]> IndexTable = new List<string[]>();

        /// <summary>
        /// 可以为空
        /// </summary>
        public bool Nullable;
    }
}
