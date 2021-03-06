﻿namespace FlatBufferData.Model.Attributes
{
#pragma warning disable CS3016 // 作为特性参数的数组不符合 CLS

    #region table专用

    [AllowMultiple(false)]
    [AllowTarget(TargetTypeID.Table)]
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

        public Index(string indexName, string fieldName1)
        {
            this.IndexName = indexName;
            this.IndexFields = new string[] { fieldName1 };
        }

        public Index(string indexName, string fieldName1, string fieldName2)
        {
            this.IndexName = indexName;
            this.IndexFields = new string[] { fieldName1, fieldName2 };
        }

        public Index(string indexName, string fieldName1, string fieldName2, string fieldName3)
        {
            this.IndexName = indexName;
            this.IndexFields = new string[] { fieldName1, fieldName2, fieldName3 };
        }

        public Index(string indexName, string fieldName1, string fieldName2, string fieldName3, string fieldName4)
        {
            this.IndexName = indexName;
            this.IndexFields = new string[] { fieldName1, fieldName2, fieldName3, fieldName4 };
        }
    }

    [AllowMultiple(false)]
    [AllowTarget(TargetTypeID.TableField)]
    [ConflictFlag(typeof(Unique))]
    public class Nullable : Attribute
    {
    }

    [AllowMultiple(false)]
    [AllowTarget(TargetTypeID.TableField)]
    [ConflictFlag(typeof(Nullable))]
    [RequiredFieldType(FieldTypeID.BOOL| FieldTypeID.INT| FieldTypeID.FLOAT| FieldTypeID.STRING| FieldTypeID.ENUM)]
    public class Unique : Attribute
    {
    }

    #endregion

    #region Excel相关


    [AllowMultiple(false)]
    [AllowTarget(TargetTypeID.Table | TargetTypeID.TableField)]
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

    #region CSV相关

    [AllowMultiple(false)]
    [AllowTarget(TargetTypeID.Table|TargetTypeID.TableField)]
    public class CSV : Attribute
    {
        public string filePath;
        public int titleRow;
        public int dataBeginRow;
        public string separators = ",";

        public CSV(string filePath)
        {
            this.filePath = filePath;
        }
        public CSV(string filePath, int titleRow)
        {
            this.filePath = filePath;
            this.titleRow = titleRow;
        }
        public CSV(string filePath,int titleRow,int dataBeginRow)
        {
            this.filePath = filePath;
            this.titleRow = titleRow;
            this.dataBeginRow = dataBeginRow;
        }
        public CSV(string filePath, int titleRow, int dataBeginRow, string separators)
        {
            this.filePath = filePath;
            this.titleRow = titleRow;
            this.dataBeginRow = dataBeginRow;
            this.separators = !string.IsNullOrEmpty(separators) ? separators.Replace("\\t", "\t") : ",";
        }
    }

    #endregion

    #region Json相关

    [AllowMultiple(false)]
    [AllowTarget(TargetTypeID.Table)]
    public class JsonFile : Attribute
    {
        public string filePath;

        public JsonFile(string filePath)
        {
            this.filePath = filePath;
        }
    }

    [AllowMultiple(false)]
    [AllowTarget(TargetTypeID.TableField | TargetTypeID.StructField)]
    public class JsonFileRef : Attribute
    {
        public string filePath;

        public JsonFileRef(string filePath)
        {
            this.filePath = filePath;
        }
    }

    [AllowMultiple(false)]
    [AllowTarget(TargetTypeID.Table| TargetTypeID.Struct| TargetTypeID.TableField| TargetTypeID.StructField)]
    public class JsonLiteral : Attribute
    {
        public static JsonLiteral NORMAL = new JsonLiteral();

        public JsonLiteral(){}
    }

    [AllowMultiple(false)]
    [AllowTarget(TargetTypeID.Table|TargetTypeID.TableField|TargetTypeID.StructField)]
    //[RequiredFlag(typeof(JsonFile),typeof(JsonFileRef),typeof(JsonLiteral))]
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
    [AllowTarget(TargetTypeID.TableField)]
    [RequiredArrayField]
    [ConflictFlag(typeof(StructLiteral), typeof(JsonLiteral))]
    public class ArrayLiteral : Attribute
    {
        public string beginning;
        public string separator;
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
    [ConflictFlag(typeof(ArrayLiteral), typeof(JsonLiteral))]
    [AllowTarget(TargetTypeID.Struct|TargetTypeID.StructField|TargetTypeID.TableField)]
    [RequiredFieldType(FieldTypeID.STRUCT)]
    public class StructLiteral : Attribute
    {
        public static StructLiteral NORMAL = new StructLiteral(null, ",", null);

        public string beginning;
        public string separator;
        public string ending;

        public StructLiteral()
        {
        }

        public StructLiteral(string separator)
        {
            this.separator = separator;
        }

        public StructLiteral(string beginning, string separator, string ending)
        {
            this.beginning = beginning;
            this.separator = separator;
            this.ending = ending;
        }
    }

    #endregion


#pragma warning restore CS3016 // 作为特性参数的数组不符合 CLS
}