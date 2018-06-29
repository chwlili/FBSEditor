using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace FBSEditor
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(OutliningRegionTag))]
    [ContentType(Constants.FBSContentType)]
    internal class FBSOutliningTagProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() =>
            {
                return (new FBSOutline(buffer)) as ITagger<T>;
            });
        }
    }

    internal class FBSOutline : ITagger<OutliningRegionTag>
    {
        private ITextBuffer textBuffer;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public FBSOutline(ITextBuffer buffer)
        {
            this.textBuffer = buffer;
            this.textBuffer.Changed += OnTextBufferChanged;
        }

        private List<int> GetOutlineList()
        {
            var key = typeof(FBSClassification);
            if (textBuffer.Properties.ContainsProperty(key))
            {
                var classification = textBuffer.Properties.GetProperty<FBSClassification>(key);
                if (classification != null)
                {
                    return classification.OutlineList;
                }
            }
            return null;
        }

        public IEnumerable<ITagSpan<OutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0){yield break;}

            var outlineList = GetOutlineList();

            if (outlineList == null || outlineList.Count == 0) { yield break; }

            var snapshot = spans[0].Snapshot;
            for (int i = 0; i < outlineList.Count; i += 2)
            {
                int start = outlineList[i];
                var length = outlineList[i + 1];

                var tag = new OutliningRegionTag(true, true, "...", snapshot.GetText(start, length).Trim());
                var snapshotSpan = new SnapshotSpan(snapshot, start, length);

                yield return new TagSpan<OutliningRegionTag>(snapshotSpan, tag);
            }
        }

        void OnTextBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            if (e.After == textBuffer.CurrentSnapshot)
            {
                var snapshot = textBuffer.CurrentSnapshot;
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
            }
        }
    }
}
