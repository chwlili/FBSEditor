using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace FBSEditor
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType(Constants.FBSContentType)]
    [TagType(typeof(TextMarkerTag))]
    internal class ShaderlabBraceMatchingTaggerProvider : IViewTaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null || buffer == null)
            {
                return null;
            }

            return (new FBSBraceMatching(textView, buffer)) as ITagger<T>;
        }
    }

    internal class FBSBraceMatching : ITagger<TextMarkerTag>
    {
        private ITextView textView;
        private ITextBuffer textBuffer;
        private List<int> tokens;
        private int caretPosition;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public FBSBraceMatching(ITextView textView, ITextBuffer buffer)
        {
            this.textView = textView;
            this.textView.LayoutChanged += OnLayoutChanged;
            this.textView.Caret.PositionChanged += OnCaretPositionChanged;

            this.textBuffer = buffer;
            this.textBuffer.Changed += OnTextBufferChanged;

            this.tokens = new List<int>();

            //RebuildTokens();
        }

        private List<int> GetBraceList()
        {
            var key = typeof(FBSClassification);
            if (textBuffer.Properties.ContainsProperty(key))
            {
                var classification = textBuffer.Properties.GetProperty<FBSClassification>(key);
                if (classification != null)
                {
                    return classification.BracePairList;
                }
            }
            return null;
        }

        public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0) { yield break; }

            List<int> tokens = GetBraceList();

            if (tokens==null || tokens.Count == 0) { yield break; }

            var snapshot = spans[0].Snapshot;
            var openOut = -1;
            var openIn = -1;
            var closeIn = -1;
            var closeOut = -1;
            for (var i = 0; i < tokens.Count; i+=2)
            {
                var l = tokens[i];
                var r = tokens[i + 1];
                if (caretPosition == l) { openOut = i; }
                if (caretPosition - 1 == l) { openIn = i; }
                if (caretPosition == r) { closeIn = i; }
                if (caretPosition - 1 == r) { closeOut = i; }
            }

            var index = -1;
            if (openOut != -1)
            {
                index = openOut;
            }
            else if (closeOut != -1)
            {
                index = closeOut;
            }
            else if (openIn != -1)
            {
                index = openIn;
            }
            else if (closeIn != -1)
            {
                index = closeIn;
            }

            if (index != -1)
            {
                var l = tokens[index];
                var r = tokens[index + 1];
                yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(snapshot, l, 1), new TextMarkerTag("FBSBraceMatching"));
                yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(snapshot, r, 1), new TextMarkerTag("FBSBraceMatching"));
            }
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (e.NewSnapshot != e.OldSnapshot)
            {
                UpdateAtCaretPosition(this.textView.Caret.Position);
            }
        }

        private void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            UpdateAtCaretPosition(e.NewPosition);
        }

        private void UpdateAtCaretPosition(CaretPosition caretPos)
        {
            caretPosition = caretPos.VirtualBufferPosition.Position;

            ITextSnapshot currentSnapshot = this.textBuffer.CurrentSnapshot;
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(currentSnapshot, 0, currentSnapshot.Length)));
        }

        void OnTextBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            if (e.After == textBuffer.CurrentSnapshot)
            {
                ITextSnapshot currentSnapshot = this.textBuffer.CurrentSnapshot;
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(currentSnapshot, 0, currentSnapshot.Length)));
            }
        }
    }
}
