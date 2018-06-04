using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.IO;

namespace FBSEditor
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the "FBSEditor" classification type.
    /// </summary>
    internal class FBSEditor : IClassifier
    {
        /// <summary>
        /// Classification type.
        /// </summary>
        public readonly IClassificationType classificationType;
        public readonly IClassificationType keyType;
        public readonly IClassificationType nameType;
        public readonly IClassificationType commentType;
        public readonly IClassificationType errorType;
        public readonly IClassificationType stringToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="FBSEditor"/> class.
        /// </summary>
        /// <param name="registry">Classification registry.</param>
        internal FBSEditor(IClassificationTypeRegistryService registry)
        {
            this.classificationType = registry.GetClassificationType("Normal");
            this.keyType = registry.GetClassificationType("Key");
            this.nameType = registry.GetClassificationType("Name");
            this.commentType = registry.GetClassificationType("Comm");
            this.errorType =  registry.GetClassificationType("ErrorToken");
            this.stringToken = registry.GetClassificationType("StringToken");
        }

        #region IClassifier

#pragma warning disable 67

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification.</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var result = new List<ClassificationSpan>();

            var aa = span.Snapshot.GetText();
                
            var lexer = new FlatbufferLexer(new AntlrInputStream(aa));
            var parser = new FlatbufferParser(new CommonTokenStream(lexer));
            //parser.ErrorHandler = new DefaultErrorStrategy();

            var listener = new ErrorListener();
            listener.editor = this;
            listener.list = result;
            listener.span = span;
            parser.RemoveErrorListeners();
            parser.AddErrorListener(listener);

            var root = parser.schema();

            var visitor = new Visitor();
            visitor.editor = this;
            visitor.list = result;
            visitor.span = span;
            root.Accept<int>(visitor);

            return result;
        }

        #endregion
    }

    class ErrorListener : BaseErrorListener
    {
        public SnapshotSpan span;
        public List<ClassificationSpan> list;
        public FBSEditor editor;

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            base.SyntaxError(output, recognizer, offendingSymbol, line, charPositionInLine, msg, e);

            IToken token = offendingSymbol;

            list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)), editor.errorType));
        }
    }

    class Visitor : FlatbufferBaseVisitor<int>
    {
        public SnapshotSpan span;
        public List<ClassificationSpan> list;
        public FBSEditor editor;

        public override int VisitTable([NotNull] FlatbufferParser.TableContext context)
        {
            MakeKeySpan(context.key);
            MakeNameSpan(context.name);
            return base.VisitTable(context);
        }

        public override int VisitTableField([NotNull] FlatbufferParser.TableFieldContext context)
        {
            MakeNameSpan(context.fieldType);
            MakeNameSpan(context.fieldMap);
            return base.VisitTableField(context);
        }

        public override int VisitStruct([NotNull] FlatbufferParser.StructContext context)
        {
            MakeKeySpan(context.key);
            MakeNameSpan(context.name);
            return base.VisitStruct(context);
        }

        public override int VisitStructField([NotNull] FlatbufferParser.StructFieldContext context)
        {
            MakeNameSpan(context.fieldType);
            return base.VisitStructField(context);
        }

        public override int VisitEnum([NotNull] FlatbufferParser.EnumContext context)
        {
            MakeKeySpan(context.key);
            MakeNameSpan(context.name);
            return base.VisitEnum(context);
        }

        public override int VisitEnumField([NotNull] FlatbufferParser.EnumFieldContext context)
        {
            return base.VisitEnumField(context);
        }

        public override int VisitUnion([NotNull] FlatbufferParser.UnionContext context)
        {
            MakeKeySpan(context.key);
            MakeNameSpan(context.name);
            return base.VisitUnion(context);
        }

        public override int VisitUnionField([NotNull] FlatbufferParser.UnionFieldContext context)
        {
            return base.VisitUnionField(context);
        }

        public override int VisitRpc([NotNull] FlatbufferParser.RpcContext context)
        {
            MakeKeySpan(context.key);
            MakeNameSpan(context.name);
            return base.VisitRpc(context);
        }

        public override int VisitRpcField([NotNull] FlatbufferParser.RpcFieldContext context)
        {
            MakeNameSpan(context.fieldName);
            MakeNameSpan(context.fieldParam);
            MakeNameSpan(context.fieldReturn);
            return base.VisitRpcField(context);
        }


        public override int VisitInclude([NotNull] FlatbufferParser.IncludeContext context)
        {
            MakeKeySpan(context.key);
            return base.VisitInclude(context);
        }
        public override int VisitNamespace([NotNull] FlatbufferParser.NamespaceContext context)
        {
            MakeKeySpan(context.key);
            return base.VisitNamespace(context);
        }
        public override int VisitAttribute([NotNull] FlatbufferParser.AttributeContext context)
        {
            MakeKeySpan(context.key);
            return base.VisitAttribute(context);
        }
        public override int VisitRootType([NotNull] FlatbufferParser.RootTypeContext context)
        {
            MakeKeySpan(context.key);
            return base.VisitRootType(context);
        }
        public override int VisitFileExtension([NotNull] FlatbufferParser.FileExtensionContext context)
        {
            MakeKeySpan(context.key);
            return base.VisitFileExtension(context);
        }
        public override int VisitFileIdentifier([NotNull] FlatbufferParser.FileIdentifierContext context)
        {
            MakeKeySpan(context.key);
            return base.VisitFileIdentifier(context);
        }



        public override int VisitComment([NotNull] FlatbufferParser.CommentContext context)
        {
            IToken token = context.text;
            if (token != null)
            {
                list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)), editor.commentType));
            }
            return base.VisitComment(context);
        }

        public override int VisitString([NotNull] FlatbufferParser.StringContext context)
        {
            MakeStringSpan(context.text);

            return base.VisitString(context);
        }

        public override int VisitBindMeta([NotNull] FlatbufferParser.BindMetaContext context)
        {
            MakeNameSpan(context.key);
            return base.VisitBindMeta(context);
        }

        public override int VisitIndexMeta([NotNull] FlatbufferParser.IndexMetaContext context)
        {
            MakeNameSpan(context.key);

            return base.VisitIndexMeta(context);
        }

        public override int VisitNullableMeta([NotNull] FlatbufferParser.NullableMetaContext context)
        {
            MakeNameSpan(context.key);
            return base.VisitNullableMeta(context);
        }

        public override int VisitReferenceMeta([NotNull] FlatbufferParser.ReferenceMetaContext context)
        {
            MakeNameSpan(context.key);
            return base.VisitReferenceMeta(context);
        }



        private void MakeKeySpan(IToken token)
        {
            if (token == null) { return; }
            list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)), editor.keyType));
        }

        private void MakeNameSpan(IToken token)
        {
            if (token == null) { return; }
            list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)), editor.nameType));
        }
        private void MakeNameSpan(ParserRuleContext context)
        {
            if(context!=null && context.Start!=null && context.Stop!=null)
            {
                list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(context.Start.StartIndex, context.Stop.StopIndex - context.Start.StartIndex + 1)), editor.nameType));
            }
        }

        private void MakeStringSpan(IToken token)
        {
            if (token == null) { return; }
            list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)), editor.stringToken));
        }

    }
}
