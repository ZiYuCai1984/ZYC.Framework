using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Commands;

[RegisterSingleInstance]
internal class SetFocusedWorkspaceCommand : CommandBase<WorkspaceNode>
{
    public SetFocusedWorkspaceCommand(
        IParallelWorkspaceManager parallelWorkspaceManager,
        IEventAggregator eventAggregator)
    {
        ParallelWorkspaceManager = parallelWorkspaceManager;

        eventAggregator.Observe<WorkspaceFocusChangedEvent>()
            .Throttle(TimeSpan.FromMilliseconds(200))
            .Distinct()
            .ObserveOnUI()
            .Subscribe(_ =>
            {
                RaiseCanExecuteChanged();
            }).DisposeWith(CompositeDisposable);
    }

    private CompositeDisposable CompositeDisposable { get; } = new();

    private IParallelWorkspaceManager ParallelWorkspaceManager { get; }

    protected override void InternalExecute(WorkspaceNode parameter)
    {
        ParallelWorkspaceManager.SetFocusedWorkspace(parameter);
    }

    protected override bool InternalCanExecute(WorkspaceNode parameter)
    {
        return ParallelWorkspaceManager.GetFocusedWorkspace() != parameter;
    }
}