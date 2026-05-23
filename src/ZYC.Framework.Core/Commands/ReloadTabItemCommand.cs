using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class ReloadTabItemCommand : AsyncCommandBase<ITabItemInstance>
{
    public ReloadTabItemCommand(ITabManager tabManager)
    {
        TabManager = tabManager;
    }

    private ITabManager TabManager { get; }

    protected override Task InternalExecuteAsync(ITabItemInstance parameter)
    {
        return TabManager.ReloadAsync(parameter);
    }
}