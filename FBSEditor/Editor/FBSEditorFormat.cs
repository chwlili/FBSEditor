using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace FBSEditor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Normal")]
    [Name("Normal")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class NormalFormat : ClassificationFormatDefinition
    {
        public NormalFormat()
        {
            this.DisplayName = "Normal";
            this.ForegroundColor = Colors.Green;
            //this.TextDecorations = System.Windows.TextDecorations.Strikethrough;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Name")]
    [Name("Name")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class NameFormat : ClassificationFormatDefinition
    {
        public NameFormat()
        {
            this.DisplayName = "Name";
            this.ForegroundColor = Color.FromRgb(43, 152, 193);
            //this.TextDecorations = System.Windows.TextDecorations.Strikethrough;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Key")]
    [Name("Key")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class KeyFormat : ClassificationFormatDefinition
    {
        public KeyFormat()
        {
            this.DisplayName = "Key";
            this.ForegroundColor = Color.FromRgb(0,33,255);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Comm")]
    [Name("Comm")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class CommentFormat : ClassificationFormatDefinition
    {
        public CommentFormat()
        {
            this.DisplayName = "Comm";
            this.ForegroundColor = Color.FromRgb(153, 153, 153);
        }
    }
    
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "ErrorToken")]
    [Name("ErrorToken")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class ErrorFormat : ClassificationFormatDefinition
    {
        public ErrorFormat()
        {
            this.DisplayName = "ErrorToken";
            this.ForegroundColor = Colors.Red;
            this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "StringToken")]
    [Name("StringToken")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class StringTokenFormat : ClassificationFormatDefinition
    {
        public StringTokenFormat()
        {
            this.DisplayName = "StringToken";
            this.ForegroundColor = Color.FromRgb(163, 21, 21);
        }
    }

    
}
