using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ZYC.Framework.Overlay;

internal partial class OverlayWindow
{
    public OverlayWindow()
    {
        InitializeComponent();
    }

    public bool BlocksInput { get; set; } = false;

    public void SetTarget(UIElement target)
    {
        Mask.TargetElement = target;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        var src = (HwndSource)PresentationSource.FromVisual(this)!;
        src.AddHook(WndProc);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_NCHITTEST = 0x0084;
        const int HTCLIENT = 1;
        const int HTTRANSPARENT = -1;

        if (msg == WM_NCHITTEST)
        {
            // Key: when not blocking, make the whole window hit-test transparent (let events pass through).
            if (!BlocksInput || Opacity <= 0.01)
            {
                handled = true;
                return new IntPtr(HTTRANSPARENT);
            }


            // 1) lParam is screen physical pixels (px)
            int sx = (short)(lParam.ToInt64() & 0xFFFF);
            int sy = (short)((lParam.ToInt64() >> 16) & 0xFFFF);

            // 2) Convert to overlay client-area physical pixels (px)
            var pt = new POINT { X = sx, Y = sy };
            ScreenToClient(hwnd, ref pt);

            // 3) Current overlay DPI (px per DIP)
            var scale = GetDpiScaleForHwnd(hwnd); // e.g. 150% => 1.5

            // 4) px -> DIP (same coordinate space as HoleRect)
            var pDip = new Point(pt.X / scale, pt.Y / scale);

            handled = true;
            return Mask.IsPassThrough(pDip)
                ? new IntPtr(HTTRANSPARENT)
                : new IntPtr(HTCLIENT);
        }

        return IntPtr.Zero;
    }

    private static double GetDpiScaleForHwnd(IntPtr hwnd)
    {
        // GetDpiForWindow returns dpi (e.g. 96/120/144...)
        var dpi = GetDpiForWindow(hwnd);
        return dpi / 96.0;
    }

    [DllImport("user32.dll")]
    private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hwnd);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }
}