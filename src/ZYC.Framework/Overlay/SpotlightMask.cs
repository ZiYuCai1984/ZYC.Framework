using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ZYC.Framework.Overlay;

internal sealed class SpotlightMask : FrameworkElement
{
    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(SpotlightMask),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty TargetElementsProperty =
        DependencyProperty.Register(nameof(TargetElements), typeof(UIElement[]), typeof(SpotlightMask),
            new FrameworkPropertyMetadata(Array.Empty<UIElement>(), FrameworkPropertyMetadataOptions.AffectsRender,
                OnTargetsChanged));

    public static readonly DependencyProperty MaskBrushProperty =
        DependencyProperty.Register(nameof(MaskBrush), typeof(Brush), typeof(SpotlightMask),
            new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(160, 0, 0, 0)),
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty HolePaddingProperty =
        DependencyProperty.Register(nameof(HolePadding), typeof(double), typeof(SpotlightMask),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(SpotlightMask),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    public SpotlightMask()
    {
        Focusable = false;

        // Avoid transparent areas not receiving hit tests (default transparent background may not intercept).
        IsHitTestVisible = true;

        // Override the mouse cursor
        Cursor = Cursors.Arrow;
    }

    public UIElement? PassThroughElement { get; set; }

    public Rect PassThroughRect => GetElementRectInThis(PassThroughElement);

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public UIElement[] TargetElements
    {
        get => (UIElement[])GetValue(TargetElementsProperty);
        set => SetValue(TargetElementsProperty, value);
    }

    public Brush MaskBrush
    {
        get => (Brush)GetValue(MaskBrushProperty);
        set => SetValue(MaskBrushProperty, value);
    }

    public double HolePadding
    {
        get => (double)GetValue(HolePaddingProperty);
        set => SetValue(HolePaddingProperty, value);
    }

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public bool IsPassThrough(Point pDip)
    {
        foreach (var target in TargetElements)
        {
            var rect = GetElementRectInThis(target);
            if (!rect.IsEmpty && rect.Contains(pDip))
            {
                return true;
            }
        }

        var s = PassThroughRect;
        if (!s.IsEmpty && s.Contains(pDip))
        {
            return true;
        }

        return false;
    }

    private Rect GetElementRectInThis(UIElement? element)
    {
        if (element is not FrameworkElement fe || !fe.IsVisible || !fe.IsLoaded)
        {
            return Rect.Empty;
        }

        if (fe.ActualWidth <= 0 || fe.ActualHeight <= 0)
        {
            return Rect.Empty;
        }

        try
        {
            // Screen (px) -> overlay (DIP)
            var tlScreen = fe.PointToScreen(new Point(0, 0));
            var brScreen = fe.PointToScreen(new Point(fe.ActualWidth, fe.ActualHeight));

            var tl = PointFromScreen(tlScreen);
            var br = PointFromScreen(brScreen);

            var left = Math.Min(tl.X, br.X);
            var top = Math.Min(tl.Y, br.Y);
            var right = Math.Max(tl.X, br.X);
            var bottom = Math.Max(tl.Y, br.Y);

            var rect = new Rect(new Point(left, top), new Point(right, bottom));
            rect.Inflate(HolePadding, HolePadding);
            return rect;
        }
        catch
        {
            return Rect.Empty;
        }
    }

    private static void OnTargetsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var mask = (SpotlightMask)d;
        if (e.OldValue is UIElement[] oldTargets)
        {
            foreach (var oldFe in oldTargets.OfType<FrameworkElement>())
            {
                oldFe.LayoutUpdated -= mask.Target_LayoutUpdated;
                oldFe.IsVisibleChanged -= mask.Target_IsVisibleChanged;
            }
        }

        if (e.NewValue is UIElement[] newTargets)
        {
            foreach (var newFe in newTargets.OfType<FrameworkElement>())
            {
                newFe.LayoutUpdated += mask.Target_LayoutUpdated;
                newFe.IsVisibleChanged += mask.Target_IsVisibleChanged;
            }
        }
    }

    private void Target_LayoutUpdated(object? sender, EventArgs e)
    {
        InvalidateVisual();
    }

    private void Target_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);

        if (!IsOpen)
        {
            return;
        }

        var fullRect = new Rect(0, 0, ActualWidth, ActualHeight);
        var holes = new GeometryGroup { FillRule = FillRule.Nonzero };

        foreach (var target in TargetElements)
        {
            var rect = GetElementRectInThis(target);
            if (!rect.IsEmpty && rect.Width > 0 && rect.Height > 0)
            {
                holes.Children.Add(new RectangleGeometry(rect, CornerRadius, CornerRadius));
            }
        }

        // Exclude the union of target areas so overlapping targets remain transparent.
        var geometry = new CombinedGeometry(GeometryCombineMode.Exclude, new RectangleGeometry(fullRect), holes);
        dc.DrawGeometry(MaskBrush, null, geometry);
    }

    // Let events pass through to target elements and the optional pass-through element.
    protected override HitTestResult? HitTestCore(PointHitTestParameters hitTestParameters)
    {
        if (!IsOpen)
        {
            return null;
        }

        var p = hitTestParameters.HitPoint;

        if (IsPassThrough(p))
        {
            return null;
        }

        // Outside hole: hit the mask and intercept interaction.
        return new PointHitTestResult(this, p);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        // Consume clicks outside the hole.
        if (IsOpen)
        {
            e.Handled = true;
        }

        base.OnMouseDown(e);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        if (IsOpen)
        {
            e.Handled = true;
        }

        base.OnMouseWheel(e);
    }
}
