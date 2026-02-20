using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Notification.Banner;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.Update.Abstractions;

namespace ZYC.Framework.Modules.Update.UI.Banner;

[Register]
internal partial class PromptNewProductBanner : IBanner
{
    public PromptNewProductBanner(
        IUpdateManager updateManager,
        NavigateCommand navigateCommand)
    {
        UpdateManager = updateManager;

        var updateContext = UpdateManager.GetCurrentUpdateContext();

        Debug.Assert(updateContext.NewProduct != null);
        NewProduct = updateContext.NewProduct;

        NavigateCommand = new ActionCommand(_ =>
        {
            CloseBannerCommand.Execute(null);
            navigateCommand.Execute(UpdateModuleConstants.Uri);
        });

        InitializeComponent();
    }

    private IUpdateManager UpdateManager { get; }

    public ICommand NavigateCommand { get; }

    public NewProduct NewProduct { get; }

}