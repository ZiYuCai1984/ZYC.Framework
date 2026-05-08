using System.Windows;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Core.Page;

[Register]
[Obsolete]
public partial class NavigateProxyView : IDisposable
{
    public NavigateProxyView(
        NavigateProxyParameter navigateProxyParameter,
        ITabManager tabManager)
    {
        NavigateProxyParameter = navigateProxyParameter;
        TabManager = tabManager;

        InitializeComponent();
    }


    private NavigateProxyParameter NavigateProxyParameter { get; }

    private ITabManager TabManager { get; }

    public void Dispose()
    {
        Loaded -= OnNavigateProxyLoaded;
    }

    private async void OnNavigateProxyLoaded(object sender, RoutedEventArgs e)
    {
        var func = NavigateProxyParameter.CanNavigateFunc;
        if (!func.Invoke())
        {
            //!WARNING Design defeat !!
            await TabManager.ReloadAsync(NavigateProxyParameter.Source);
            return;
        }

        await TabManager.NavigateAsync(NavigateProxyParameter.Target);

        NavigateProxyParameter.NavigatedCallback?.Invoke();
    }
}
