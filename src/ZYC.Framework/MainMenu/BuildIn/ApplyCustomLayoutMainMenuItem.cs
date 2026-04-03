using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.MainMenu.BuildIn;

[RegisterSingleInstance]
internal class ApplyCustomLayoutMainMenuItem : MainMenuItem
{
    public ApplyCustomLayoutMainMenuItem(
        CustomWorkspaceLayout customWorkspaceLayout)
    {
        CustomWorkspaceLayout = customWorkspaceLayout;

        Info = new MenuItemInfo
        {
            Title = customWorkspaceLayout.Name,
            Localization = false
        };


        Command = new RelayCommand(_=>true, _ =>
        {
            //TODO-zyc ApplyCustomLayoutMainMenuItem
        });
    }

    private CustomWorkspaceLayout CustomWorkspaceLayout { get; }
}