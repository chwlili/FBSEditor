using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media;

namespace FBSEditor.Editor
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(Constants.FBSContentType)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class FBSTextAdornmentTextViewCreationListener : IWpfTextViewCreationListener
    {
#pragma warning disable 649, 169

        [Export(typeof(AdornmentLayerDefinition))]
        [Name("FBSEditorTextAdornment")]
        [Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)]
        private AdornmentLayerDefinition editorAdornmentLayer;

#pragma warning restore 649, 169

        #region IWpfTextViewCreationListener

        public void TextViewCreated(IWpfTextView textView)
        {
            new FBSTextAdornment(textView);
        }

        #endregion
    }


    internal sealed class FBSTextAdornment
    {
        private readonly IAdornmentLayer layer;
        private readonly IWpfTextView view;
        private readonly Brush brush;
        private readonly Pen pen;

        public FBSTextAdornment(IWpfTextView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            this.layer = view.GetAdornmentLayer("FBSEditorTextAdornment");

            this.view = view;
            this.view.LayoutChanged += this.OnLayoutChanged;

            this.brush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0xff));
            this.brush.Freeze();
            
            var penBrush = new SolidColorBrush(Colors.Red);
            penBrush.Freeze();

            this.pen = new Pen(penBrush, 0.5);
            this.pen.Freeze();
        }

        private List<SnapshotSpan> GetErrorList()
        {
            var key = typeof(FBSClassification);
            if (view.TextBuffer.Properties.ContainsProperty(key))
            {
                var classification = view.TextBuffer.Properties.GetProperty<FBSClassification>(key);
                if (classification != null)
                {
                    return classification.ErrorList;
                }
            }
            return null;
        }

        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            this.layer.RemoveAllAdornments();

            IWpfTextViewLineCollection textViewLines = this.view.TextViewLines;

            var errors = GetErrorList();
            foreach (var span in errors)
            {
                Geometry geometry = textViewLines.GetMarkerGeometry(span);

                if (geometry != null)
                {
                    var drawing = new GeometryDrawing(this.brush, this.pen, geometry);
                    drawing.Freeze();

                    var drawingImage = new DrawingImage(drawing);
                    drawingImage.Freeze();

                    var image = new Image
                    {
                        Source = drawingImage,
                    };

                    // Align the image with the top of the bounds of the text geometry
                    Canvas.SetLeft(image, geometry.Bounds.Left);
                    Canvas.SetTop(image, geometry.Bounds.Top);

                    this.layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
                }
            }
        }
    }
}
