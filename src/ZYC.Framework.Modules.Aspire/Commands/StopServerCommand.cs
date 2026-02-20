using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.Aspire.Abstractions;
using ZYC.Framework.Modules.Aspire.Abstractions.Event;

namespace ZYC.Framework.Modules.Aspire.Commands;

[RegisterSingleInstance]
internal class StopServerCommand : AsyncCommandBase, IDisposable
{
    public StopServerCommand(
        IAppContext appContext,
        IAspireServiceManager aspireServiceManager,
        IEventAggregator eventAggregator)
    {
        AppContext = appContext;
        AspireServiceManager = aspireServiceManager;

        eventAggregator.Subscribe<AspireServiceStatusChangedEvent>(OnAspireServiceStatusChanged, true)
            .DisposeWith(CompositeDisposable);
    }

    private CompositeDisposable CompositeDisposable { get; } = new();

    private IAppContext AppContext { get; }

    private IAspireServiceManager AspireServiceManager { get; }

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    private void OnAspireServiceStatusChanged(AspireServiceStatusChangedEvent e)
    {
        RaiseCanExecuteChanged();
    }

    protected override Task InternalExecuteAsync(object? parameter)
    {
        return AspireServiceManager.StopServerAsync();
    }

    public override bool CanExecute(object? parameter)
    {
        var status = AspireServiceManager.GetStatusSnapshot().Status;
        return status == ServiceStatus.Running;
    }
}