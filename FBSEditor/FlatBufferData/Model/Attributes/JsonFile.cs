namespace FlatBufferData.Model.Attributes
{
    public class JsonFile : Attribute
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath;

        public JsonFile(string filePath)
        {
            this.filePath = filePath;
        }
    }

    public class JsonPath : Attribute
    {
        /// <summary>
        /// 根节点路径
        /// </summary>
        public string path;

        public JsonPath(string path)
        {
            this.path = path;
        }
    }
}