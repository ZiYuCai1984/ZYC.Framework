using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.DragDrop;

/// <summary>
///     !WARNING Design by chatgpt 5.4
/// </summary>
internal sealed class TabInsertAdorner : Adorner
{
    private TabInsertPosition _insertPosition;

    public TabInsertAdorner(UIElement adornedElement) : base(adornedElement)
    {
        IsHitTestVisible = false;
        SnapsToDevicePixels = true;
    }

    public void Update(TabInsertPosition insertPosition)
    {
        _insertPosition = insertPosition;
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        var size = AdornedElement.RenderSize;
        if (size.Width <= 0 || size.Height <= 0)
        {
            return;
        }

        var brush = SystemColors.HighlightBrush;
        var fill = brush.Clone();
        fill.Opacity = 0.10;

        const double penThickness = 2.0;
        var pen = new Pen(brush, penThickness)
        {
            StartLineCap = PenLineCap.Round,
            EndLineCap = PenLineCap.Round,
            LineJoin = PenLineJoin.Round
        };

        var rect = _insertPosition == TabInsertPosition.Before
            ? new Rect(0, 0, size.Width / 2.0, size.Height)
            : new Rect(size.Width / 2.0, 0, size.Width / 2.0, size.Height);

        drawingContext.DrawRoundedRectangle(fill, null, rect, 4, 4);

        DrawInsertionIBeam(drawingContext, size, pen);
    }

    private void DrawInsertionIBeam(
        DrawingContext drawingContext,
        Size size,
        Pen pen)
    {
        //!WARNING If here use 0 and Width, it will result in incomplete display at the boundaries.
        var x = _insertPosition == TabInsertPosition.Before
            ? 1.5
            : size.Width - 1.5;

        const double topMargin = 0.0;
        const double bottomMargin = 0.0;

        var top = topMargin;
        var bottom = Math.Max(top + 8.0, size.Height - bottomMargin);

        var armLength = Math.Min(10.0, Math.Max(6.0, size.Width * 0.18));
        var halfArmLength = armLength / 2.0;

        drawingContext.DrawLine(pen, new Point(x, top), new Point(x, bottom));

        drawingContext.DrawLine(
            pen,
            new Point(x - halfArmLength, top),
            new Point(x + halfArmLength, top));

        drawingContext.DrawLine(
            pen,
            new Point(x - halfArmLength, bottom),
            new Point(x + halfArmLength, bottom));
    }
}