using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Modules.NuGet.Abstractions;

namespace ZYC.Framework.Modules.NuGet;

internal class Module : ModuleBase
{
    public override string Icon => NuGetModuleConstants.Icon;
}