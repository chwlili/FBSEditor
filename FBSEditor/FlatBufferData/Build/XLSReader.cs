using FlatBufferData.Model;
using FlatBufferData.Model.Attributes;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace FlatBufferData.Build
{
    public class XLSReader : DataReader
    {
        private List<string> integerTypes = new List<string>() { "byte", "int8", "ubyte", "uint8", "short", "int16", "ushort", "uint16", "int", "int32", "uint", "uint32", "long", "int64", "ulong", "uint64" };
        private List<string> floatTypes = new List<string>() { "float", "float32", "double", "double64" };

        private bool IsInteger(string type) { return integerTypes.Contains(type); }
        private bool IsFloat(string type) { return floatTypes.Contains(type); }

        public override void Read(Table table)
        {
            var xls = table.Attributes.GetAttribte<XLS>();
            var filePath = xls.filePath;
            var sheetName = xls.sheetName;
            var titleRowIndex = xls.titleRow - 1;
            var dataBeginRowIndex = xls.dataBeginRow - 1;

            var dataSet = new List<object[]>();
            var dataSetWidth = table.Fields.Count;

            var indexKeys = new HashSet<string>();
            foreach (var index in table.Attributes.GetAttributes<Index>())
            {
                foreach (var dataKey in index.fields)
                {
                    foreach (var field in table.Fields)
                    {
                        if (field.Name.Equals(dataKey))
                            indexKeys.Add(field.DataField);
                    }
                }
            }

            var dataKeyList = new List<string>();
            var dataKey2FieldSchema = new Dictionary<string, TableField>();
            foreach (var field in table.Fields)
            {
                dataKeyList.Add(field.DataField);
                dataKey2FieldSchema.Add(field.DataField, field);
            }

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var workbook = new HSSFWorkbook(stream);
                var sheet = workbook.GetSheet(sheetName);
                if (sheet == null)
                {
                    ReportXlsError("文件未找到表:" + sheetName + "。", filePath, sheetName);
                    return;
                }

                var titleRow = sheet.GetRow(titleRowIndex);
                if (titleRow == null)
                {
                    ReportXlsError("标题行不存在，无法导出。", filePath, sheetName, titleRowIndex);
                    return;
                }

                var fieldName2CellIndex = new Dictionary<string, int>();
                var cellIndex2FieldName = new Dictionary<int, string>();
                //title
                for (var i = 0; i <= titleRow.LastCellNum; i++)
                {
                    var cell = titleRow.GetCell(i);
                    if (cell == null || cell.CellType == CellType.BLANK) continue;
                    if (cell.CellType == CellType.STRING)
                    {
                        if (fieldName2CellIndex.ContainsKey(cell.StringCellValue))
                            ReportXlsError("标题列名重复出现。", filePath, sheetName, titleRowIndex, i);
                        else
                        {
                            fieldName2CellIndex.Add(cell.StringCellValue, i);
                            cellIndex2FieldName.Add(i, cell.StringCellValue);
                        }
                    }
                    else
                        ReportXlsError("标题列内容不是字符格式。", filePath, sheetName, titleRowIndex, i);
                }
                foreach (var fieldName in dataKey2FieldSchema.Keys)
                {
                    if (!fieldName2CellIndex.ContainsKey(fieldName))
                        ReportXlsError("标题行中未找到列 " + fieldName + ":" + dataKey2FieldSchema[fieldName].Type + "。", filePath, sheetName, titleRowIndex);
                }
                //data
                var uniqueChecker = new Dictionary<int, Dictionary<object, List<int>>>();
                for (var rowIndex = dataBeginRowIndex; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) { continue; }
                    var colCount = 0;
                    for (var j = 0; j < dataKeyList.Count; j++)
                    {
                        if (fieldName2CellIndex.ContainsKey(dataKeyList[j]))
                        {
                            var cellData = row.GetCell(fieldName2CellIndex[dataKeyList[j]]);
                            if (cellData != null && cellData.CellType != CellType.BLANK)
                                colCount++;
                        }
                    }
                    if (colCount == 0) continue;
                    var dataSetRow = new object[dataSetWidth];
                    for (var j = 0; j < dataKeyList.Count; j++)
                    {
                        var linkName = dataKeyList[j];
                        if (!fieldName2CellIndex.ContainsKey(linkName)) { continue; }

                        var cellIndex = fieldName2CellIndex[linkName];
                        var cellData = row.GetCell(cellIndex);

                        var fieldSchema = dataKey2FieldSchema[linkName];
                        var fieldSchemaName = fieldSchema.Name;
                        var fieldSchemaType = fieldSchema.Type;
                        var isUnique = fieldSchema.Attributes.GetAttribte<Unique>() != null;
                        var isIndex = indexKeys.Contains(fieldSchema.DataField);
                        var isNullable = fieldSchema.Attributes.GetAttribte<Model.Attributes.Nullable>() != null ? fieldSchema.Attributes.GetAttribte<Model.Attributes.Nullable>().nullable : true;
                        var defaultValue = fieldSchema.DefaultValue;
                        object fieldValue = null;
                        if (IsInteger(fieldSchemaType))
                        {
                            if (cellData != null && cellData.CellType == CellType.NUMERIC)
                            {
                                if (fieldSchemaType.Equals("byte") || fieldSchemaType.Equals("int8"))
                                {
                                    if (cellData.NumericCellValue < sbyte.MinValue || cellData.NumericCellValue > sbyte.MaxValue)
                                        ReportXlsError("数值范围超出" + fieldSchemaType + "的范围。", filePath, sheetName, rowIndex, cellIndex);
                                    else
                                        fieldValue = (sbyte)cellData.NumericCellValue;
                                }
                                else if (fieldSchemaType.Equals("ubyte") || fieldSchemaType.Equals("uint8"))
                                {
                                    if (cellData.NumericCellValue < byte.MinValue || cellData.NumericCellValue > byte.MaxValue)
                                        ReportXlsError("数值范围超出" + fieldSchemaType + "的范围。", filePath, sheetName, rowIndex, cellIndex);
                                    else
                                        fieldValue = (byte)cellData.NumericCellValue;
                                }
                                else if (fieldSchemaType.Equals("short") || fieldSchemaType.Equals("int16"))
                                {
                                    if (cellData.NumericCellValue < short.MinValue || cellData.NumericCellValue > short.MaxValue)
                                        ReportXlsError("数值范围超出" + fieldSchemaType + "的范围。", filePath, sheetName, rowIndex, cellIndex);
                                    else
                                        fieldValue = (short)cellData.NumericCellValue;
                                }
                                else if (fieldSchemaType.Equals("ushort") || fieldSchemaType.Equals("uint16"))
                                {
                                    if (cellData.NumericCellValue < ushort.MinValue || cellData.NumericCellValue > ushort.MaxValue)
                                        ReportXlsError("数值范围超出" + fieldSchemaType + "的范围。", filePath, sheetName, rowIndex, cellIndex);
                                    else
                                        fieldValue = (ushort)cellData.NumericCellValue;
                                }
                                else if (fieldSchemaType.Equals("int") || fieldSchemaType.Equals("int32"))
                                {
                                    if (cellData.NumericCellValue < int.MinValue || cellData.NumericCellValue > int.MaxValue)
                                        ReportXlsError("数值范围超出" + fieldSchemaType + "的范围。", filePath, sheetName, rowIndex, cellIndex);
                                    else
                                        fieldValue = (int)cellData.NumericCellValue;
                                }
                                else if (fieldSchemaType.Equals("uint") || fieldSchemaType.Equals("uint32"))
                                {
                                    if (cellData.NumericCellValue < uint.MinValue || cellData.NumericCellValue > uint.MaxValue)
                                        ReportXlsError("数值范围超出" + fieldSchemaType + "的范围。", filePath, sheetName, rowIndex, cellIndex);
                                    else
                                        fieldValue = (uint)cellData.NumericCellValue;
                                }
                                else if (fieldSchemaType.Equals("long") || fieldSchemaType.Equals("int64"))
                                {
                                    if (cellData.NumericCellValue < long.MinValue || cellData.NumericCellValue > long.MaxValue)
                                        ReportXlsError("数值范围超出" + fieldSchemaType + "的范围。", filePath, sheetName, rowIndex, cellIndex);
                                    else
                                        fieldValue = (long)cellData.NumericCellValue;
                                }
                                else if (fieldSchemaType.Equals("ulong") || fieldSchemaType.Equals("uint64"))
                                {
                                    if (cellData.NumericCellValue < ulong.MinValue || cellData.NumericCellValue > ulong.MaxValue)
                                        ReportXlsError("数值范围超出" + fieldSchemaType + "的范围。", filePath, sheetName, rowIndex, cellIndex);
                                    else
                                        fieldValue = (ulong)cellData.NumericCellValue;
                                }
                            }
                            else if (cellData != null && cellData.CellType == CellType.STRING)
                                ReportXlsError(String.Format("\"{0}\"不是一个有效的{1}", cellData.StringCellValue, fieldSchemaType), filePath, sheetName, rowIndex, cellIndex);
                            else if (cellData != null && cellData.CellType == CellType.BOOLEAN)
                                ReportXlsError(String.Format("\"{0}\"不是一个有效的{1}", cellData.BooleanCellValue, fieldSchemaType), filePath, sheetName, rowIndex, cellIndex);
                            else if (cellData != null && (cellData.CellType == CellType.FORMULA || cellData.CellType == CellType.ERROR || cellData.CellType == CellType.Unknown))
                                ReportXlsError(String.Format("内容不是一个有效的{0}", fieldSchemaType), filePath, sheetName, rowIndex, cellIndex);
                            else if (cellData == null || cellData.CellType == CellType.BLANK)
                            {
                                if (!isNullable || isUnique)
                                    ReportXlsError(String.Format("内容不允许为空!"), filePath, sheetName, rowIndex, cellIndex);
                                else if (isIndex)
                                    ReportXlsError(String.Format("索引字段不允许为空!"), filePath, sheetName, rowIndex, cellIndex);
                                else
                                {
                                    if (defaultValue != null)
                                        fieldValue = defaultValue;
                                    else
                                        fieldValue = 0;
                                }
                            }
                        }
                        else if (IsFloat(fieldSchemaType))
                        {
                            if (cellData != null && cellData.CellType == CellType.NUMERIC)
                            {
                                if (fieldSchemaType.Equals("float") || fieldSchemaType.Equals("float32"))
                                {
                                    if (cellData.NumericCellValue < float.MinValue || cellData.NumericCellValue > float.MaxValue)
                                        ReportXlsError("数值范围超出" + fieldSchemaType + "的范围。", filePath, sheetName, rowIndex, cellIndex);
                                    else
                                        fieldValue = (float)cellData.NumericCellValue;
                                }
                                else if (fieldSchemaType.Equals("double") || fieldSchemaType.Equals("double64"))
                                {
                                    if (cellData.NumericCellValue < double.MinValue || cellData.NumericCellValue > double.MaxValue)
                                        ReportXlsError("数值范围超出" + fieldSchemaType + "的范围。", filePath, sheetName, rowIndex, cellIndex);
                                    else
                                        fieldValue = cellData.NumericCellValue;
                                }
                            }
                            else if (cellData != null && cellData.CellType == CellType.STRING)
                                ReportXlsError(String.Format("\"{0}\"不是一个有效的{1}", cellData.StringCellValue, fieldSchemaType), filePath, sheetName, rowIndex, cellIndex);
                            else if (cellData != null && cellData.CellType == CellType.BOOLEAN)
                                ReportXlsError(String.Format("\"{0}\"不是一个有效的{1}", cellData.BooleanCellValue, fieldSchemaType), filePath, sheetName, rowIndex, cellIndex);
                            else if (cellData != null && (cellData.CellType == CellType.FORMULA || cellData.CellType == CellType.ERROR || cellData.CellType == CellType.Unknown))
                                ReportXlsError(String.Format("内容不是一个有效的{0}", fieldSchemaType), filePath, sheetName, rowIndex, cellIndex);
                            else if (cellData == null || cellData.CellType == CellType.BLANK)
                            {
                                if (!isNullable || isUnique)
                                    ReportXlsError(String.Format("内容不允许为空!"), filePath, sheetName, rowIndex, cellIndex);
                                else if (isIndex)
                                    ReportXlsError(String.Format("索引字段不允许为空!"), filePath, sheetName, rowIndex, cellIndex);
                                else
                                {
                                    if (defaultValue != null)
                                        fieldValue = defaultValue;
                                    else
                                        fieldValue = 0;
                                }
                            }
                        }
                        else if ("bool".Equals(fieldSchemaType))
                        {
                            if (cellData != null && cellData.CellType == CellType.BOOLEAN)
                                fieldValue = cellData.BooleanCellValue;
                            else if (cellData != null && cellData.CellType == CellType.NUMERIC)
                                fieldValue = cellData.NumericCellValue != 0;
                            else if (cellData != null && cellData.CellType == CellType.STRING)
                                ReportXlsError(String.Format("\"{0}\"不是一个有效的{1}", cellData.StringCellValue, fieldSchemaType), filePath, sheetName, rowIndex, cellIndex);
                            else if (cellData != null && (cellData.CellType == CellType.FORMULA || cellData.CellType == CellType.ERROR || cellData.CellType == CellType.Unknown))
                                ReportXlsError(String.Format("内容不是一个有效的{0}", fieldSchemaType), filePath, sheetName, rowIndex, cellIndex);
                            else if (cellData != null || cellData.CellType == CellType.BLANK)
                            {
                                if (!isNullable || isUnique)
                                    ReportXlsError(String.Format("内容不允许为空!"), filePath, sheetName, rowIndex, cellIndex);
                                else if (isIndex)
                                    ReportXlsError(String.Format("索引字段不允许为空!"), filePath, sheetName, rowIndex, cellIndex);
                                else
                                {
                                    if (defaultValue != null)
                                        fieldValue = defaultValue;
                                    else
                                        fieldValue = 0;
                                }
                            }
                        }
                        else if ("string".Equals(fieldSchemaType))
                        {
                            if (cellData != null && cellData.CellType == CellType.STRING)
                                fieldValue = cellData.StringCellValue.ToString();
                            else if (cellData != null && cellData.CellType == CellType.BOOLEAN)
                                fieldValue = cellData.BooleanCellValue.ToString();
                            else if (cellData != null && cellData.CellType == CellType.NUMERIC)
                                fieldValue = cellData.NumericCellValue.ToString();
                            else if (cellData != null && (cellData.CellType == CellType.FORMULA || cellData.CellType == CellType.ERROR || cellData.CellType == CellType.Unknown))
                                ReportXlsError(String.Format("内容不是一个有效的{0}", fieldSchemaType), filePath, sheetName, rowIndex, cellIndex);
                            else if (cellData == null || cellData.CellType == CellType.BLANK)
                            {
                                if (!isNullable || isUnique)
                                    ReportXlsError(String.Format("内容不允许为空!"), filePath, sheetName, rowIndex, cellIndex);
                                else if (isIndex)
                                    ReportXlsError(String.Format("索引字段不允许为空!"), filePath, sheetName, rowIndex, cellIndex);
                                else
                                    fieldValue = "";
                            }
                        }
                        else
                        {
                                /*
                            if (fieldSchema.TypeDefined is Model.Enum)
                            {
                                Model.Enum
                            }
                            else
                            {
                                if(Factory!=null)
                                {
                                    //Factory.ReadData(fieldSchema.TypeDefined,cellData.)
                                    fieldSchema.TypeDefined
                                }
                            }*/
                        }

                        dataSetRow[j] = fieldValue;

                        if (isUnique && fieldValue != null)
                        {
                            if (!uniqueChecker.ContainsKey(cellIndex))
                                uniqueChecker.Add(cellIndex, new Dictionary<object, List<int>>());
                            if (!uniqueChecker[cellIndex].ContainsKey(fieldValue))
                                uniqueChecker[cellIndex].Add(fieldValue, new List<int>());
                            uniqueChecker[cellIndex][fieldValue].Add(rowIndex);
                        }
                    }
                    dataSet.Add(dataSetRow);
                }

                foreach (var cellIndex in uniqueChecker.Keys)
                {
                    var formatedCellIndex = FormatXlsColumnName(cellIndex);
                    foreach (var val in uniqueChecker[cellIndex].Keys)
                    {
                        if (uniqueChecker[cellIndex][val].Count > 1)
                        {
                            var txts = new List<string>();
                            var firstRowIndex = uniqueChecker[cellIndex][val][0];
                            foreach (var rowIndex in uniqueChecker[cellIndex][val])
                            {
                                txts.Add((rowIndex + 1).ToString() + "行");
                            }
                            ReportXlsError(String.Format("{0} 列在 {1} 出现了相同的内容:\"{2}\"。", cellIndex2FieldName[cellIndex], string.Join(",", txts), val), filePath, sheetName);
                        }
                    }
                }
            }
        }

        #region 错误记录

        private void ReportXlsError(string text, string filePath)
        {
            AddError(filePath, text + string.Format(" ( {0} )", filePath), 0, 0);
        }
        private void ReportXlsError(string text, string filePath, string sheet)
        {
            AddError(filePath, text + string.Format(" ( {0} # {1} )", filePath, sheet), 0, 0);
        }
        private void ReportXlsError(string text, string filePath, string sheet, int row)
        {
            AddError(filePath, text + string.Format(" ( {0} # {1} : {2} )", filePath, sheet, row + 1), 0, 0);
        }
        private void ReportXlsError(string text, string filePath, string sheet, int row, int col)
        {
            AddError(filePath, text + string.Format(" ( {0} # {1} : {2} )", filePath, sheet, (row + 1) + FormatXlsColumnName(col)), 0, 0);
        }

        /// <summary>
        /// 格式化Xls的列名
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private string FormatXlsColumnName(int index)
        {
            if (index < 24)
                return ((char)('A' + index)).ToString();
            else
                return FormatXlsColumnName((int)Math.Floor(index / 24.0f)) + FormatXlsColumnName(index % 24);
        }

        #endregion
    }
}
