using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification;

namespace ZYC.Framework.Notification.Banner;

[Register]
public sealed class BottomBannerPopupHost : NotificationPopupHost
{
    private const double BannerHeight = 48;

    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;

    private bool _hooked;
    private Window? _ownerWindow;

    public BottomBannerPopupHost(IMainWindow mainWindow, IRootGrid rootGrid, INotification notification)
        : base(mainWindow, rootGrid, notification)
    {
    }

    protected override CustomPopupPlacement[] PlacePopupPlacement(Popup popup, Size popupSize, Size targetSize,
        Point offset)
    {
        if (popup.Child is FrameworkElement fe)
        {
            fe.UseLayoutRounding = true;
            fe.SnapsToDevicePixels = true;
        }

        var x = 0.0;
        var y = Math.Max(0, targetSize.Height - popupSize.Height);
        return [new CustomPopupPlacement(new Point(x, y), PopupPrimaryAxis.None)];
    }

    protected override void SetupNotificationElement(FrameworkElement notificationElement, Popup popup)
    {
        notificationElement.SetBinding(FrameworkElement.WidthProperty, new Binding(nameof(FrameworkElement.ActualWidth))
        {
            Source = (FrameworkElement)popup.PlacementTarget,
            Mode = BindingMode.OneWay
        });

        notificationElement.Height = BannerHeight;

        HookPopupNudgeOverride(popup);
    }

    private void HookPopupNudgeOverride(Popup popup)
    {
        if (_hooked)
        {
            return;
        }

        _hooked = true;

        popup.Opened += (_, __) =>
        {
            // Get the host Window (follow updates when the window moves).
            _ownerWindow ??= Window.GetWindow(popup.PlacementTarget);

            if (_ownerWindow != null)
            {
                _ownerWindow.LocationChanged -= OwnerChanged;
                _ownerWindow.SizeChanged -= OwnerChanged;
                _ownerWindow.StateChanged -= OwnerChanged;

                _ownerWindow.LocationChanged += OwnerChanged;
                _ownerWindow.SizeChanged += OwnerChanged;
                _ownerWindow.StateChanged += OwnerChanged;
            }

            ScheduleForcePosition(popup);
        };

        popup.Closed += (_, __) =>
        {
            if (_ownerWindow != null)
            {
                _ownerWindow.LocationChanged -= OwnerChanged;
                _ownerWindow.SizeChanged -= OwnerChanged;
                _ownerWindow.StateChanged -= OwnerChanged;
            }
        };

        void OwnerChanged(object? s, EventArgs e)
        {
            ScheduleForcePosition(popup);
        }
    }

    private static void ScheduleForcePosition(Popup popup)
    {
        if (!popup.IsOpen)
        {
            return;
        }

        // Key: run after WPF finishes its own placement/adjustment.
        popup.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
        {
            ForcePopupFollowTarget(popup);
        }));
    }

    private static void ForcePopupFollowTarget(Popup popup)
    {
        if (!popup.IsOpen)
        {
            return;
        }

        if (popup.Child is not Visual child)
        {
            return;
        }

        if (popup.PlacementTarget is not Visual target)
        {
            return;
        }

        var src = PresentationSource.FromVisual(child) as HwndSource;
        if (src == null)
        {
            return;
        }

        // Target screen pixel coordinates (may be negative).
        var tlPx = target.PointToScreen(new Point(0, 0));

        // DPI (convert DIP sizes to pixels).
        var dpi = VisualTreeHelper.GetDpi(target);
        var scaleX = dpi.DpiScaleX;
        var scaleY = dpi.DpiScaleY;

        var targetWidthPx = (int)Math.Round(((FrameworkElement)popup.PlacementTarget).ActualWidth * scaleX);
        var targetHeightPx = (int)Math.Round(((FrameworkElement)popup.PlacementTarget).ActualHeight * scaleY);

        var bannerHeightPx = (int)Math.Round(BannerHeight * scaleY);

        // Place at the bottom of the target.
        var x = (int)Math.Round(tlPx.X);
        var y = (int)Math.Round(tlPx.Y + targetHeightPx - bannerHeightPx);

        SetWindowPos(
            src.Handle,
            IntPtr.Zero,
            x, y,
            targetWidthPx, bannerHeightPx,
            SWP_NOZORDER | SWP_NOACTIVATE);
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags);
}