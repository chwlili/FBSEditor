namespace FlatBufferData.Model.Attributes
{
    [AllowMultiple(false)]
    [AllowOwnerAttribute(TargetTypeID.TableField)]
    [RequiredIsArray]
    [ConflictType(typeof(StructLiteral),typeof(JsonLiteral))]
    public class ArrayLiteral : Attribute
    {
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

        public ArrayLiteral()
        {
        }

        public ArrayLiteral(string separator)
        {
            this.separator = separator;
        }

        public ArrayLiteral(string beginning, string separator, string ending)
        {
            this.beginning = beginning;
            this.separator = separator;
            this.ending = ending;
        }
    }
}
