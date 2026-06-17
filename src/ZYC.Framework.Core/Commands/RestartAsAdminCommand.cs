using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.State;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class RestartAsAdminCommand : RestartCommand
{
    public RestartAsAdminCommand(
        IAppContext appContext,
        IEventAggregator eventAggregator,
        ILogger<RestartCommand> logger,
        DesktopWindowState desktopWindowState) : base(
        appContext,
        eventAggregator,
        logger,
        desktopWindowState)
    {
    }

    protected override bool IsAdministrator => true;
}