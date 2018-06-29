using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.BraceCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace FBSEditor
{

    public class FBSCompletionSource : ICompletionSource
    {
        private bool isDispose = false;

        private ShaderlabCompletionSourceProvider sourceProvider;
        private ITextBuffer textBuffer;
        private ITextDocument textDocument;

        public FBSCompletionSource(ShaderlabCompletionSourceProvider completonSourceProvider, ITextBuffer textBuffer, ITextDocument document)
        {
            this.sourceProvider = completonSourceProvider;
            this.textBuffer = textBuffer;
            this.textDocument = document;
        }
        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            List<Completion> completionList;

            completionList = new List<Completion>();
            completionList.Add(new Completion("int", "int", "insert int type", null, null));
            completionList.Add(new Completion("str", "str", "insert str type", null, null));
            completionSets.Add(new CompletionSet("Y1", "Y2", FindTokenSpanAtPosition(session.GetTriggerPoint(this.textBuffer), session), completionList, null));

            completionList = new List<Completion>();
            completionList.Add(new Completion("uint", "uint", "inert uint type", null, null));
            completionList.Add(new Completion("long", "long", "inert long type", null, null));
            completionSets.Add(new CompletionSet("X1", "X2", FindTokenSpanAtPosition(session.GetTriggerPoint(this.textBuffer), session), completionList, null));
        }

        private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint point, ICompletionSession completionSession)
        {
            SnapshotPoint ssPoint = (completionSession.TextView.Caret.Position.BufferPosition) - 1;
            ITextStructureNavigator navigator = sourceProvider.TextNavigatorService.GetTextStructureNavigator(this.textBuffer);
            TextExtent textExtent = navigator.GetExtentOfWord(ssPoint);
            return ssPoint.Snapshot.CreateTrackingSpan(textExtent.Span, SpanTrackingMode.EdgeInclusive);

        }

        public void Dispose()
        {
            if (isDispose)
            {
                GC.SuppressFinalize(this);
                isDispose = true;
            }
        }
    }

    [Export(typeof(ICompletionSourceProvider))]
    [Name("ShaderlabCompletionSourceProvider")]
    [ContentType(Constants.FBSContentType)]
    public class ShaderlabCompletionSourceProvider : ICompletionSourceProvider
    {

        [Import]
        public ITextStructureNavigatorSelectorService TextNavigatorService { get; set; }

        [Import]
        public ITextDocumentFactoryService textDocumentFactoryService { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            ITextDocument textDocument;
            textDocumentFactoryService.TryGetTextDocument(textBuffer, out textDocument);
            return new FBSCompletionSource(this, textBuffer, textDocument);
        }
    }


    public class FBSCompletionCommandHandlder : IOleCommandTarget
    {
        private ITextView textView;
        private FBSCompletionHandlerPrvider completionHandlerProvider;

        private IOleCommandTarget nextCommandHandler;
        private ICompletionSession completionSession;

        public FBSCompletionCommandHandlder(IVsTextView textViewAdapter, ITextView textView, FBSCompletionHandlerPrvider handlerProvider)
        {
            this.textView = textView;
            this.completionHandlerProvider = handlerProvider;

            textViewAdapter.AddCommandFilter(this, out nextCommandHandler);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return nextCommandHandler.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            uint cmdID = nCmdID;

            char typedChar = char.MinValue;
            if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
            {
                typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
            }

            if (nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB || nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN)
            {
                if (completionSession != null && !completionSession.IsDismissed)
                {
                    if (completionSession.SelectedCompletionSet.SelectionStatus.IsSelected)
                    {
                        completionSession.Commit();
                        return VSConstants.S_OK;
                    }
                    else
                    {
                        completionSession.Dismiss();
                    }
                }
            }

            int returnValue = nextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

            if (!typedChar.Equals(char.MinValue))
            {
                if (completionSession == null || completionSession.IsDismissed)
                {
                    SnapshotPoint? caretPoint = textView.Caret.Position.Point.GetPoint(textBuffer => !textBuffer.ContentType.IsOfType("projection"), PositionAffinity.Predecessor);
                    if (caretPoint.HasValue)
                    {
                        completionSession = completionHandlerProvider.CompletionBroker.CreateCompletionSession(textView, caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive), false);
                        completionSession.Dismissed += OnCompletionSessionDismissed;
                        completionSession.Start();
                    }

                    if (completionSession != null)
                    {
                        completionSession.Filter();
                    }
                }
                else
                {
                    completionSession.Filter();
                }

                return VSConstants.S_OK;
            }
            else if (cmdID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE || cmdID == (uint)VSConstants.VSStd2KCmdID.DELETE)
            {
                if (completionSession != null && !completionSession.IsDismissed)
                {
                    completionSession.Filter();
                }

                return VSConstants.S_OK;
            }

            return returnValue;
        }

        private void OnCompletionSessionDismissed(object sender, EventArgs e)
        {
            completionSession.Dismissed -= OnCompletionSessionDismissed;
            completionSession = null;
        }
    }

    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(Constants.FBSContentType)]
    [Name("FBSCompletionHandlerPrvider")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    public class FBSCompletionHandlerPrvider : IVsTextViewCreationListener
    {
        [Import]
        public IVsEditorAdaptersFactoryService AdapterService = null;

        [Import]
        public ICompletionBroker CompletionBroker { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
            if (textView == null)
            {
                return;
            }

            textView.Properties.GetOrCreateSingletonProperty(() =>
            {
                return new FBSCompletionCommandHandlder(textViewAdapter, textView, this);
            });
        }
    }
}
