using System.Windows.Data;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.About.UI;

[Register]
internal partial class AboutView
{
    public AboutView(IProduct product)
    {
        CurrentProduct = product;

        StackPanel.SetBinding(DataContextProperty, new Binding
        {
            Source = this
        });
    }

    public IProduct CurrentProduct { get; }

    public global::System.Collections.Generic.IReadOnlyDictionary<string, string> ProductProperties =>
        ProductInfo.Properties;
}
