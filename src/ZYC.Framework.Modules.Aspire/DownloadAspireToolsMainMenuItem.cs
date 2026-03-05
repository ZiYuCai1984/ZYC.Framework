using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Modules.Aspire.Commands;

namespace ZYC.Framework.Modules.Aspire;

[RegisterSingleInstance]
internal class DownloadAspireToolsMainMenuItem : MainMenuItem
{
    public DownloadAspireToolsMainMenuItem(DownloadAspireToolsCommand downloadAspireToolsCommand)
    {
        Info = new MenuItemInfo
        {
            Title = "Download Aspire Tools",
            Icon = "MonitorArrowDown"
        };

        Command = downloadAspireToolsCommand;
    }
}