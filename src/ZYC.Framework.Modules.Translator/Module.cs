using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Modules.Aspire.Abstractions;
using ZYC.Framework.Modules.Translator.Abstractions;

namespace ZYC.Framework.Modules.Translator;

internal class Module : ModuleBase
{
    public override string Icon => TranslatorModuleConstants.Icon;


    public override async Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        await Task.CompletedTask;

        if (lifetimeScope.TryResolve<ICommandlineResourcesProvider>(out var commandlineResourcesProvider))
        {
            commandlineResourcesProvider.Register(
                new CommandlineServiceOptions
                {
                    Name = "libretranslate",
                    Command = "libretranslate --load-only en,ja,zh,zt,ko"
                });
        }
    }
}