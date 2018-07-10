using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace FBSEditor.Build
{
    public class FBSBuilder
    {
        private Project project;
        private string[] paths;

        public FBSBuilder(Project project, string[] paths)
        {
            this.project = project;
            this.paths = paths;
        }

        public void Build()
        {
            var files = new List<FBSFile>();

            foreach (var path in paths)
            {
                if (path.ToLower().EndsWith(".fbs"))
                {
                    var text = File.ReadAllText(path);

                    var lexer = new FlatbufferLexer(new AntlrInputStream(text));
                    var parser = new FlatbufferParser(new CommonTokenStream(lexer));
                    var visitor = new Visitor();

                    parser.schema().Accept<int>(visitor);

                    var file = visitor.file;
                    file.Path = path;
                    files.Add(file);
                }
            }

            Console.WriteLine(files.Count);
        }

        class Visitor : FlatbufferBaseVisitor<int>
        {
            public FBSFile file = new FBSFile();

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
                var table = new Table();
                table.Name = context.name.Text;
                table.Metas = GetMetas(context.metas(), "Bind", "Index");

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

                        table.Fields.Add(field);
                    }
                }

                file.Tables.Add(table);

                return 0;
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
                            var meta = new Meta();
                            meta.Name = bindMeta.key.Text;
                            meta.Value = bindMeta.path != null ? bindMeta.path.GetText().Trim('"') : "";
                            metas.Add(meta);
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

                            var meta = new Meta();
                            meta.Name = indexMeta.key.Text;
                            meta.Value = string.Join(",", fieldNames);
                            metas.Add(meta);
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
                    }
                }
                return metas;
            }
        }
    }
}
