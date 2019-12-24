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
                                        ErrorTracking.LogCsvError(csv.filePath, i, columnIndex, string.Format("列{0}不允许为空!", fieldName));
                                    if (isUnique)
                                        ErrorTracking.LogCsvError(csv.filePath, i, columnIndex, string.Format("列{0}有唯一性约束，不允许为空！", fieldName));
                                    if (isIndex)
                                        ErrorTracking.LogCsvError(csv.filePath, i, columnIndex, string.Format("列{0}有索引，不允许为空！", fieldName));

                                    scalarValue = BaseUtil.GetDefaultValue(field.Type, fieldDefaultValue);
                                }
                                else
                                {
                                    scalarValue = BaseUtil.GetScalar(field.Type, columnText);

                                    if (scalarValue == null)
                                        ErrorTracking.LogCsvError(csv.filePath, i, columnIndex, string.Format("'{0}'无法转换成一个'{1}'", columnText, field.Type));
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
