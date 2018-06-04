using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace FBSEditor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FBSName")]
    [Name("FBSName")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class NameFormat : ClassificationFormatDefinition
    {
        public NameFormat()
        {
            this.DisplayName = "FBS名称";
            this.ForegroundColor = Color.FromRgb(43, 152, 193);
            //this.TextDecorations = System.Windows.TextDecorations.Strikethrough;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FBSKey")]
    [Name("FBSKey")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class KeyFormat : ClassificationFormatDefinition
    {
        public KeyFormat()
        {
            this.DisplayName = "FBS关键字";
            this.ForegroundColor = Color.FromRgb(0,33,255);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FBSComment")]
    [Name("FBSComment")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class CommentFormat : ClassificationFormatDefinition
    {
        public CommentFormat()
        {
            this.DisplayName = "FBS注释";
            this.ForegroundColor = Color.FromRgb(153, 153, 153);
        }
    }
    
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FBSError")]
    [Name("FBSError")]
    [UserVisible(true)]
    [Order(Before = Priority.High)]
    internal sealed class ErrorFormat : ClassificationFormatDefinition
    {
        public ErrorFormat()
        {
            this.DisplayName = "FBS错误";
            this.ForegroundColor = Colors.Red;
        }
    }
    
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FBSString")]
    [Name("FBSString")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class StringTokenFormat : ClassificationFormatDefinition
    {
        public StringTokenFormat()
        {
            this.DisplayName = "FBS字符串";
            this.ForegroundColor = Color.FromRgb(163, 21, 21);
        }
    }

    
}
