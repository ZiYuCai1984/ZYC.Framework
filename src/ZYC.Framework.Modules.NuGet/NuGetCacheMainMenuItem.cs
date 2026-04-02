using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Modules.NuGet.Commands;

namespace ZYC.Framework.Modules.NuGet;

[RegisterSingleInstance]
internal class NuGetCacheMainMenuItem : MainMenuItem
{
    public NuGetCacheMainMenuItem(OpenNuGetCacheFolderCommand openNuGetCacheFolderCommand)
    {
        Info = new MenuItemInfo
        {
            Title = "NuGet Cache",
            Anchor = FileMainMenuAnchors.Open
        };

        Command = openNuGetCacheFolderCommand;
    }
}