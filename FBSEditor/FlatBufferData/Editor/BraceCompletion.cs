using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.BraceCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace FlatBufferData.Editor
{
    [Export(typeof(IBraceCompletionContextProvider))]
    [ContentType(Constants.ContentType)]
    [BracePair('(', ')')]
    [BracePair('[', ']')]
    [BracePair('{', '}')]
    [BracePair('"', '"')]
    internal sealed class BraceCompletionProvider : IBraceCompletionContextProvider
    {
        public bool TryCreateContext(ITextView textView, SnapshotPoint openingPoint, char openingBrace, char closingBrace, out IBraceCompletionContext context)
        {
            context = null;
            if (IsValidBraceCompletionContext(openingPoint))
            {
                context = new BraceCompletion();
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
    internal sealed class BraceCompletion : IBraceCompletionContext
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
                var line = session.SubjectBuffer.CurrentSnapshot.GetLineFromPosition(beginPos).GetText();

                var indent = line.Substring(0, line.Length - line.TrimStart().Length);
                if (line.Equals(indent + "{}"))
                {
                    session.SubjectBuffer.Insert(beginPos + 1, "\n" + indent + "\t\n" + indent);
                    session.TextView.Caret.MoveTo(new SnapshotPoint(session.SubjectBuffer.CurrentSnapshot, beginPos + 3 + indent.Length * 1));
                }
                else
                {
                    session.SubjectBuffer.Insert(beginPos, "\n" + indent);
                    session.SubjectBuffer.Insert(beginPos + indent.Length + 2, "\n" + indent + "\t\n" + indent);
                    session.TextView.Caret.MoveTo(new SnapshotPoint(session.SubjectBuffer.CurrentSnapshot, beginPos + 4 + indent.Length * 2));
                }
            }
        }
    }
}
