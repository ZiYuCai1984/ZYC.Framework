using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.State;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class RestartCommand : CommandBase, IDisposable
{
    public RestartCommand(
        IAppContext appContext,
        IEventAggregator eventAggregator,
        IAppLogger<RestartCommand> logger,
        DesktopWindowState desktopWindowState)
    {
        AppContext = appContext;
        Logger = logger;
        DesktopWindowState = desktopWindowState;

        eventAggregator.Subscribe<SetPreventExitCommandExecutedEvent>(OnSetPreventExitCommandExecuted)
            .DisposeWith(CompositeDisposable);
    }


    private CompositeDisposable CompositeDisposable { get; } = new();

    protected virtual bool IsAdministrator => false;

    private IAppContext AppContext { get; }

    private IAppLogger<RestartCommand> Logger { get; }

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
        try
        {
            var fileName = AppContext.GetProcessFileName();
            var argumentString = AppContext.GetArgumentString();

            ProcessTools.Start(fileName, argumentString, IsAdministrator);

            //!WARNING Must save the configuration first and then try to restart
            AppContext.SaveAllConfig();
            AppContext.SaveAllState();

            AppContext.FocusExitProcess();
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}