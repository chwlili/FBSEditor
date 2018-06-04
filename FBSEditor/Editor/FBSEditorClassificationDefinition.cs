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
        [Name("Normal")]
        private static ClassificationTypeDefinition normalDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Name")]
        private static ClassificationTypeDefinition nameDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Key")]
        private static ClassificationTypeDefinition keyDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Comm")]
        private static ClassificationTypeDefinition commentDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("ErrorToken")]
        private static ClassificationTypeDefinition errorDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("StringToken")]
        private static ClassificationTypeDefinition StringToken;



#pragma warning restore 169
    }
}
