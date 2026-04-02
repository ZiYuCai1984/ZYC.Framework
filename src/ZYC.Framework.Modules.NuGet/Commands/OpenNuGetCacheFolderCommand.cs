using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.FileExplorer.Abstractions;

namespace ZYC.Framework.Modules.NuGet.Commands;

[RegisterSingleInstance]
internal class OpenNuGetCacheFolderCommand : CommandBase
{
    public OpenNuGetCacheFolderCommand(
        FileExplorerConfig fileExplorerConfig,
        IToastManager toastManager,
        IAppContext appContext,
        ITabManager tabManager,
        ILogger<OpenNuGetCacheFolderCommand> logger)
    {
        FileExplorerConfig = fileExplorerConfig;
        ToastManager = toastManager;
        AppContext = appContext;
        TabManager = tabManager;
        Logger = logger;
    }

    private FileExplorerConfig FileExplorerConfig { get; }

    private IToastManager ToastManager { get; }

    private IAppContext AppContext { get; }

    private ITabManager TabManager { get; }

    private ILogger<OpenNuGetCacheFolderCommand> Logger { get; }

    protected override void InternalExecute(object? parameter)
    {
        var path = $"C:/Users/{Environment.UserName}/.nuget/packages";

        try
        {
            if (FileExplorerConfig.UseBuiltInFileExplorer)
            {
                TabManager.NavigateAsync(path);
            }
            else
            {
                Process.Start("explorer.exe", path);
            }
        }
        catch (Exception e)
        {
            ToastManager.PromptException(e);
            Logger.Error(e);
        }
    }
}