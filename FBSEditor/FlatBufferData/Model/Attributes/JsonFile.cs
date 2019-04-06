namespace FlatBufferData.Model.Attributes
{
    class JsonFile : Attribute
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath;

        /// <summary>
        /// 根节点路径
        /// </summary>
        public string rootNodePath;

        public JsonFile(string filePath, string rootNodePath)
        {
            this.filePath = filePath;
            this.rootNodePath = rootNodePath;
        }
    }
}