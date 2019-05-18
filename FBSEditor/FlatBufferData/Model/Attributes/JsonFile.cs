using System.Text.RegularExpressions;

namespace FlatBufferData.Model.Attributes
{
    public class JsonFile : Attribute
    {
        public string filePath;

        static JsonFile()
        {

        }

        public JsonFile(string filePath)
        {
            this.filePath = filePath;
        }
    }

    public class JsonFileRef : Attribute
    {
        public string filePath;

        public JsonFileRef(string filePath)
        {
            this.filePath = filePath;
        }
    }

    public class JsonPath : Attribute
    {
        public string path;

        public JsonPath(string path)
        {
            this.path = path;
        }
    }

    public class JsonLiteral : Attribute
    {
        public static JsonLiteral NORMAL = new JsonLiteral(null);

        /// <summary>
        /// 起始符
        /// </summary>
        public string path;

        public JsonLiteral(string path)
        {
            this.path = path;
        }
    }
}