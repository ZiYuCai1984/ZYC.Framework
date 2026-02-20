using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Modules.Aspire.Abstractions;
using ZYC.Framework.Modules.Aspire.Abstractions.Event;

namespace ZYC.Framework.Modules.Aspire;

[Register]
internal partial class AspireService
{
    public AspireService(
        IToastManager toastManager,
        IEventAggregator eventAggregator,
        Uri dashboardUri,
        IAppLogger<AspireService> logger,
        DistributedApplication distributedApplication)
    {
        ToastManager = toastManager;
        EventAggregator = eventAggregator;
        DashboardUri = dashboardUri;
        Logger = logger;

        DistributedApplication = distributedApplication;

        EventAggregator.Subscribe<AspireDashboardReadyEvent>(OnDashboardReady)
            .DisposeWith(CompositeDisposable);
    }

    private CompositeDisposable CompositeDisposable { get; } = new();

    private SemaphoreSlim Gate { get; } = new(1, 1);

    private IToastManager ToastManager { get; }

    private IEventAggregator EventAggregator { get; }

    private Uri DashboardUri { get; }

    private IAppLogger<AspireService> Logger { get; }

    private DistributedApplication DistributedApplication { get; }

    private AspireServiceStatus AspireServiceStatus
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            EventAggregator.Publish(new AspireServiceStatusChangedEvent(value));
        }
    } = AspireServiceStatus.Stopped();


    private void OnDashboardReady(AspireDashboardReadyEvent obj)
    {
        AspireServiceStatus = AspireServiceStatus.Running(DashboardUri);
    }

    public async Task StartAsync()
    {
        if (!CanStart())
        {
            return;
        }

        try
        {
            await Gate.WaitAsync();
            if (!CanStart())
            {
                return;
            }

            await ImplStartAsync();
        }
        finally
        {
            Gate.Release();
        }

        return;


        bool CanStart()
        {
            return AspireServiceStatus.Status == ServiceStatus.Stopped;
        }
    }

    public async Task StopAsync()
    {
        if (!CanStop())
        {
            return;
        }

        try
        {
            if (!CanStop())
            {
                return;
            }

            await Gate.WaitAsync();
            await ImplStopAsync();
        }
        finally
        {
            try
            {
                Gate.Release();
            }
            catch (ObjectDisposedException)
            {
                //ignore
            }
        }

        return;


        bool CanStop()
        {
            return AspireServiceStatus.Status == ServiceStatus.Running;
        }
    }

    private async Task ImplStartAsync()
    {
        await Task.CompletedTask;

        AspireServiceStatus = AspireServiceStatus.Starting(DashboardUri);

        _ = Task.Run(async () =>
        {
            try
            {
                //!WARNING Using RunAsync here will result in an exception (it cannot be closed normally).
                await DistributedApplication.StartAsync();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ToastManager.PromptException(e);

                await ImplStopAsync(e);
                EventAggregator.Publish(new AspireServiceStartFaultedAndDisposeFinishedEvent(e));
            }
        });
    }

    private async Task ImplStopAsync(Exception? exception = null)
    {
        AspireServiceStatus = AspireServiceStatus.Stopping(exception);
        try
        {
            await DistributedApplication.StopAsync();
        }
        catch (Exception e)
        {
            Logger.Error(e);
            DebuggerTools.Break();
        }

        Dispose();
        AspireServiceStatus = AspireServiceStatus.Stopped(exception);
    }


    public AspireServiceStatus GetStatusSnapshot()
    {
        return AspireServiceStatus;
    }
}