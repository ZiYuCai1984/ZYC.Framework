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

    public override bool CanExecute(object? parameter)
    {
        return CanShowNavigationBarCommandExecute(parameter);
    }


    public static bool CanShowNavigationBarCommandExecute(object? parameter)
    {
        if (parameter == null)
        {
            return false;
        }

        var node = (WorkspaceNode)parameter;
        return !node.IsNavigationBarVisible;
    }

    protected override void InternalExecute(WorkspaceNode parameter)
    {
        base.InternalExecute(parameter);

        parameter.IsNavigationBarVisible = true;
    }
}