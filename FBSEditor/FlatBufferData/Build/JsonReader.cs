using FlatBufferData.Model;
using Antlr4.Runtime;
using FlatBufferData.Model;
using FlatBufferData.Model.Attributes;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace FlatBufferData.Build
{
    public class JsonReader : DataReader
    {
        public override void Read(Struct schema)
        {
            base.Read(schema);
        }
    }
}