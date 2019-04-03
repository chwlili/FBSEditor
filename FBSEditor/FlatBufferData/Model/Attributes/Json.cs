namespace FlatBufferData.Model.Attributes
{
    class JsonLiteral : Attribute
    {
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