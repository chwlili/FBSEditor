namespace FlatBufferData.Model
{
    public class UnionField : Base
    {
        /// <summary>
        /// 值
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 类型定义
        /// </summary>
        public object TypeDefined { get; set; }
    }
}
