using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Modules.Aspire.Abstractions;

namespace ZYC.Framework.Modules.Aspire.ExtensionResources;

[RegisterSingleInstanceAs(
    typeof(ICommandlineResourcesProvider), typeof(IExtensionResourcesProvider), PreserveExistingDefaults = true)]
internal class CommandlineResourcesProvider : ResourcesProviderBase, ICommandlineResourcesProvider
{
    private IList<CommandlineServiceOptions> CommandlineServiceOptions { get; } = new List<CommandlineServiceOptions>();

    public void Register(CommandlineServiceOptions options)
    {
        CommandlineServiceOptions.Add(options);
    }

    public override void ConfigureResources(IDistributedApplicationBuilder builder)
    {
        foreach (var option in CommandlineServiceOptions.ToArray())
        {
            builder.AddCommandlineExecutable(option);
        }
    }
}

internal static class CommandlineResourcesProviderEx
{
    public static IResourceBuilder<ExecutableResource> AddCommandlineExecutable(
        this IDistributedApplicationBuilder builder,
        CommandlineServiceOptions options)
    {
        var exe = builder.AddExecutable(
                options.Name,
                "cmd.exe",
                options.WorkDirectory)
            .WithArgs("/c", options.Command);

        return exe;
    }
}