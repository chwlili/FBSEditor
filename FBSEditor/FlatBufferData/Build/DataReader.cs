using FlatBufferData.Model;
using System;

namespace FlatBufferData.Build
{
    public class DataReader
    {
        public DataReaderFactory Factory { get; set; }

        public int ErrorLogCount { get; private set; }

        protected void AddError(string path,string text,int line,int column)
        {
            ErrorLogCount++;

            if(Factory!=null)
                Factory.AddError(path, text, line, column);
        }

        public virtual void Read(Table schema)
        {
            throw new Exception("未实现的功能!");
        }

        public virtual void Read(Table schema, string text)
        {
            throw new Exception("未实现的功能!");
        }

        public virtual void Read(Struct schema)
        {
            throw new Exception("未实现的功能!");
        }

        public virtual void Read(Struct schema, string text)
        {
            throw new Exception("未实现的功能!");
        }
    }
}
