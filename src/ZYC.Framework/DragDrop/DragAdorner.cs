using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ZYC.Framework.DragDrop;

internal class DragAdorner : Adorner
{
    private readonly Rectangle _child;
    private readonly TranslateTransform _transform;

    public DragAdorner(UIElement adornedElement, UIElement visualElement) : base(adornedElement)
    {
        _transform = new TranslateTransform();
        var brush = new VisualBrush(visualElement) { Opacity = 0.6 };

        _child = new Rectangle
        {
            Width = visualElement.RenderSize.Width,
            Height = visualElement.RenderSize.Height,
            Fill = brush,
            IsHitTestVisible = false,
            RenderTransform = _transform
        };
    }

    protected override int VisualChildrenCount => 1;

    public void UpdatePosition(double x, double y)
    {
        _transform.X = x;
        _transform.Y = y;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _child.Arrange(new Rect(new Point(0, 0), _child.DesiredSize));
        return finalSize;
    }

    protected override Visual GetVisualChild(int index)
    {
        return _child;
    }
}