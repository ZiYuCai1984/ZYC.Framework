using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Commands;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.MainMenu.BuildIn;

[Register]
internal class RemoveCustomLayoutMainMenuItem : MainMenuItem
{
    public RemoveCustomLayoutMainMenuItem(
        RemoveCustomLayoutCommand removeCustomLayoutCommand,
        CustomWorkspaceLayout customWorkspaceLayout)
    {
        RemoveCustomLayoutCommand = removeCustomLayoutCommand;
        CustomWorkspaceLayout = customWorkspaceLayout;

        Info = new MenuItemInfo
        {
            Title = customWorkspaceLayout.Name,
            Icon = customWorkspaceLayout.Thumbnail,
            Localization = false
        };

        Command = new RelayCommand(
            _ => RemoveCustomLayoutCommand.CanExecute(CustomWorkspaceLayout),
            _ => RemoveCustomLayoutCommand.Execute(CustomWorkspaceLayout));
    }

    private RemoveCustomLayoutCommand RemoveCustomLayoutCommand { get; }

    private CustomWorkspaceLayout CustomWorkspaceLayout { get; }
}
