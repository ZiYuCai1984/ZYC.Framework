using System.IO;
using Autofac;
using Microsoft.Web.WebView2.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.ApiReference.Abstractions;

namespace ZYC.Framework.Modules.ApiReference.UI;

[Register]
internal partial class ApiReferenceView
{
    private string? _homePageUri;

    public ApiReferenceView(
        ILifetimeScope lifetimeScope,
        ApiReferenceNavigationState apiReferenceNavigationState,
        IAppContext appContext) : base(lifetimeScope)
    {
        ApiReferenceNavigationState = apiReferenceNavigationState;
        AppContext = appContext;

        CustomBrowserArguments.Add("--disable-web-security");
    }

    protected override string WebView2UserDataFolder => Path.Combine(AppContext.GetCurrentDirectory(),
        $"{ApiReferenceModuleConstants.DocFolder}.WebView2");

    private ApiReferenceNavigationState ApiReferenceNavigationState { get; }
    private IAppContext AppContext { get; }

    public override string HomePageUri => _homePageUri ??= Path.Combine(
        Path.GetDirectoryName(typeof(ApiReferenceView).Assembly.Location)!,
        ApiReferenceModuleConstants.DocFolder,
        "_api",
        "ZYC.Framework.Abstractions.html");

    protected override async Task InternalWebViewHostLoadedAsync()
    {
        if (string.IsNullOrWhiteSpace(ApiReferenceNavigationState.Uri))
        {
            await NavigateAsync(HomePageUri);
        }
        else
        {
            await NavigateAsync(ApiReferenceNavigationState.Uri);
        }
    }

    protected override void InternalOnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        base.InternalOnNavigationCompleted(sender, e);

        ApiReferenceNavigationState.Uri = CoreWebView2.Source;
    }
}
