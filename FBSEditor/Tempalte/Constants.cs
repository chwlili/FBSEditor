using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace Tempalte
{
    class Constants
    {
        public const string ContentType = "fbstemplate";
        public const string ExtName = ".fbstemplate";

        public const string ClassificationKey = "ClassificationKey";
        public const string ClassificationText = "ClassificationText";
        public const string ClassificationCode = "ClassificationCode";
        public const string ClassificationTag = "ClassificationTag";
        public const string ClassificationBraceMatching = "ClassificationBraceMatching";

#pragma warning disable 649, 169

        [Export]
        [Name(Constants.ContentType)]
        [BaseDefinition("text")]
        private static ContentTypeDefinition contentType;

        [Export]
        [FileExtension(Constants.ExtName)]
        [ContentType(Constants.ContentType)]
        private static FileExtensionToContentTypeDefinition extensionContentType;

        [Name(ClassificationKey)]
        [Export(typeof(ClassificationTypeDefinition))]
        private static ClassificationTypeDefinition Key;

        [Name(ClassificationText)]
        [Export(typeof(ClassificationTypeDefinition))]
        private static ClassificationTypeDefinition Text;

        [Name(ClassificationCode)]
        [Export(typeof(ClassificationTypeDefinition))]
        private static ClassificationTypeDefinition Code;

        [Name(ClassificationTag)]
        [Export(typeof(ClassificationTypeDefinition))]
        private static ClassificationTypeDefinition Tag;

        [Name(ClassificationBraceMatching)]
        [Export(typeof(ClassificationTypeDefinition))]
        private static ClassificationTypeDefinition BraceMatching;

#pragma warning restore 649, 169
    }
}
