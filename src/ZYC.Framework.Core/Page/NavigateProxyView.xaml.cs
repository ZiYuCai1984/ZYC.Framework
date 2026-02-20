using System.Windows;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Core.Page;

[Register]
[Obsolete]
public partial class NavigateProxyView : IDisposable
{
    public NavigateProxyView(
        NavigateProxyParameter navigateProxyParameter,
        ITabManager tabManager,
        IDialogManager dialogManager)
    {
        NavigateProxyParameter = navigateProxyParameter;
        TabManager = tabManager;
        DialogManager = dialogManager;

        InitializeComponent();
    }


    private NavigateProxyParameter NavigateProxyParameter { get; }

    private ITabManager TabManager { get; }

    private IDialogManager DialogManager { get; }

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

        await DialogManager.ShowMessageDialogAsync(
            NavigateProxyParameter.Content,
            NavigateProxyParameter.Caption,
            NavigateProxyParameter.ButtonText,
            NavigateProxyParameter.Localization);

        await TabManager.NavigateAsync(NavigateProxyParameter.Target);

        NavigateProxyParameter.NavigatedCallback?.Invoke();
    }
}