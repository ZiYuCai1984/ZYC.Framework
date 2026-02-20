using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class NavigateCommand : AsyncCommandBase
{
    public NavigateCommand(ITabManager tabManager)
    {
        TabManager = tabManager;
    }

    private ITabManager TabManager { get; }


    protected override async Task InternalExecuteAsync(object? parameter)
    {
        Uri target;

        if (parameter is Uri uri)
        {
            target = uri;
        }
        else
        {
            target = new Uri(parameter?.ToString() ?? "");
        }

        await TabManager.NavigateAsync(target);
    }
}