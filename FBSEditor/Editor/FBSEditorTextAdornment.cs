using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media;

namespace FBSEditor.Editor
{
    /// <summary>
    /// FBSEditorTextAdornment places red boxes behind all the "a"s in the editor window
    /// </summary>
    internal sealed class FBSEditorTextAdornment
    {
        private readonly IAdornmentLayer layer;
        private readonly IWpfTextView view;
        private readonly Brush brush;
        private readonly Pen pen;

        public FBSEditorTextAdornment(IWpfTextView view)
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

        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            this.layer.RemoveAllAdornments();

            IWpfTextViewLineCollection textViewLines = this.view.TextViewLines;

            foreach (var span in FBSEditor.ErrorSpans)
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
