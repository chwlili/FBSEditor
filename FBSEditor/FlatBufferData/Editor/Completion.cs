using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace FlatBufferData.Editor
{

    public class CompletionSource : ICompletionSource
    {
        private bool isDispose = false;

        private CompletionSourceProvider sourceProvider;
        private ITextBuffer textBuffer;

        private BitmapSource fbsTypeIcon;
        private BitmapSource userTypeIcon;

        public CompletionSource(CompletionSourceProvider completonSourceProvider, ITextBuffer textBuffer)
        {
            this.sourceProvider = completonSourceProvider;
            this.textBuffer = textBuffer;

            fbsTypeIcon = Imaging.CreateBitmapSourceFromHBitmap(FBSEditor.Properties.Resources.FbsTypeIcon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            userTypeIcon = Imaging.CreateBitmapSourceFromHBitmap(FBSEditor.Properties.Resources.UserTypeIcon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private List<string> GetStructNameList()
        {
            var key = typeof(Classification);
            if (textBuffer.Properties.ContainsProperty(key))
            {
                var classification = textBuffer.Properties.GetProperty<Classification>(key);
                if (classification != null)
                {
                    return classification.StructNameList;
                }
            }
            return null;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {

            List<Completion> completionList = new List<Completion>();
            foreach (var item in Constants.FBSLangTypes)
            {
                completionList.Add(new Completion(item, item, item, fbsTypeIcon, null));
            }
            foreach(var item in GetStructNameList())
            {
                completionList.Add(new Completion(item, item, item, userTypeIcon, null));
            }
            completionSets.Add(new CompletionSet("", "", FindTokenSpanAtPosition(session.GetTriggerPoint(this.textBuffer), session), completionList, null));
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
    [Name("FBSCompletionSourceProvider")]
    [ContentType(Constants.ContentType)]
    public class CompletionSourceProvider : ICompletionSourceProvider
    {

        [Import]
        public ITextStructureNavigatorSelectorService TextNavigatorService { get; set; }

        [Import]
        public ITextDocumentFactoryService textDocumentFactoryService { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new CompletionSource(this, textBuffer);
        }
    }


    public class CompletionCommandHandlder : IOleCommandTarget
    {
        private ITextView textView;
        private CompletionHandlerPrvider completionHandlerProvider;

        private IOleCommandTarget nextCommandHandler;
        private ICompletionSession completionSession;
        private bool checking;
        private int checkPosition;
        private int startPosition;

        public CompletionCommandHandlder(IVsTextView textViewAdapter, ITextView textView, CompletionHandlerPrvider handlerProvider)
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

            if (typedChar == ':' && (completionSession == null || completionSession.IsDismissed))
            {
                SnapshotPoint? caretPoint = textView.Caret.Position.Point.GetPoint(textBuffer => !textBuffer.ContentType.IsOfType("projection"), PositionAffinity.Predecessor);
                if (caretPoint.HasValue)
                {
                    checking = true;
                    checkPosition = caretPoint.Value.Position;
                    startPosition = -1;
                }
            }
            else if(checking)
            {
                if (!typedChar.Equals(char.MinValue) && typedChar != ' ' && typedChar != '\t' && typedChar != '\r' && typedChar != '\n' && typedChar!='[')
                {
                    if (completionSession != null) { completionSession.Dismiss(); }

                    SnapshotPoint? caretPoint = textView.Caret.Position.Point.GetPoint(textBuffer => !textBuffer.ContentType.IsOfType("projection"), PositionAffinity.Predecessor);
                    if (caretPoint.HasValue && caretPoint.Value.Position > checkPosition)
                    {
                        checking = false;
                        startPosition = caretPoint.Value.Position;

                        completionSession = completionHandlerProvider.CompletionBroker.CreateCompletionSession(textView, caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position + 1, PointTrackingMode.Positive), false);
                        completionSession.Dismissed += OnCompletionSessionDismissed;
                        completionSession.Start();

                        if (completionSession != null) { completionSession.Filter(); }
                        return VSConstants.S_OK;
                    }
                }
            }
            else if(cmdID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE || cmdID == (uint)VSConstants.VSStd2KCmdID.DELETE)
            {
                if (completionSession != null && !completionSession.IsDismissed)
                {
                    SnapshotPoint? caretPoint = textView.Caret.Position.Point.GetPoint(textBuffer => !textBuffer.ContentType.IsOfType("projection"), PositionAffinity.Predecessor);
                    if (caretPoint.HasValue)
                    {
                        if (caretPoint.Value.Position < checkPosition)
                        {
                            completionSession.Dismiss();
                            checking = false;
                            checkPosition = -1;
                            startPosition = -1;
                        }
                        else if (caretPoint.Value.Position < startPosition)
                        {
                            completionSession.Dismiss();
                            checking = checkPosition != -1;
                            startPosition = -1;
                        }
                        else
                        {
                            completionSession.Filter();
                        }
                        return VSConstants.S_OK;
                    }
                }
                else
                {
                    SnapshotPoint? caretPoint = textView.Caret.Position.Point.GetPoint(textBuffer => !textBuffer.ContentType.IsOfType("projection"), PositionAffinity.Predecessor);
                    if (caretPoint.HasValue && caretPoint.Value.Position <= checkPosition)
                    {
                        checking = false;
                        checkPosition = -1;
                        startPosition = -1;
                    }
                }
            }
            else if(completionSession != null && !completionSession.IsDismissed)
            {
                if (!typedChar.Equals(char.MinValue))
                {
                    if ((typedChar == '=' || typedChar == ';' || typedChar == '(' || cmdID == '\n' || cmdID == '\r' || cmdID == ']'))
                    {
                        completionSession.Dismiss();
                    }
                    else
                    {
                        completionSession.Filter();
                    }
                    return VSConstants.S_OK;
                }
                else if (nCmdID == (uint)VSConstants.VSStd2KCmdID.LEFT || nCmdID == (uint)VSConstants.VSStd2KCmdID.RIGHT)
                {
                    completionSession.Dismiss();
                }
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
    [ContentType(Constants.ContentType)]
    [Name("FBSCompletionHandlerPrvider")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    public class CompletionHandlerPrvider : IVsTextViewCreationListener
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
                return new CompletionCommandHandlder(textViewAdapter, textView, this);
            });
        }
    }
}
