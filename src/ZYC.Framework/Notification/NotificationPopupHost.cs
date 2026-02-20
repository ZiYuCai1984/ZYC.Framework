using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification;
using WindowState = System.Windows.WindowState;

namespace ZYC.Framework.Notification;

public abstract class NotificationPopupHost : IDisposable
{
    private readonly INotification _notification;
    private readonly Window _owner;
    private readonly Popup _popup;

    private HwndSource? _hwndSource;

    protected NotificationPopupHost(
        IMainWindow mainWindow,
        IRootGrid rootGrid,
        INotification notification)
    {
        var ownerWindow = (Window)mainWindow.GetMainWindow();

        _owner = ownerWindow;
        _notification = notification;


        var notificationElement = (FrameworkElement)notification;

        _popup = new Popup
        {
            AllowsTransparency = true,
            Placement = PlacementMode.Custom,
            PlacementTarget = (FrameworkElement)rootGrid.GetRootGrid(),
            StaysOpen = true,
            Focusable = false,
            Child = notificationElement,
            CustomPopupPlacementCallback = CustomPopupPlacementCallback
        };

        // ReSharper disable once VirtualMemberCallInConstructor
        SetupNotificationElement(notificationElement, _popup);

        notificationElement.UseLayoutRounding = true;
        notificationElement.SnapsToDevicePixels = true;


        notification.Closed += OnNotificationClosed;

        _owner.LocationChanged += OwnerOnLayoutChanged;
        _owner.SizeChanged += OwnerOnLayoutChanged;
        _owner.StateChanged += OwnerOnStateChanged;
        _owner.IsVisibleChanged += OwnerOnIsVisibleChanged;

        _owner.Deactivated += OwnerOnDeactivated;
        _owner.Activated += OwnerOnActivated;

        _owner.SourceInitialized += OwnerOnSourceInitialized;
        _owner.Closed += OwnerOnClosed;
    }

    public void Dispose()
    {
        _notification.Closed -= OnNotificationClosed;

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

    protected abstract void SetupNotificationElement(FrameworkElement notificationElement, Popup popup);

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
        return _owner.IsVisible && _owner.WindowState != WindowState.Minimized &&
               (Visibility)_notification.GetVisibility() == Visibility.Visible;
    }

    private void OwnerOnLayoutChanged(object? sender, EventArgs e)
    {
        RefreshPlacement();
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

    private void OwnerOnIsVisibleChanged(object? sender, DependencyPropertyChangedEventArgs e)
    {
        if (!_owner.IsVisible)
        {
            _popup.IsOpen = false;
        }
        else
        {
            Show();
        }
    }

    private void OwnerOnDeactivated(object? sender, EventArgs e)
    {
        _popup.IsOpen = false;
    }

    private void OwnerOnActivated(object? sender, EventArgs e)
    {
        Show();
    }

    private void OwnerOnClosed(object? sender, EventArgs e)
    {
        Dispose();
    }

    private void OnNotificationClosed(object? sender, EventArgs e)
    {
        _popup.IsOpen = false;
        Dispose();
    }

    private CustomPopupPlacement[] CustomPopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
    {
        return PlacePopupPlacement(_popup, popupSize, targetSize, offset);
    }

    protected abstract CustomPopupPlacement[] PlacePopupPlacement(Popup popup, Size popupSize, Size targetSize,
        Point offset);

    private void RefreshPlacement()
    {
        if (!_popup.IsOpen)
        {
            return;
        }

        _popup.HorizontalOffset += 0.001;
        _popup.HorizontalOffset -= 0.001;
    }

    private void OwnerOnSourceInitialized(object? sender, EventArgs e)
    {
        _hwndSource = (HwndSource)PresentationSource.FromVisual(_owner)!;
        _hwndSource.AddHook(WndProc);
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
}