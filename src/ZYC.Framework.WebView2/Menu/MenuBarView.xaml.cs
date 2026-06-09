using System.Windows.Controls;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.WebView2.Menu;

[Register]
internal partial class MenuBarView
{
    public MenuBarView(
        ExtendedMenuItem[] extendedMenuItems,
        ExtendedMenuItem[] pluginMenuItems)
    {
        ExtendedMenuItems = extendedMenuItems;
        PluginMenuItems = pluginMenuItems;

        InitializeComponent();
    }

    public ExtendedMenuItem[] ExtendedMenuItems { get; }

    public ExtendedMenuItem[] PluginMenuItems { get; }

    private void OnPluginMenuItemSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not MahApps.Metro.Controls.SplitButton splitButton
            || splitButton.SelectedItem is not ExtendedMenuItem item)
        {
            return;
        }

        splitButton.SelectedIndex = -1;

        if (item.Command?.CanExecute(item.CommandParameter) == true)
        {
            item.Command.Execute(item.CommandParameter);
        }
    }
}