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
        private static ClassificationTypeDefinition FBSKey;


        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSTableName")]
        private static ClassificationTypeDefinition FBSTableName;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSStructName")]
        private static ClassificationTypeDefinition FBSStructName;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSEnumName")]
        private static ClassificationTypeDefinition FBSEnumName;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSUnionName")]
        private static ClassificationTypeDefinition FBSUnionName;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSRpcName")]
        private static ClassificationTypeDefinition FBSRpcName;



        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSFieldName")]
        private static ClassificationTypeDefinition FBSFieldName;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSFieldType")]
        private static ClassificationTypeDefinition FBSFieldType;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSFieldValue")]
        private static ClassificationTypeDefinition FBSFieldValue;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSFieldMap")]
        private static ClassificationTypeDefinition FBSFieldMap;




        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSString")]
        private static ClassificationTypeDefinition StringToken;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSComment")]
        private static ClassificationTypeDefinition commentDefinition;

        

#pragma warning restore 169
    }
}
