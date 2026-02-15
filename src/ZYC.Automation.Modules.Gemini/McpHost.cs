using System.Windows.Threading;
using ZYC.Automation.Abstractions.Tab;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Gemini;

[Register]
public sealed class McpHost : IAsyncDisposable
{
    private WebApplication? _app;

    public McpHost(ITabManager tabManager)
    {
        TabManager = tabManager;
    }

    private ITabManager TabManager { get; }

    public async ValueTask DisposeAsync()
    {
        if (_app is null)
        {
            return;
        }

        await _app.StopAsync();
        await _app.DisposeAsync();
    }

    public async Task StartAsync(Dispatcher dispatcher, int port = 3004)
    {
        var builder = WebApplication.CreateBuilder();


        builder.WebHost.UseUrls($"http://127.0.0.1:{port}");

        builder.Services.AddSingleton(TabManager);
        builder.Services.AddSingleton<IUIDispatcher>(_ => new WpfUIDispatcher(dispatcher));
        builder.Services.AddSingleton<TabInstanceRegistry>();


        builder.Services
            .AddMcpServer()
            .WithHttpTransport()
            .WithToolsFromAssembly();

        var app = builder.Build();

        app.MapMcp();

        await app.StartAsync();
        _app = app;
    }
}