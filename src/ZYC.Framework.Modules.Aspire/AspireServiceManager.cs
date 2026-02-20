using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Abstractions;
using ZYC.CoreToolkit.Dotnet;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Modules.Aspire.Abstractions;
using ZYC.Framework.Modules.Aspire.Abstractions.Event;

namespace ZYC.Framework.Modules.Aspire;

[RegisterSingleInstanceAs(typeof(IAspireServiceManager))]
internal sealed partial class AspireServiceManager : IAspireServiceManager, IDisposable
{
    public AspireServiceManager(
        AspireConfig aspireConfig,
        IToastManager toastManager,
        IEventAggregator eventAggregator,
        AspireServiceEnvironment aspireServiceEnvironment,
        ILifetimeScope lifetimeScope,
        IAppLogger<AspireServiceManager> logger)
    {
        AspireConfig = aspireConfig;
        ToastManager = toastManager;
        EventAggregator = eventAggregator;
        AspireServiceEnvironment = aspireServiceEnvironment;
        LifetimeScope = lifetimeScope;
        Logger = logger;

        eventAggregator.Subscribe<AspireServiceStartFaultedAndDisposeFinishedEvent>(
            OnAspireServiceStartFaultedAndDisposeFinished).DisposeWith(CompositeDisposable);
    }

    private SemaphoreSlim Gate { get; } = new(1, 1);

    private CompositeDisposable CompositeDisposable { get; } = new();

    private AspireConfig AspireConfig { get; }

    private IToastManager ToastManager { get; }

    private IEventAggregator EventAggregator { get; }

    private AspireServiceEnvironment AspireServiceEnvironment { get; }

    private ILifetimeScope LifetimeScope { get; }

    private IAppLogger<AspireServiceManager> Logger { get; }

    private AspireService? AspireService { get; set; }

    public async Task StartServerAsync()
    {
        try
        {
            await Gate.WaitAsync();

            if (AspireService != null)
            {
                await AspireService.StartAsync();
                return;
            }

            AspireService = AspireService.Build(LifetimeScope);
            await AspireService.StartAsync();
        }
        catch (Exception e)
        {
            DebuggerTools.Break();
            Logger.Error(e);
        }
        finally
        {
            Gate.Release();
        }
    }


    public async Task StopServerAsync()
    {
        try
        {
            await Gate.WaitAsync();

            if (AspireService == null)
            {
                throw new InvalidOperationException();
            }

            await AspireService.StopAsync();
            //!WARNING AspireService disposed by self
            AspireService = null;
        }
        catch (Exception e)
        {
            DebuggerTools.Break();
            Logger.Error(e);
        }
        finally
        {
            Gate.Release();
        }
    }

    public AspireServiceStatus GetStatusSnapshot()
    {
        if (AspireService == null)
        {
            return AspireServiceStatus.Stopped();
        }

        return AspireService.GetStatusSnapshot();
    }

    public void SetAspireBinarySource(AspireBinarySource source)
    {
        AspireConfig.AspireBinarySource = source;
    }

    public async Task DownloadAspireToolsAsync()
    {
        var packages = new List<NuGetPackage>
        {
            new()
            {
                Name = AspireServiceEnvironment.OrchestrationPackageName,
                Version = AspireServiceEnvironment.AspirePackageVersion
            },
            new()
            {
                Name = AspireServiceEnvironment.DashboardPackageName,
                Version = AspireServiceEnvironment.AspirePackageVersion
            }
        };

        IOTools.DeleteDirectoryIfExists(AspireServiceEnvironment.AspireToolsFolder);
        await DotnetNuGetTools
            .DownloadNuGetPackagesAsync(packages.ToArray(), AspireServiceEnvironment.AspireToolsFolder)
            .ConfigureAwait(false);
    }

    private void OnAspireServiceStartFaultedAndDisposeFinished(AspireServiceStartFaultedAndDisposeFinishedEvent obj)
    {
        //!WARNING AspireService disposed by self
        AspireService = null;
    }
}