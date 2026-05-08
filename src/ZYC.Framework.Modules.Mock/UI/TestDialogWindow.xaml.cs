using System.Windows;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.Modules.Mock.UI;

[Register]
public partial class TestDialogWindow
{
    public TestDialogWindow()
    {
        InitializeComponent();
    }

    private void OnCloseBtnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
