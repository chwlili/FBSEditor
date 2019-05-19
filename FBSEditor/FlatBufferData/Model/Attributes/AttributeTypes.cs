namespace FlatBufferData.Model.Attributes
{
    #region table专用

    [AllowMultiple(false)]
    [AllowOwner(TargetTypeID.Table)]
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

    [AllowMultiple(false)]
    [AllowOwner(TargetTypeID.TableField)]
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

    [AllowMultiple(false)]
    [AllowOwner(TargetTypeID.TableField)]
    public class Unique : Attribute
    {
    }

    #endregion

    #region Excel相关


    [AllowMultiple(false)]
    [AllowOwner(TargetTypeID.Table | TargetTypeID.TableField)]
    public class XLS : Attribute
    {
        public string filePath;
        public string sheetName;
        public int titleRow;
        public int dataBeginRow;

        public XLS(string filePath, string sheetName)
        {
            this.filePath = filePath;
            this.sheetName = sheetName;
            this.titleRow = 1;
            this.dataBeginRow = this.titleRow + 1;
        }

        public XLS(string filePath, string sheetName, int titleRow)
        {
            this.filePath = filePath;
            this.sheetName = sheetName;
            this.titleRow = titleRow;
            this.dataBeginRow = this.titleRow + 1;
        }

        public XLS(string filePath, string sheetName, int titleRow, int dataBeginRow)
        {
            this.filePath = filePath;
            this.sheetName = sheetName;
            this.titleRow = titleRow;
            this.dataBeginRow = dataBeginRow;
        }
    }

    #endregion

    #region Json相关

    [AllowMultiple(false)]
    [AllowOwner(TargetTypeID.Table)]
    public class JsonFile : Attribute
    {
        public string filePath;

        public JsonFile(string filePath)
        {
            this.filePath = filePath;
        }
    }

    [AllowMultiple(false)]
    [AllowOwner(TargetTypeID.TableField)]
    public class JsonFileRef : Attribute
    {
        public string filePath;

        public JsonFileRef(string filePath)
        {
            this.filePath = filePath;
        }
    }

    [AllowMultiple(false)]
    [AllowOwner(TargetTypeID.Table| TargetTypeID.Struct| TargetTypeID.TableField| TargetTypeID.StructField)]
    public class JsonLiteral : Attribute
    {
        public static JsonLiteral NORMAL = new JsonLiteral();

        public JsonLiteral(){}
    }

    [AllowMultiple(false)]
    [AllowOwner(TargetTypeID.Table|TargetTypeID.TableField|TargetTypeID.StructField)]
    [RequiredType(typeof(JsonFile),typeof(JsonFileRef),typeof(JsonLiteral))]
    public class JsonPath : Attribute
    {
        public string path;

        public JsonPath(string path)
        {
            this.path = path;
        }
    }

    #endregion

    #region 字面量

    [AllowMultiple(false)]
    [AllowOwner(TargetTypeID.TableField)]
    [RequiredArrayField]
    [ConflictType(typeof(StructLiteral), typeof(JsonLiteral))]
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

    [AllowMultiple(false)]
    [AllowOwner(TargetTypeID.Struct|TargetTypeID.StructField|TargetTypeID.TableField)]
    [RequiredArrayField]
    [ConflictType(typeof(ArrayLiteral), typeof(JsonLiteral))]
    public class StructLiteral : Attribute
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

    #endregion
}