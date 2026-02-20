using ZYC.CoreToolkit.Abstractions.Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.Notification.Banner;
using ZYC.Framework.Modules.ModuleManager.Abstractions;
using ZYC.Framework.Modules.ModuleManager.Abstractions.Event;

namespace ZYC.Framework.Modules.ModuleManager;

[RegisterSingleInstanceAs(typeof(ILocalModuleManager))]
internal class LocalModuleManager : ILocalModuleManager
{
    public LocalModuleManager(
        IEventAggregator eventAggregator,
        IBannerManager bannerManager,
        IModuleInfo[] localModules,
        IPendingDeleteManager pendingDeleteManager,
        ModuleConfig moduleConfig)
    {
        EventAggregator = eventAggregator;
        BannerManager = bannerManager;

        LocalModules = localModules;
        PendingDeleteManager = pendingDeleteManager;
        ModuleConfig = moduleConfig;
    }

    private IEventAggregator EventAggregator { get; }
    private IBannerManager BannerManager { get; }

    private IModuleInfo[] LocalModules { get; }
    private IPendingDeleteManager PendingDeleteManager { get; }
    private ModuleConfig ModuleConfig { get; }


    public IModuleInfo[] GetModules()
    {
        return LocalModules.ToArray();
    }

    public IModuleInfo[] GetDependency(IModuleInfo moduleInfo)
    {
        var dependencyModules = moduleInfo.ReferenceAssemblyNames;

        var modules = new List<IModuleInfo>();

        foreach (var module in dependencyModules)
        {
            if (PendingDeleteManager.Contains(module))
            {
                continue;
            }

            modules.Add(LocalModules.First(t => t.ModuleAssemblyName == module));
        }

        return modules.ToArray();
    }

    public IModuleInfo[] GetDependencyBy(IModuleInfo moduleInfo)
    {
        var dependencyBy = new List<IModuleInfo>();


        foreach (var module in LocalModules)
        {
            if (module.ReferenceAssemblyNames.Contains(moduleInfo.ModuleAssemblyName))
            {
                dependencyBy.Add(module);
            }
        }

        return dependencyBy.ToArray();
    }

    public void DisableModule(IModuleInfo moduleInfo)
    {
        var modules = ModuleConfig.DisabledAssemblyNames.ToList();
        modules.Add(moduleInfo.ModuleAssemblyName);

        ModuleConfig.DisabledAssemblyNames = modules.ToArray();
        EventAggregator.Publish(new DisableModuleEvent(moduleInfo));

        BannerManager.PromptRestart();
    }

    public void EnableModule(IModuleInfo moduleInfo)
    {
        var modules = ModuleConfig.DisabledAssemblyNames.ToList();
        modules.Remove(moduleInfo.ModuleAssemblyName);

        ModuleConfig.DisabledAssemblyNames = modules.ToArray();
        EventAggregator.Publish(new EnableModuleEvent(moduleInfo));

        BannerManager.PromptRestart();
    }


    public void DeleteModuleRecursive(IModuleInfo moduleInfo)
    {
        InternalDeleteModuleRecursive(moduleInfo);
        BannerManager.PromptRestart();
    }

    public string[] GetPendingRemoveDlls()
    {
        return PendingDeleteManager.GetFiles().Where(t => t.EndsWith(".dll")).ToArray();
    }

    public bool IsModuleDisabled(string moduleAssemblyName)
    {
        return ModuleConfig.DisabledAssemblyNames.Contains(moduleAssemblyName);
    }

    public bool IsMoudlePendingDelete(string moduleAssemblyName)
    {
        return PendingDeleteManager.Contains(moduleAssemblyName);
    }

    private void InternalDeleteModuleRecursive(IModuleInfo moduleInfo)
    {
        if (PendingDeleteManager.Contains(moduleInfo.ModuleAssemblyName))
        {
            return;
        }

        SetModuleDeleteState(moduleInfo);

        var dependencyModules = GetDependencyBy(moduleInfo);
        foreach (var dependencyModule in dependencyModules)
        {
            InternalDeleteModuleRecursive(dependencyModule);
        }
    }

    private void SetModuleDeleteState(IModuleInfo moduleInfo)
    {
        PendingDeleteManager.Add(moduleInfo.ModuleAssemblyName);
        EventAggregator.Publish(new DeleteModuleEvent(moduleInfo));
    }
}