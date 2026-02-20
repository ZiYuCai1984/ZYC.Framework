using System.Text;
using ZYC.CoreToolkit.Abstractions.Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.ModuleManager.Abstractions;

namespace ZYC.Framework.Modules.ModuleManager.Commands;

[RegisterSingleInstance]
internal class DeleteModuleCommand : CommandBase
{
    public DeleteModuleCommand(
        ILocalModuleManager localModuleManager)
    {
        LocalModuleManager = localModuleManager;
    }

    private ILocalModuleManager LocalModuleManager { get; }

    public override bool CanExecute(object? parameter)
    {
        var module = (IModuleInfo)parameter!;
        return !LocalModuleManager.IsMoudlePendingDelete(module);
    }

    protected override void InternalExecute(object? parameter)
    {
        if (parameter == null)
        {
            return;
        }

        var module = (IModuleInfo)parameter;


        var warningPrompt = new StringBuilder();
        warningPrompt.AppendLine("This action cannot be undone !!");


        var dependencyBy = LocalModuleManager.GetDependencyBy(module);

        if (dependencyBy.Length != 0)
        {
            warningPrompt.AppendLine("The following dependencies will also be automatically deleted.");
            warningPrompt.AppendLine();

            foreach (var m in dependencyBy)
            {
                warningPrompt.AppendLine($"- {m.ModuleAssemblyName}");
            }
        }


        if (!MessageBoxTools.Confirm(warningPrompt.ToString(), "Warning", false))
        {
            return;
        }

        LocalModuleManager.DeleteModuleRecursive(module);

        RaiseCanExecuteChanged();
    }
}