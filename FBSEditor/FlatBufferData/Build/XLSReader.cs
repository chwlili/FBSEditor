﻿using FlatBufferData.Model;
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

                        var fieldSchema = dataKey2FieldSchema[linkName];
                        var fieldSchemaName = fieldSchema.Name;
                        var fieldSchemaType = fieldSchema.Type;

                        var isArray = fieldSchema.IsArray;
                        var isUnique = fieldSchema.Attributes.GetAttribte<Unique>() != null;
                        var isIndex = indexKeys.Contains(fieldSchema.DataField);
                        var isNullable = fieldSchema.Attributes.GetAttribte<Model.Attributes.Nullable>() != null ? fieldSchema.Attributes.GetAttribte<Model.Attributes.Nullable>().nullable : true;
                        var defaultValue = fieldSchema.DefaultValue;

                        string fieldError = null;
                        object fieldValue = null;

                        if (isArray)
                        {
                            string cellText = null;

                            if (cellData == null || cellData.CellType == CellType.Blank)
                                cellText = "";
                            else if (cellData.CellType == CellType.String)
                                cellText = cellData.StringCellValue.Trim();
                            else if (cellData.CellType == CellType.Numeric)
                                cellText = cellData.NumericCellValue.ToString();

                            if (cellText == null)
                            {
                                cellText = "";
                                fieldError = string.Format("内容无法转换成有效的{0}数组。", fieldSchemaType);
                            }

                            cellText = cellText.Trim();

                            var separator = ",";
                            var arrayValue = fieldSchema.Attributes.GetAttribte<ArraySeparator>();
                            if (arrayValue != null)
                                separator = arrayValue.splite;

                            var cellTextParts = string.IsNullOrEmpty(cellText) ? new string[] { } : cellText.Split(new string[] { separator }, StringSplitOptions.None);
                            if (IsInteger(fieldSchemaType))
                                fieldValue = GetIntegerList(fieldSchemaType, cellTextParts, out fieldError);
                            else if (IsFloat(fieldSchemaType))
                                fieldValue = GetFloatList(fieldSchemaType, cellTextParts, out fieldError);
                            else if (IsBool(fieldSchemaType))
                                fieldValue = GetBoolList(fieldSchemaType, cellTextParts, out fieldError);
                            else if (IsString(fieldSchemaType))
                                fieldValue = GetStringList(fieldSchemaType, cellTextParts, out fieldError);
                            else
                            {
                                if(fieldSchema.TypeDefined is Model.Enum)
                                    fieldValue = GetEnumList(fieldSchema.TypeDefined as Model.Enum, cellTextParts, out fieldError);
                            }
                        }
                        else
                        {
                            if (cellData == null || cellData.CellType == CellType.Blank)
                            {
                                if (!isNullable || isUnique)
                                    fieldError = String.Format("内容不允许为空!");
                                else if (isIndex)
                                    fieldError = String.Format("索引字段不允许为空!");
                                else
                                    fieldValue = defaultValue != null ? defaultValue : 0;
                            }
                            else
                            {
                                if (IsInteger(fieldSchemaType))
                                    fieldValue = GetInteger(cellData, fieldSchemaType, isUnique, isIndex, isNullable, defaultValue, out fieldError);
                                else if (IsFloat(fieldSchemaType))
                                    fieldValue = GetFloat(cellData, fieldSchemaType, isUnique, isIndex, isNullable, defaultValue, out fieldError);
                                else if (IsBool(fieldSchemaType))
                                    fieldValue = GetBool(cellData, fieldSchemaType, isUnique, isIndex, isNullable, defaultValue, out fieldError);
                                else if (IsString(fieldSchemaType))
                                    fieldValue = GetString(cellData, fieldSchemaType, isUnique, isIndex, isNullable, defaultValue, out fieldError);
                                else
                                {
                                    if (fieldSchema.TypeDefined is Model.Enum)
                                        fieldValue = GetEnum(cellData, fieldSchema.TypeDefined as Model.Enum, isUnique, isIndex, isNullable, defaultValue, out fieldError);
                                    else if(fieldSchema.TypeDefined is Model.Struct)
                                    {
                                        // struct
                                    }
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
                                }
                            }
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


        #region 整数

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

        private object GetIntegerList(string fieldSchemaType, string[] texts, out string error)
        {
            var errors = new List<int>();

            object result = null;
            if (fieldSchemaType.Equals("byte") || fieldSchemaType.Equals("int8"))
            {
                var array = new sbyte[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!sbyte.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (fieldSchemaType.Equals("ubyte") || fieldSchemaType.Equals("uint8"))
            {
                var array = new byte[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!byte.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (fieldSchemaType.Equals("short") || fieldSchemaType.Equals("int16"))
            {
                var array = new short[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!short.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (fieldSchemaType.Equals("ushort") || fieldSchemaType.Equals("uint16"))
            {
                var array = new ushort[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!ushort.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (fieldSchemaType.Equals("int") || fieldSchemaType.Equals("int32"))
            {
                var array = new int[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!int.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (fieldSchemaType.Equals("uint") || fieldSchemaType.Equals("uint32"))
            {
                var array = new uint[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!uint.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (fieldSchemaType.Equals("long") || fieldSchemaType.Equals("int64"))
            {
                var array = new long[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!long.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (fieldSchemaType.Equals("ulong") || fieldSchemaType.Equals("uint64"))
            {
                var array = new ulong[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!ulong.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }

            error = errors.Count > 0 ? string.Format("第{0}个列表元素不合法，或不在{1}的数值范围内，已当作0处理。", string.Join(",", errors), fieldSchemaType) : null;

            return result;
        }

        #endregion

        #region 浮点数
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

        private object GetFloatList(string fieldSchemaType, string[] texts, out string error)
        {
            var errors = new List<int>();

            object result = null;
            if (fieldSchemaType.Equals("float") || fieldSchemaType.Equals("float32"))
            {
                var array = new float[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!float.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (fieldSchemaType.Equals("double") || fieldSchemaType.Equals("double64"))
            {
                var array = new double[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!double.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }

            error = errors.Count > 0 ? string.Format("第{0}个列表元素不合法，或不在{1}的数值范围内，已当作0处理。", string.Join(",", errors), fieldSchemaType) : null;

            return result;
        }

        #endregion

        #region 布尔

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
        private object GetBoolList(string fieldSchemaType, string[] texts, out string error)
        {
            var errors = new List<int>();

            var array = new bool[texts.Length];

            for (var i = 0; i < texts.Length; i++)
            {
                double value = 0;
                if (double.TryParse(texts[i], out value))
                    array[i] = value != 0;
                else if (!bool.TryParse(texts[i], out array[i]))
                    errors.Add(i);
            }

            error = errors.Count > 0 ? string.Format("第{0}个列表元素不合法，已当作false处理。", string.Join(",", errors), fieldSchemaType) : null;

            return array;
        }

        #endregion

        #region 字符串

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

        private object GetStringList(string fieldSchemaType, string[] texts, out string error)
        {
            var array = new object[texts.Length];
            for (var i = 0; i < texts.Length; i++) { array[i] = texts[i]; }

            error = null;

            return array;
        }

        #endregion



        #region 枚举

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
                var fined = false;

                var id = (int)cellData.NumericCellValue;
                for (var i = 0; i < type.Fields.Count; i++)
                {
                    if (type.Fields[i].ID == id)
                    {
                        fieldValue = id;
                        fined = true;
                        break;
                    }
                }

                if (!fined)
                {
                    fieldValue = 0;
                    fieldError = "无效的枚举值!";
                }
            }
            else if (cellData.CellType == CellType.String)
            {
                var fined = false;
                for (var i = 0; i < type.Fields.Count; i++)
                {
                    if (type.Fields[i].Name.Equals(cellData.StringCellValue))
                    {
                        fieldValue = type.Fields[i].ID;
                        fined = true;
                        break;
                    }
                }
                if (!fined)
                {
                    fieldValue = 0;
                    fieldError = "无效的枚举值!";
                }
            }
            else
            {
                fieldValue = 0;
                fieldError = "无效的枚举值!";
            }

            error = fieldError;

            return fieldValue;
        }

        private object GetEnumList(Model.Enum type, string[] texts, out string error)
        {
            var errors = new List<int>();

            var array = new int[texts.Length];

            for (var i = 0; i < texts.Length; i++)
            {
                var enumValue = 0l;
                var enumValueValidate = false;
                if (long.TryParse(texts[i], out enumValue))
                    enumValueValidate = true;

                var fined = false;
                var finedID = 0;
                for(var j=0;j<type.Fields.Count;j++)
                {
                    var field = type.Fields[j];
                    if (field.Name.Equals(texts[i].Trim()) || (enumValueValidate && enumValue == field.ID))
                    {
                        fined = true;
                        finedID = field.ID;
                        break;
                    }
                }

                if (fined)
                    array[i] = finedID;
                else
                    errors.Add(i);
            }

            error = errors.Count > 0 ? string.Format("第{0}个列表元素不是有效的{1}枚举值，已当作0处理。", string.Join(",", errors), type.Name) : null;

            return array;
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