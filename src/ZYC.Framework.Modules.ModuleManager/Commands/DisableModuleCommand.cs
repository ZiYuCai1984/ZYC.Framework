using Autofac;
using ZYC.CoreToolkit.Abstractions.Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.ModuleManager.Abstractions;

namespace ZYC.Framework.Modules.ModuleManager.Commands;

[RegisterSingleInstance]
internal class DisableModuleCommand : PairCommandBase<EnableModuleCommand, DisableModuleCommand>
{
    public DisableModuleCommand(ILifetimeScope lifetimeScope, ILocalModuleManager localModuleManager) : base(
        lifetimeScope)
    {
        LocalModuleManager = localModuleManager;
    }

    private ILocalModuleManager LocalModuleManager { get; }

    protected override void InternalExecute(object? parameter)
    {
        if (parameter == null)
        {
            return;
        }

        var module = (IModuleInfo)parameter;
        LocalModuleManager.DisableModule(module);
    }


    public override bool CanExecute(object? parameter)
    {
        if (parameter == null)
        {
            return false;
        }

        var module = (IModuleInfo)parameter;
        return !LocalModuleManager.IsModuleDisabled(module);
    }
}