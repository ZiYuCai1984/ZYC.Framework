using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.StatusBar;
using WindowState = System.Windows.WindowState;

namespace ZYC.Framework.Notification.Toast;

[RegisterSingleInstance]
internal sealed class ToastStackPopupHost : IDisposable
{
    private const double EdgeMarginRight = 8;

    private const double EdgeMarginBottom = 0;

    private const double ToastMinWidth = 240;

    private const double ToastMaxWidth = 420;

    private const double ToastMinHeight = 48;

    private const double ToastGap = 8;

    private readonly Dictionary<IToast, EventHandler> _closedHandlers = new();

    private readonly Dictionary<FrameworkElement, IToast> _elementToToast = new();
    private readonly Window _owner;

    private readonly Popup _popup;

    private readonly StackPanel _stack;

    private readonly IStatusBarView _statusBarView;

    private HwndSource? _hwndSource;

    public ToastStackPopupHost(
        IMainWindow mainWindow,
        IStatusBarView statusBarView,
        IRootGrid rootGrid,
        ToastConfig toastConfig)
    {
        var ownerWindow = (Window)mainWindow.GetMainWindow();

        _owner = ownerWindow;

        ToastConfig = toastConfig;
        _statusBarView = statusBarView;

        _stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            SnapsToDevicePixels = true,
            UseLayoutRounding = true
        };

        var container = new Border
        {
            Background = Brushes.Transparent,
            Child = _stack,
            SnapsToDevicePixels = true,
            UseLayoutRounding = true
        };

        _popup = new Popup
        {
            AllowsTransparency = true,
            Placement = PlacementMode.Custom,
            CustomPopupPlacementCallback = PlacePopupPlacement,
            PlacementTarget = (UIElement)rootGrid,
            StaysOpen = true,
            Child = container,
            IsHitTestVisible = true
        };

        _owner.LocationChanged += OwnerOnLayoutChanged;
        _owner.SizeChanged += OwnerOnLayoutChanged;
        _owner.StateChanged += OwnerOnStateChanged;
        _owner.IsVisibleChanged += OwnerOnIsVisibleChanged;

        _owner.Deactivated += OwnerOnDeactivated;
        _owner.Activated += OwnerOnActivated;

        _owner.SourceInitialized += OwnerOnSourceInitialized;
        _owner.Closed += OwnerOnClosed;
    }

    private ToastConfig ToastConfig { get; }

    private int MaxToasts => ToastConfig.MaxToasts;

    private void OwnerOnClosed(object? sender, EventArgs e)
    {
        Dispose();
    }

    private void OwnerOnSourceInitialized(object? sender, EventArgs e)
    {
        _hwndSource = (HwndSource)PresentationSource.FromVisual(_owner)!;
        _hwndSource.AddHook(WndProc);
    }


    private void OwnerOnDeactivated(object? sender, EventArgs e)
    {
        _popup.IsOpen = false;
    }

    private void OwnerOnActivated(object? sender, EventArgs e)
    {
        Show();
    }

    private void OwnerOnIsVisibleChanged(object? sender, DependencyPropertyChangedEventArgs e)
    {
        if (!_owner.IsVisible)
        {
            _popup.IsOpen = false;
        }
        else
        {
            Show();
            RefreshPlacement();
        }
    }

    private void OwnerOnLayoutChanged(object? sender, EventArgs e)
    {
        RefreshPlacement();
    }

    private void RefreshPlacement()
    {
        if (!_popup.IsOpen)
        {
            return;
        }

        _popup.HorizontalOffset += 0.001;
        _popup.HorizontalOffset -= 0.001;
    }

    private void OwnerOnStateChanged(object? sender, EventArgs e)
    {
        if (_owner.WindowState == WindowState.Minimized)
        {
            _popup.IsOpen = false;
        }
        else
        {
            Show();
        }
    }


    private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        // ReSharper disable InconsistentNaming
        const int WM_SIZE = 0x0005;
        const int SIZE_MINIMIZED = 1;

        const int WM_SHOWWINDOW = 0x0018;

        if (msg == WM_SIZE)
        {
            var sizeType = wParam.ToInt32();
            if (sizeType == SIZE_MINIMIZED)
            {
                _popup.IsOpen = false;
            }
        }
        else if (msg == WM_SHOWWINDOW)
        {
            var shown = wParam != nint.Zero;
            if (!shown)
            {
                _popup.IsOpen = false;
            }
            else
            {
                Show();
            }
        }

        return nint.Zero;
    }

    public void Show()
    {
        if (!ShouldBeVisible())
        {
            return;
        }

        _popup.IsOpen = true;
        RefreshPlacement();
    }

    private bool ShouldBeVisible()
    {
        return _owner.IsVisible
               && _owner.WindowState != WindowState.Minimized
               && _owner.IsActive;
    }

    private CustomPopupPlacement[] PlacePopupPlacement(Size popupSize, Size targetSize, Point offset)
    {
        if (_popup.Child is FrameworkElement fe)
        {
            fe.UseLayoutRounding = true;
            fe.SnapsToDevicePixels = true;
        }

        var v = (Visual)(_popup.PlacementTarget
                         ?? _popup.Child
                         ?? Application.Current.MainWindow)!;

        var dpi = VisualTreeHelper.GetDpi(v);
        var marginX = EdgeMarginRight * dpi.DpiScaleX;
        var marginY = EdgeMarginBottom * dpi.DpiScaleY;

        var statusBarH = _statusBarView.GetActualHeight() * dpi.DpiScaleY;

        var x = Math.Max(0, targetSize.Width - popupSize.Width - marginX);
        var y = Math.Max(0, targetSize.Height - popupSize.Height - marginY - statusBarH);

        x = Math.Round(x);
        y = Math.Round(y);

        return [new CustomPopupPlacement(new Point(x, y), PopupPrimaryAxis.None)];
    }

    public void Add(IToast toast)
    {
        if (toast is not FrameworkElement element)
        {
            throw new InvalidOperationException("IToast must be a FrameworkElement.");
        }

        if (!_owner.Dispatcher.CheckAccess())
        {
            _owner.Dispatcher.InvokeAsync(() => Add(toast));
            return;
        }

        element.MinWidth = ToastMinWidth;
        element.MaxWidth = ToastMaxWidth;
        element.MinHeight = ToastMinHeight;
        element.Height = double.NaN;
        element.HorizontalAlignment = HorizontalAlignment.Right;
        element.Margin = new Thickness(0, 0, 0, ToastGap);

        _stack.Children.Insert(0, element);

        _elementToToast[element] = toast;

        EventHandler handler = null!;
        handler = (_, _) =>
        {
            toast.Closed -= handler;
            _closedHandlers.Remove(toast);
            _elementToToast.Remove(element);
            RemoveCore(element);
        };

        _closedHandlers[toast] = handler;
        toast.Closed += handler;

        if (_stack.Children.Count > MaxToasts)
        {
            var oldestElement = (FrameworkElement)_stack.Children[^1];
            ForceRemove(oldestElement);
        }

        if (ShouldBeVisible())
        {
            _popup.IsOpen = true;
            Reposition();
        }
    }

    private void ForceRemove(FrameworkElement element)
    {
        if (_elementToToast.Remove(element, out var toast))
        {
            if (_closedHandlers.TryGetValue(toast, out var handler))
            {
                toast.Closed -= handler;
                _closedHandlers.Remove(toast);
            }
        }

        RemoveCore(element);
    }

    private void RemoveCore(FrameworkElement element)
    {
        if (_stack.Children.Contains(element))
        {
            _stack.Children.Remove(element);
        }

        if (_stack.Children.Count == 0)
        {
            _popup.IsOpen = false;
        }
        else
        {
            Reposition();
        }
    }


    private void Reposition()
    {
        var x = _popup.HorizontalOffset;
        _popup.HorizontalOffset = x + 1;
        _popup.HorizontalOffset = x;
    }

    public void Dispose()
    {
        _owner.LocationChanged -= OwnerOnLayoutChanged;
        _owner.SizeChanged -= OwnerOnLayoutChanged;
        _owner.StateChanged -= OwnerOnStateChanged;
        _owner.IsVisibleChanged -= OwnerOnIsVisibleChanged;
        _owner.Deactivated -= OwnerOnDeactivated;
        _owner.Activated -= OwnerOnActivated;

        _owner.SourceInitialized -= OwnerOnSourceInitialized;
        _owner.Closed -= OwnerOnClosed;

        if (_hwndSource != null)
        {
            _hwndSource.RemoveHook(WndProc);
            _hwndSource = null;
        }

        _popup.IsOpen = false;
    }
}