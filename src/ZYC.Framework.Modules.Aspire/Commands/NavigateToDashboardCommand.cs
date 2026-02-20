using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.Aspire.Abstractions;
using ZYC.Framework.Modules.Aspire.Abstractions.Event;

namespace ZYC.Framework.Modules.Aspire.Commands;

[RegisterSingleInstance]
public class NavigateToDashboardCommand : CommandBase, IDisposable
{
    public NavigateToDashboardCommand(
        IEventAggregator eventAggregator,
        ITabManager tabManager,
        IAspireServiceManager aspireServiceManager)
    {
        TabManager = tabManager;
        AspireServiceManager = aspireServiceManager;

        eventAggregator.Subscribe<AspireServiceStatusChangedEvent>(
            OnAspireServiceStatusChanged, true).DisposeWith(CompositeDisposable);
    }


    private CompositeDisposable CompositeDisposable { get; } = new();

    private ITabManager TabManager { get; }

    private IAspireServiceManager AspireServiceManager { get; }

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    private void OnAspireServiceStatusChanged(AspireServiceStatusChangedEvent obj)
    {
        RaiseCanExecuteChanged();
    }

    public override bool CanExecute(object? parameter)
    {
        var status = AspireServiceManager.GetStatusSnapshot().Status;
        return status == ServiceStatus.Running || status == ServiceStatus.Starting;
    }

    protected override void InternalExecute(object? parameter)
    {
        TabManager.NavigateAsync(AspireModuleContansts.Uri);
    }
}