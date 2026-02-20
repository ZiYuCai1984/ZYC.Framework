using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.ModuleManager.UI;

namespace ZYC.Framework.Modules.ModuleManager.Commands;

[RegisterSingleInstance]
internal class RefreshNuGetModuleCommand : AsyncCommandBase<NuGetModuleManagerView>
{
    protected override async Task InternalExecuteAsync(NuGetModuleManagerView view)
    {
        await view.RefreshNuGetModulesAsync();
    }
}