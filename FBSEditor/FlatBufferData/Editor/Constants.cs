using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace FlatBufferData.Editor
{
    class Constants
    {
        public const string BaseContentType = "text";

        public const string ContentType = "fbs";

        public const string ExtName = ".fbs";


        public static readonly string[] FBSLangTypes = new string[] { "bool" , "byte" , "ubyte" , "short" , "ushort" , "int" , "uint" , "float" , "long" , "ulong" , "double" , "int8" , "uint8" , "int16" , "uint16" , "int32" , "uint32" , "int64" , "uint64" , "float32" , "float64" , "string" };


#pragma warning disable 169

        [Export]
        [Name(Constants.ContentType)]
        [BaseDefinition(Constants.BaseContentType)]
        private static ContentTypeDefinition contentType;

        [Export]
        [FileExtension(Constants.ExtName)]
        [ContentType(Constants.ContentType)]
        private static FileExtensionToContentTypeDefinition extensionContentType;


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

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSError")]
        private static ClassificationTypeDefinition errorDefinition;




        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FBSBraceMatching")]
        private static ClassificationTypeDefinition braceMatching;

#pragma warning restore 169
    }
}
