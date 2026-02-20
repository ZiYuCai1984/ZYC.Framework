using System.Windows;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Modules.Mock.UI;

[Register]
public partial class TestCLIView
{
    public TestCLIView(ITabManager tabManager)
    {
        TabManager = tabManager;
        InitializeComponent();
    }

    private ITabManager TabManager { get; }

    private async void OnNavigate50TimesButtonClick(object sender, RoutedEventArgs e)
    {
        try
        {
            for (var i = 0; i < 50; ++i)
            {
                await TabManager.NavigateAsync("zyc://cli");
            }
        }
        catch
        {
            //ignore
        }
    }
}