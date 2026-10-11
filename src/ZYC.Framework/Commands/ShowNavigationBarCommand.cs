using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Commands;

[RegisterSingleInstance]
internal class
    ShowNavigationBarCommand : ParameterPairCommandBase<ShowNavigationBarCommand, HideNavigationBarCommand,
    WorkspaceNode>
{
    public ShowNavigationBarCommand(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
    }

    protected override bool InternalCanExecute(WorkspaceNode parameter)
    {
        return !parameter.IsNavigationBarVisible;
    }

    protected override void InternalExecute(WorkspaceNode parameter)
    {
        base.InternalExecute(parameter);

        parameter.IsNavigationBarVisible = true;
    }
}
