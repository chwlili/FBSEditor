﻿using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using FlatBufferData.Editor;
using FlatBufferData.Model;
using System.Collections.Generic;
using System.IO;

namespace FlatBufferData.Build
{
    public class FBSBuilder
    {
        public delegate void ErrorReport(string projectName,string path,string text,int line,int column);


        public static FBSFile[] Build(string projectName, string[] paths, ErrorReport report)
        {
            var files = new List<FBSFile>();

            foreach (var path in paths)
            {
                if (path.ToLower().EndsWith(Constants.ExtName))
                {
                    var text = File.ReadAllText(path);

                    var lexer = new FlatbufferLexer(new AntlrInputStream(text));
                    var parser = new FlatbufferParser(new CommonTokenStream(lexer));
                    var visitor = new Visitor(projectName, path, GetComments(lexer), report);

                    parser.schema().Accept<int>(visitor);

                    var file = visitor.file;
                    file.Path = path;
                    files.Add(file);
                }
            }

            return files.ToArray();
        }

        private static Dictionary<int, string> GetComments(FlatbufferLexer lexer)
        {
            var commentTable = new Dictionary<int, string>();

            foreach (var token in lexer.GetAllTokens())
            {
                if (token.Type != FlatbufferLexer.COMMENT) { continue; }
                var txt = token.Text;
                if (txt.StartsWith("//"))
                {
                    txt = txt.Substring(2).Trim();
                }
                else if (txt.StartsWith("/*"))
                {
                    txt = txt.Substring(2, txt.Length - 4).Trim().Trim('*').Trim();
                }
                var lines = txt.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    commentTable.Add(token.Line + i, txt);
                }
            }
            lexer.Reset();

            return commentTable;
        }

        private class Visitor : FlatbufferBaseVisitor<int>
        {
            public string projectName;
            public ErrorReport report;
            public FBSFile file;
            public Dictionary<int, string> commentTable;

            public Visitor(string projectName, string path, Dictionary<int, string> commentTable,ErrorReport report)
            {
                this.projectName = projectName;
                this.file = new FBSFile();
                this.file.Path = path;
                this.commentTable = commentTable;
                this.report = report;
            }

            private void ReportError(string text,int line, int column)
            {
                report?.Invoke(projectName, file.Path, text, line, column);
            }

            public override int VisitNamespace([NotNull] FlatbufferParser.NamespaceContext context)
            {
                var tokens = new string[context.IDENT().Length];
                for(int i=0;i<tokens.Length;i++)
                {
                    tokens[i] = context.IDENT(i).GetText();
                }
                file.NameSpace = string.Join(".", tokens);
                return 0;
            }

            /// <summary>
            /// 检查表定义的完整性
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            public override int VisitTable([NotNull] FlatbufferParser.TableContext context)
            {
                var data = new Table();
                data.Name = context.name.Text;
                data.Comment = GetComment(context, context.name);
                data.Metas = GetMetaDatas(context.metaList);
                data.Attributes = GetAttributes(context.attr());

                var fieldsContext = context.tableField();
                if (fieldsContext != null)
                {
                    var nameTable = new HashSet<string>();
                    foreach (var fieldContext in fieldsContext)
                    {
                        var fieldComm = GetComment(fieldContext, fieldContext.fieldName);
                        var fieldName = fieldContext.fieldName != null ? fieldContext.fieldName.Text : "";
                        var fieldType = fieldContext.fieldType != null ? fieldContext.fieldType.GetText() : "";
                        var arrayType = fieldContext.arrayType != null && fieldContext.arrayType.type != null ? fieldContext.arrayType.type.GetText() : "";
                        var fieldValue = fieldContext.fieldValue != null ? fieldContext.fieldValue.GetText() : "";
                        var fieldLink = fieldContext.fieldMap != null ? fieldContext.fieldMap.Text : "";
                        var fieldMetas = GetMetaDatas(fieldContext.metaList);
                        var fieldAttrs = GetAttributes(fieldContext.attr());

                        if (string.IsNullOrEmpty(fieldName))
                        {
                            ReportError("字段名不能为空", fieldContext.Start.Line, fieldContext.Start.Column);
                            continue;
                        }
                        if(string.IsNullOrEmpty(fieldType) && string.IsNullOrEmpty(arrayType))
                        {
                            ReportError("字段类型不能为空", fieldContext.fieldName.Line, fieldContext.fieldName.Column);
                            continue;
                        }
                        if(nameTable.Contains(fieldName))
                        {
                            ReportError(string.Format("字段名称重复定义:\"{0}\"", fieldName), fieldContext.fieldName.Line, fieldContext.fieldName.Column);
                            continue;
                        }

                        var field = new TableField();
                        field.Comment = fieldComm;
                        field.Name = fieldName;
                        field.Type = !string.IsNullOrEmpty(fieldType) ? fieldType : arrayType;
                        field.IsArray = !string.IsNullOrEmpty(arrayType);
                        field.DefaultValue = fieldValue;
                        field.LinkField = !string.IsNullOrEmpty(fieldLink) ? fieldLink : fieldName;
                        field.Metas = fieldMetas;
                        field.Attributes = fieldAttrs;

                        data.Fields.Add(field);
                        nameTable.Add(fieldName);
                    }
                }

                if (file.HasDefine(data.Name))
                {
                    ReportError(string.Format("表名重复定义:\"{0}\"",data.Name), context.name.Line, context.name.Column);
                }
                else
                {
                    file.Tables.Add(data);
                }

                return 0;
            }


            public override int VisitStruct([NotNull] FlatbufferParser.StructContext context)
            {
                var data = new Struct();
                data.Name = context.name.Text;
                data.Comment = GetComment(context, context.name);
                data.Metas = GetMetaDatas(context.metaList);
                data.Attributes = GetAttributes(context.attr());

                var fieldsContext = context.structField();
                if (fieldsContext != null)
                {
                    var nameTable = new HashSet<string>();
                    foreach (var fieldContext in fieldsContext)
                    {
                        var fieldComm = GetComment(fieldContext, fieldContext.fieldName);
                        var fieldName = fieldContext.fieldName != null ? fieldContext.fieldName.Text : "";
                        var fieldType = fieldContext.fieldType != null ? fieldContext.fieldType.GetText() : "";
                        var arrayType = fieldContext.arrayType != null && fieldContext.arrayType.type != null ? fieldContext.arrayType.type.GetText() : "";
                        var fieldValue = fieldContext.fieldValue != null ? fieldContext.fieldValue.GetText() : "";
                        var fieldLink = fieldContext.fieldMap != null ? fieldContext.fieldMap.Text : "";
                        var fieldMetas = GetMetaDatas(fieldContext.metaList);
                        var fieldAttrs = GetAttributes(fieldContext.attr());

                        if (string.IsNullOrEmpty(fieldName))
                        {
                            ReportError("字段名不能为空", fieldContext.Start.Line, fieldContext.Start.Column);
                            continue;
                        }
                        if (string.IsNullOrEmpty(fieldType) && string.IsNullOrEmpty(arrayType))
                        {
                            ReportError("字段类型不能为空", fieldContext.fieldName.Line, fieldContext.fieldName.Column);
                            continue;
                        }
                        if (nameTable.Contains(fieldName))
                        {
                            ReportError(string.Format("字段名称重复定义:\"{0}\"", fieldName), fieldContext.fieldName.Line, fieldContext.fieldName.Column);
                            continue;
                        }

                        var field = new StructField();
                        field.Comment = fieldComm;
                        field.Name = fieldName;
                        field.Type = !string.IsNullOrEmpty(fieldType) ? fieldType : arrayType;
                        field.IsArray = !string.IsNullOrEmpty(arrayType);
                        field.DefaultValue = fieldValue;
                        field.Metas = fieldMetas;
                        field.LinkField = !string.IsNullOrEmpty(fieldLink) ? fieldLink : fieldName;
                        field.Attributes = fieldAttrs;

                        data.Fields.Add(field);
                        nameTable.Add(fieldName);
                    }
                }

                if (file.HasDefine(data.Name))
                {
                    ReportError(string.Format("结构名称重复定义:\"{0}\"", data.Name), context.name.Line, context.name.Column);
                }
                else
                {
                    file.Structs.Add(data);
                }

                return 0;
            }

            public override int VisitEnum([NotNull] FlatbufferParser.EnumContext context)
            {
                var data = new Enum();
                data.Name = context.name.Text;
                data.Metas = GetMetaDatas(context.metaList);
                data.Attributes = GetAttributes(context.attr());
                data.Comment = GetComment(context, context.name);

                var fieldsContext = context.enumField();
                if (fieldsContext != null)
                {
                    var nameTable = new HashSet<string>();
                    foreach (var fieldContext in fieldsContext)
                    {
                        var fieldName = fieldContext.fieldName != null ? fieldContext.fieldName.Text : "";
                        var fieldValue = fieldContext.fieldValue != null ? fieldContext.fieldValue.Text : "";
                        var fieldComm = GetComment(fieldContext, fieldContext.fieldName);
                        var fieldAttrs = GetAttributes(fieldContext.attr());

                        if (string.IsNullOrEmpty(fieldName))
                        {
                            ReportError("字段名不能为空", fieldContext.Start.Line, fieldContext.Start.Column);
                            continue;
                        }
                        if (nameTable.Contains(fieldName))
                        {
                            ReportError(string.Format("字段名称重复定义:\"{0}\"", fieldName), fieldContext.fieldName.Line, fieldContext.fieldName.Column);
                            continue;
                        }

                        var field = new EnumField();
                        field.Name = fieldName;
                        field.Value = fieldValue;
                        field.Comment = fieldComm;
                        field.Attributes = fieldAttrs;

                        data.Fields.Add(field);
                        nameTable.Add(fieldName);
                    }
                }

                if (file.HasDefine(data.Name))
                {
                    ReportError(string.Format("枚举名称重复定义:\"{0}\"", data.Name), context.name.Line, context.name.Column);
                }
                else
                {
                    file.Enums.Add(data);
                }

                return 0;
            }

            public override int VisitUnion([NotNull] FlatbufferParser.UnionContext context)
            {
                var data = new Union();
                data.Name = context.name.Text;
                data.Metas = GetMetaDatas(context.metaList);
                data.Attributes = GetAttributes(context.attr());
                data.Comment = GetComment(context, context.name);

                var fieldsContext = context.unionField();
                if (fieldsContext != null)
                {
                    var nameTable = new HashSet<string>();
                    foreach (var fieldContext in fieldsContext)
                    {
                        var fieldName = fieldContext.fieldName != null ? fieldContext.fieldName.Text : "";
                        var fieldValue = fieldContext.fieldValue != null ? fieldContext.fieldValue.Text : "";
                        var fieldComm = GetComment(fieldContext, fieldContext.fieldName);
                        var fieldAttrs = GetAttributes(fieldContext.attr());

                        if (string.IsNullOrEmpty(fieldName))
                        {
                            ReportError("字段名不能为空", fieldContext.Start.Line, fieldContext.Start.Column);
                            continue;
                        }
                        if (nameTable.Contains(fieldName))
                        {
                            ReportError(string.Format("字段名称重复定义:\"{0}\"", fieldName), fieldContext.fieldName.Line, fieldContext.fieldName.Column);
                            continue;
                        }

                        var field = new UnionField();
                        field.Name = fieldName;
                        field.Value = fieldValue;
                        field.Comment = fieldComm;
                        field.Attributes = fieldAttrs;

                        data.Fields.Add(field);
                        nameTable.Add(fieldName);
                    }
                }

                if (file.HasDefine(data.Name))
                {
                    ReportError(string.Format("联合名称重复定义:\"{0}\"", data.Name), context.name.Line, context.name.Column);
                }
                else
                {
                    file.Unions.Add(data);
                }

                return 0;
            }

            public override int VisitRpc([NotNull] FlatbufferParser.RpcContext context)
            {
                var data = new Rpc();
                data.Name = context.name.Text;
                data.Attributes = GetAttributes(context.attr());
                data.Comment = GetComment(context, context.name);

                var fieldsContext = context.rpcField();
                if (fieldsContext != null)
                {
                    var nameTable = new HashSet<string>();
                    foreach (var fieldContext in fieldsContext)
                    {
                        var fieldName = fieldContext.fieldName != null ? fieldContext.fieldName.Text : "";
                        var fieldParam = fieldContext.fieldParam != null ? fieldContext.fieldParam.Text : "";
                        var fieldReturn = fieldContext.fieldReturn != null ? fieldContext.fieldReturn.Text : "";
                        var fieldComm = GetComment(fieldContext, fieldContext.fieldName);
                        var fieldMetas = GetMetaDatas(fieldContext.metaList);
                        var fieldAttrs = GetAttributes(fieldContext.attr());

                        if (string.IsNullOrEmpty(fieldName))
                        {
                            ReportError("方法名不能为空", fieldContext.Start.Line, fieldContext.Start.Column);
                            continue;
                        }
                        if (nameTable.Contains(fieldName))
                        {
                            ReportError(string.Format("方法名称重复定义:\"{0}\"", fieldName), fieldContext.fieldName.Line, fieldContext.fieldName.Column);
                            continue;
                        }

                        var field = new RpcMethod();
                        field.Name = fieldName;
                        field.Param = fieldParam;
                        field.Return = fieldReturn;
                        field.Comment = fieldComm;
                        field.Metas = fieldMetas;
                        field.Attributes = fieldAttrs;

                        data.Fields.Add(field);
                        nameTable.Add(fieldName);
                    }
                }

                if (file.HasDefine(data.Name))
                {
                    ReportError(string.Format("RPC名称重复定义:\"{0}\"", data.Name), context.name.Line, context.name.Column);
                }
                else
                {
                    file.Rpcs.Add(data);
                }
                return 0;
            }

            private List<Meta> GetMetaDatas(FlatbufferParser.MetadataContext metaList)
            {
                var fieldMetas = new List<Meta>();
                if (metaList != null)
                {
                    foreach (var meta in metaList.metadataField())
                    {
                        var metaName = meta.metaName != null ? meta.metaName.Text : "";
                        var metaValue = meta.metaValue != null ? meta.metaValue.GetText() : "";
                        if (string.IsNullOrEmpty(metaName))
                        {
                            ReportError("元数据键不能为空", meta.Start.Line, meta.Start.Column);
                            continue;
                        }
                        fieldMetas.Add(new Meta(metaName, metaValue));
                    }
                }
                return fieldMetas;
            }

            private List<Attribute> GetAttributes(FlatbufferParser.AttrContext[] context)
            {
                var attributes = new List<Attribute>();
                foreach (var item in context)
                {
                    var name = item.key != null ? item.key.Text : null;
                    var values = new List<string>();
                    if (string.IsNullOrEmpty(name))
                    {
                        ReportError("特性名称不能为空", item.Start.Line, item.Start.Column);
                        continue;
                    }
                    foreach (var value in item.attrField())
                    {
                        values.Add(value.GetText());
                    }

                    var attribute = new Attribute();
                    attribute.Name = name;
                    attribute.Values = values;
                    attributes.Add(attribute);
                }
                return attributes;
            }

            private string GetComment(ParserRuleContext context, IToken token)
            {
                if (token == null) { return null; }

                var startLine = token.Line;
                var stopLine = context.Start.Line - 1;

                while (startLine > 0)
                {
                    if (commentTable.ContainsKey(startLine))
                    {
                        return commentTable[startLine];
                    }
                    startLine--;
                    if (startLine < stopLine) { break; }
                }
                return null;
            }
        }
    }
}