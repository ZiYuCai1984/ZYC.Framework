using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.WebView2.Menu;

[Register]
internal partial class MenuBarView
{
    public MenuBarView(ExtendedMenuItem[] items)
    {
        ExtendedMenuItems = items;

        InitializeComponent();
    }

    public ExtendedMenuItem[] ExtendedMenuItems { get; }
}