using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatBufferData.Model
{
    public class AttributeInfo
    {
        /// <summary>
        /// 特性列表
        /// </summary>
        public List<Attribute> Attributes { get; set; } = new List<Attribute>();

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAttribte<T>() where T : Attribute
        {
            foreach(var item in Attributes)
            {
                if(item is T)
                {
                    return item as T;
                }
            }
            return null;
        }

        public T[] GetAttributes<T>() where T :Attribute
        {
            List<T> list = new List<T>();
            foreach (var item in Attributes)
            {
                if (item is T)
                {
                    list.Add(item as T);
                }
            }
            return list.ToArray();
        }
    }
}
