using System.Diagnostics;
using System.Windows;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Page;

namespace ZYC.Framework.Modules.Update.UI.Faulted;

[Register]
internal partial class DownloadFaultedView
{
    public DownloadFaultedView(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
        InitializeComponent();
    }

    private ILifetimeScope LifetimeScope { get; }

    private void OnDownloadFaultedViewLoaded(object sender, RoutedEventArgs e)
    {
        var ex = (Exception)Tag;
        Debug.Assert(ex != null);

        Border.Child = LifetimeScope.Resolve<InnerErrorView>(
            new TypedParameter(typeof(Exception), ex));
    }
}