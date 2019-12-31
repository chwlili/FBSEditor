using Antlr4.Runtime;
using FBSEditor.FlatBufferData.Editor;
using FlatBufferData.Model;
using FlatBufferData.Model.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using static CsvParser;

namespace FlatBufferData.Build
{
    public class CsvUtil
    {
        public static object ParseCSV(string filePath, CSV csv, Table type, AttributeTable attributes)
        {
            var csvLexer = new CsvLexer(new AntlrInputStream(File.ReadAllText(filePath)));
            var csvParser = new CsvParser(new CommonTokenStream(csvLexer));

            //var tokens = csvLexer.GetAllTokens();
            //return null;
            //csvLexer.separators = csv.separators;
            //if (string.IsNullOrEmpty(csvLexer.separators))
            //    csvLexer.separators = ",";

            csvLexer.IgnoreSpace = true;
            csvParser.IgnoreSpace = true;

            var tab = csvParser.csvTab();
            var ast = CreateCsvAST(filePath, tab);

            ParseTable(type, ast, csv);

            return null;
        }

        #region CSV数据表

        /// <summary>
        /// 创建CSV的AST树
        /// </summary>
        /// <param name="parseTree">解析树</param>
        /// <returns>CSV结构</returns>
        private static CsvTable CreateCsvAST(string filePath, CsvParser.CsvTabContext parseTree)
        {
            var table = new CsvTable();
            
            for (int i = 0; i < parseTree._rows.Count; i++)
            {
                var row = new CsvRow();
                row.isEmpty = false;

                var emptyCount = 0;
                var cols = parseTree._rows[i]._cols;
                for (int j = 0; j < cols.Count + 1; j++)
                {
                    var col = new CsvCol();

                    ParserRuleContext colField = null;
                    CsvTxtContext txtField = null;

                    if(j<cols.Count)
                    {
                        colField = cols[j];
                        txtField = cols[j].content;
                    }
                    else
                    {
                        colField = parseTree._rows[i].end;
                        txtField = colField != null ? parseTree._rows[i].end.content : null;
                    }

                    if (txtField != null && txtField._txt.Count > 0)
                    {
                        IList<IToken> tokens = txtField._txt;

                        StringBuilder sb = new StringBuilder();
                        foreach (var token in tokens) { sb.Append(token.Text); }

                        col.text = sb.ToString();
                        col.row = tokens[0].Line;
                        col.col = tokens[0].Column;
                        col.start = tokens[0].StartIndex;
                        col.stop = tokens[tokens.Count - 1].StopIndex;
                    }
                    else
                    {
                        col.text = string.Empty;
                        col.row = colField.Start.Line;
                        col.col = colField.Start.Column;
                        col.start = colField.Start.StartIndex;
                        col.stop = colField.Stop.StartIndex;
                    }
                    row.Add(col);

                    if (string.IsNullOrEmpty(col.text))
                        emptyCount++;
                }

                row.isEmpty = emptyCount == row.Count;
                table.Add(row);
            }
            return table;
        }

        /// <summary>
        /// CSV表
        /// </summary>
        private class CsvTable : List<CsvRow>
        {
        }

        /// <summary>
        /// CSV行
        /// </summary>
        private class CsvRow :List<CsvCol>
        {
            public bool isEmpty;
        }

        /// <summary>
        /// CSV列
        /// </summary>
        private class CsvCol
        {
            public string text;
            public int row;
            public int col;
            public int start;
            public int stop;
        }

        #endregion

        #region 解析Table

        private static void ParseTable(Table table, CsvTable dataset, CSV csv)
        {
            var titleRowIndex = csv.titleRow - 1;
            var dataBeginRowIndex = csv.dataBeginRow - 1;
            var columnName2ColumnIndex = new Dictionary<string, int>();

            //验证标题行有效性
            if (titleRowIndex < 0 || titleRowIndex >= dataset.Count)
            {
                LogError(csv.filePath, titleRowIndex, -1, "标题行不存在！");
                return;
            }

            //验证数据起始行有效性
            if ((dataBeginRowIndex < 0 || dataBeginRowIndex >= dataset.Count))
            {
                LogError(csv.filePath, dataBeginRowIndex, -1, "数据起始行不存在！");
                return;
            }

            //检查标题行是否有重复的列名
            CsvRow titleRow = dataset[titleRowIndex];
            for(int i=0;i<titleRow.Count;i++)
            {
                string colName = titleRow[i].text != null ? titleRow[i].text.Trim() : "";
                if (string.IsNullOrEmpty(colName))
                    continue;

                if (columnName2ColumnIndex.ContainsKey(colName))
                    LogError(csv.filePath, titleRowIndex, i, string.Format("标题行有重复列名:{0}！", colName));
                else
                    columnName2ColumnIndex.Add(colName, i);
            }

            //检查标题行是否有有效数据
            if (columnName2ColumnIndex.Count == 0)
            {
                LogError(csv.filePath, titleRowIndex, -1, "标题行没有有效的数据！");
                return;
            }

            //检测数据表中是否缺少字段
            foreach (var field in table.Fields)
            {
                var fieldName = field.DataField;
                if(!columnName2ColumnIndex.ContainsKey(fieldName))
                    LogError(csv.filePath, titleRowIndex,-1, string.Format("标题行缺少名为\"{0}\"的列！", fieldName));
            }

            //找出所有带索引的列
            var indexKeys = new HashSet<string>();
            foreach (var index in table.Attributes.GetAttributes<Index>())
            {
                foreach (var indexKey in index.IndexFields)
                {
                    foreach (var field in table.Fields)
                    {
                        if (field.Name.Equals(indexKey))
                            indexKeys.Add(field.DataField);
                    }
                }
            }

            //转换数据
            for (int i = dataBeginRowIndex; i < dataset.Count; i++)
            {
                //忽略空行
                if (dataset[i].isEmpty)
                {
                    LogWarning(csv.filePath, i, -1, "整行内容为空，忽略！");
                    continue;
                }

                foreach (var field in table.Fields)
                {
                    string fieldName = field.DataField;
                    object fieldValue = null;
                    object fieldDefaultValue = field.DefaultValue;

                    if (!columnName2ColumnIndex.ContainsKey(fieldName) 
                        || (columnName2ColumnIndex[fieldName] < 0 || columnName2ColumnIndex[fieldName] >= dataset[i].Count) )
                    {
                        //缺少对应的数据列 | 列索引超出数据行总列数
                        if (BaseUtil.IsBaseType(field.Type))
                            fieldValue = BaseUtil.GetDefaultValue(field.Type, fieldDefaultValue);
                    }
                    else
                    {
                        bool isIndex = indexKeys.Contains(fieldName);
                        bool isUnique = field.Attributes.GetAttribute<Unique>() != null;
                        bool isNullable = field.Attributes.HasAttribute<Nullable>();

                        int columnIndex = columnName2ColumnIndex[fieldName];
                        CsvCol column = dataset[i][columnIndex];
                        string columnText = column.text;
                        bool isEmpty = string.IsNullOrEmpty(columnText);

                        if(BaseUtil.IsBaseType(field.Type))
                        {
                            if (field.IsArray)
                            {

                            }
                            else
                            {
                                object scalarValue = null;

                                if (isEmpty)
                                {
                                    if (!isNullable)
                                        LogError(csv.filePath, i, columnIndex, string.Format("列{0}不允许为空！", fieldName));
                                    if (isUnique)
                                        LogError(csv.filePath, i, columnIndex, string.Format("列{0}有唯一性约束，不允许为空！", fieldName));
                                    if (isIndex)
                                        LogError(csv.filePath, i, columnIndex, string.Format("列{0}有索引，不允许为空！", fieldName));

                                    scalarValue = BaseUtil.GetDefaultValue(field.Type, fieldDefaultValue);
                                }
                                else
                                {
                                    scalarValue = BaseUtil.GetScalar(field.Type, columnText);

                                    if (scalarValue == null)
                                        LogError(csv.filePath, i, columnIndex, string.Format("'{0}'无法转换成一个'{1}'！", columnText, field.Type));
                                }
                            }
                        }
                        else if(field.TypeDefined is Model.Struct)
                        {

                        }
                        else if(field.TypeDefined is Model.Enum)
                        {

                        }
                    }
                }
            }
        }

        /// <summary>
        /// 解析常量数组
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        private object ParseScalarArray(string json, string type)
        {
            var jsonLexer = new JsonLexer(new AntlrInputStream(json));
            var jsonParser = new JsonParser(new CommonTokenStream(jsonLexer));

            var jsonValue = jsonParser.jsonValue();
            if (jsonValue == null)
            {
                ErrorTracking.LogError("", "无法解析成一个Json.", 0, 0);
                return null;
            }

            if (jsonValue.arraryValue != null)
            {
                foreach (var element in jsonValue.arraryValue._arrayElement)
                {

                }
            }

            return null;
        }

        private void ParseScalarValue()
        {

        }

        #endregion

        #region 错误处理部分

        /// <summary>
        /// 记录错误
        /// </summary>
        public static void LogError(string filePath, int row, int col, string text)
        {
            ErrorTracking.LogError(filePath, filePath + " <" + FormatIndex(row, col) + "> " + text, -1, -1);
        }
        /// <summary>
        /// 记录警告
        /// </summary>
        public static void LogWarning(string filePath, int row, int col, string text)
        {
            ErrorTracking.LogWarning(filePath, filePath + " <" + FormatIndex(row, col) + "> " + text, -1, -1);
        }

        /// <summary>
        /// 格式化索引
        /// </summary>
        private static string FormatIndex(int row)
        {
            return (row + 1).ToString();
        }

        /// <summary>
        /// 格式化索引
        /// </summary>
        private static string FormatIndex(int row, int col)
        {
            return (row + 1) + "," + (col == -1 ? "*" : FormatColumnName(col));
        }

        /// <summary>
        /// 格式化列名
        /// </summary>
        private static string FormatColumnName(int colIndex)
        {
            if (colIndex < 24)
                return ((char)('A' + colIndex)).ToString();
            else
                return FormatColumnName((int)System.Math.Floor(colIndex / 24.0f)) + FormatColumnName(colIndex % 24);
        }

        #endregion
    }
}
