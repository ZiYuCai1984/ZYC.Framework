using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.TaskManager.Abstractions;

namespace ZYC.Framework.Modules.TaskManager;

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