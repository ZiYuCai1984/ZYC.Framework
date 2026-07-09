using System.Windows;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.Update.Abstractions;

namespace ZYC.Framework.Modules.Update.UI.Toast;

[Register]
internal partial class PromptNewProductToastView
{
    public PromptNewProductToastView(
        NewProduct newProduct,
        NavigateCommand navigateCommand)
    {
        NewProduct = newProduct;
        NavigateCommand = navigateCommand;

        InitializeComponent();
    }

    public NewProduct NewProduct { get; }

    private NavigateCommand NavigateCommand { get; }

    public Uri TargetUri => UriTools.CreateAppUri("update");

    private void OnNavigateButtonClick(object sender, RoutedEventArgs e)
    {
        NavigateCommand.Execute(TargetUri);
        CloseNotificationCommand.Execute(null);
    }
}