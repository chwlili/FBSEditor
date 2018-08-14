using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace Tempalte
{
    class FormatProvider
    {
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = Constants.ClassificationKeyword)]
        [Name(Constants.ClassificationKeyword)]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class KeyFormat : ClassificationFormatDefinition
        {
            public KeyFormat()
            {
                this.DisplayName = "Temaplte关键字";
                this.ForegroundColor = Color.FromRgb(0, 33, 255);
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = Constants.ClassificationText)]
        [Name(Constants.ClassificationText)]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class TextFormat : ClassificationFormatDefinition
        {
            public TextFormat()
            {
                this.DisplayName = "Temaplte文本";
                this.ForegroundColor = Color.FromRgb(153, 153, 153);
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = Constants.ClassificationCode)]
        [Name(Constants.ClassificationCode)]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class CodeFormat : ClassificationFormatDefinition
        {
            public CodeFormat()
            {
                this.DisplayName = "Temaplte代码";
                this.ForegroundColor = Color.FromRgb(0, 0, 0);
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = Constants.ClassificationTag)]
        [Name(Constants.ClassificationTag)]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class TagFormat : ClassificationFormatDefinition
        {
            public TagFormat()
            {
                this.DisplayName = "Temaplte标记";
                this.ForegroundColor = Color.FromRgb(255, 0, 0);
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = Constants.ClassificationComment)]
        [Name(Constants.ClassificationComment)]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class CommentFormat : ClassificationFormatDefinition
        {
            public CommentFormat()
            {
                this.DisplayName = "Temaplte注释";
                this.ForegroundColor = Color.FromRgb(153, 153, 153);
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = Constants.ClassificationBraceMatching)]
        [Name(Constants.ClassificationBraceMatching)]
        [UserVisible(true)]
        [Order(Before = Priority.High)]
        internal sealed class BraceMatchingFormat : ClassificationFormatDefinition
        {
            public BraceMatchingFormat()
            {
                this.DisplayName = "Temaplte括号匹配";
                this.BackgroundColor = Color.FromRgb(203, 203, 203);
            }
        }

        
    }
}
