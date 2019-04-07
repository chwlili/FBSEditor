using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class Enum : Base
    {
        /// <summary>
        /// 基础类型
        /// </summary>
        public string BaseType { get; set; }

        /// <summary>
        /// 元数据列表
        /// </summary>
        public List<Meta> Metas { get; set; } = new List<Meta>();

        /// <summary>
        /// 字段列表
        /// </summary>
        public List<EnumField> Fields { get; } = new List<EnumField>();

        public EnumField FindFieldByName(string fieldName)
        {
            foreach(var field in Fields)
            {
                if (field.Name.Equals(fieldName))
                    return field;
            }
            return null;
        }

        public EnumField FindFieldByID(int id)
        {
            foreach (var field in Fields)
            {
                if (field.ID == id)
                {
                    return field;
                }
            }
            return null;
        }
    }
}
