using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;

namespace ZYC.Framework.Tab.BuildIn;

[RegisterSingleInstanceAs(typeof(ModuleLoadTabItemFactory), typeof(IModuleLoadInfoManager))]
[TabItemRoute(Host = ModuleLoadErrorTabItem.Constants.Host)]
internal class ModuleLoadTabItemFactory : TabItemFactoryBase, IModuleLoadInfoManager
{
    public ModuleLoadTabItemFactory(
        ILifetimeScope lifetimeScope,
        ITabManager tabManager,
        IEventAggregator eventAggregator)
    {
        TabManager = tabManager;

        eventAggregator.Subscribe<TabManagerRestoreCompleted>(_ =>
        {
            if (ModuleLoadErrorInfos.Length == 0)
            {
                return;
            }

            var appConfig = lifetimeScope.Resolve<AppConfig>();
            if (appConfig.SuppressModuleLoadError)
            {
                return;
            }

            DisplayLoadErrorInfoPage();
        }).DisposeWith(CompositeDisposable);
    }

    private CompositeDisposable CompositeDisposable { get; } = new();

    private ITabManager TabManager { get; }

    private ModuleLoadErrorInfo[] ModuleLoadErrorInfos { get; set; } = [];

    public void DisplayLoadErrorInfoPage()
    {
        TabManager.NavigateAsync(UriTools.CreateAppUri(ModuleLoadErrorTabItem.Constants.Host));
    }

    public ModuleLoadErrorInfo[] GetLoadErrorInfos()
    {
        return ModuleLoadErrorInfos;
    }

    public void SetLoadErrorInfos(ModuleLoadErrorInfo[] moduleLoadErrorInfos)
    {
        DebuggerTools.CheckCalledOnce();
        ModuleLoadErrorInfos = moduleLoadErrorInfos;
    }

    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<ModuleLoadErrorTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }
}