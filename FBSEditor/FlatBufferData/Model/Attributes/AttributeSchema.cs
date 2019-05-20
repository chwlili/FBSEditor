using System;

namespace FlatBufferData.Model.Attributes
{
    [Flags]
    public enum TargetTypeID
    {
        ANY = 0,
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

    public class AllowOwner : System.Attribute
    {
        public TargetTypeID AllowTargets;

        public AllowOwner(TargetTypeID targetType)
        {
            this.AllowTargets = targetType;
        }
    }

    public class AllowMultiple : System.Attribute
    {
        public bool Allow;

        public AllowMultiple(bool allowMultiple)
        {
            this.Allow = allowMultiple;
        }
    }

    public class RequiredArrayField : System.Attribute
    {
    }

    public class RequiredFieldType : System.Attribute
    {
        public FieldTypeID FieldType;

        public RequiredFieldType(FieldTypeID FieldType)
        {
            this.FieldType = FieldType;
        }
    }

    public enum FieldTypeID
    {
        ANY = 0,
        BOOL = 1,
        INT = 2,
        FLOAT = 4,
        STRING = 8,
        ENUM = 16,
        UNION = 32,
        STRUCT = 64,
        TABLE = 128
    }

    public class RequiredStructField : System.Attribute
    {

    }

    public class ConflictFlag : System.Attribute
    {
        public Type[] ConflictTypes;

        public ConflictFlag(params Type[] conflictTypes)
        {
            this.ConflictTypes = conflictTypes;
        }
    }

    public class RequiredFlag : System.Attribute
    {
        public Type[] RequiredTypes;

        public RequiredFlag(params Type[] requiredTypes)
        {
            this.RequiredTypes = requiredTypes;
        }
    }
}
