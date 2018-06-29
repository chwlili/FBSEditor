using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace FBSEditor
{

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
            //this.TextDecorations = System.Windows.TextDecorations.Strikethrough;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FBSTableName")]
    [Name("FBSTableName")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class NameFormat : ClassificationFormatDefinition
    {
        public NameFormat()
        {
            this.DisplayName = "FBS表名";
            this.ForegroundColor = Color.FromRgb(43, 152, 193);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FBSStructName")]
    [Name("FBSStructName")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class FBSStructNameFormat : ClassificationFormatDefinition
    {
        public FBSStructNameFormat()
        {
            this.DisplayName = "FBS结构名";
            this.ForegroundColor = Color.FromRgb(43, 152, 193);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FBSEnumName")]
    [Name("FBSEnumName")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class FBSEnumNameFormat : ClassificationFormatDefinition
    {
        public FBSEnumNameFormat()
        {
            this.DisplayName = "FBS枚举名";
            this.ForegroundColor = Color.FromRgb(43, 152, 193);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FBSUnionName")]
    [Name("FBSUnionName")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class FBSUnionNameFormat : ClassificationFormatDefinition
    {
        public FBSUnionNameFormat()
        {
            this.DisplayName = "FBS联合名";
            this.ForegroundColor = Color.FromRgb(43, 152, 193);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FBSRpcName")]
    [Name("FBSRpcName")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class FBSRpcNameFormat : ClassificationFormatDefinition
    {
        public FBSRpcNameFormat()
        {
            this.DisplayName = "FBS服务名";
            this.ForegroundColor = Color.FromRgb(43, 152, 193);
        }
    }


    


    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FBSFieldName")]
    [Name("FBSFieldName")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class FBSFieldNameFormat : ClassificationFormatDefinition
    {
        public FBSFieldNameFormat()
        {
            this.DisplayName = "FBS字段名";
            this.ForegroundColor = Color.FromRgb(43, 152, 193);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FBSFieldType")]
    [Name("FBSFieldType")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class FBSFieldTypeFormat : ClassificationFormatDefinition
    {
        public FBSFieldTypeFormat()
        {
            this.DisplayName = "FBS字段类型";
            this.ForegroundColor = Color.FromRgb(43, 152, 193);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FBSFieldValue")]
    [Name("FBSFieldValue")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class FBSFieldValueFormat : ClassificationFormatDefinition
    {
        public FBSFieldValueFormat()
        {
            this.DisplayName = "FBS字段值";
            this.ForegroundColor = Color.FromRgb(43, 152, 193);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FBSFieldMap")]
    [Name("FBSFieldMap")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class FBSFieldMapFormat : ClassificationFormatDefinition
    {
        public FBSFieldMapFormat()
        {
            this.DisplayName = "FBS字段链接";
            this.ForegroundColor = Colors.Gold;// FromRgb(43, 152, 193);
            this.IsBold = true;
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
    [ClassificationType(ClassificationTypeNames = "FBSBraceMatching")]
    [Name("FBSBraceMatching")]
    [UserVisible(true)]
    [Order(Before = Priority.High)]
    internal sealed class BraceMatchingFormat : ClassificationFormatDefinition
    {
        public BraceMatchingFormat()
        {
            this.DisplayName = "FBS括号区配";
            this.BackgroundColor = Color.FromRgb(203, 203, 203);
        }
    }
}
