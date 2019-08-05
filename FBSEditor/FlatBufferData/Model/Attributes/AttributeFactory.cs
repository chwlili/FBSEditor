using System.Collections.Generic;

namespace FlatBufferData.Model.Attributes
{
    public class AttributeFactory
    {
        /// <summary>
        /// 特性架构表
        /// </summary>
        private static Dictionary<string, System.Type> name2schema = null;

        /// <summary>
        /// 获取特性架构
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static System.Type GetAttributeSchema(string name)
        {
            if (name2schema == null)
            {
                name2schema = new Dictionary<string, System.Type>();
                name2schema.Add(typeof(XLS).Name, typeof(XLS));
                name2schema.Add(typeof(JsonFile).Name, typeof(JsonFile));
                name2schema.Add(typeof(JsonFileRef).Name, typeof(JsonFileRef));
                name2schema.Add(typeof(JsonLiteral).Name, typeof(JsonLiteral));
                name2schema.Add(typeof(JsonPath).Name, typeof(JsonPath));
                name2schema.Add(typeof(ArrayLiteral).Name, typeof(ArrayLiteral));
                name2schema.Add(typeof(StructLiteral).Name, typeof(StructLiteral));
                name2schema.Add(typeof(Index).Name, typeof(Index));
                name2schema.Add(typeof(Nullable).Name, typeof(Nullable));
                name2schema.Add(typeof(Unique).Name, typeof(Unique));
            }

            if (name2schema.ContainsKey(name))
                return name2schema[name];
            else
                return null;
        }
    }
}
