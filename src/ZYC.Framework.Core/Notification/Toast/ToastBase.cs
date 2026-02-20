using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using ZYC.Framework.Abstractions.Notification.Toast;

namespace ZYC.Framework.Core.Notification.Toast;

[ContentProperty(nameof(ToastContent))]
public class ToastBase : NotificationBase, IToast
{
    public ToastBase()
    {
        ToastBorder = CreateToastBorder();
        Content = ToastBorder;
    }

    private Border ToastBorder { get; }

    public UIElement ToastContent
    {
        get => ToastBorder.Child;
        set => ToastBorder.Child = value;
    }

    public static Border CreateToastBorder()
    {
        var border = new Border
        {
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(12),
            Margin = new Thickness(0),
            ClipToBounds = true,
            BorderThickness = new Thickness(1),
            SnapsToDevicePixels = true
        };

        border.SetResourceReference(Border.BackgroundProperty, "MahApps.Brushes.Flyout.Background");
        border.SetResourceReference(Border.BorderBrushProperty, "MahApps.Brushes.Control.Border");

        return border;
    }
}