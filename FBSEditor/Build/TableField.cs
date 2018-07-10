using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBSEditor.Build
{
    class TableField
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

        //是否为数组
        public bool IsArray { get; set; }

        /// <summary>
        /// 是否可以为空
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }



        public string LinkFieldName { get; set; }
    }
}
