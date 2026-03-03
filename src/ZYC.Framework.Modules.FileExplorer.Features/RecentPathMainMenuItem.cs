using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Modules.FileExplorer.Features;

[Register]
internal class RecentPathMainMenuItem : MainMenuItem
{
    public RecentPathMainMenuItem(int index, string path, ITabManager tabManager)
    {
        Info = new MenuItemInfo
        {
            Title = $"{index + 1}.   {path}",
            Icon = null,
            Anchor = "",
            Localization = false,
            Priority = 0
        };

        Command = new RelayCommand(_ => true, _ =>
        {
            tabManager.NavigateAsync(path);
        });
    }
}