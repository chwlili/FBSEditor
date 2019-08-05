using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class Document
    {
        public Document(string path)
        {
            this.Path = path;
        }

        /// <summary>
        /// 导入的文件
        /// </summary>
        public Document[] Includes { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// 主表
        /// </summary>
        public Table RootTable { get; set; }

        /// <summary>
        /// 名称空间
        /// </summary>
        public string NameSpace { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// 文件标识符
        /// </summary>
        public string FileIdentifier { get; set; }

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

        /// <summary>
        /// RPC
        /// </summary>
        public List<Rpc> Rpcs { get; } = new List<Rpc>();
    }
}
