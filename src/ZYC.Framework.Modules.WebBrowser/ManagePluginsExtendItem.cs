using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Modules.WebBrowser.Commands;
using ZYC.Framework.WebView2.Menu;

namespace ZYC.Framework.Modules.WebBrowser;

[Register]
internal class ManagePluginsExtendItem : ExtendedMenuItem
{
    public ManagePluginsExtendItem(ManagePluginsCommand managePluginsCommand)
    {
        Command = managePluginsCommand;
        Title = "Manage Plugins";
        Icon = "PuzzleEditOutline";
    }
}