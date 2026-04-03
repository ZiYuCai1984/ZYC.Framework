using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.FileExplorer.Abstractions;

namespace ZYC.Framework.Modules.Log.Commands;

[RegisterSingleInstance]
internal class OpenLogFolderCommand : CommandBase
{
    public OpenLogFolderCommand(
        IToastManager toastManager,
        IAppContext appContext,
        ITabManager tabManager,
        FileExplorerConfig fileExplorerConfig,
        ILogger<OpenLogFolderCommand> logger)
    {
        ToastManager = toastManager;
        AppContext = appContext;
        TabManager = tabManager;
        FileExplorerConfig = fileExplorerConfig;
        Logger = logger;
    }

    private IToastManager ToastManager { get; }
    private IAppContext AppContext { get; }

    private ITabManager TabManager { get; }

    private FileExplorerConfig FileExplorerConfig { get; }
    private ILogger<OpenLogFolderCommand> Logger { get; }

    protected override void InternalExecute(object? parameter)
    {
        var path = AppContext.GetLogsDirectory();

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