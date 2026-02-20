using System.Net;
using System.Net.Security;
using Autofac;
using Microsoft.Web.WebView2.Core;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;
using ZYC.CoreToolkit;

namespace ZYC.Framework.WebView2;

public class SelfProxyWebViewHostBase : WebViewHostBase
{
    private int? _proxyPort;

    public SelfProxyWebViewHostBase(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        ProxyServer = new ProxyServer();
        ProxyServer.BeforeRequest += OnProxyRequest;
        ProxyServer.BeforeResponse += OnProxyResponse;
        ProxyServer.ServerCertificateValidationCallback += OnProxyCertificateValidation;
        ProxyServer.ClientCertificateSelectionCallback += OnProxyCertificateSelection;


        ExplicitProxyEndPoint = new ExplicitProxyEndPoint(
            IPAddress.Any,
            ProxyPort);
        ExplicitProxyEndPoint.BeforeTunnelConnectRequest += OnProxyBeforeTunnelConnectRequest;
        ExplicitProxyEndPoint.BeforeTunnelConnectResponse += OnProxyBeforeTunnelConnectResponse;
        ProxyServer.AddEndPoint(ExplicitProxyEndPoint);

        ProxyServer.Start();
    }


    protected ExplicitProxyEndPoint ExplicitProxyEndPoint { get; set; }

    public int ProxyPort => _proxyPort ??= TempTools.GetAvailablePort(1000);

    protected ProxyServer ProxyServer { get; set; }

    protected virtual Task OnProxyBeforeTunnelConnectResponse(object sender, TunnelConnectSessionEventArgs e)
    {
        return Task.CompletedTask;
    }

    protected override Task<CoreWebView2Environment> GetCoreWebView2Environment()
    {
        var options = new CoreWebView2EnvironmentOptions
        {
            IsCustomCrashReportingEnabled = true,
            AdditionalBrowserArguments = $"--proxy-server=localhost:{ProxyPort}"
        };
        return CoreWebView2Environment.CreateAsync(
            null,
            null,
            options);
    }

    public override void Dispose()
    {
        if (Disposing)
        {
            return;
        }

        base.Dispose();

        ProxyServer.BeforeRequest -= OnProxyRequest;
        ProxyServer.BeforeResponse -= OnProxyResponse;
        ProxyServer.ServerCertificateValidationCallback -= OnProxyCertificateValidation;
        ProxyServer.ClientCertificateSelectionCallback -= OnProxyCertificateSelection;

        ExplicitProxyEndPoint.BeforeTunnelConnectRequest -= OnProxyBeforeTunnelConnectRequest;
        ExplicitProxyEndPoint.BeforeTunnelConnectResponse -= OnProxyBeforeTunnelConnectResponse;

        ProxyServer.Stop();
        ProxyServer.Dispose();
    }

    protected virtual Task OnProxyBeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnProxyCertificateSelection(object sender, CertificateSelectionEventArgs e)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnProxyCertificateValidation(object sender, CertificateValidationEventArgs e)
    {
        if (e.SslPolicyErrors == SslPolicyErrors.None)
        {
            e.IsValid = true;
        }

        return Task.CompletedTask;
    }

    protected virtual Task OnProxyResponse(object sender, SessionEventArgs e)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnProxyRequest(object sender, SessionEventArgs e)
    {
        return Task.CompletedTask;
    }
}