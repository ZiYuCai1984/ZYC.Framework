using Aspire.Hosting.Eventing;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ZYC.Framework.Modules.Aspire.ExtensionResources;

internal sealed class TalkingClockEventingSubscriber(
    ResourceNotificationService notification,
    ResourceLoggerService loggerSvc,
    IServiceProvider services) : IDistributedApplicationEventingSubscriber
{
    public Task SubscribeAsync(
        IDistributedApplicationEventing eventing,
        DistributedApplicationExecutionContext executionContext,
        CancellationToken cancellationToken)
    {
        eventing.Subscribe<AfterResourcesCreatedEvent>((@event, ct) =>
        {
            var model = @event.Model;
            var sp = @event.Services;
            foreach (var clock in model.Resources.OfType<TalkingClockResource>())
            {
                var log = loggerSvc.GetLogger(clock);

                _ = Task.Run(async () =>
                {
                    await sp.GetRequiredService<IDistributedApplicationEventing>()
                        .PublishAsync(new BeforeResourceStartedEvent(clock, services), ct);

                    log.LogInformation("Starting Talking Clock...");

                    await notification.PublishUpdateAsync(clock, s => s with
                    {
                        StartTimeStamp = DateTime.UtcNow,
                        State = KnownResourceStates.Running
                    });

                    while (!ct.IsCancellationRequested)
                    {
                        log.LogInformation("The time is {time}", DateTime.UtcNow);

                        await notification.PublishUpdateAsync(clock,
                            s => s with
                            {
                                State = new ResourceStateSnapshot("Tick", KnownResourceStateStyles.Info)
                            });

                        await Task.Delay(1000, ct);

                        await notification.PublishUpdateAsync(clock,
                            s => s with
                            {
                                State = new ResourceStateSnapshot("Tock", KnownResourceStateStyles.Success)
                            });

                        await Task.Delay(1000, ct);
                    }
                }, ct);
            }

            return Task.CompletedTask;
        });

        return Task.CompletedTask;
    }
}

internal class TalkingClockResource : Resource
{
    public TalkingClockResource(string name) : base(name)
    {
    }
}

internal static class TalkingClockExtensions
{
    public static IResourceBuilder<TalkingClockResource> AddTalkingClock(
        this IDistributedApplicationBuilder builder,
        string name)
    {
        builder.Services.TryAddEventingSubscriber<TalkingClockEventingSubscriber>();

        var clockResource = new TalkingClockResource(name);

        return builder.AddResource(clockResource)
            .ExcludeFromManifest()
            .WithInitialState(new CustomResourceSnapshot
            {
                ResourceType = "TalkingClock",
                CreationTimeStamp = DateTime.UtcNow,
                State = KnownResourceStates.NotStarted,
                Properties =
                [
                    new ResourcePropertySnapshot(CustomResourceKnownProperties.Source, "Talking Clock")
                ],
                Urls =
                [
                    new UrlSnapshot("Speaking Clock", "https://www.speaking-clock.com/", false)
                ]
            });
    }
}