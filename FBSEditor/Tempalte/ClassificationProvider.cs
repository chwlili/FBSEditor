using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Tempalte
{
    [Export(typeof(IClassifierProvider))]
    [ContentType(Constants.ContentType)]
    internal class ClassificationProvider : IClassifierProvider
    {
#pragma warning disable 649
        [Import]
        private IClassificationTypeRegistryService classificationRegistry;
#pragma warning restore 649

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty<Classification>(() => { return new Classification(this.classificationRegistry, buffer); });
        }
    }

    internal class Classification : IClassifier
    {
        public readonly IClassificationType ClassificationKeyword;
        public readonly IClassificationType ClassificationText;
        public readonly IClassificationType ClassificationCode;
        public readonly IClassificationType ClassificationTag;
        public readonly IClassificationType ClassificationComment;

        private ClassificationVisitor classificationVisitor;

        public ITextBuffer Buffer { get; set; }
        public List<int> BracePairList { get; } = new List<int>();
        public List<int> OutlineList { get; } = new List<int>();
        public List<ClassificationSpan> ClassfificationList { get; } = new List<ClassificationSpan>();
        public Dictionary<int, string> CommentTable { get; } = new Dictionary<int, String>();
        public List<string> StructNameList { get; } = new List<string>();
        public List<SnapshotSpan> ErrorList { get; } = new List<SnapshotSpan>();

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        internal Classification(IClassificationTypeRegistryService registry,ITextBuffer buffer)
        {
            this.classificationVisitor = new ClassificationVisitor(this);
            //this.classificationErrorHandler = new ClassificationErrorHandler(this);
            //this.classificationErrorListener = new ClassificationErrorListener(this);

            this.ClassificationKeyword = registry.GetClassificationType(Constants.ClassificationKeyword);
            this.ClassificationText = registry.GetClassificationType(Constants.ClassificationText);
            this.ClassificationCode = registry.GetClassificationType(Constants.ClassificationCode);
            this.ClassificationTag = registry.GetClassificationType(Constants.ClassificationTag);
            this.ClassificationComment = registry.GetClassificationType(Constants.ClassificationComment);

            this.Buffer = buffer;
            this.Buffer.ChangedHighPriority += OnTextBufferChanged;
            
            RebuidTokens();
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            return ClassfificationList;
        }

        private void OnTextBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            RebuidTokens();
        }

        private void RebuidTokens()
        {
            CommentTable.Clear();
            ClassfificationList.Clear();
            StructNameList.Clear();
            BracePairList.Clear();
            OutlineList.Clear();
            ErrorList.Clear();

            var snapshot = this.Buffer.CurrentSnapshot;
            
            var lexer = new TemplateLexer(new AntlrInputStream(snapshot.GetText()));
            var parser = new TemplateParser(new CommonTokenStream(lexer));
            //parser.ErrorHandler = classificationErrorHandler;
            //parser.RemoveErrorListeners();
            //parser.AddErrorListener(classificationErrorListener);

            parser.document().Accept<int>(classificationVisitor);

            FlagAllHiddenToken(snapshot,lexer);

            //ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
        }

        private void FlagAllHiddenToken(ITextSnapshot snapshot, TemplateLexer lexer)
        {
            IToken open = null;

            lexer.Reset();
            foreach (var token in lexer.GetAllTokens())
            {
                if (token.Type == TemplateLexer.COMMENT)
                {
                    ClassfificationList.Add(new ClassificationSpan(new SnapshotSpan(snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)), ClassificationComment));
                }
                else if (token.Type == TemplateLexer.OPEN)
                {
                    if (open == null) { open = token; }
                    ClassfificationList.Add(new ClassificationSpan(new SnapshotSpan(snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)), ClassificationTag));
                }
                else if(token.Type==TemplateLexer.CLOSE)
                {
                    ClassfificationList.Add(new ClassificationSpan(new SnapshotSpan(snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)), ClassificationTag));
                    if (open != null)
                    {
                        OutlineList.Add(open.StopIndex + 1);
                        OutlineList.Add(token.StartIndex - open.StopIndex - 1);
                    }
                    open = null;
                }
                else if(token.Type == TemplateLexer.TEXT)
                {
                    ClassfificationList.Add(new ClassificationSpan(new SnapshotSpan(snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)), ClassificationText));
                }
            }
            lexer.Reset();
        }
    }
    

    class ClassificationVisitor : TemplateParserBaseVisitor<int>
    {
        public Classification classificater;

        public ClassificationVisitor(Classification classificater)
        {
            this.classificater = classificater;
        }
        public override int VisitVar([NotNull] TemplateParser.VarContext context)
        {
            MakeClassificationSpan(context.keyword, classificater.ClassificationKeyword);
            return base.VisitVar(context);
        }
        public override int VisitIf([NotNull] TemplateParser.IfContext context)
        {
            MakeClassificationSpan(context.keyword, classificater.ClassificationKeyword);
            MakeBracePair(context.PARENTHESES_L(), context.PARENTHESES_R());
            MakeBracePair(context.BRACE_L(), context.BRACE_R());
            return base.VisitIf(context);
        }
        public override int VisitSwitch([NotNull] TemplateParser.SwitchContext context)
        {
            MakeClassificationSpan(context.keywordA, classificater.ClassificationKeyword);
            foreach(var keyword in context._keywordB)
            {
                MakeClassificationSpan(keyword, classificater.ClassificationKeyword);
            }
            MakeBracePair(context.PARENTHESES_L(), context.PARENTHESES_R());
            MakeBracePair(context.BRACE_L(), context.BRACE_R());
            return base.VisitSwitch(context);
        }
        public override int VisitWhile([NotNull] TemplateParser.WhileContext context)
        {
            MakeClassificationSpan(context.keyword, classificater.ClassificationKeyword);
            MakeBracePair(context.PARENTHESES_L(), context.PARENTHESES_R());
            MakeBracePair(context.BRACE_L(), context.BRACE_R());
            return base.VisitWhile(context);
        }
        public override int VisitDowhile([NotNull] TemplateParser.DowhileContext context)
        {
            MakeClassificationSpan(context.keywordA, classificater.ClassificationKeyword);
            MakeClassificationSpan(context.keywordB, classificater.ClassificationKeyword);
            MakeBracePair(context.PARENTHESES_L(), context.PARENTHESES_R());
            MakeBracePair(context.BRACE_L(), context.BRACE_R());
            return base.VisitDowhile(context);
        }
        public override int VisitFor([NotNull] TemplateParser.ForContext context)
        {
            MakeClassificationSpan(context.keyword, classificater.ClassificationKeyword);
            MakeBracePair(context.PARENTHESES_L(), context.PARENTHESES_R());
            MakeBracePair(context.BRACE_L(), context.BRACE_R());
            return base.VisitFor(context);
        }
        public override int VisitForeach([NotNull] TemplateParser.ForeachContext context)
        {
            MakeClassificationSpan(context.keywordA, classificater.ClassificationKeyword);
            MakeClassificationSpan(context.keywordB, classificater.ClassificationKeyword);
            MakeBracePair(context.PARENTHESES_L(), context.PARENTHESES_R());
            MakeBracePair(context.BRACE_L(), context.BRACE_R());
            return base.VisitForeach(context);
        }



        private void MakeClassificationSpan(IToken token, IClassificationType classification)
        {
            if (token == null || token.TokenIndex < 0) { return; }
            var span = new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1);
            var snapshotSpan = new SnapshotSpan(classificater.Buffer.CurrentSnapshot, span);
            var classificationSpan = new ClassificationSpan(snapshotSpan, classification);

            classificater.ClassfificationList.Add(classificationSpan);
        }
        private void MakeClassificationSpan(ParserRuleContext context, IClassificationType classification)
        {
            if (context != null && context.Start != null && context.Stop != null && context.Stop.StopIndex >= context.Start.StartIndex)
            {
                var span = new Span(context.Start.StartIndex, context.Stop.StopIndex - context.Start.StartIndex + 1);
                var snapshotSpan = new SnapshotSpan(classificater.Buffer.CurrentSnapshot, span);
                var classificationSpan = new ClassificationSpan(snapshotSpan, classification);

                classificater.ClassfificationList.Add(classificationSpan);
            }
        }
        private void MakeBracePair(ITerminalNode l, ITerminalNode r)
        {
            if (l != null && r != null && l.Symbol.StartIndex != -1 && r.Symbol.StartIndex != -1)
            {
                classificater.BracePairList.Add(l.Symbol.StartIndex);
                classificater.BracePairList.Add(r.Symbol.StartIndex);
            }
        }
    }
}
