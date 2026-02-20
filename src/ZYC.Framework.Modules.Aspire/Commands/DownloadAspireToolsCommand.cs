using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.Aspire.Abstractions;

namespace ZYC.Framework.Modules.Aspire.Commands;

[RegisterSingleInstance]
internal class DownloadAspireToolsCommand : AsyncCommandBase
{
    public DownloadAspireToolsCommand(IAspireServiceManager aspireServiceManager)
    {
        AspireServiceManager = aspireServiceManager;
    }

    private IAspireServiceManager AspireServiceManager { get; }

    protected override Task InternalExecuteAsync(object? parameter)
    {
        return AspireServiceManager.DownloadAspireToolsAsync();
    }
}