namespace FlatBufferData.Model.Attributes
{
    public class Index : Attribute
    {
        /// <summary>
        /// 索引名称
        /// </summary>
        public string name;

        /// <summary>
        /// 索引列表
        /// </summary>
        public string[] fields;

        public Index(string name, string[] fields)
        {
            this.name = name;
            this.fields = fields;
        }
    }
}
