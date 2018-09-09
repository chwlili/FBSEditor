using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace FlatBufferCode.Editor
{
    class Constants
    {
        public const string BaseContentType = "text";
        public const string ContentType = "fbstemplate";
        public const string ExtName = ".fbstemplate";

        public const string ClassificationKeyword = "ClassificationKeyword";
        public const string ClassificationText = "ClassificationText";
        public const string ClassificationCode = "ClassificationCode";
        public const string ClassificationTag = "ClassificationTag";
        public const string ClassificationComment = "ClassificationComment";
        public const string ClassificationBraceMatching = "ClassificationBraceMatching";

#pragma warning disable 649, 169

        [Export]
        [Name(Constants.ContentType)]
        [BaseDefinition(BaseContentType)]
        private static ContentTypeDefinition contentType;

        [Export]
        [FileExtension(Constants.ExtName)]
        [ContentType(Constants.ContentType)]
        private static FileExtensionToContentTypeDefinition extensionContentType;

        [Name(ClassificationKeyword)]
        [Export(typeof(ClassificationTypeDefinition))]
        private static ClassificationTypeDefinition Keyword;

        [Name(ClassificationText)]
        [Export(typeof(ClassificationTypeDefinition))]
        private static ClassificationTypeDefinition Text;

        [Name(ClassificationCode)]
        [Export(typeof(ClassificationTypeDefinition))]
        private static ClassificationTypeDefinition Code;

        [Name(ClassificationTag)]
        [Export(typeof(ClassificationTypeDefinition))]
        private static ClassificationTypeDefinition Tag;

        [Name(ClassificationComment)]
        [Export(typeof(ClassificationTypeDefinition))]
        private static ClassificationTypeDefinition Comment;

        [Name(ClassificationBraceMatching)]
        [Export(typeof(ClassificationTypeDefinition))]
        private static ClassificationTypeDefinition BraceMatching;

#pragma warning restore 649, 169
    }
}
