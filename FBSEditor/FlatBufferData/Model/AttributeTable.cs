using System.Collections.Generic;

namespace FlatBufferData.Model
{
    public class AttributeTable
    {
        /// <summary>
        /// 特性列表
        /// </summary>
        private List<Attribute> attributes = new List<Attribute>();

        /// <summary>
        /// 添加特性
        /// </summary>
        /// <param name="attribute"></param>
        public void Add(Attribute attribute)
        {
            attributes.Add(attribute);
        }

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>

        public List<Attribute> GetAll()
        {
            return attributes;
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAttribute<T>() where T : Attribute
        {
            foreach (var item in attributes)
            {
                if (item is T)
                {
                    return item as T;
                }
            }
            return null;
        }

        public bool HasAttribute(System.Type type)
        {
            foreach (var item in attributes)
            {
                if (item.GetType().Equals(type))
                {
                    return true;
                }
            }
            return false;
        }

        public Attribute GetAttribute<T, K>() where T : Attribute where K : Attribute
        {
            foreach (var item in attributes)
            {
                if (item is T)
                    return item;
                if (item is K)
                    return item;
            }
            return null;
        }

        public Attribute GetAttribute<T, K, M>() where T : Attribute where K : Attribute where M:Attribute
        {
            foreach (var item in attributes)
            {
                if (item is T)
                    return item;
                if (item is K)
                    return item;
                if (item is M)
                    return item;
            }
            return null;
        }

        public T[] GetAttributes<T>() where T :Attribute
        {
            List<T> list = new List<T>();
            foreach (var item in attributes)
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
