using System;

namespace FlatBufferData.Model.Attributes
{
    [Flags]
    public enum TargetTypeID
    {
        All = 0,
        Table = 1,
        TableField = 2,
        Struct = 4,
        StructField = 8,
        Enum = 16,
        EnumField = 32,
        Union = 64,
        UnionField = 128,
        Rpc = 256,
        RpcMethod = 512,
    }

    public class AllowOwnerAttribute : System.Attribute
    {
        public TargetTypeID targetType;

        public AllowOwnerAttribute(TargetTypeID targetType)
        {
            this.targetType = targetType;
        }
    }

    public class AllowMultipleAttribute : System.Attribute
    {
        public bool allowMultiple;

        public AllowMultipleAttribute(bool allowMultiple)
        {
            this.allowMultiple = allowMultiple;
        }
    }

    public class RequiredIsArray : System.Attribute
    {

    }

    public class ConflictType : System.Attribute
    {
        public Type[] conflictTypes;

        public ConflictType(params Type[] conflictTypes)
        {
            this.conflictTypes = conflictTypes;
        }
    }

    public class RequiredType : System.Attribute
    {
        public Type[] requiredTypes;

        public RequiredType(params Type[] requiredTypes)
        {
            this.requiredTypes = requiredTypes;
        }
    }
}
