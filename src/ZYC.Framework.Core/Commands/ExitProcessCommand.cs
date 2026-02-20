using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.State;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class ExitProcessCommand : CommandBase, IDisposable
{
    public ExitProcessCommand(
        IEventAggregator eventAggregator,
        IAppContext appContext,
        DesktopWindowState desktopWindowState)
    {
        AppContext = appContext;
        DesktopWindowState = desktopWindowState;

        eventAggregator.Subscribe<SetPreventExitCommandExecutedEvent>(OnSetPreventExitCommandExecuted)
            .DisposeWith(CompositeDisposable);
    }

    private CompositeDisposable CompositeDisposable { get; } = new();

    private IAppContext AppContext { get; }

    private DesktopWindowState DesktopWindowState { get; }


    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    private void OnSetPreventExitCommandExecuted(SetPreventExitCommandExecutedEvent obj)
    {
        RaiseCanExecuteChanged();
    }

    public override bool CanExecute(object? parameter)
    {
        return !DesktopWindowState.IsPreventExit;
    }

    protected override void InternalExecute(object? parameter)
    {
        if (!CanExecute(parameter))
        {
            return;
        }

        AppContext.ExitProcess();
    }
}