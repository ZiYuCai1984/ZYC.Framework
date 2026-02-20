using System.IO;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.BlazorDemo.Abstractions;

namespace ZYC.Framework.Modules.BlazorDemo;

[RegisterSingleInstanceAs(typeof(IBlazorServerManager))]
internal class BlazorServerManager : IBlazorServerManager
{
    public BlazorServerManager(ILifetimeScope lifetimeScope, IAppContext appContext)
    {
        LifetimeScope = lifetimeScope;
        AppContext = appContext;
    }

    private ILifetimeScope LifetimeScope { get; }

    private IAppContext AppContext { get; }

    public async Task<IBlazorServer> StartBlazorServerAsync<TRootComponent>(string wwwrootFolder)
    {
        var contentRoot = AppContext.GetCurrentDirectory();
        var webRoot = Path.Combine(contentRoot, wwwrootFolder);

        var options = new WebApplicationOptions
        {
            //ApplicationName = asm.GetName().Name,
            ContentRootPath = contentRoot,
            WebRootPath = Directory.Exists(webRoot) ? webRoot : null
        };

        var builder = WebApplication.CreateBuilder(options);
        builder.Host.UseServiceProviderFactory(
            new AutofacChildScopeServiceProviderFactory(LifetimeScope));
        builder.Services.AddControllers();


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

        app.MapControllers();

        app.UseStaticFiles();

        app.UseAntiforgery();

        app.MapRazorPages();

        app.MapRazorComponents<TRootComponent>()
            .AddInteractiveServerRenderMode();

        var hostCts = new CancellationTokenSource();
        await app.StartAsync(hostCts.Token);

        var boundUrl = app.Urls.First(u => u.StartsWith("http://", StringComparison.OrdinalIgnoreCase));

        return new BlazorServer(new Uri(boundUrl), app, hostCts);
    }
}