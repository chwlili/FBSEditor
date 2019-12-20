using Antlr4.Runtime;
using FBSEditor.FlatBufferData.Editor;
using FlatBufferData.Model;
using FlatBufferData.Model.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FlatBufferData.Build
{
    public class CsvUtil
    {
        public static object ParseCSV(string filePath, CSV csv, Table type, AttributeTable attributes)
        {
            var csvLexer = new CsvLexer(new AntlrInputStream(File.ReadAllText(filePath)));
            var csvParser = new CsvParser(new CommonTokenStream(csvLexer));

            csvLexer.separators = csv.separators;
            if (string.IsNullOrEmpty(csvLexer.separators))
                csvLexer.separators = ",";

            var tab = csvParser.csvTab();
            var ast = CreateCsvAST(tab);

            ParseTable(type, ast, csv);

            return null;
        }

        #region CSV数据表

        /// <summary>
        /// 创建CSV的AST树
        /// </summary>
        /// <param name="parseTree">解析树</param>
        /// <returns>CSV结构</returns>
        private static CsvTable CreateCsvAST(CsvParser.CsvTabContext parseTree)
        {
            var table = new CsvTable();
            for (int i = 0; i < parseTree._rows.Count; i++)
            {
                var row = new CsvRow();

                var cols = parseTree._rows[i]._cols;
                for (int j = 0; j < cols.Count; j++)
                {
                    var col = new CsvCol();

                    if (cols[j].txt != null && cols[j].txt._txt != null && cols[j].txt._txt.Count > 0)
                    {
                        IList<IToken> tokens = cols[j].txt._txt;
                        StringBuilder sb = new StringBuilder();
                        foreach (var token in tokens)
                        {
                            sb.Append(token.Text);
                        }
                        col.text = sb.ToString();
                        col.row = tokens[0].Line;
                        col.col = tokens[0].Column;
                        col.start = tokens[0].StartIndex;
                        col.stop = tokens[tokens.Count - 1].StopIndex;
                    }
                    else if (cols[j].str != null && cols[j].str.txt != null)
                    {
                        IToken token = cols[j].str.txt;
                        col.text = token.Text.Trim('"');
                        col.row = token.Line;
                        col.col = token.Column;
                        col.start = token.StartIndex;
                    }
                    else
                    {
                        col.text = "";
                        col.row = cols[j].Start.Line;
                        col.col = cols[j].Start.Column;
                        col.start = cols[j].Start.StartIndex;
                        col.stop = cols[j].Stop.StopIndex;
                    }
                    row.Add(col);
                }
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
                ErrorTracking.LogCsvError(csv.filePath, titleRowIndex, string.Format("找不到标题行({0})",CsvIndex(titleRowIndex)));
                return;
            }

            //验证数据起始行有效性
            if ((dataBeginRowIndex < 0 || dataBeginRowIndex >= dataset.Count))
            {
                ErrorTracking.LogCsvError(csv.filePath, dataBeginRowIndex, string.Format("找不到数据起始行({0})", CsvIndex(dataBeginRowIndex)));
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
                    ErrorTracking.LogCsvError(csv.filePath, titleRowIndex, string.Format("标题行({0})名称重复({1})", CsvIndex(titleRowIndex, i), colName));
                else
                    columnName2ColumnIndex.Add(colName, i);
            }

            //检查标题行是否有有效数据
            if (columnName2ColumnIndex.Count == 0)
            {
                ErrorTracking.LogCsvError(csv.filePath, titleRowIndex, string.Format("标题行({0})没有有效的数据！", CsvIndex(dataBeginRowIndex)));
                return;
            }

            //检测数据表中是否缺少字段
            foreach (var field in table.Fields)
            {
                var fieldName = field.DataField;
                if(!columnName2ColumnIndex.ContainsKey(fieldName))
                {
                    ErrorTracking.LogCsvError(csv.filePath, titleRowIndex, string.Format("标题行{0}缺少名为\"{1}\"的列！", CsvIndex(dataBeginRowIndex), fieldName));
                }
            }


        }

        private static string CsvIndex(int row)
        {
            return (row + 1).ToString();
        }
        private static string CsvIndex(int row,int col)
        {
            return (row + 1) + "," + (col + 1);
        }

        #endregion
    }
}
