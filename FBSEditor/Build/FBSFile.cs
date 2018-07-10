using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBSEditor.Build
{
    class FBSFile
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 名称空间
        /// </summary>
        public string NameSpace { get; set; }

        /// <summary>
        /// 表列表
        /// </summary>
        public List<Table> Tables { get; } = new List<Table>();

        public bool HasDefine(string name)
        {
            foreach(var item in Tables)
            {
                if(name.Equals(item.Name))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
