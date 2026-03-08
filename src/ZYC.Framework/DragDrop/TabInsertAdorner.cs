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

        var pen = new Pen(brush, 2)
        {
            StartLineCap = PenLineCap.Round,
            EndLineCap = PenLineCap.Round,
            LineJoin = PenLineJoin.Round
        };

        var x = _insertPosition == TabInsertPosition.Before
            ? 2.0
            : size.Width - 2.0;

        var top = 4.0;
        var bottom = Math.Max(top + 4, size.Height - 4.0);

        var rect = _insertPosition == TabInsertPosition.Before
            ? new Rect(0, 0, size.Width / 2.0, size.Height)
            : new Rect(size.Width / 2.0, 0, size.Width / 2.0, size.Height);

        drawingContext.DrawRoundedRectangle(fill, null, rect, 4, 4);

        var waveGeometry = CreateVerticalRandomCurveGeometry(
            x,
            top,
            bottom,
            3.0,
            10.0, Random.Shared);

        drawingContext.DrawGeometry(null, pen, waveGeometry);
    }

    private static Geometry CreateVerticalRandomCurveGeometry(
        double x,
        double top,
        double bottom,
        double amplitude,
        double segmentHeight,
        Random random)
    {
        var geometry = new StreamGeometry();

        using var ctx = geometry.Open();

        var y = top;
        ctx.BeginFigure(new Point(x, y), false, false);

        while (y < bottom)
        {
            var h = segmentHeight * (0.8 + random.NextDouble() * 0.4);
            var nextY = Math.Min(y + h, bottom);

            var dx1 = (random.NextDouble() * 2.0 - 1.0) * amplitude;
            var dx2 = (random.NextDouble() * 2.0 - 1.0) * amplitude;
            var dx3 = (random.NextDouble() * 2.0 - 1.0) * amplitude;

            var c1 = new Point(x + dx1, y + (nextY - y) * 0.30);
            var c2 = new Point(x + dx2, y + (nextY - y) * 0.70);
            var end = new Point(x + dx3 * 0.2, nextY);

            ctx.BezierTo(c1, c2, end, true, false);

            y = nextY;
            x = end.X;
        }

        geometry.Freeze();
        return geometry;
    }
}