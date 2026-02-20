using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.Update.Abstractions;

namespace ZYC.Framework.Modules.Update.UI.Toast;

[Register]
internal partial class PromptNewProductToastView
{
    public PromptNewProductToastView(NewProduct newProduct)
    {
        NewProduct = newProduct;

        InitializeComponent();
    }

    public NewProduct NewProduct { get; }

    public Uri TargetUri => UriTools.CreateAppUri("update");
}