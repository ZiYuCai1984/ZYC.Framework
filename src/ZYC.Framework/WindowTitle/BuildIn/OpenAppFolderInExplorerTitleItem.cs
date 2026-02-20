using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.WindowTitle;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.WindowTitle.BuildIn;

[RegisterSingleInstance]
internal class OpenAppFolderInExplorerTitleItem : IWindowTitleItem
{
    public OpenAppFolderInExplorerTitleItem(
        OpenAppFolderInExplorerCommand openAppFolderInExplorerCommand,
        AppConfig appConfig)
    {
        OpenAppFolderInExplorerCommand = openAppFolderInExplorerCommand;
        AppConfig = appConfig;
    }

    private OpenAppFolderInExplorerCommand OpenAppFolderInExplorerCommand { get; }

    private AppConfig AppConfig { get; }

    public string Icon => "FolderOutline";

    public ICommand Command => OpenAppFolderInExplorerCommand;

    public bool IsVisible => true;

    //public bool IsVisible => AppConfig.GetIsDebugItemVisible();
}