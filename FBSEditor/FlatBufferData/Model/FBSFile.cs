using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class FBSFile
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

        /// <summary>
        /// 结构表
        /// </summary>
        public List<Struct> Structs { get; } = new List<Struct>();

        /// <summary>
        /// 枚举表
        /// </summary>
        public List<Enum> Enums { get; } = new List<Enum>();

        /// <summary>
        /// 联合表
        /// </summary>
        public List<Union> Unions { get; } = new List<Union>();

        public List<Rpc> Rpcs { get; } = new List<Rpc>();

        public bool HasDefine(string name)
        {
            foreach (var item in Tables) { if (name.Equals(item.Name)) { return true; } }
            foreach (var item in Structs) { if (name.Equals(item.Name)) { return true; } }
            foreach (var item in Enums) { if (name.Equals(item.Name)) { return true; } }
            foreach (var item in Unions) { if (name.Equals(item.Name)) { return true; } }
            foreach (var item in Rpcs) { if (name.Equals(item.Name)) { return true; } }
            return false;
        }
    }
}
