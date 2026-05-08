using System.Windows;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Mock.UI;

[Register]
public partial class TestDialogView
{
    public TestDialogView(IDialogManager dialogManager)
    {
        DialogManager = dialogManager;

        InitializeComponent();
    }

    private IDialogManager DialogManager { get; }

    private void OnShowDialogBtnClick(object sender, RoutedEventArgs e)
    {
        DialogManager.Show<TestDialogWindow>();
    }
}
