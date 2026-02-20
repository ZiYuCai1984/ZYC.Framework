using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.Update.Abstractions;
using ZYC.MdXaml;
using ZYC.MdXaml.MdXaml;

namespace ZYC.Framework.Modules.Update.UI;

[Register]
internal partial class UpdateAvailableView
{
    public UpdateAvailableView(
        ILifetimeScope lifetimeScope,
        IAppLogger<UpdateAvailableView> logger)
    {
        LifetimeScope = lifetimeScope;
        Logger = logger;
        InitializeComponent();

        SetBinding(TagProperty, new Binding(nameof(UpdateView.NewProduct)));
    }


    private ILifetimeScope LifetimeScope { get; }

    private IProduct? Product => LifetimeScope.Resolve<IUpdateManager>().GetCurrentUpdateContext().NewProduct;

    private IAppLogger<UpdateAvailableView> Logger { get; }

    private void OnUpdateAvailableViewLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var product = Product!;

            var markdownScrollViewer = new MarkdownScrollViewer();

            markdownScrollViewer.Syntax = SyntaxVersion.MdXaml;
            markdownScrollViewer.VerticalAlignment = VerticalAlignment.Stretch;
            markdownScrollViewer.HorizontalAlignment = HorizontalAlignment.Stretch;
            markdownScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            markdownScrollViewer.DisabledLazyLoad = true;

            markdownScrollViewer.HereMarkdown = product.PatchNote;


            PatchNodeGrid.Children.Clear();
            PatchNodeGrid.Children.Add(markdownScrollViewer);
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
        }
    }
}