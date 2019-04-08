namespace FlatBufferData.Model.Attributes
{
    class StructLiteral : Attribute
    {
        public static StructLiteral NORMAL = new StructLiteral(null, ",", null);

        /// <summary>
        /// 起始符
        /// </summary>
        public string beginning;

        /// <summary>
        /// 分隔符
        /// </summary>
        public string separator;

        /// <summary>
        /// 分隔符
        /// </summary>
        public string ending;

        public StructLiteral(string beginning, string separator, string ending)
        {
            this.beginning = beginning;
            this.separator = separator;
            this.ending = ending;
        }
    }
}