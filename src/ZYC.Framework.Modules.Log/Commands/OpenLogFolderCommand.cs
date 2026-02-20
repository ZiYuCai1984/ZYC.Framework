using System.IO;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Modules.Log.Commands;

[RegisterSingleInstance]
internal class OpenLogFolderCommand : CommandBase
{
    public OpenLogFolderCommand(IAppContext appContext, ITabManager tabManager)
    {
        AppContext = appContext;
        TabManager = tabManager;
    }

    private IAppContext AppContext { get; }
    private ITabManager TabManager { get; }

    protected override void InternalExecute(object? parameter)
    {
        var dir = Path.Combine(AppContext.GetMainAppDirectory(), "logs");
        TabManager.NavigateAsync(dir);
    }
}