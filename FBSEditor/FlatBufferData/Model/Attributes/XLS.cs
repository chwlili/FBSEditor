namespace FlatBufferData.Model.Attributes
{
    [AllowMultiple(false)]
    [AllowOwnerAttribute(TargetTypeID.Table|TargetTypeID.TableField)]
    public class XLS : Attribute
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath;

        /// <summary>
        /// 表名
        /// </summary>
        public string sheetName;

        /// <summary>
        /// 标题索引
        /// </summary>
        public int titleRow;

        /// <summary>
        /// 数据行开始索引
        /// </summary>
        public int dataBeginRow;

        public XLS(string filePath, string sheetName, int titleRow,int dataBeginRow)
        {
            this.filePath = filePath;
            this.sheetName = sheetName;
            this.titleRow = titleRow;
            this.dataBeginRow = dataBeginRow;
        }
    }
}
