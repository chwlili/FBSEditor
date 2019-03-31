namespace FlatBufferData.Model.Attributes
{
    class Json : Attribute
    {
        /// <summary>
        /// 起始符
        /// </summary>
        public string path;

        public Json(string path)
        {
            this.path = path;
        }
    }
}