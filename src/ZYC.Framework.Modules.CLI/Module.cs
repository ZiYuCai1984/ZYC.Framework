using System.IO;
using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.CLI.Abstractions;

namespace ZYC.Framework.Modules.CLI;

internal class Module : ModuleBase
{
    public override string Icon => CLIModuleConstants.Icon;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<CLITabItemFactory>();
        lifetimeScope.RegisterToolsMainMenuItem<CLIMainMenuItem>();


        var appContext = lifetimeScope.Resolve<IAppContext>();

        NativeDllLoaderTools.LoadFrom(
            Path.Combine(
                //!WARNING Using appContext.GetMainAppDirectory() here will cause the application to fail during the update apply !!
                appContext.GetCurrentDirectory(),
                "runtimes",
                "win10-x64",
                "native",
                "conpty.dll"));

        NativeDllLoaderTools.LoadFrom(
            Path.Combine(
                appContext.GetCurrentDirectory(),
                "runtimes",
                "win-x64",
                "native",
                "Microsoft.Terminal.Control.dll"));

        return Task.CompletedTask;
    }
}