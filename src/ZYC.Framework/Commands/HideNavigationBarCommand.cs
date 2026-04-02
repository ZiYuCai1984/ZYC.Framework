using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Commands;

[RegisterSingleInstance]
internal class
    HideNavigationBarCommand : ParameterPairCommandBase<ShowNavigationBarCommand, HideNavigationBarCommand,
    WorkspaceNode>
{
    public HideNavigationBarCommand(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
    }

    public override bool CanExecute(object? parameter)
    {
        return !ShowNavigationBarCommand.CanShowNavigationBarCommandExecute(parameter);
    }

    protected override void InternalExecute(WorkspaceNode parameter)
    {
        base.InternalExecute(parameter);


        parameter.IsNavigationBarVisible = false;
    }
}