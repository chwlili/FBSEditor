﻿using Antlr4.Runtime;
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

namespace FBSEditor
{
    [Export(typeof(IClassifierProvider))]
    [ContentType(Constants.FBSContentType)]
    internal class FBSClassificationProvider : IClassifierProvider
    {
#pragma warning disable 649

        [Import]
        private IClassificationTypeRegistryService classificationRegistry;

#pragma warning restore 649

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty<FBSClassification>(() =>
            {
                return new FBSClassification(this.classificationRegistry, buffer);
            });
        }
    }

    internal class FBSClassification : IClassifier
    {
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

        private ClassificationVisitor classificationVisitor;
        private ClassificationErrorHandler classificationErrorHandler;
        private ClassificationErrorListener classificationErrorListener;

        public ITextBuffer Buffer { get; set; }
        public List<int> BraceList { get; } = new List<int>();
        public List<int> OutlineList { get; } = new List<int>();
        public List<ClassificationSpan> ClassfificationList { get; } = new List<ClassificationSpan>();
        public Dictionary<int, string> CommentTable { get; } = new Dictionary<int, String>();
        public List<CommentData> CommentTipList { get; } = new List<CommentData>();
        public List<SnapshotSpan> ErrorList { get; } = new List<SnapshotSpan>();

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        internal FBSClassification(IClassificationTypeRegistryService registry,ITextBuffer buffer)
        {
            this.classificationVisitor = new ClassificationVisitor(this);
            this.classificationErrorHandler = new ClassificationErrorHandler(this);
            this.classificationErrorListener = new ClassificationErrorListener(this);

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
            BraceList.Clear();
            OutlineList.Clear();
            ClassfificationList.Clear();
            CommentTable.Clear();
            CommentTipList.Clear();
            ErrorList.Clear();

            var snapshot = this.Buffer.CurrentSnapshot;
            
            var lexer = new FlatbufferLexer(new AntlrInputStream(snapshot.GetText()));

            foreach (var token in lexer.GetAllTokens())
            {
                if (token.Type == FlatbufferLexer.COMMENT)
                {
                    ClassfificationList.Add(new ClassificationSpan(new SnapshotSpan(snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)), FBSComment));
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
                        CommentTable.Add(token.Line + i, txt);
                    }
                }
            }
            lexer.Reset();

            var parser = new FlatbufferParser(new CommonTokenStream(lexer));
            parser.ErrorHandler = classificationErrorHandler;
            parser.RemoveErrorListeners();
            parser.AddErrorListener(classificationErrorListener);

            parser.schema().Accept<int>(classificationVisitor);

            ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
        }
    }

    class CommentData
    {
        public int start;
        public int stop;
        public string text;

        public CommentData(int start,int stop,string text)
        {
            this.start = start;
            this.stop = stop;
            this.text = text;
        }
    }
    
    class ClassificationErrorHandler : DefaultErrorStrategy
    {
        private FBSClassification classificater;

        public ClassificationErrorHandler(FBSClassification classificater)
        {
            this.classificater = classificater;
        }

        protected internal override void ReportUnwantedToken(Parser recognizer)
        {
            if (!InErrorRecoveryMode(recognizer))
            {
                var token = recognizer.CurrentToken;
                var span = new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                var snapshotSpan = new SnapshotSpan(classificater.Buffer.CurrentSnapshot, span);
                classificater.ErrorList.Add(snapshotSpan);
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
                    var span = new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                    var snapshotSpan = new SnapshotSpan(classificater.Buffer.CurrentSnapshot, span);
                    classificater.ErrorList.Add(snapshotSpan);
                }
                else
                {
                    var token = recognizer.CurrentToken;
                    var span = new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                    var snapshotSpan = new SnapshotSpan(classificater.Buffer.CurrentSnapshot, span);
                    classificater.ErrorList.Add(snapshotSpan);
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


    class ClassificationErrorListener : BaseErrorListener
    {
        private FBSClassification classification;

        public ClassificationErrorListener(FBSClassification classification)
        {
            this.classification = classification;
        }

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            base.SyntaxError(output, recognizer, offendingSymbol, line, charPositionInLine, msg, e);

            Console.Error.WriteLine(msg);

            //IToken token = offendingSymbol;

            //list.Add(new ClassificationSpan(new SnapshotSpan(snapshot, new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1)), editor.errorType));
        }
    }

    class ClassificationVisitor : FlatbufferBaseVisitor<int>
    {
        public FBSClassification classificater;

        public ClassificationVisitor(FBSClassification classificater)
        {
            this.classificater = classificater;
        }

        public override int VisitTable([NotNull] FlatbufferParser.TableContext context)
        {
            AddBrace(context.BRACE_L(), context.BRACE_R());
            MakeSpan(context.key,classificater.FBSKey);
            MakeSpan(context.name,classificater.FBSTableName);
            MakeTip(context, context.name);
            if (context.name != null && context.Stop != null)
            {
                classificater.OutlineList.Add(context.name.StopIndex + 1);
                classificater.OutlineList.Add(context.Stop.StopIndex - context.name.StopIndex);
            }
            return base.VisitTable(context);
        }

        public override int VisitTableField([NotNull] FlatbufferParser.TableFieldContext context)
        {
            MakeSpan(context.fieldName, classificater.FBSFieldName);
            MakeSpan(context.fieldType, classificater.FBSFieldType);
            MakeSpan(context.fieldValue, classificater.FBSFieldValue);
            MakeSpan(context.fieldArrow, classificater.FBSFieldMap);
            MakeSpan(context.fieldMap, classificater.FBSFieldMap);
            MakeTip(context, context.fieldName);
            return base.VisitTableField(context);
        }

        public override int VisitStruct([NotNull] FlatbufferParser.StructContext context)
        {
            AddBrace(context.BRACE_L(), context.BRACE_R());
            MakeSpan(context.key, classificater.FBSKey);
            MakeSpan(context.name, classificater.FBSStructName);
            MakeTip(context, context.name);
            if (context.name != null && context.Stop != null)
            {
                classificater.OutlineList.Add(context.name.StopIndex + 1);
                classificater.OutlineList.Add(context.Stop.StopIndex - context.name.StopIndex);
            }
            return base.VisitStruct(context);
        }

        public override int VisitStructField([NotNull] FlatbufferParser.StructFieldContext context)
        {
            MakeSpan(context.fieldName, classificater.FBSFieldName);
            MakeSpan(context.fieldType, classificater.FBSFieldType);
            MakeSpan(context.fieldValue, classificater.FBSFieldValue);
            MakeTip(context, context.fieldName);
            return base.VisitStructField(context);
        }

        public override int VisitEnum([NotNull] FlatbufferParser.EnumContext context)
        {
            MakeTip(context, context.name);
            AddBrace(context.BRACE_L(), context.BRACE_R());
            MakeSpan(context.key, classificater.FBSKey);
            MakeSpan(context.name, classificater.FBSEnumName);
            if (context.name != null && context.Stop != null)
            {
                classificater.OutlineList.Add(context.name.StopIndex + 1);
                classificater.OutlineList.Add(context.Stop.StopIndex - context.name.StopIndex);
            }
            return base.VisitEnum(context);
        }

        public override int VisitEnumField([NotNull] FlatbufferParser.EnumFieldContext context)
        {
            MakeTip(context, context.fieldName);
            return base.VisitEnumField(context);
        }

        public override int VisitUnion([NotNull] FlatbufferParser.UnionContext context)
        {
            AddBrace(context.BRACE_L(), context.BRACE_R());
            MakeSpan(context.key, classificater.FBSKey);
            MakeSpan(context.name, classificater.FBSUnionName);
            MakeTip(context, context.name);
            if (context.name != null && context.Stop != null)
            {
                classificater.OutlineList.Add(context.name.StopIndex + 1);
                classificater.OutlineList.Add(context.Stop.StopIndex - context.name.StopIndex);
            }
            return base.VisitUnion(context);
        }

        public override int VisitUnionField([NotNull] FlatbufferParser.UnionFieldContext context)
        {
            MakeTip(context, context.fieldName);
            return base.VisitUnionField(context);
        }

        public override int VisitRpc([NotNull] FlatbufferParser.RpcContext context)
        {
            AddBrace(context.BRACE_L(), context.BRACE_R());
            MakeSpan(context.key, classificater.FBSKey);
            MakeSpan(context.name, classificater.FBSRpcName);
            MakeTip(context, context.name);
            if (context.name != null && context.Stop != null)
            {
                classificater.OutlineList.Add(context.name.StopIndex + 1);
                classificater.OutlineList.Add(context.Stop.StopIndex - context.name.StopIndex);
            }
            return base.VisitRpc(context);
        }

        public override int VisitRpcField([NotNull] FlatbufferParser.RpcFieldContext context)
        {
            AddBrace(context.PARENTHESES_L(), context.PARENTHESES_R());
            MakeSpan(context.fieldName, classificater.FBSFieldName);
            MakeSpan(context.fieldParam, classificater.FBSFieldType);
            MakeSpan(context.fieldReturn, classificater.FBSFieldType);
            MakeTip(context, context.fieldName);
            return base.VisitRpcField(context);
        }

        public override int VisitInclude([NotNull] FlatbufferParser.IncludeContext context)
        {
            MakeSpan(context.key, classificater.FBSKey);
            return base.VisitInclude(context);
        }
        public override int VisitNamespace([NotNull] FlatbufferParser.NamespaceContext context)
        {
            MakeSpan(context.key, classificater.FBSKey);
            return base.VisitNamespace(context);
        }
        public override int VisitAttribute([NotNull] FlatbufferParser.AttributeContext context)
        {
            MakeSpan(context.key, classificater.FBSKey);
            return base.VisitAttribute(context);
        }
        public override int VisitRootType([NotNull] FlatbufferParser.RootTypeContext context)
        {
            MakeSpan(context.key, classificater.FBSKey);
            return base.VisitRootType(context);
        }
        public override int VisitFileExtension([NotNull] FlatbufferParser.FileExtensionContext context)
        {
            MakeSpan(context.key, classificater.FBSKey);
            return base.VisitFileExtension(context);
        }
        public override int VisitFileIdentifier([NotNull] FlatbufferParser.FileIdentifierContext context)
        {
            MakeSpan(context.key, classificater.FBSKey);
            return base.VisitFileIdentifier(context);
        }

        public override int VisitBindMeta([NotNull] FlatbufferParser.BindMetaContext context)
        {
            AddBrace(context.BRACKET_L(), context.BRACKET_R());
            AddBrace(context.PARENTHESES_L(), context.PARENTHESES_R());
            MakeSpan(context.key, classificater.FBSKey);
            return base.VisitBindMeta(context);
        }

        public override int VisitIndexMeta([NotNull] FlatbufferParser.IndexMetaContext context)
        {
            AddBrace(context.BRACKET_L(), context.BRACKET_R());
            AddBrace(context.PARENTHESES_L(), context.PARENTHESES_R());
            MakeSpan(context.key, classificater.FBSKey);
            return base.VisitIndexMeta(context);
        }

        public override int VisitNullableMeta([NotNull] FlatbufferParser.NullableMetaContext context)
        {
            AddBrace(context.BRACKET_L(), context.BRACKET_R());
            AddBrace(context.PARENTHESES_L(), context.PARENTHESES_R());
            MakeSpan(context.key, classificater.FBSKey);
            return base.VisitNullableMeta(context);
        }

        public override int VisitReferenceMeta([NotNull] FlatbufferParser.ReferenceMetaContext context)
        {
            AddBrace(context.BRACKET_L(), context.BRACKET_R());
            AddBrace(context.PARENTHESES_L(), context.PARENTHESES_R());
            MakeSpan(context.key, classificater.FBSKey);
            return base.VisitReferenceMeta(context);
        }
        public override int VisitMetadata([NotNull] FlatbufferParser.MetadataContext context)
        {
            AddBrace(context.PARENTHESES_L(), context.PARENTHESES_R());
            return base.VisitMetadata(context);
        }
        public override int VisitArrayValue([NotNull] FlatbufferParser.ArrayValueContext context)
        {
            AddBrace(context.BRACKET_L(), context.BRACKET_R());
            return base.VisitArrayValue(context);
        }
        public override int VisitObjectValue([NotNull] FlatbufferParser.ObjectValueContext context)
        {
            AddBrace(context.BRACE_L(), context.BRACE_R());
            return base.VisitObjectValue(context);
        }

        public override int VisitString([NotNull] FlatbufferParser.StringContext context)
        {
            MakeSpan(context.text, classificater.FBSString);
            return base.VisitString(context);
        }



        private void AddBrace(ITerminalNode l, ITerminalNode r)
        {
            if (l != null && r != null && l.Symbol.StartIndex != -1 && r.Symbol.StartIndex != -1)
            {
                classificater.BraceList.Add(l.Symbol.StartIndex);
                classificater.BraceList.Add(r.Symbol.StartIndex);
            }
        }
        private void MakeSpan(IToken token, IClassificationType classification)
        {
            if (token == null || token.TokenIndex < 0) { return; }
            var span = new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1);
            var snapshotSpan = new SnapshotSpan(classificater.Buffer.CurrentSnapshot, span);
            var classificationSpan = new ClassificationSpan(snapshotSpan, classification);

            classificater.ClassfificationList.Add(classificationSpan);
        }
        private void MakeSpan(ParserRuleContext context, IClassificationType classification)
        {
            if (context != null && context.Start != null && context.Stop != null && context.Stop.StopIndex >= context.Start.StartIndex)
            {
                var span = new Span(context.Start.StartIndex, context.Stop.StopIndex - context.Start.StartIndex + 1);
                var snapshotSpan = new SnapshotSpan(classificater.Buffer.CurrentSnapshot, span);
                var classificationSpan = new ClassificationSpan(snapshotSpan, classification);

                classificater.ClassfificationList.Add(classificationSpan);
            }
        }

        private void MakeTip(ParserRuleContext context,IToken token)
        {
            if (token == null) { return; }

            var startLine = token.Line;
            var stopLine = context.Start.Line - 1;

            while (startLine > 0)
            {
                if (classificater.CommentTable.ContainsKey(startLine))
                {
                    classificater.CommentTipList.Add(new CommentData(token.StartIndex, token.StopIndex, classificater.CommentTable[startLine]));
                    break;
                }
                startLine--;
                if (startLine > stopLine) { break; }
            }
        }
    }
}
