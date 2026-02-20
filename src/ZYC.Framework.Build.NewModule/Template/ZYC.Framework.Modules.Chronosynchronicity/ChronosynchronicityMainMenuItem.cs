using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Chronosynchronicity.Abstractions;

// ReSharper disable once CheckNamespace
namespace ZYC.Framework.Modules.Chronosynchronicity;

[RegisterSingleInstance]
internal class ChronosynchronicityMainMenuItem : MainMenuItem
{
    public ChronosynchronicityMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = ChronosynchronicityModuleConstants.Title,
            Icon = ChronosynchronicityModuleConstants.Icon
        };

        Command = lifetimeScope.CreateNavigateCommand(ChronosynchronicityModuleConstants.Uri);
    }
}