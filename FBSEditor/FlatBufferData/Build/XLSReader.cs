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
            var xls = table.Attributes.GetAttribute<XLS>();
            var filePath = xls.filePath;
            var sheetName = xls.sheetName;
            var titleRowIndex = xls.titleRow - 1;
            var dataBeginRowIndex = xls.dataBeginRow - 1;

            var dataSet = new List<object[]>();
            var dataSetWidth = table.Fields.Count;

            var indexKeys = new HashSet<string>();
            foreach (var index in table.Attributes.GetAttributes<Index>())
            {
                foreach (var dataKey in index.IndexFields)
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
                else if (ext == ".xls")
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
                        ReportXlsError("标题行中未找到列 " + fieldName + "。", filePath, sheetName, titleRowIndex);
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

                        var field = dataKey2FieldSchema[linkName];
                        var isUnique = field.Attributes.GetAttribute<Unique>() != null;
                        var isBaseType = BaseUtil.IsBaseType(field.Type) && field.TypeDefined == null;

                        string fieldError = null;
                        object fieldValue = null;

                        var errors = new List<string>();
                        if (field.IsArray)
                            fieldValue = GetArray(field.Attributes, field.Type, field.TypeDefined, cellData, errors);
                        else
                        {
                            if (field.TypeDefined is Model.Struct)
                                fieldValue = GetStruct(field.Attributes, field.TypeDefined as Struct, cellData, errors);
                            else if (field.TypeDefined is Model.Enum)
                                fieldValue = GetEnum(field.TypeDefined as Model.Enum, cellData, errors);
                            else if(isBaseType)
                            {
                                var isIndex = indexKeys.Contains(field.DataField);
                                var isNullable = field.Attributes.HasAttribute<Model.Attributes.Nullable>();
                                var defaultValue = field.DefaultValue;

                                if (cellData == null || cellData.CellType == CellType.Blank)
                                {
                                    if (!isNullable || isUnique)
                                        errors.Add(String.Format("内容不允许为空!"));
                                    else if (isIndex)
                                        errors.Add(String.Format("索引字段不允许为空!"));
                                    else
                                        fieldValue = defaultValue != null ? defaultValue : 0;
                                }
                                else if(BaseUtil.IsBaseType(field.Type))
                                    fieldValue = GetScalar(field.Type, cellData, errors);
                            }
                        }

                        if (errors.Count > 0)
                            fieldError = string.Join("\n", errors);

                        if (fieldError != null)
                            ReportXlsError(fieldError, filePath, sheetName, rowIndex, cellIndex);

                        dataSetRow[j] = fieldValue;

                        if (isBaseType && isUnique && fieldValue != null)
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



        /// <summary>
        /// 标量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cell"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private object GetScalar(string type, ICell cell, List<string> errors)
        {
            object result = null;
            if (cell.CellType == CellType.Numeric)
            {
                if ((type.Equals("byte") || type.Equals("int8")) && sbyte.MinValue <= cell.NumericCellValue && cell.NumericCellValue <= sbyte.MaxValue)
                    result = (sbyte)cell.NumericCellValue;
                else if ((type.Equals("ubyte") || type.Equals("uint8")) && byte.MinValue <= cell.NumericCellValue && cell.NumericCellValue <= byte.MaxValue)
                    result = (byte)cell.NumericCellValue;
                else if ((type.Equals("short") || type.Equals("int16")) && short.MinValue <= cell.NumericCellValue && cell.NumericCellValue <= short.MaxValue)
                    result = (short)cell.NumericCellValue;
                else if ((type.Equals("ushort") || type.Equals("uint16")) && ushort.MinValue <= cell.NumericCellValue && cell.NumericCellValue <= ushort.MaxValue)
                    result = (ushort)cell.NumericCellValue;
                else if (type.Equals("int") || type.Equals("int32") && int.MinValue <= cell.NumericCellValue && cell.NumericCellValue <= int.MaxValue)
                    result = (int)cell.NumericCellValue;
                else if (type.Equals("uint") || type.Equals("uint32") && uint.MinValue <= cell.NumericCellValue && cell.NumericCellValue <= uint.MaxValue)
                    result = (uint)cell.NumericCellValue;
                else if (type.Equals("long") || type.Equals("int64") && long.MinValue <= cell.NumericCellValue && cell.NumericCellValue <= long.MaxValue)
                    result = (long)cell.NumericCellValue;
                else if (type.Equals("ulong") || type.Equals("uint64") && ulong.MinValue <= cell.NumericCellValue && cell.NumericCellValue <= ulong.MaxValue)
                    result = (ulong)cell.NumericCellValue;
                else if ((type.Equals("float") || type.Equals("float32")) && float.MinValue <= cell.NumericCellValue && cell.NumericCellValue <= float.MaxValue)
                    result = (float)cell.NumericCellValue;
                else if ((type.Equals("double") || type.Equals("double64")) && double.MinValue <= cell.NumericCellValue && cell.NumericCellValue > double.MaxValue)
                    result = cell.NumericCellValue;
                else if (type.Equals("bool"))
                    result = cell.NumericCellValue != 0;
                else if (type.Equals("string"))
                    result = cell.NumericCellValue.ToString();
            }
            else if (cell.CellType == CellType.Boolean)
            {
                if (type.Equals("bool"))
                    result = cell.BooleanCellValue;
                else if (type.Equals("string"))
                    result = cell.BooleanCellValue.ToString();
            }
            else if (cell.CellType == CellType.String)
            {
                if (type.Equals("string"))
                    result = cell.StringCellValue.ToString();
            }

            if (result == null)
            {
                if (BaseUtil.IsInteger(type) || BaseUtil.IsFloat(type))
                    errors.Add(string.Format("内容不是一个有效的{0},或者超出了{0}的有效精度.", type));
                else
                    errors.Add(string.Format("内容不是一个有效的{0}.", type));
            }

            return result;
        }

        /// <summary>
        /// 枚举
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cell"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private object GetEnum(Model.Enum type, ICell cell, List<string> errors)
        {
            object result = null;
            if (cell.CellType == CellType.Numeric)
            {
                var field = type.FindFieldByID((int)cell.NumericCellValue);
                if (field != null)
                    result = field.ID;
            }
            else if (cell.CellType == CellType.String)
            {
                var field = type.FindFieldByName(cell.StringCellValue);
                if (field != null)
                    result = field.ID;
            }

            if(result==null)
                errors.Add("无效的枚举值");

            return result;
        }

        /// <summary>
        /// 结构
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="type"></param>
        /// <param name="cellData"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private object GetStruct(AttributeTable attributes,Struct type,ICell cellData, List<string> errors)
        {
            string text = null;

            if (cellData == null || cellData.CellType == CellType.Blank)
                text = "";
            else if (cellData.CellType == CellType.String)
                text = cellData.StringCellValue;
            else if (cellData.CellType == CellType.Numeric)
                text = cellData.NumericCellValue.ToString();
            else if (cellData.CellType == CellType.Boolean)
                text = cellData.BooleanCellValue.ToString();

            text = text.Trim();

            if (!string.IsNullOrEmpty(text))
                return BaseUtil.GetStruct(attributes, type, text, errors);

            errors.Add(string.Format("内容无法转换成有效的{0}。", type.Name));
            return null;
        }

        /// <summary>
        /// 数组
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="type"></param>
        /// <param name="typeDefined"></param>
        /// <param name="cellData"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private object GetArray(AttributeTable attributes, string type, object typeDefined, ICell cellData, List<string> errors)
        {
            string cellText = null;

            if (cellData == null || cellData.CellType == CellType.Blank)
                cellText = "";
            else if (cellData.CellType == CellType.String)
                cellText = cellData.StringCellValue.Trim();
            else if (cellData.CellType == CellType.Numeric)
                cellText = cellData.NumericCellValue.ToString();
            else if (cellData.CellType == CellType.Boolean)
                cellText = cellData.BooleanCellValue.ToString();

            if (cellText == null)
            {
                cellText = "";
                errors.Add(string.Format("内容无法转换成有效的{0}数组。", type));
            }

            cellText = cellText.Trim();

            var separator = ",";
            var arrayValue = attributes.GetAttribute<ArrayLiteral>();
            if (arrayValue != null)
            {
                separator = arrayValue.separator;
                if (cellText.StartsWith(arrayValue.beginning))
                    cellText = cellText.Substring(arrayValue.beginning.Length);
                if (cellText.EndsWith(arrayValue.ending))
                    cellText = cellText.Substring(0, cellText.Length - arrayValue.ending.Length);
            }

            var texts = string.IsNullOrEmpty(cellText) ? new string[] { } : cellText.Split(new string[] { separator }, StringSplitOptions.None);
            if (BaseUtil.IsBaseType(type))
                return BaseUtil.GetScalarArray(type, texts, errors);
            else if (typeDefined is Model.Enum)
                return BaseUtil.GetEnumArray(typeDefined as Model.Enum, texts, errors);
            else if (typeDefined is Model.Struct)
                return BaseUtil.GetStructArray(attributes, typeDefined as Model.Struct, texts, errors);

            return null;
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
