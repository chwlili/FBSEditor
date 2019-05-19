namespace FlatBufferData.Model.Attributes
{
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
    public class JsonPath : Attribute
    {
        public string path;

        public JsonPath(string path)
        {
            this.path = path;
        }
    }
}