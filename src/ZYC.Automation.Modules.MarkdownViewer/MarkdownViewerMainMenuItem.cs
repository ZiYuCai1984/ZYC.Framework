using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.MarkdownViewer.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;


namespace ZYC.Automation.Modules.MarkdownViewer;

[RegisterSingleInstance]
internal class MarkdownViewerMainMenuItem : MainMenuItem
{
    public MarkdownViewerMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = MarkdownViewerModuleConstants.Title,
            Icon = MarkdownViewerModuleConstants.Icon
        };

        Command = lifetimeScope.CreateNavigateCommand(MarkdownViewerModuleConstants.Uri);
    }
}