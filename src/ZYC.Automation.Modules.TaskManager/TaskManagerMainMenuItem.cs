using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.TaskManager.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.TaskManager;

[RegisterSingleInstance]
internal class TaskManagerMainMenuItem : MainMenuItem
{
    public TaskManagerMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = TaskManagerModuleConstants.Title,
            Icon = TaskManagerModuleConstants.Icon,
        };

        Command = lifetimeScope.CreateNavigateCommand(TaskManagerModuleConstants.Uri);
    }
}