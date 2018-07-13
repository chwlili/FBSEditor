﻿using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.IO;

namespace FBSEditor.Build
{
    public class FBSBuilder
    {
        public BuildCommandPackage pack;
        public string projectFileName;
        public string[] paths;

        public FBSBuilder(BuildCommandPackage pack, string projectFileName, string[] paths)
        {
            this.pack = pack;
            this.projectFileName = projectFileName;
            this.paths = paths;
        }

        public FBSFile[] Build()
        {
            var files = new List<FBSFile>();

            foreach (var path in paths)
            {
                if (path.ToLower().EndsWith(".fbs"))
                {
                    var text = File.ReadAllText(path);

                    var lexer = new FlatbufferLexer(new AntlrInputStream(text));
                    var parser = new FlatbufferParser(new CommonTokenStream(lexer));
                    var visitor = new Visitor(this, path, GetComments(lexer));

                    parser.schema().Accept<int>(visitor);

                    var file = visitor.file;
                    file.Path = path;
                    files.Add(file);
                }
            }

            return files.ToArray();
        }

        private Dictionary<int, string> GetComments(FlatbufferLexer lexer)
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

        class Visitor : FlatbufferBaseVisitor<int>
        {
            public FBSBuilder builder;
            public FBSFile file;
            public Dictionary<int, string> commentTable;

            public Visitor(FBSBuilder builder, string path, Dictionary<int, string> commentTable)
            {
                this.builder = builder;
                this.file = new FBSFile();
                this.file.Path = path;
                this.commentTable = commentTable;
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

            public override int VisitTable([NotNull] FlatbufferParser.TableContext context)
            {
                var data = new Table();
                data.Name = context.name.Text;
                data.Metas = GetMetas(context.metas(), "Bind", "Index");
                data.Comment = GetComment(context, context.name);

                var fieldsContext = context.tableField();
                if (fieldsContext != null)
                {
                    foreach (var fieldContext in fieldsContext)
                    {
                        var field = new TableField();
                        field.Name = fieldContext.fieldName != null ? fieldContext.fieldName.Text : "";
                        field.Type = fieldContext.fieldType != null ? fieldContext.fieldType.GetText().Trim('[').Trim(']') : "";
                        field.IsArray = fieldContext.fieldType != null ? fieldContext.fieldType.GetText().StartsWith("[") : true;
                        field.DefaultValue = fieldContext.fieldValue != null ? fieldContext.fieldValue.GetText() : "";
                        field.LinkFieldName = fieldContext.fieldMap != null ? fieldContext.fieldMap.Text : field.Name;
                        field.Metas = GetMetas(fieldContext.metas(), "Nullable");
                        field.Comment = GetComment(fieldContext, fieldContext.fieldName);

                        data.Fields.Add(field);
                    }
                }

                if (file.HasDefine(data.Name))
                {
                    builder.pack.AddError(builder.projectFileName, file.Path, string.Format("文件\"{0}\"中重复包含名称为\"{1}\"的定义",file.Path,data.Name), context.name.Line, context.name.Column);
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
                data.Metas = GetMetas(context.metas(), "Bind", "Index");
                data.Comment = GetComment(context, context.name);

                var fieldsContext = context.structField();
                if (fieldsContext != null)
                {
                    foreach (var fieldContext in fieldsContext)
                    {
                        var field = new StructField();
                        field.Name = fieldContext.fieldName != null ? fieldContext.fieldName.Text : "";
                        field.Type = fieldContext.fieldType != null ? fieldContext.fieldType.GetText().Trim('[').Trim(']') : "";
                        field.IsArray = fieldContext.fieldType != null ? fieldContext.fieldType.GetText().StartsWith("[") : true;
                        field.DefaultValue = fieldContext.fieldValue != null ? fieldContext.fieldValue.GetText() : "";
                        //field.LinkFieldName = fieldContext.fieldMap != null ? fieldContext.fieldMap.Text : field.Name;
                        field.Metas = GetMetas(fieldContext.metas(), "Nullable");
                        field.Comment = GetComment(fieldContext, fieldContext.fieldName);

                        data.Fields.Add(field);
                    }
                }

                if (file.HasDefine(data.Name))
                {
                    builder.pack.AddError(builder.projectFileName, file.Path, string.Format("文件\"{0}\"中重复包含名称为\"{1}\"的定义", file.Path, data.Name), context.name.Line, context.name.Column);
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
                data.Metas = GetMetas(context.metas());
                data.Comment = GetComment(context, context.name);

                var fieldsContext = context.enumField();
                if (fieldsContext != null)
                {
                    foreach (var fieldContext in fieldsContext)
                    {
                        var field = new EnumField();
                        field.Name = fieldContext.fieldName != null ? fieldContext.fieldName.Text : "";
                        field.Value = fieldContext.fieldValue != null ? fieldContext.fieldValue.Text : "";
                        field.Comment = GetComment(fieldContext, fieldContext.fieldName);

                        data.Fields.Add(field);
                    }
                }

                if (file.HasDefine(data.Name))
                {
                    builder.pack.AddError(builder.projectFileName, file.Path, string.Format("文件\"{0}\"中重复包含名称为\"{1}\"的定义", file.Path, data.Name), context.name.Line, context.name.Column);
                }
                else
                {
                    file.Enums.Add(data);
                }

                return 0;
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

            private List<Meta> GetMetas(FlatbufferParser.MetasContext metaContext, params string[] metaNames)
            {
                var metas = new List<Meta>();
                if (metaContext != null)
                {
                    var names = new HashSet<string>(metaNames);

                    foreach (var bindMeta in metaContext.bindMeta())
                    {
                        if (names.Count==0 || (bindMeta.key!=null && names.Contains(bindMeta.key.Text)))
                        {
                            var value = bindMeta.path != null ? bindMeta.path.GetText().Trim('"') : "";

                            if (!string.IsNullOrEmpty(value))
                            {

                                var meta = new Meta();
                                meta.Name = bindMeta.key.Text;
                                meta.Value = value;
                                metas.Add(meta);
                            }
                            else
                            {
                                builder.pack.AddError(builder.projectFileName, file.Path, "绑定内容不能为空", bindMeta.Start.Line, bindMeta.Start.Column);
                            }
                        }
                        else
                        {
                            builder.pack.AddError(builder.projectFileName, file.Path, "无效的元数据标记", bindMeta.Start.Line, bindMeta.Start.Column);
                        }
                    }
                    foreach (var indexMeta in metaContext.indexMeta())
                    {
                        if (names.Count == 0 || (indexMeta.key != null && names.Contains(indexMeta.key.Text)))
                        {
                            var fieldNames = new string[indexMeta._fields.Count];
                            for (int i = 0; i < fieldNames.Length; i++)
                            {
                                fieldNames[i] = indexMeta._fields[i].Text;
                            }
                            if (fieldNames.Length > 0)
                            {
                                var meta = new Meta();
                                meta.Name = indexMeta.key.Text;
                                meta.Value = string.Join(",", fieldNames);
                                metas.Add(meta);
                            }
                            else
                            {
                                builder.pack.AddError(builder.projectFileName, file.Path, "无效的元数据标记", indexMeta.key.Line, indexMeta.key.Column);
                            }
                        }
                        else
                        {
                            builder.pack.AddError(builder.projectFileName, file.Path, "无效的元数据标记", indexMeta.Start.Line, indexMeta.Start.Column);
                        }
                    }
                    foreach (var nullableMeta in metaContext.nullableMeta())
                    {
                        if (names.Count == 0 || (nullableMeta.key != null && names.Contains(nullableMeta.key.Text)))
                        {
                            var meta = new Meta();
                            meta.Name = nullableMeta.key.Text;
                            meta.Value = nullableMeta.val != null ? nullableMeta.val.Text : "true";
                            metas.Add(meta);
                        }
                        else
                        {
                            builder.pack.AddError(builder.projectFileName, file.Path, "无效的元数据标记", nullableMeta.Start.Line, nullableMeta.Start.Column);
                        }
                    }
                }
                return metas;
            }
        }
    }
}
