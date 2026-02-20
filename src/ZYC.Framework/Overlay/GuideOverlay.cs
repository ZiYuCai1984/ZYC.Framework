using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using ZYC.Framework.Abstractions.Overlay;

namespace ZYC.Framework.Overlay;

internal sealed class GuideOverlay : IOverlay
{
    private static readonly Duration FadeDuration = TimeSpan.FromMilliseconds(140);
    private readonly OverlayWindow _overlay;
    private readonly Window _owner;

    public GuideOverlay(Window owner)
    {
        _owner = owner;
        _overlay = new OverlayWindow { Owner = owner };

        owner.LocationChanged += (_, _) => SyncBounds();
        owner.SizeChanged += (_, _) => SyncBounds();
        owner.StateChanged += (_, _) => SyncBounds();
        owner.IsVisibleChanged += (_, _) => SyncBounds();
    }

    public void Dispose()
    {
        _overlay.Close();
    }

    public void Show(UIElement target, UIElement? passThrough = null)
    {
        _overlay.Mask.TargetElement = target;
        _overlay.Mask.PassThroughElement = passThrough;

        SyncBounds();

        if (!_overlay.IsVisible)
        {
            _overlay.Opacity = 0;
            _overlay.Show();
        }

        // Start intercepting
        _overlay.BlocksInput = true;

        FadeTo(1.0, true, new Duration(TimeSpan.FromMilliseconds(256)), null);
    }

    public void Hide()
    {
        // Do not block the mouse during fade-out (feels smoother).
        _overlay.BlocksInput = false;

        if (!_overlay.IsVisible)
        {
            return;
        }

        FadeTo(0.0, true, new Duration(TimeSpan.FromMilliseconds(512)), () =>
        {
            // Always Hide at the end; otherwise the transparent window might still affect hit-testing
            // (even though BlocksInput=false already covers it).
            _overlay.Hide();
        });
    }


    private void FadeTo(double to, bool easeOut, Duration duration, Action? onCompleted)
    {
        // SnapshotAndReplace: new animations replace old ones, enabling reverse animations without flashing.
        _overlay.BeginAnimation(UIElement.OpacityProperty, null);

        var anim = new DoubleAnimation
        {
            From = _overlay.Opacity,
            To = to,
            Duration = duration,
            EasingFunction = easeOut ? new CubicEase { EasingMode = EasingMode.EaseOut } : null,
            FillBehavior = FillBehavior.HoldEnd
        };

        if (onCompleted != null)
        {
            anim.Completed += (_, _) => onCompleted();
        }

        _overlay.BeginAnimation(UIElement.OpacityProperty, anim, HandoffBehavior.SnapshotAndReplace);
    }


    private void SyncBounds()
    {
        if (!_owner.IsVisible || _owner.WindowState == WindowState.Minimized)
        {
            _overlay.Hide();
            return;
        }

        var ownerHwnd = new WindowInteropHelper(_owner).Handle;
        if (ownerHwnd == IntPtr.Zero)
        {
            return;
        }

        // 1) Owner client size (physical pixels px)
        GetClientRect(ownerHwnd, out var rcClient);
        var widthPx = rcClient.Right - rcClient.Left;
        var heightPx = rcClient.Bottom - rcClient.Top;

        // 2) Client origin (0,0) -> screen coordinates (physical pixels px)
        var pt = new POINT { X = 0, Y = 0 };
        ClientToScreen(ownerHwnd, ref pt);

        // 3) Use owner's DPI for px -> DIP
        var scale = GetDpiScaleForHwnd(ownerHwnd);

        _overlay.Left = pt.X / scale;
        _overlay.Top = pt.Y / scale;
        _overlay.Width = widthPx / scale;
        _overlay.Height = heightPx / scale;
    }

    private static double GetDpiScaleForHwnd(IntPtr hwnd)
    {
        return GetDpiForWindow(hwnd) / 96.0;
    }

    [DllImport("user32.dll")]
    private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hwnd);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X, Y;
    }
}