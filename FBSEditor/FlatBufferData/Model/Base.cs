using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class Base
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
            foreach (var item in Attributes)
            {
                if (item is T)
                {
                    return item as T;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取特性列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] GetAttributes<T>() where T : Attribute
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
