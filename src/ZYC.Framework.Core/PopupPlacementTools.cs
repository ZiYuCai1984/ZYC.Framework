using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ZYC.Framework.Core;

public enum AnchorCorner
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}

public static class PopupPlacementTools
{
    public static readonly DependencyProperty TargetCornerProperty =
        DependencyProperty.RegisterAttached(
            "TargetCorner",
            typeof(AnchorCorner),
            typeof(PopupPlacementTools),
            new PropertyMetadata(AnchorCorner.TopLeft, OnAnyChanged));

    public static readonly DependencyProperty PopupCornerProperty =
        DependencyProperty.RegisterAttached(
            "PopupCorner",
            typeof(AnchorCorner),
            typeof(PopupPlacementTools),
            new PropertyMetadata(AnchorCorner.TopLeft, OnAnyChanged));

    public static readonly DependencyProperty OffsetProperty =
        DependencyProperty.RegisterAttached(
            "Offset",
            typeof(Vector),
            typeof(PopupPlacementTools),
            new PropertyMetadata(new Vector(0, 0), OnAnyChanged));

    public static void SetTargetCorner(DependencyObject d, AnchorCorner value)
    {
        d.SetValue(TargetCornerProperty, value);
    }

    public static AnchorCorner GetTargetCorner(DependencyObject d)
    {
        return (AnchorCorner)d.GetValue(TargetCornerProperty);
    }

    public static void SetPopupCorner(DependencyObject d, AnchorCorner value)
    {
        d.SetValue(PopupCornerProperty, value);
    }

    public static AnchorCorner GetPopupCorner(DependencyObject d)
    {
        return (AnchorCorner)d.GetValue(PopupCornerProperty);
    }

    public static void SetOffset(DependencyObject d, Vector value)
    {
        d.SetValue(OffsetProperty, value);
    }

    public static Vector GetOffset(DependencyObject d)
    {
        return (Vector)d.GetValue(OffsetProperty);
    }

    private static void OnAnyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ContextMenu cm)
        {
            cm.Placement = PlacementMode.Custom;
            cm.CustomPopupPlacementCallback = BuildCallback(cm);
        }
        else if (d is ToolTip tt)
        {
            tt.Placement = PlacementMode.Custom;
            tt.CustomPopupPlacementCallback = BuildCallback(tt);
        }
        else if (d is Popup popup)
        {
            popup.Placement = PlacementMode.Custom;
            popup.CustomPopupPlacementCallback = BuildCallback(popup);
        }
    }

    private static CustomPopupPlacementCallback BuildCallback(DependencyObject d)
    {
        var targetCorner = GetTargetCorner(d);
        var popupCorner = GetPopupCorner(d);
        var extraOffset = GetOffset(d);

        return (popupSize, targetSize, _) =>
        {
            var a0 = new CornerAlign(targetCorner, popupCorner);
            var a1 = a0.FlipH();
            var a2 = a0.FlipV();
            var a3 = a0.FlipH().FlipV();

            return new[]
            {
                MakePlacement(a0, popupSize, targetSize, extraOffset),
                MakePlacement(a1, popupSize, targetSize, extraOffset),
                MakePlacement(a2, popupSize, targetSize, extraOffset),
                MakePlacement(a3, popupSize, targetSize, extraOffset)
            };
        };
    }

    private static CustomPopupPlacement MakePlacement(CornerAlign align, Size popupSize, Size targetSize,
        Vector extraOffset)
    {
        var targetPoint = GetCornerPoint(align.TargetCorner, targetSize);
        var popupCornerPoint = GetCornerPoint(align.PopupCorner, popupSize);

        var topLeft = new Point(
            targetPoint.X - popupCornerPoint.X + extraOffset.X,
            targetPoint.Y - popupCornerPoint.Y + extraOffset.Y);

        return new CustomPopupPlacement(topLeft, PopupPrimaryAxis.None);
    }

    private static Point GetCornerPoint(AnchorCorner corner, Size size)
    {
        return corner switch
        {
            AnchorCorner.TopLeft => new Point(0, 0),
            AnchorCorner.TopRight => new Point(size.Width, 0),
            AnchorCorner.BottomLeft => new Point(0, size.Height),
            AnchorCorner.BottomRight => new Point(size.Width, size.Height),
            _ => new Point(0, 0)
        };
    }

    private readonly record struct CornerAlign(AnchorCorner TargetCorner, AnchorCorner PopupCorner)
    {
        public CornerAlign FlipH()
        {
            return new CornerAlign(FlipH(TargetCorner), FlipH(PopupCorner));
        }

        public CornerAlign FlipV()
        {
            return new CornerAlign(FlipV(TargetCorner), FlipV(PopupCorner));
        }

        private static AnchorCorner FlipH(AnchorCorner c)
        {
            return c switch
            {
                AnchorCorner.TopLeft => AnchorCorner.TopRight,
                AnchorCorner.TopRight => AnchorCorner.TopLeft,
                AnchorCorner.BottomLeft => AnchorCorner.BottomRight,
                AnchorCorner.BottomRight => AnchorCorner.BottomLeft,
                _ => c
            };
        }

        private static AnchorCorner FlipV(AnchorCorner c)
        {
            return c switch
            {
                AnchorCorner.TopLeft => AnchorCorner.BottomLeft,
                AnchorCorner.BottomLeft => AnchorCorner.TopLeft,
                AnchorCorner.TopRight => AnchorCorner.BottomRight,
                AnchorCorner.BottomRight => AnchorCorner.TopRight,
                _ => c
            };
        }
    }
}