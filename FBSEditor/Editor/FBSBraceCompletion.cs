using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.BraceCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace FBSEditor
{
    [Export(typeof(IBraceCompletionContextProvider))]
    [ContentType(Constants.FBSContentType)]
    [BracePair('(', ')')]
    [BracePair('[', ']')]
    [BracePair('{', '}')]
    [BracePair('"', '"')]
    internal sealed class FBSBraceCompletionProvider : IBraceCompletionContextProvider
    {
        public bool TryCreateContext(ITextView textView, SnapshotPoint openingPoint, char openingBrace, char closingBrace, out IBraceCompletionContext context)
        {
            context = null;
            if (IsValidBraceCompletionContext(openingPoint))
            {
                context = new FBSBraceCompletion();
                return true;
            }

            return false;
        }

        private bool IsValidBraceCompletionContext(SnapshotPoint openingPoint)
        {
            if (openingPoint.Position >= 0)
            {
                return true;
            }

            return false;
        }
    }

    [Export(typeof(IBraceCompletionContext))]
    internal sealed class FBSBraceCompletion : IBraceCompletionContext
    {

        public bool AllowOverType(IBraceCompletionSession session)
        {
            return true;
        }

        public void Finish(IBraceCompletionSession session)
        {

        }

        public void OnReturn(IBraceCompletionSession session)
        {
            
        }

        public void Start(IBraceCompletionSession session)
        {
            if (session.OpeningBrace == '{')
            {
                var beginPos = session.OpeningPoint.GetPosition(session.SubjectBuffer.CurrentSnapshot);
                session.SubjectBuffer.Insert(beginPos, "\n");
                session.SubjectBuffer.Insert(beginPos + 2, "\n\t\n");
                session.TextView.Caret.MoveTo(new SnapshotPoint(session.SubjectBuffer.CurrentSnapshot, beginPos + 4));
            }
        }
    }
}
