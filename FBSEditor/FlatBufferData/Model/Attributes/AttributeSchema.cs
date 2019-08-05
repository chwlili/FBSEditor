using System;

namespace FlatBufferData.Model.Attributes
{
    /// <summary>
    /// 目标类型ID
    /// </summary>
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

    /// <summary>
    /// 允许应用到的目标
    /// </summary>
    public class AllowTarget : System.Attribute
    {
        public TargetTypeID AllowTargets;

        public AllowTarget(TargetTypeID targetType)
        {
            this.AllowTargets = targetType;
        }
    }

    /// <summary>
    /// 是否允许多个应用到同一目标
    /// </summary>
    public class AllowMultiple : System.Attribute
    {
        public bool Allow;

        public AllowMultiple(bool allowMultiple)
        {
            this.Allow = allowMultiple;
        }
    }

    /// <summary>
    /// 字段类型ID
    /// </summary>
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

    /// <summary>
    /// 必需是指定的字段类型
    /// </summary>
    public class RequiredFieldType : System.Attribute
    {
        public FieldTypeID FieldType;

        public RequiredFieldType(FieldTypeID FieldType)
        {
            this.FieldType = FieldType;
        }
    }

    /// <summary>
    /// 必须是数组字段
    /// </summary>
    public class RequiredArrayField : System.Attribute { }

    /// <summary>
    /// 必须是结构字段
    /// </summary>
    public class RequiredStructField : System.Attribute { }

    /// <summary>
    /// 必须的标记
    /// </summary>
    public class RequiredFlag : System.Attribute
    {
        public Type[] RequiredTypes;

        public RequiredFlag(params Type[] requiredTypes)
        {
            this.RequiredTypes = requiredTypes;
        }
    }

    /// <summary>
    /// 冲突的标记
    /// </summary>
    public class ConflictFlag : System.Attribute
    {
        public Type[] ConflictTypes;

        public ConflictFlag(params Type[] conflictTypes)
        {
            this.ConflictTypes = conflictTypes;
        }
    }
}
