﻿using System.Collections.Generic;

namespace FBSEditor.Build
{
    class Enum
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

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
