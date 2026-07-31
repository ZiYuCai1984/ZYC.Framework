using System.Windows;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.Aspire.Abstractions;

namespace ZYC.Framework.Modules.Aspire.UI.Toast;

[Register]
internal partial class AspireServiceStartSuccessToastView
{
    public AspireServiceStartSuccessToastView(NavigateCommand navigateCommand)
    {
        NavigateCommand = navigateCommand;
        InitializeComponent();
    }

    public Uri TargetUri => AspireModuleContansts.Uri;

    private NavigateCommand NavigateCommand { get; }

    private void OnNavigateButtonClick(object sender, RoutedEventArgs e)
    {
        NavigateCommand.Execute(TargetUri);
        CloseNotificationCommand.Execute(null);
    }
}