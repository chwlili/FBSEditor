using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class Enum
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get; set; }

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

        /// <summary>
        /// 特性列表
        /// </summary>
        public AttributeTable Attributes { get; } = new AttributeTable();


        /// <summary>
        /// 按ID查找字段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EnumField FindFieldByID(int id)
        {
            foreach (var field in Fields)
            {
                if (field.ID == id)
                    return field;
            }
            return null;
        }

        /// <summary>
        /// 按名称查找字段
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public EnumField FindFieldByName(string fieldName)
        {
            foreach(var field in Fields)
            {
                if (field.Name.Equals(fieldName))
                    return field;
            }
            return null;
        }
    }
}
