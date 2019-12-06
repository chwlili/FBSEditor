using Antlr4.Runtime;
using FlatBufferData.Model;
using FlatBufferData.Model.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FlatBufferData.Build
{
    public class CsvUtil
    {
        public static object ParseCSV(string filePath, Table type, AttributeTable attributes, ErrorReport errorReport)
        {
            var csvLexer = new CsvLexer(new AntlrInputStream(File.ReadAllText(filePath)));
            var csvParser = new CsvParser(new CommonTokenStream(csvLexer));
            var tab = csvParser.csvTab();

            return null;
        }

        /*
        private static JsonParser.JsonValueContext ParseFile(string filePath, string text, AttributeTable attributes, ReportWrap errorReport)
        {
            var report = new ReportWrap(filePath, errorReport.report);

            var jsonLexer = new JsonLexer(new AntlrInputStream(text));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            var jsonValue = jsonParser.jsonValue();
            if (jsonValue == null)
            {
                report.OnError(string.Format("无法解析成一个Json."));
                return null;
            }

            var jsonPathAttribute = attributes.GetAttribute<JsonPath>();
            if (jsonPathAttribute == null)
                return jsonValue;

            var jsonPath = jsonPathAttribute.path;
            if (string.IsNullOrEmpty(jsonPath))
                return jsonValue;

            var jsonNode = FindJsonNode(jsonValue, jsonPath);
            if (jsonNode == null)
                report.OnError(string.Format("\"{0}\" 无法过滤出有效的Json节点", jsonPath), jsonValue);

            return jsonNode;
        }*/
    }
}
