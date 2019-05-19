namespace FlatBufferData.Model.Attributes
{
    [AllowMultiple(false)]
    [AllowOwnerAttribute(TargetTypeID.TableField)]
    public class Nullable : Attribute
    {
        /// <summary>
        /// 是否可以为空
        /// </summary>
        public bool nullable;

        public Nullable(bool nullable)
        {
            this.nullable = nullable;
        }
    }
}
