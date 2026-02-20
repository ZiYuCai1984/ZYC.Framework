using ZYC.Framework.Modules.Aspire.Abstractions;

namespace ZYC.Framework.Modules.Aspire.ExtensionResources;

internal abstract class ResourcesProviderBase : IExtensionResourcesProvider
{
    public void ConfigureResources(object builder)
    {
        ConfigureResources((IDistributedApplicationBuilder)builder);
    }

    public abstract void ConfigureResources(IDistributedApplicationBuilder builder);
}