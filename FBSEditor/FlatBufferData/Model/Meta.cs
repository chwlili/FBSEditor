namespace FlatBufferData.Model
{
    public class Meta : Node
    {
        public Meta(string name,string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
