using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Aspire.Abstractions;
using ZYC.Framework.Modules.Aspire.Abstractions.Event;
using ZYC.Framework.Modules.Aspire.UI.Toast;

namespace ZYC.Framework.Modules.Aspire;

internal partial class AspireService
{
    public static AspireService Build(ILifetimeScope lifetimeScope)
    {
        var options = new DistributedApplicationOptions
        {
            AssemblyName = typeof(Module).Assembly.GetName().Name
        };

        var builder = DistributedApplication.CreateBuilder(options);

        var hostApplicationBuilder = GetInnerHostApplicationBuilder(builder);
        hostApplicationBuilder.ConfigureContainer(
            new AutofacChildScopeServiceProviderFactory(lifetimeScope));

        var aspireConfig = lifetimeScope.Resolve<AspireConfig>();
        foreach (var kv in aspireConfig.Environment)
        {
            builder.Configuration[kv.Key] = kv.Value;
        }

        var aspireServiceEnvironment = lifetimeScope.Resolve<AspireServiceEnvironment>();

        HackDcpOptions(builder, aspireServiceEnvironment);

        var extensionResourcesProviders = lifetimeScope.Resolve<IExtensionResourcesProvider[]>();
        foreach (var provider in extensionResourcesProviders)
        {
            provider.ConfigureResources(builder);
        }

        var dashboardUri = GetDashboardUri(builder);


        var states = lifetimeScope.Resolve<IState[]>();
        foreach (var s in states)
        {
            builder.AddParameter(s.GetType().Name, () => s.ToJsonText());
        }


        builder.Eventing.Subscribe<ResourceReadyEvent>((@event, _) =>
        {
            if (string.Equals(@event.Resource.Name, "aspire-dashboard", StringComparison.OrdinalIgnoreCase))
            {
                lifetimeScope.PublishEvent(
                    new AspireDashboardReadyEvent(dashboardUri));

                var toastManager = lifetimeScope.Resolve<IToastManager>();
                toastManager.Prompt<AspireServiceStartSuccessToastView>();
            }

            return Task.CompletedTask;
        });

        var app = builder.Build();

        var aspireService =
            lifetimeScope.Resolve<AspireService>(
                new TypedParameter(typeof(DistributedApplication), app),
                new TypedParameter(typeof(Uri), dashboardUri));
        return aspireService;
    }


    private static Uri GetDashboardUri(IDistributedApplicationBuilder builder)
    {
        var browserToken = builder.Configuration["AppHost:BrowserToken"];
        var urlsRaw = builder.Configuration["ASPNETCORE_URLS"];

        if (string.IsNullOrWhiteSpace(urlsRaw) || string.IsNullOrWhiteSpace(browserToken))
        {
            throw new InvalidOperationException("Missing ASPNETCORE_URLS or AppHost:BrowserToken from configuration.");
        }

        var dashboardUrl = $"{urlsRaw.TrimEnd('/')}/login?t={browserToken}";
        return new Uri(dashboardUrl);
    }
}