using System.Diagnostics;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Modules.Update.Abstractions;
using ZYC.Framework.Modules.Update.Commands;

namespace ZYC.Framework.Modules.Update.UI.Banner;

[Register]
internal partial class PromptPendingRestartBanner
{
    public PromptPendingRestartBanner(
        ApplyAndRestartCommand applyAndRestartCommand,
        IUpdateManager updateManager)
    {
        ApplyAndRestartCommand = applyAndRestartCommand;

        var product = updateManager.GetCurrentUpdateContext().NewProduct;
        Debug.Assert(product != null);

        NewProduct = product;


        InitializeComponent();
    }

    public ApplyAndRestartCommand ApplyAndRestartCommand { get; }

    public NewProduct NewProduct { get; }
}