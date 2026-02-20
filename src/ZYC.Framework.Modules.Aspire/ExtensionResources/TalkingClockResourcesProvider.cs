using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Modules.Aspire.Abstractions;

namespace ZYC.Framework.Modules.Aspire.ExtensionResources;

[RegisterSingleInstanceAs(typeof(IExtensionResourcesProvider), PreserveExistingDefaults = true)]
internal class TalkingClockResourcesProvider : ResourcesProviderBase
{
    public override void ConfigureResources(IDistributedApplicationBuilder builder)
    {
        builder.AddTalkingClock("TalkingClock");
    }
}