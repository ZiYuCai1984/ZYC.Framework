using System.IO;
using Autofac;
using Microsoft.AspNetCore.Authentication.Cookies;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Modules.BlazorDemo.Components;

namespace ZYC.Framework.Modules.BlazorDemo.UI;

[Register]
internal partial class BlazorDemoView
{
    private WebApplication? _app;
    private string? _boundUrl;
    private CancellationTokenSource? _hostCts;

    public BlazorDemoView(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
    }

    protected override async Task InternalWebViewHostLoadedAsync()
    {
        await base.InternalWebViewHostLoadedAsync();
        await EnsureServerStartedAsync();
        if (_boundUrl is not null)
        {
            await NavigateAsync(_boundUrl);
        }
    }


    public async Task EnsureServerStartedAsync()
    {
        if (_app is not null)
        {
            return;
        }

        var asm = typeof(App).Assembly;
        var contentRoot = Path.GetDirectoryName(asm.Location)!;
        var webRoot = Path.Combine(contentRoot, "wwwroot");

        var options = new WebApplicationOptions
        {
            ApplicationName = asm.GetName().Name,
            ContentRootPath = contentRoot,
            WebRootPath = Directory.Exists(webRoot) ? webRoot : null
        };

        var builder = WebApplication.CreateBuilder(options);
        builder.Host.UseServiceProviderFactory(
            new AutofacChildScopeServiceProviderFactory(LifetimeScope));
        builder.Services.AddControllers();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();


        builder.WebHost.UseStaticWebAssets();

        // Random port
        builder.WebHost.UseUrls("http://127.0.0.1:0");

        builder.Services.AddRazorPages();
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        // app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseAntiforgery();

        app.MapRazorPages();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .RequireAuthorization();

        _hostCts = new CancellationTokenSource();
        await app.StartAsync(_hostCts.Token);

        _boundUrl = app.Urls.First(u => u.StartsWith("http://", StringComparison.OrdinalIgnoreCase));
        _app = app;

        await NavigateAsync(_boundUrl);
    }


    protected override void InternalDispose()
    {
        base.InternalDispose();

        var cts = Interlocked.Exchange(ref _hostCts, null);
        var app = Interlocked.Exchange(ref _app, null);

        try
        {
            cts?.Cancel();
        }
        catch
        {
            //ignore
        }
        finally
        {
            cts?.Dispose();
        }


        // ReSharper disable once MethodSupportsCancellation
        _ = Task.Run(async () =>
        {
            try
            {
                if (app is not null)
                {
                    //!WARNING To prevent UI jams, skip <StopAsync> here
                    await app.DisposeAsync().ConfigureAwait(false);
                }
            }
            catch
            {
                //ignore
            }
        });
    }
}