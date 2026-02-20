using System.Windows.Data;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.About.UI;

[Register]
internal partial class AboutView
{
    public AboutView(IAppContext appContext, IProduct product)
    {
        CurrentProduct = product;
        IsSelfAlternate = appContext.IsSelfAlternate();

        StackPanel.SetBinding(DataContextProperty, new Binding
        {
            Source = this
        });
    }

    public IProduct CurrentProduct { get; }

    public bool IsSelfAlternate { get; }
}