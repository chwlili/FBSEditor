namespace FlatBufferData.Model.Attributes
{
    public class ArraySeparator : Attribute
    {
        /// <summary>
        /// 索引名称
        /// </summary>
        public string splite;

        public ArraySeparator(string splite)
        {
            this.splite = splite;
        }
    }
}
