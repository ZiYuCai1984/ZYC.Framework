using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification;
using ZYC.Framework.Abstractions.StatusBar;

namespace ZYC.Framework.Notification.Toast;

[Register]
internal class ToastPopupHost : NotificationPopupHost
{
    // Bottom-right padding.
    private const double EdgeMargin = 8;

    // Common toast dimensions (adjust to your design).
    private const double ToastMinWidth = 240;
    private const double ToastMaxWidth = 420;
    private const double ToastMinHeight = 48;

    public ToastPopupHost(
        IStatusBarView statusBarView,
        IMainWindow mainWindow,
        IRootGrid rootGrid,
        INotification notification)
        : base(mainWindow, rootGrid, notification)
    {
        StatusBarView = statusBarView;
    }

    private IStatusBarView StatusBarView { get; }

    protected override void SetupNotificationElement(FrameworkElement notificationElement, Popup popup)
    {
        // Toasts usually don't stretch to the full window width; let content size itself, with a max width.
        notificationElement.MinWidth = ToastMinWidth;
        notificationElement.MaxWidth = ToastMaxWidth;
        notificationElement.MinHeight = ToastMinHeight;

        // Let the content decide its own height (do not force Height).
        notificationElement.Height = double.NaN;

        // Avoid a "jitter" when the first measured width is 0 after opening.
        notificationElement.HorizontalAlignment = HorizontalAlignment.Stretch;
        notificationElement.VerticalAlignment = VerticalAlignment.Stretch;
    }

    protected override CustomPopupPlacement[] PlacePopupPlacement(
        Popup popup,
        Size popupSize,
        Size targetSize,
        Point offset)
    {
        if (popup.Child is FrameworkElement fe)
        {
            fe.UseLayoutRounding = true;
            fe.SnapsToDevicePixels = true;
        }

        // Pick a Visual on the same monitor to read DPI (PlacementTarget is the most reliable).
        var v = (Visual)(popup.PlacementTarget
                         ?? popup.Child
                         ?? StatusBarView as Visual
                         ?? Application.Current.MainWindow)!;

        var dpi = VisualTreeHelper.GetDpi(v);
        var marginX = EdgeMargin * dpi.DpiScaleX;
        var marginY = EdgeMargin * dpi.DpiScaleY;

        // StatusBarView.GetActualHeight() is in DIP; convert it to device px.
        var statusBarH = StatusBarView.GetActualHeight() * dpi.DpiScaleY;

        var x = Math.Max(0, targetSize.Width - popupSize.Width - marginX);
        var y = Math.Max(0, targetSize.Height - popupSize.Height - marginY - statusBarH);

        // Optional: reduce "0.5 pixel" jitter (Popup often ends up on int positions).
        x = Math.Round(x);
        y = Math.Round(y);

        return [new CustomPopupPlacement(new Point(x, y), PopupPrimaryAxis.None)];
    }
}