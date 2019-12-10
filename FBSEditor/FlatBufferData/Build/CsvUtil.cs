using Antlr4.Runtime;
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
        public static object ParseCSV(string filePath, CSV csv, Table type, AttributeTable attributes, ErrorReport errorReport)
        {
            var csvLexer = new CsvLexer(new AntlrInputStream(File.ReadAllText(filePath)));
            var csvParser = new CsvParser(new CommonTokenStream(csvLexer));

            csvLexer.separators = csv.separators;
            if (string.IsNullOrEmpty(csvLexer.separators))
                csvLexer.separators = ",";

            var tab = csvParser.csvTab();
            var ast = CreateCsvAST(tab);

            ParseTable(type, ast, csv, errorReport);

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

        private static void ParseTable(Table table, CsvTable dataset, CSV csv, ErrorReport errorReport)
        {
            int titleRowIndex = csv.titleRow - 1;
            int dataBeginRowIndex = csv.dataBeginRow - 1;

            //验证标题行有效性
            if (titleRowIndex < 0 || titleRowIndex >= dataset.Count)
            {
                errorReport.Invoke(csv.filePath, "指定的标题行("+csv.titleRow+")超出了数据表的范围!", 0, 0, 0, 0);
                return;
            }

            //验证数据起始行有效性
            if ((dataBeginRowIndex < 0 || dataBeginRowIndex >= dataset.Count))
            {
                errorReport.Invoke(csv.filePath, "指定的数据起始行(" + csv.dataBeginRow + ")超出了数据表的范围!", 0, 0, 0, 0);
                return;
            }

            //收集数据表列名
            Dictionary<string, int> columnName2ColumnIndex = new Dictionary<string, int>();
            CsvRow titleRow = dataset[titleRowIndex];
            for(int i=0;i<titleRow.Count;i++)
            {
                string colName = titleRow[i].text != null ? titleRow[i].text.Trim() : "";
                if (string.IsNullOrEmpty(colName))
                    continue;

                if (columnName2ColumnIndex.ContainsKey(colName))
                    errorReport.Invoke(csv.filePath, "重复的标题名" + colName + "(" + (titleRowIndex + 1) + "," + (i + 1) + "/" + (titleRowIndex + 1) + "," + (columnName2ColumnIndex[colName] + 1) + ")", 0, 0, 0, 0);
                else
                    columnName2ColumnIndex.Add(colName, i);
            }
            if (columnName2ColumnIndex.Count == 0)
            {
                errorReport.Invoke(csv.filePath, "指定的标题行(" + csv.titleRow + "),没有有效的数据！", 0, 0, 0, 0);
                return;
            }


        }

        #endregion
    }
}
