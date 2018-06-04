using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace FBSEditor
{
    /// <summary>
    /// Classification type definition export for FBSEditor
    /// </summary>
    internal static class FBSEditorClassificationDefinition
    {
        // This disables "The field is never used" compiler's warning. Justification: the field is used by MEF.
#pragma warning disable 169

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSKey")]
        private static ClassificationTypeDefinition keyDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSName")]
        private static ClassificationTypeDefinition nameDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSComment")]
        private static ClassificationTypeDefinition commentDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSString")]
        private static ClassificationTypeDefinition StringToken;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSError")]
        private static ClassificationTypeDefinition errorDefinition;

        

#pragma warning restore 169
    }
}
