using FlatBufferData.Model;
using FlatBufferData.Model.Attributes;

namespace FlatBufferData.Build
{
    public class DataReaderFactory
    {
        public delegate void ErrorHandler(string project, string path, string text, int line, int column);

        private string projectName;
        private ErrorHandler errorHandler;
        private int errorCount;

        public DataReaderFactory(string projectName, ErrorHandler errorHandler)
        {
            this.projectName = projectName;
            this.errorHandler = errorHandler;
        }

        public void AddError(string path, string text, int line, int column)
        {
            errorHandler?.Invoke(projectName, path, text, line, column);
        }


        public void ReadData(Document file)
        {
            if (file.RootTable == null) return;

            var xls = file.RootTable.Attributes.GetAttribute<XLS>();
            if (xls != null)
            {
                var reader = new XLSReader();
                reader.Factory = this;
                reader.Read(file.RootTable);
            }
        }

        public void ReadData(Table schema)
        {
            var reader = GetReader(schema.Attributes);
            if (reader != null)
                reader.Read(schema);
        }

        public void ReadData(Table schema, string text)
        {
            var reader = GetReader(schema.Attributes);
            if (reader != null)
                reader.Read(schema, text);
        }

        public void ReadData(Struct schema)
        {
            var reader = GetReader(schema.Attributes);
            if (reader != null)
                reader.Read(schema);
        }

        public void ReadData(Struct schema, string text)
        {
            var reader = GetReader(schema.Attributes);
            if (reader != null)
                reader.Read(schema, text);
        }

        protected virtual DataReader GetReader(AttributeTable attributes)
        {
            DataReader reader = null;
            var xls = attributes.GetAttribute<XLS>();
            if (xls != null)
            {
                reader = new XLSReader();
                reader.Factory = this;
            }
            return reader;
        }
    }
}
