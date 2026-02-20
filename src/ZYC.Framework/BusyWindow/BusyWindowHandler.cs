using System.Diagnostics;
using ZYC.Framework.Abstractions.BusyWindow;

namespace ZYC.Framework.BusyWindow;

internal class BusyWindowHandler : IBusyWindowHandler
{
    private bool _isProgressIndeterminate =
        (bool)AppBusyWindow.IsProgressIndeterminateProperty.DefaultMetadata.DefaultValue;

    private double _progressValue = (double)AppBusyWindow.ProgressValueProperty.DefaultMetadata.DefaultValue;

    private bool _showProgress = (bool)AppBusyWindow.ShowProgressProperty.DefaultMetadata.DefaultValue;

    private string _statusText = (string)AppBusyWindow.StatusTextProperty.DefaultMetadata.DefaultValue;

    private string _title = AppBusyWindow.DefaultTitle;

    internal BusyWindowHandler(AppBusyWindow busyWindow)
    {
        BusyWindow = busyWindow;
    }

    private bool IsActive { get; set; }

    private AppBusyWindow BusyWindow { get; }

    public string StatusText
    {
        get => _statusText;
        set
        {
            _statusText = value;
            if (!IsActive)
            {
                return;
            }

            Debug.Assert(BusyWindow.Dispatcher != null);
            BusyWindow.Dispatcher.Invoke(() => BusyWindow.StatusText = value);
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            if (!IsActive)
            {
                return;
            }

            Debug.Assert(BusyWindow.Dispatcher != null);
            BusyWindow.Dispatcher.Invoke(() => BusyWindow.Title = value);
        }
    }

    public bool ShowProgress
    {
        get => _showProgress;
        set
        {
            _showProgress = value;
            if (!IsActive)
            {
                return;
            }

            Debug.Assert(BusyWindow.Dispatcher != null);
            BusyWindow.Dispatcher.Invoke(() => BusyWindow.ShowProgress = value);
        }
    }

    public bool IsProgressIndeterminate
    {
        get => _isProgressIndeterminate;
        set
        {
            _isProgressIndeterminate = value;
            if (!IsActive)
            {
                return;
            }

            Debug.Assert(BusyWindow.Dispatcher != null);
            BusyWindow.Dispatcher.Invoke(() => BusyWindow.IsProgressIndeterminate = value);
        }
    }

    public double ProgressValue
    {
        get => _progressValue;
        set
        {
            _progressValue = value;
            if (!IsActive)
            {
                return;
            }

            Debug.Assert(BusyWindow.Dispatcher != null);
            BusyWindow.Dispatcher.Invoke(() => BusyWindow.ProgressValue = value);
        }
    }

    public void Dispose()
    {
        Deactivate();
        Close();
    }

    public void Close()
    {
        BusyWindow.Close(this);
    }

    internal void Activate()
    {
        IsActive = true;
        Debug.Assert(BusyWindow.Dispatcher != null);
        BusyWindow.Dispatcher.Invoke(() =>
        {
            BusyWindow.StatusText = StatusText;
            BusyWindow.ShowProgress = ShowProgress;
            BusyWindow.IsProgressIndeterminate = IsProgressIndeterminate;
            BusyWindow.ProgressValue = ProgressValue;
        });
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}