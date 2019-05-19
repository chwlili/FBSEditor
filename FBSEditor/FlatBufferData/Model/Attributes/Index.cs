namespace FlatBufferData.Model.Attributes
{
    [AllowMultiple(false)]
    [AllowOwnerAttribute(TargetTypeID.Table)]
    public class Index : Attribute
    {
        /// <summary>
        /// 索引名称
        /// </summary>
        public string IndexName;

        /// <summary>
        /// 索引列表
        /// </summary>
        public string[] IndexFields;

        public Index(string indexName, string[] indexFields)
        {
            this.IndexName = indexName;
            this.IndexFields = indexFields;
        }
    }
}
