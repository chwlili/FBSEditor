using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.IO;
using System.ComponentModel.Composition;

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
        public readonly IClassificationType FBSKey;
        public readonly IClassificationType FBSTableName;
        public readonly IClassificationType FBSStructName;
        public readonly IClassificationType FBSEnumName;
        public readonly IClassificationType FBSUnionName;
        public readonly IClassificationType FBSRpcName;
        public readonly IClassificationType FBSFieldName;
        public readonly IClassificationType FBSFieldType;
        public readonly IClassificationType FBSFieldValue;
        public readonly IClassificationType FBSFieldMap;
        public readonly IClassificationType FBSString;
        public readonly IClassificationType FBSComment;

        public static List<SnapshotSpan> ErrorSpans = new List<SnapshotSpan>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FBSEditor"/> class.
        /// </summary>
        /// <param name="registry">Classification registry.</param>
        internal FBSEditor(IClassificationTypeRegistryService registry)
        {
            this.FBSKey = registry.GetClassificationType("FBSKey");
            this.FBSTableName = registry.GetClassificationType("FBSTableName");
            this.FBSStructName = registry.GetClassificationType("FBSStructName");
            this.FBSEnumName = registry.GetClassificationType("FBSEnumName");
            this.FBSUnionName = registry.GetClassificationType("FBSUnionName");
            this.FBSRpcName = registry.GetClassificationType("FBSRpcName");

            this.FBSFieldName = registry.GetClassificationType("FBSFieldName");
            this.FBSFieldType = registry.GetClassificationType("FBSFieldType");
            this.FBSFieldValue = registry.GetClassificationType("FBSFieldValue");
            this.FBSFieldMap = registry.GetClassificationType("FBSFieldMap");
            this.FBSString = registry.GetClassificationType("FBSString");
            this.FBSComment = registry.GetClassificationType("FBSComment");
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

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var result = new List<ClassificationSpan>();

            ErrorSpans.Clear();

            var aa = span.Snapshot.GetText();
                
            var lexer = new FlatbufferLexer(new AntlrInputStream(aa));
            var parser = new FlatbufferParser(new CommonTokenStream(lexer));

            var handler = new MyErrorHandler();
            handler.editor = this;
            handler.list = result;
            handler.span = span;
            parser.ErrorHandler = handler;


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

            lexer.Reset();
            foreach (var token in lexer.GetAllTokens())
            {
                if (token.Type == FlatbufferLexer.COMMENT || token.Type == FlatbufferLexer.COMMENT1)
                {
                    result.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)), FBSComment));
                }
            }

            return result;
        }

        #endregion
    }


    class MyErrorHandler : DefaultErrorStrategy
    {
        public SnapshotSpan span;
        public List<ClassificationSpan> list;
        public FBSEditor editor;

        protected internal override void ReportUnwantedToken(Parser recognizer)
        {
            if (!InErrorRecoveryMode(recognizer))
            {
                var token = recognizer.CurrentToken;

                FBSEditor.ErrorSpans.Add(new SnapshotSpan(span.Snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)));
            }
            base.ReportUnwantedToken(recognizer);
        }

        protected internal override void ReportMissingToken(Parser recognizer)
        {
            if (!InErrorRecoveryMode(recognizer))
            {
                IToken current = recognizer.CurrentToken;
                IToken lookback = ((ITokenStream)recognizer.InputStream).LT(-1);
                if (current.Type == TokenConstants.EOF && lookback != null)
                {
                    current = lookback;

                    var token = current;
                    FBSEditor.ErrorSpans.Add(new SnapshotSpan(span.Snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)));
                }
                else
                {
                    var token = recognizer.CurrentToken;

                    FBSEditor.ErrorSpans.Add(new SnapshotSpan(span.Snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)));
                }
            }
            base.ReportMissingToken(recognizer);
        }

        protected internal override void ReportNoViableAlternative(Parser recognizer, NoViableAltException e)
        {
            base.ReportNoViableAlternative(recognizer, e);
        }

        protected internal override void ReportInputMismatch(Parser recognizer, InputMismatchException e)
        {
            var token = recognizer.CurrentToken;
            //FBSEditor.ErrorSpans.Add(new SnapshotSpan(span.Snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)));
            base.ReportInputMismatch(recognizer, e);
        }
    }


    class ErrorListener : BaseErrorListener
    {
        public SnapshotSpan span;
        public List<ClassificationSpan> list;
        public FBSEditor editor;

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            base.SyntaxError(output, recognizer, offendingSymbol, line, charPositionInLine, msg, e);

            Console.Error.WriteLine(msg);

            //IToken token = offendingSymbol;

            //list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)), editor.errorType));
        }
    }

    class Visitor : FlatbufferBaseVisitor<int>
    {
        public SnapshotSpan span;
        public List<ClassificationSpan> list;
        public FBSEditor editor;

        public override int VisitTable([NotNull] FlatbufferParser.TableContext context)
        {
            MakeSpan(context.key,editor.FBSKey);
            MakeSpan(context.name,editor.FBSTableName);
            return base.VisitTable(context);
        }

        public override int VisitTableField([NotNull] FlatbufferParser.TableFieldContext context)
        {
            MakeSpan(context.fieldName, editor.FBSFieldName);
            MakeSpan(context.fieldType, editor.FBSFieldType);
            MakeSpan(context.fieldValue, editor.FBSFieldValue);
            MakeSpan(context.fieldArrow, editor.FBSFieldMap);
            MakeSpan(context.fieldMap, editor.FBSFieldMap);
            return base.VisitTableField(context);
        }

        public override int VisitStruct([NotNull] FlatbufferParser.StructContext context)
        {
            MakeSpan(context.key, editor.FBSKey);
            MakeSpan(context.name, editor.FBSStructName);
            return base.VisitStruct(context);
        }

        public override int VisitStructField([NotNull] FlatbufferParser.StructFieldContext context)
        {
            MakeSpan(context.fieldName, editor.FBSFieldName);
            MakeSpan(context.fieldType, editor.FBSFieldType);
            MakeSpan(context.fieldValue, editor.FBSFieldValue);
            return base.VisitStructField(context);
        }

        public override int VisitEnum([NotNull] FlatbufferParser.EnumContext context)
        {
            MakeSpan(context.key, editor.FBSKey);
            MakeSpan(context.name, editor.FBSEnumName);
            return base.VisitEnum(context);
        }

        public override int VisitEnumField([NotNull] FlatbufferParser.EnumFieldContext context)
        {
            return base.VisitEnumField(context);
        }

        public override int VisitUnion([NotNull] FlatbufferParser.UnionContext context)
        {
            MakeSpan(context.key, editor.FBSKey);
            MakeSpan(context.name, editor.FBSUnionName);
            return base.VisitUnion(context);
        }

        public override int VisitUnionField([NotNull] FlatbufferParser.UnionFieldContext context)
        {
            return base.VisitUnionField(context);
        }

        public override int VisitRpc([NotNull] FlatbufferParser.RpcContext context)
        {
            MakeSpan(context.key, editor.FBSKey);
            MakeSpan(context.name, editor.FBSRpcName);
            return base.VisitRpc(context);
        }

        public override int VisitRpcField([NotNull] FlatbufferParser.RpcFieldContext context)
        {
            MakeSpan(context.fieldName, editor.FBSFieldName);
            MakeSpan(context.fieldParam, editor.FBSFieldType);
            MakeSpan(context.fieldReturn, editor.FBSFieldType);
            return base.VisitRpcField(context);
        }

        public override int VisitInclude([NotNull] FlatbufferParser.IncludeContext context)
        {
            MakeSpan(context.key, editor.FBSKey);
            return base.VisitInclude(context);
        }
        public override int VisitNamespace([NotNull] FlatbufferParser.NamespaceContext context)
        {
            MakeSpan(context.key, editor.FBSKey);
            return base.VisitNamespace(context);
        }
        public override int VisitAttribute([NotNull] FlatbufferParser.AttributeContext context)
        {
            MakeSpan(context.key, editor.FBSKey);
            return base.VisitAttribute(context);
        }
        public override int VisitRootType([NotNull] FlatbufferParser.RootTypeContext context)
        {
            MakeSpan(context.key, editor.FBSKey);
            return base.VisitRootType(context);
        }
        public override int VisitFileExtension([NotNull] FlatbufferParser.FileExtensionContext context)
        {
            MakeSpan(context.key, editor.FBSKey);
            return base.VisitFileExtension(context);
        }
        public override int VisitFileIdentifier([NotNull] FlatbufferParser.FileIdentifierContext context)
        {
            MakeSpan(context.key, editor.FBSKey);
            return base.VisitFileIdentifier(context);
        }

        public override int VisitBindMeta([NotNull] FlatbufferParser.BindMetaContext context)
        {
            MakeSpan(context.key, editor.FBSKey);
            return base.VisitBindMeta(context);
        }

        public override int VisitIndexMeta([NotNull] FlatbufferParser.IndexMetaContext context)
        {
            MakeSpan(context.key, editor.FBSKey);
            return base.VisitIndexMeta(context);
        }

        public override int VisitNullableMeta([NotNull] FlatbufferParser.NullableMetaContext context)
        {
            MakeSpan(context.key, editor.FBSKey);
            return base.VisitNullableMeta(context);
        }

        public override int VisitReferenceMeta([NotNull] FlatbufferParser.ReferenceMetaContext context)
        {
            MakeSpan(context.key, editor.FBSKey);
            return base.VisitReferenceMeta(context);
        }

        public override int VisitString([NotNull] FlatbufferParser.StringContext context)
        {
            MakeSpan(context.text, editor.FBSString);
            return base.VisitString(context);
        }



        private void MakeSpan(IToken token, IClassificationType classification)
        {
            if (token == null || token.TokenIndex < 0) { return; }
            list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)), classification));
        }
        private void MakeSpan(ParserRuleContext context, IClassificationType classification)
        {
            if (context != null && context.Start != null && context.Stop != null && context.Stop.StopIndex >= context.Start.StartIndex)
            {
                list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(context.Start.StartIndex, context.Stop.StopIndex - context.Start.StartIndex + 1)), classification));
            }
        }

    }
}
