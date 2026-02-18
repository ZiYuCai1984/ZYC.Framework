using System.Windows.Threading;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Protocol;
using ZYC.Automation.Abstractions;
using ZYC.Automation.Modules.MCP.Server.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.MCP.Server;

[Register]
public sealed class MCPServer : IAsyncDisposable
{
    private WebApplication? _app;

    public MCPServer(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    private ILifetimeScope LifetimeScope { get; }

    public async ValueTask DisposeAsync()
    {
        if (_app is null)
        {
            return;
        }

        await _app.StopAsync();
        await _app.DisposeAsync();

        _app = null;
    }

    public async Task StopAsync()
    {
        await DisposeAsync();
    }

    public async Task StartAsync(Dispatcher dispatcher, int port)
    {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.UseUrls($"http://127.0.0.1:{port}");
        builder.Host.UseServiceProviderFactory(
            new AutofacChildScopeServiceProviderFactory(LifetimeScope));

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddMcpServer(options =>
            {
                options.ServerInfo = new Implementation
                {
                    Name = ProductInfo.ProductName,
                    Version = ProductInfo.Version
                };
            })
            .WithHttpTransport()
            .AddAutoDiscoveredTools(LifetimeScope);


        var app = builder.Build();
        app.MapMcp();

        await app.StartAsync();
        _app = app;
    }
}