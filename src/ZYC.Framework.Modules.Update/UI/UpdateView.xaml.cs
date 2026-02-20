using System.Reactive.Disposables.Fluent;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.NuGet.Abstractions.Commands;
using ZYC.Framework.Modules.Update.Abstractions;
using ZYC.Framework.Modules.Update.Abstractions.Event;
using ZYC.Framework.Modules.Update.Commands;

namespace ZYC.Framework.Modules.Update.UI;

[Register]
internal sealed partial class UpdateView
{
    public UpdateView(
        IEventAggregator eventAggregator,
        IClearNuGetHttpCacheCommand clearNuGetHttpCacheCommand,
        IUpdateManager updateManager,
        DownloadCommand downloadCommand,
        CheckUpdateCommand checkUpdateCommand,
        ILifetimeScope lifetimeScope,
        ApplyAndRestartCommand applyAndRestartCommand)
    {
        EventAggregator = eventAggregator;
        ClearNuGetHttpCacheCommand = clearNuGetHttpCacheCommand;
        UpdateManager = updateManager;

        DownloadCommand = downloadCommand;
        CheckUpdateCommand = checkUpdateCommand;
        ApplyAndRestartCommand = applyAndRestartCommand;

        LifetimeScope = lifetimeScope;

        EventAggregator.Subscribe<UpdateContextChangedEvent>(OnUpdateContextChangedEvent)
            .DisposeWith(CompositeDisposable);
    }

    public CheckUpdateCommand CheckUpdateCommand { get; }

    public ApplyAndRestartCommand ApplyAndRestartCommand { get; }

    /// <summary>
    ///     !WARNING Used for binding, cannot be set to private
    /// </summary>
    public ILifetimeScope LifetimeScope { get; }

    private IEventAggregator EventAggregator { get; }

    public IClearNuGetHttpCacheCommand ClearNuGetHttpCacheCommand { get; }

    private IUpdateManager UpdateManager { get; }

    public DownloadCommand DownloadCommand { get; }

    public UpdateStatus UpdateStatus => UpdateContext.UpdateStatus;

    public Exception? UpdateException => UpdateContext.Exception;

    public IProduct? NewProduct => UpdateContext.NewProduct;

    private UpdateContext UpdateContext => UpdateManager.GetCurrentUpdateContext();


    private void OnUpdateContextChangedEvent(UpdateContextChangedEvent e)
    {
        OnPropertyChanged(nameof(UpdateException));
        OnPropertyChanged(nameof(NewProduct));
        OnPropertyChanged(nameof(UpdateStatus));
    }

    protected override void InternalOnLoaded()
    {
        //!WARNING UpdateStatus.Free -> Start check update
        if (UpdateStatus == UpdateStatus.Free)
        {
            CheckUpdateCommand.Execute(null);
        }
    }
}