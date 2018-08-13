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
        public readonly IClassificationType ClassificationKey;
        public readonly IClassificationType ClassificationText;
        public readonly IClassificationType ClassificationCode;
        public readonly IClassificationType ClassificationTag;

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

            this.ClassificationKey = registry.GetClassificationType(Constants.ClassificationKey);
            this.ClassificationText = registry.GetClassificationType(Constants.ClassificationText);
            this.ClassificationCode = registry.GetClassificationType(Constants.ClassificationCode);
            this.ClassificationTag = registry.GetClassificationType(Constants.ClassificationTag);

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

            //ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
        }
    }
    

    class ClassificationVisitor : TemplateParserBaseVisitor<int>
    {
        public Classification classificater;

        public ClassificationVisitor(Classification classificater)
        {
            this.classificater = classificater;
        }

        public override int VisitTextRegion([NotNull] TemplateParser.TextRegionContext context)
        {
            MakeClassificationSpan(context, classificater.ClassificationText);
            return base.VisitTextRegion(context);
        }
        public override int VisitCodeRegion([NotNull] TemplateParser.CodeRegionContext context)
        {
            MakeClassificationSpan(context.begin, classificater.ClassificationTag);
            MakeClassificationSpan(context.end, classificater.ClassificationTag);
            MakeOutline(context.begin, context.end);
            return base.VisitCodeRegion(context);
        }
        public override int VisitExpr([NotNull] TemplateParser.ExprContext context)
        {
            //MakeClassificationSpan(context, classificater.ClassificationCode);
            return base.VisitExpr(context);
        }
        public override int VisitIf([NotNull] TemplateParser.IfContext context)
        {
            MakeClassificationSpan(context.id, classificater.ClassificationKey);
            MakeBracePair(context.PARENTHESES_L(), context.PARENTHESES_R());
            MakeBracePair(context.BRACE_L(), context.BRACE_R());
            return base.VisitIf(context);
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
        private void MakeOutline(IToken l, IToken r)
        {
            if (l != null && r != null)
            {
                classificater.OutlineList.Add(l.StopIndex + 1);
                classificater.OutlineList.Add(r.StartIndex - l.StopIndex - 1);
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
