using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Gemini.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;


namespace ZYC.Automation.Modules.Gemini;

[RegisterSingleInstance]
internal class GeminiMainMenuItem : MainMenuItem
{
    public GeminiMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = GeminiModuleConstants.Title,
            Icon = GeminiModuleConstants.Icon,
            Anchor = MainMenuAnchors.Tools
        };

        Command = lifetimeScope.CreateNavigateCommand(GeminiModuleConstants.Uri);
    }
}