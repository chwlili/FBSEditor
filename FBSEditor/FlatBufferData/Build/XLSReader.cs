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
    public class XLSReader : DataReader
    {

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
                var ext = Path.GetExtension(filePath).ToLower();

                IWorkbook workbook = null;
                if (ext == ".xlsx")
                    workbook = new XSSFWorkbook(stream);
                else if(ext==".xls")
                    workbook = new HSSFWorkbook(stream);
                else
                {
                    ReportXlsError("未知的文件类型!", filePath);
                    return;
                }

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
                    if (cell == null || cell.CellType == CellType.Blank) continue;
                    if (cell.CellType == CellType.String)
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
                        ReportXlsError("标题行中未找到列 " + fieldName +  "。", filePath, sheetName, titleRowIndex);
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
                            if (cellData != null && cellData.CellType != CellType.Blank)
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
                        var fieldSchemaList = fieldSchema.IsArray;

                        var isUnique = fieldSchema.Attributes.GetAttribte<Unique>() != null;
                        var isIndex = indexKeys.Contains(fieldSchema.DataField);
                        var isNullable = fieldSchema.Attributes.GetAttribte<Model.Attributes.Nullable>() != null ? fieldSchema.Attributes.GetAttribte<Model.Attributes.Nullable>().nullable : true;
                        var defaultValue = fieldSchema.DefaultValue;

                        string fieldError = null;
                        object fieldValue = null;

                        if (IsInteger(fieldSchemaType))
                            fieldValue = GetInteger(cellData, fieldSchemaType, isUnique, isIndex, isNullable, defaultValue, out fieldError);
                        else if (IsFloat(fieldSchemaType))
                            fieldValue = GetFloat(cellData, fieldSchemaType, isUnique, isIndex, isNullable, defaultValue, out fieldError);
                        else if (IsBool(fieldSchemaType))
                            fieldValue = GetBool(cellData, fieldSchemaType, isUnique, isIndex, isNullable, defaultValue, out fieldError);
                        else if (IsString(fieldSchemaType))
                            fieldValue = GetString(cellData, fieldSchemaType, isUnique, isIndex, isNullable, defaultValue, out fieldError);
                        else if (fieldSchema.TypeDefined is Model.Enum)
                            fieldValue = GetEnum(cellData, fieldSchema.TypeDefined as Model.Enum, isUnique, isIndex, isNullable, defaultValue, out fieldError);
                        else
                        {
                            /*
                            if(Factory!=null)
                            {
                                //Factory.ReadData(fieldSchema.TypeDefined,cellData.)
                                fieldSchema.TypeDefined
                            }
                            */
                        }

                        if (fieldError != null)
                            ReportXlsError(fieldError, filePath, sheetName, rowIndex, cellIndex);

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

        #region 单元格数据转换

        private List<string> integerTypes = new List<string>() { "byte", "int8", "ubyte", "uint8", "short", "int16", "ushort", "uint16", "int", "int32", "uint", "uint32", "long", "int64", "ulong", "uint64" };

        private bool IsInteger(string type) { return integerTypes.Contains(type); }

        private object GetInteger(ICell cellData,string fieldSchemaType,bool isUnique,bool isIndex,bool isNullable,object defaultValue,out string error)
        {
            string fieldError = null;
            object fieldValue = null;

            if (cellData == null || cellData.CellType == CellType.Blank)
            {
                if (!isNullable || isUnique)
                    fieldError = String.Format("内容不允许为空!");
                else if (isIndex)
                    fieldError = String.Format("索引字段不允许为空!");
                else
                    fieldValue = defaultValue != null ? defaultValue : 0;
            }
            else if (cellData.CellType == CellType.Numeric)
            {
                if ((fieldSchemaType.Equals("byte") || fieldSchemaType.Equals("int8")) && sbyte.MinValue <= cellData.NumericCellValue && cellData.NumericCellValue <= sbyte.MaxValue)
                    fieldValue = (sbyte)cellData.NumericCellValue;
                else if ((fieldSchemaType.Equals("ubyte") || fieldSchemaType.Equals("uint8")) && byte.MinValue <= cellData.NumericCellValue && cellData.NumericCellValue <= byte.MaxValue)
                    fieldValue = (byte)cellData.NumericCellValue;
                else if ((fieldSchemaType.Equals("short") || fieldSchemaType.Equals("int16")) && short.MinValue <= cellData.NumericCellValue && cellData.NumericCellValue <= short.MaxValue)
                    fieldValue = (short)cellData.NumericCellValue;
                else if ((fieldSchemaType.Equals("ushort") || fieldSchemaType.Equals("uint16")) && ushort.MinValue <= cellData.NumericCellValue && cellData.NumericCellValue <= ushort.MaxValue)
                    fieldValue = (ushort)cellData.NumericCellValue;
                else if (fieldSchemaType.Equals("int") || fieldSchemaType.Equals("int32") && int.MinValue <= cellData.NumericCellValue && cellData.NumericCellValue <= int.MaxValue)
                    fieldValue = (int)cellData.NumericCellValue;
                else if (fieldSchemaType.Equals("uint") || fieldSchemaType.Equals("uint32") && uint.MinValue <= cellData.NumericCellValue && cellData.NumericCellValue <= uint.MaxValue)
                    fieldValue = (uint)cellData.NumericCellValue;
                else if (fieldSchemaType.Equals("long") || fieldSchemaType.Equals("int64") && long.MinValue <= cellData.NumericCellValue && cellData.NumericCellValue <= long.MaxValue)
                    fieldValue = (long)cellData.NumericCellValue;
                else if (fieldSchemaType.Equals("ulong") || fieldSchemaType.Equals("uint64") && ulong.MinValue <= cellData.NumericCellValue && cellData.NumericCellValue <= ulong.MaxValue)
                    fieldValue = (ulong)cellData.NumericCellValue;

                fieldError = fieldValue == null ? "数值范围超出" + fieldSchemaType + "的范围。" : null;
            }
            else
                fieldError = String.Format("内容不是一个有效的{0}", fieldSchemaType);

            error = fieldError;

            return fieldValue;
        }


        private List<string> floatTypes = new List<string>() { "float", "float32", "double", "double64" };

        private bool IsFloat(string type) { return floatTypes.Contains(type); }

        private object GetFloat(ICell cellData, string fieldSchemaType, bool isUnique, bool isIndex, bool isNullable, object defaultValue, out string error)
        {
            string fieldError = null;
            object fieldValue = null;

            if (cellData == null || cellData.CellType == CellType.Blank)
            {
                if (!isNullable || isUnique)
                    fieldError = "内容不允许为空!";
                else if (isIndex)
                    fieldError = "索引字段不允许为空!";
                else
                    fieldValue = defaultValue != null ? defaultValue : 0;
            }
            else if (cellData.CellType == CellType.Numeric)
            {
                if ((fieldSchemaType.Equals("float") || fieldSchemaType.Equals("float32")) && float.MinValue <= cellData.NumericCellValue && cellData.NumericCellValue <= float.MaxValue)
                    fieldValue = (float)cellData.NumericCellValue;
                else if ((fieldSchemaType.Equals("double") || fieldSchemaType.Equals("double64")) && double.MinValue <= cellData.NumericCellValue && cellData.NumericCellValue > double.MaxValue)
                    fieldValue = cellData.NumericCellValue;
            }
            else
                fieldError = String.Format("内容不是一个有效的{0}", fieldSchemaType);

            error = fieldError;

            return fieldValue;
        }

        private bool IsBool(string type)
        {
            return "bool".Equals(type);
        }

        private object GetBool(ICell cellData, string fieldSchemaType, bool isUnique, bool isIndex, bool isNullable, object defaultValue, out string error)
        {
            string fieldError = null;
            object fieldValue = null;

            if (cellData == null || cellData.CellType == CellType.Blank)
            {
                if (!isNullable || isUnique)
                    fieldError = "内容不允许为空!";
                else if (isIndex)
                    fieldError = "索引字段不允许为空!";
                else
                    fieldValue = defaultValue != null ? defaultValue : false;
            }
            else if (cellData.CellType == CellType.Boolean)
                fieldValue = cellData.BooleanCellValue;
            else if (cellData.CellType == CellType.Numeric)
                fieldValue = cellData.NumericCellValue != 0;
            else
                fieldError = String.Format("内容不是一个有效的{0}", fieldSchemaType);

            error = fieldError;

            return fieldValue;
        }

        private bool IsString(string type)
        {
            return "string".Equals(type);
        }

        private object GetString(ICell cellData,string fieldSchemaType,bool isUnique,bool isIndex,bool isNullable,object defaultValue,out string error)
        {
            string fieldError = null;
            object fieldValue = null;

            if (cellData == null || cellData.CellType == CellType.Blank)
            {
                if (!isNullable || isUnique)
                    fieldError = "内容不允许为空!";
                else if (isIndex)
                    fieldError = "索引字段不允许为空!";
                else
                    fieldValue = defaultValue != null ? defaultValue : "";
            }
            else if (cellData.CellType == CellType.String)
                fieldValue = cellData.StringCellValue.ToString();
            else if (cellData.CellType == CellType.Boolean)
                fieldValue = cellData.BooleanCellValue.ToString();
            else if (cellData.CellType == CellType.Numeric)
                fieldValue = cellData.NumericCellValue.ToString();
            else
                fieldError = String.Format("内容不是一个有效的{0}", fieldSchemaType);

            error = fieldError;

            return fieldValue;
        }

        private object GetEnum(ICell cellData, Model.Enum type, bool isUnique,bool isIndex,bool isNullable,object defaultValue,out string error)
        {
            string fieldError = null;
            object fieldValue = null;

            if (cellData == null || cellData.CellType == CellType.Blank)
            {
                if (!isNullable || isUnique)
                    fieldError = "内容不允许为空!";
                else if (isIndex)
                    fieldError = "索引字段不允许为空!";
                //else
                    //fieldValue = defaultValue != null ? defaultValue : "";
            }
            else if (cellData.CellType == CellType.Numeric)
            {
                var enumIndex = (int)cellData.NumericCellValue;
                if (enumIndex < type.Fields.Count)
                    fieldValue = enumIndex;
                else
                    fieldError = "无效的枚举值!";
            }
            else if (cellData.CellType == CellType.String)
            {
                var enumIndex = -1;
                for (var enumItem = 0; enumItem < type.Fields.Count; enumItem++)
                {
                    if (type.Fields[enumItem].Name.Equals(cellData.StringCellValue))
                    {
                        enumIndex = enumItem;
                        fieldValue = enumIndex;
                        break;
                    }
                }
                if (enumIndex == -1)
                    fieldError = "无效的枚举值!";
            }
            else
                fieldError = "无效的枚举值!";

            error = fieldError;

            return fieldValue;
        }

        #endregion

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
