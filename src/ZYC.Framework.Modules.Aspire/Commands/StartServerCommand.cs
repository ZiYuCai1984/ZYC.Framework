using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.Aspire.Abstractions;
using ZYC.Framework.Modules.Aspire.Abstractions.Event;

namespace ZYC.Framework.Modules.Aspire.Commands;

[RegisterSingleInstance]
internal class StartServerCommand : AsyncCommandBase, IDisposable
{
    public StartServerCommand(
        IAspireServiceManager aspireServiceManager,
        IEventAggregator eventAggregator)
    {
        AspireServiceManager = aspireServiceManager;

        eventAggregator.Subscribe<AspireServiceStatusChangedEvent>(
                OnAspireServiceStatusChanged, true)
            .DisposeWith(CompositeDisposable);
    }

    private IAspireServiceManager AspireServiceManager { get; }

    private CompositeDisposable CompositeDisposable { get; } = new();

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    private void OnAspireServiceStatusChanged(AspireServiceStatusChangedEvent obj)
    {
        RaiseCanExecuteChanged();
    }

    protected override Task InternalExecuteAsync(object? parameter)
    {
        return AspireServiceManager.StartServerAsync();
    }

    public override bool CanExecute(object? parameter)
    {
        var status = AspireServiceManager.GetStatusSnapshot().Status;
        return status == ServiceStatus.Stopped;
    }
}