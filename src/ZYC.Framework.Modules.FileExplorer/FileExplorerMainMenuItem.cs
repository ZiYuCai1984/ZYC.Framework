using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.FileExplorer.Abstractions;

namespace ZYC.Framework.Modules.FileExplorer;

[RegisterSingleInstance]
internal class FileExplorerMainMenuItem : MainMenuItem
{
    public FileExplorerMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Icon = FileExplorerModuleConstants.Icon,
            Title = FileExplorerModuleConstants.MenuTitle
        };

        Command = lifetimeScope.CreateNavigateCommand(FileExplorerModuleConstants.InitialUri);
    }
}