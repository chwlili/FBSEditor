using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace FBSEditor.Build
{
    public class FBSBuilder
    {
        private BuildCommandPackage pack;
        private Project project;
        private string[] paths;

        public FBSBuilder(BuildCommandPackage pack, Project project, string[] paths)
        {
            this.pack = pack;
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
                    var visitor = new Visitor(pack);
                    visitor.file.Path = path;
                    visitor.project = project;

                    parser.schema().Accept<int>(visitor);

                    var file = visitor.file;
                    file.Path = path;
                    files.Add(file);
                }
            }
        }

        class Visitor : FlatbufferBaseVisitor<int>
        {
            public BuildCommandPackage pack;
            public Project project;
            public FBSFile file = new FBSFile();

            public Visitor(BuildCommandPackage pack)
            {
                this.pack = pack;
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

                if (file.HasDefine(table.Name))
                {
                    var ivsSolution = (IVsSolution)Package.GetGlobalService(typeof(IVsSolution));
                    IVsHierarchy hierarchyItem;
                    ivsSolution.GetProjectOfUniqueName(project.FileName, out hierarchyItem);

                    var error = new ErrorTask();
                    error.Line = context.name.Line-1;
                    error.Column = context.name.Column;
                    error.Text = "name exits!";
                    error.ErrorCategory = TaskErrorCategory.Error;
                    error.Category = TaskCategory.BuildCompile;
                    error.Document = file.Path;
                    error.HierarchyItem = hierarchyItem;

                    error.Navigate += (sender, e) =>
                    {
                        //there are two Bugs in the errorListProvider.Navigate method:
                        // Line number needs adjusting
                        // Column is not shown
                        error.Line++;
                        pack.ErrorList.Navigate(error, new Guid(EnvDTE.Constants.vsViewKindCode));
                        error.Line--;
                    };
                    pack.ErrorList.Tasks.Add(error);
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
