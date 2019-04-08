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
                        var isUnique = fieldSchema.Attributes.GetAttribute<Unique>() != null;
                        var isIndex = indexKeys.Contains(fieldSchema.DataField);
                        var isNullable = fieldSchema.Attributes.GetAttribute<Model.Attributes.Nullable>() != null ? fieldSchema.Attributes.GetAttribute<Model.Attributes.Nullable>().nullable : true;
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
                            var arrayValue = fieldSchema.Attributes.GetAttribute<ArrayLiteral>();
                            if (arrayValue != null)
                            {
                                separator = arrayValue.separator;
                                if (cellText.StartsWith(arrayValue.beginning))
                                    cellText = cellText.Substring(arrayValue.beginning.Length);
                                if (cellText.EndsWith(arrayValue.ending))
                                    cellText = cellText.Substring(0, cellText.Length - arrayValue.ending.Length);
                            }

                            var cellTextParts = string.IsNullOrEmpty(cellText) ? new string[] { } : cellText.Split(new string[] { separator }, StringSplitOptions.None);
                            if (BaseUtil.IsInteger(fieldSchemaType))
                                fieldValue = GetIntegerList(fieldSchemaType, cellTextParts, out fieldError);
                            else if (BaseUtil.IsFloat(fieldSchemaType))
                                fieldValue = GetFloatList(fieldSchemaType, cellTextParts, out fieldError);
                            else if (BaseUtil.IsBool(fieldSchemaType))
                                fieldValue = GetBoolList(fieldSchemaType, cellTextParts, out fieldError);
                            else if (BaseUtil.IsString(fieldSchemaType))
                                fieldValue = GetStringList(fieldSchemaType, cellTextParts, out fieldError);
                            else
                            {
                                if(fieldSchema.TypeDefined is Model.Enum)
                                    fieldValue = GetEnumList(fieldSchema.TypeDefined as Model.Enum, cellTextParts, out fieldError);
                            }
                        }
                        else
                        {
                            var errors = new List<string>();
                            if (fieldSchema.TypeDefined is Model.Enum)
                                fieldValue = GetEnum(fieldSchema.TypeDefined as Model.Enum, cellData, errors);
                            else if (fieldSchema.TypeDefined is Model.Struct)
                                fieldValue = GetStruct(fieldSchema.Attributes, fieldSchema.TypeDefined as Struct, cellData, errors);
                            else
                            {
                                if (cellData == null || cellData.CellType == CellType.Blank)
                                {
                                    if (!isNullable || isUnique)
                                        errors.Add(String.Format("内容不允许为空!"));
                                    else if (isIndex)
                                        errors.Add(String.Format("索引字段不允许为空!"));
                                    else
                                        fieldValue = defaultValue != null ? defaultValue : 0;
                                }
                                else
                                    fieldValue = GetScalar(fieldSchemaType, cellData, errors);
                            }
                            if (errors.Count > 0)
                                fieldError = string.Join("\n", errors);
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


        #region 单值

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
                return GetStruct(attributes, type, text, errors);

            errors.Add(string.Format("内容无法转换成有效的{0}。", type.Name));
            return null;
        }


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
                errors.Add(string.Format("内容无法转换成有效的{0}数组。", type));
                return null;
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

            var cellTextParts = string.IsNullOrEmpty(cellText) ? new string[] { } : cellText.Split(new string[] { separator }, StringSplitOptions.None);

            if (BaseUtil.IsInteger(type))
                fieldValue = GetIntegerList(type, cellTextParts, out fieldError);
            else if (BaseUtil.IsFloat(type))
                fieldValue = GetFloatList(type, cellTextParts, out fieldError);
            else if (BaseUtil.IsBool(type))
                fieldValue = GetBoolList(type, cellTextParts, out fieldError);
            else if (BaseUtil.IsString(type))
                fieldValue = GetStringList(type, cellTextParts, out fieldError);
            else
            {
                if (typeDefined is Model.Enum)
                    fieldValue = GetEnumList(typeDefined as Model.Enum, cellTextParts, out fieldError);
            }


            var isInteger = BaseUtil.IsInteger(type);
            var isFloat = BaseUtil.IsFloat(type);
            var isBool = BaseUtil.IsBool(type);
            var isString = BaseUtil.IsString(type);
            var isBase = isInteger || isFloat || isBool || isString;

            var isEnum = typeDefined is Model.Enum;
            var isTable = typeDefined is Model.Table;
            var isStruct = typeDefined is Model.Struct;

            if(isBase)
            {

            }
        }



        #endregion



        #region 通用
        /// <summary>
        /// 获取结构
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private object GetStruct(AttributeTable attributes, Struct type, string text, List<string> errors)
        {
            var format = type.Attributes.GetAttribute<JsonLiteral, StructLiteral>();
            if (format == null)
                format = type.Attributes.GetAttribute<JsonLiteral, StructLiteral>();

            if (format == null)
                return GetStruct(JsonLiteral.NORMAL, type, text, errors);
            else if (format is JsonLiteral)
                return GetStruct(format as JsonLiteral, type, text, errors);
            else if (format is StructLiteral)
                return GetStruct(format as StructLiteral, type, text, errors);
            else
                return null;
        }

        /// <summary>
        /// 获取Json格式的结构
        /// </summary>
        /// <param name="literal"></param>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private object GetStruct(JsonLiteral literal, Struct type, string text, List<string> errors)
        {
            return JsonUtil.ParseJsonText(text, literal.path, type, errors);
        }

        /// <summary>
        /// 获取列表格式的结构
        /// </summary>
        /// <param name="literal"></param>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private object GetStruct(StructLiteral literal, Struct type, string text, List<string> errors)
        {
            text = text.Trim();

            //查找自定义的分割规则
            var separator = ",";
            if (literal != null)
            {
                separator = literal.separator;
                if (text.StartsWith(literal.beginning))
                    text = text.Substring(literal.beginning.Length);
                if (text.EndsWith(literal.ending))
                    text = text.Substring(0, text.Length - literal.ending.Length);
            }

            //分割字符串
            var texts = new string[] { };
            if (!string.IsNullOrEmpty(text))
                texts = text.Split(new string[] { separator }, StringSplitOptions.None);

            //解析字符串
            var values = new Dictionary<string, object>();
            for (int i = 0; i < texts.Length; i++)
            {
                var txt = texts[i].Trim();
                if (i < type.Fields.Count)
                {
                    var field = type.Fields[i];
                    var fieldType = field.Type;
                    if (BaseUtil.IsInteger(fieldType) || BaseUtil.IsFloat(fieldType) || BaseUtil.IsBool(fieldType))
                    {
                        object value = BaseUtil.GetScalar(fieldType, txt);
                        if (value != null)
                            values.Add(field.Name, value);
                        else
                            errors.Add(string.Format("第{0}个元素{1}无法解析成{2}。", i, txt, fieldType));
                    }
                    else if (BaseUtil.IsString(fieldType))
                    {
                        values.Add(field.Name, txt);
                    }
                    else if (field.TypeDefined is Model.Enum)
                    {
                        var value = BaseUtil.GetEnum(field.TypeDefined as Model.Enum, txt);
                        if (value != null)
                            values.Add(field.Name, value);
                        else
                            errors.Add(string.Format("第{0}个元素{1}无法解析成{2}。", i, txt, fieldType));
                    }
                    else if (field.TypeDefined is Struct)
                    {
                        var value = GetStruct(field.Attributes, field.TypeDefined as Struct, txt, errors);
                        if (value != null)
                            values.Add(field.Name, value);
                    }
                }
                else
                    errors.Add(string.Format("第{0}个元素将被忽略，因为{1}的字段数量只有{2}个。", i, type.Name, type.Fields.Count));
            }

            for (int i = texts.Length; i < type.Fields.Count; i++)
                errors.Add(String.Format("字段{0}没有对应的数据项，因为数据只有{1}个元素。", type.Fields[i].Name, texts.Length));

            return values;
        }

        #endregion


        #region 列表

        private object GetIntegerList(string type, string[] texts, out string error)
        {
            var errors = new List<int>();

            object result = null;
            if (type.Equals("byte") || type.Equals("int8"))
            {
                var array = new sbyte[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!sbyte.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (type.Equals("ubyte") || type.Equals("uint8"))
            {
                var array = new byte[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!byte.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (type.Equals("short") || type.Equals("int16"))
            {
                var array = new short[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!short.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (type.Equals("ushort") || type.Equals("uint16"))
            {
                var array = new ushort[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!ushort.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (type.Equals("int") || type.Equals("int32"))
            {
                var array = new int[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!int.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (type.Equals("uint") || type.Equals("uint32"))
            {
                var array = new uint[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!uint.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (type.Equals("long") || type.Equals("int64"))
            {
                var array = new long[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!long.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (type.Equals("ulong") || type.Equals("uint64"))
            {
                var array = new ulong[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!ulong.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (type.Equals("float") || type.Equals("float32"))
            {
                var array = new float[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!float.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }
            else if (type.Equals("double") || type.Equals("double64"))
            {
                var array = new double[texts.Length];
                for (var i = 0; i < texts.Length; i++) { if (!double.TryParse(texts[i], out array[i])) errors.Add(i); }
                result = array;
            }

            error = errors.Count > 0 ? string.Format("第{0}个列表元素不合法，或不在{1}的数值范围内，已当作0处理。", string.Join(",", errors), type) : null;

            return result;
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

        private object GetStringList(string fieldSchemaType, string[] texts, out string error)
        {
            var array = new object[texts.Length];
            for (var i = 0; i < texts.Length; i++) { array[i] = texts[i]; }

            error = null;

            return array;
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
