using System.Diagnostics;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class OpenFolderInExplorerCommand : CommandBase
{
    public OpenFolderInExplorerCommand(IAppContext appContext)
    {
        AppContext = appContext;
    }

    private IAppContext AppContext { get; }

    protected override void InternalExecute(object? parameter)
    {
        var dir = parameter?.ToString();
        Process.Start("explorer.exe", dir ?? "");
    }
}