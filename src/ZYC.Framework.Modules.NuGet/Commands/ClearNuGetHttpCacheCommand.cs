using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.NuGet.Abstractions;
using ZYC.Framework.Modules.NuGet.Abstractions.Commands;

namespace ZYC.Framework.Modules.NuGet.Commands;

[RegisterSingleInstanceAs(typeof(IClearNuGetHttpCacheCommand))]
internal class ClearNuGetHttpCacheCommand : AsyncCommandBase, IClearNuGetHttpCacheCommand
{
    public ClearNuGetHttpCacheCommand(
        INuGetManager nuGetManager, 
        IAppLogger<ClearNuGetHttpCacheCommand> logger)
    {
        NuGetManager = nuGetManager;
        Logger = logger;
    }

    private INuGetManager NuGetManager { get; }
    private IAppLogger<ClearNuGetHttpCacheCommand> Logger { get; }

    protected override async Task InternalExecuteAsync(object? parameter)
    {
        try
        {
            await NuGetManager.ClearNuGetHttpCacheAsync();
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}