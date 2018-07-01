using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Adornments;

namespace FBSEditor
{
    class FBSQuickInfoSource :IAsyncQuickInfoSource
    {
        private FBSQuickInfoSourceProvider provider;
        private ITextBuffer textBuffer;

        public FBSQuickInfoSource(FBSQuickInfoSourceProvider provider,ITextBuffer textBuffer)
        {
            this.provider = provider;
            this.textBuffer = textBuffer;
        }

        private List<QuickInfoData> GetTipList()
        {
            var key = typeof(FBSClassification);
            if (textBuffer.Properties.ContainsProperty(key))
            {
                var classification = textBuffer.Properties.GetProperty<FBSClassification>(key);
                if (classification != null)
                {
                    return classification.QuickInfoList;
                }
            }
            return null;
        }

        public Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            QuickInfoData comment = null;

            SnapshotPoint? triggerPoint = session.GetTriggerPoint(textBuffer.CurrentSnapshot);
            if(triggerPoint.HasValue)
            {
                var tips = GetTipList();
                var position = triggerPoint.Value.Position;
                foreach (var tip in tips)
                {
                    if (position >= tip.start && position <= tip.stop)
                    {
                        comment = tip;
                        break;
                    }
                }
            }

            if(comment!=null)
            {
                var task = new Task<QuickInfoItem>(() =>
                    {
                        ITextSnapshot currentSnapshot = triggerPoint.Value.Snapshot;
                        return new QuickInfoItem(currentSnapshot.CreateTrackingSpan(comment.start, comment.stop - comment.start + 1, SpanTrackingMode.EdgeInclusive), comment.text);
                    },
                    cancellationToken
                );
                if (!task.IsCanceled)
                {
                    task.RunSynchronously();
                }

                return task;
            }

            session.DismissAsync();

            return null;
        }

        private bool m_isDisposed;
        public void Dispose()
        {
            if (!m_isDisposed)
            {
                GC.SuppressFinalize(this);
                m_isDisposed = true;
            }
        }
    }

    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name("FBSQuickInfoSourceProvider")]
    [ContentType(Constants.FBSContentType)]
    class FBSQuickInfoSourceProvider : IAsyncQuickInfoSourceProvider
    {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        [Import]
        internal ITextBufferFactoryService TextBufferFactoryService { get; set; }

        public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new FBSQuickInfoSource(this, textBuffer);
        }
    }

    class FBSQuickInfoController : IIntellisenseController
    {
        private ITextView textView;
        private IList<ITextBuffer> subjectBuffers;
        private FBSQuickInfoControllerProvider provider;

        private Task<IAsyncQuickInfoSession> task;

        public FBSQuickInfoController(ITextView textView, IList<ITextBuffer> subjectBuffers, FBSQuickInfoControllerProvider provider)
        {
            this.textView = textView;
            this.subjectBuffers = subjectBuffers;
            this.provider = provider;

            this.textView.MouseHover += OnTextViewMouseHover;
        }

        private void OnTextViewMouseHover(object sender, MouseHoverEventArgs e)
        {
            if(task!=null)
            {
                task.Dispose();
                task = null;
            }

            var spanPoint = new SnapshotPoint(textView.TextSnapshot, e.Position);

            SnapshotPoint? point = textView.BufferGraph.MapDownToFirstMatch(spanPoint, PointTrackingMode.Positive, snapshot => subjectBuffers.Contains(snapshot.TextBuffer), PositionAffinity.Predecessor);
            if (point != null)
            {
                var triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position, PointTrackingMode.Positive);
                if(!provider.broker.IsQuickInfoActive(textView))
                {
                    task = provider.broker.TriggerQuickInfoAsync(textView, triggerPoint);
                }
            }
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        public void Detach(ITextView textView)
        {
            if(this.textView==textView)
            {
                this.textView.MouseHover -= OnTextViewMouseHover;
                this.textView = null;
            }
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }
    }

    [Export(typeof(IIntellisenseControllerProvider))]
    [Name("FBSQuickInfoControllerProvider")]
    [ContentType(Constants.FBSContentType)]
    class FBSQuickInfoControllerProvider : IIntellisenseControllerProvider
    {
        [Import]
        public IAsyncQuickInfoBroker broker;

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            return new FBSQuickInfoController(textView, subjectBuffers, this);
        }
    }
}
