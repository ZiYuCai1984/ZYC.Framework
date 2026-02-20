using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.MarkdownViewer.Abstractions;

namespace ZYC.Framework.Modules.MarkdownViewer;

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