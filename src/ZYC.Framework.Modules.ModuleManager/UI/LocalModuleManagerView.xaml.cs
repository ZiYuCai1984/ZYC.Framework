using System.Collections.ObjectModel;
using System.Reactive.Disposables.Fluent;
using ZYC.CoreToolkit.Abstractions.Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Modules.ModuleManager.Abstractions;
using ZYC.Framework.Modules.ModuleManager.Abstractions.Event;

namespace ZYC.Framework.Modules.ModuleManager.UI;

[Register]
public partial class LocalModuleManagerView
{
    public LocalModuleManagerView(
        IEventAggregator eventAggregator,
        ILocalModuleManager localModuleManager,
        ModuleConfig moduleConfig)
    {
        EventAggregator = eventAggregator;
        LocalModuleManager = localModuleManager;
        ModuleConfig = moduleConfig;

        EventAggregator.Subscribe<DeleteModuleEvent>(_ =>
        {
            OnPropertyChanged(nameof(ModulePendingDeleteAssemblyNames));
        }).DisposeWith(CompositeDisposable);

        EventAggregator.Subscribe<DisableModuleEvent>(_ =>
        {
            OnPropertyChanged(nameof(ModuleDisabledAssemblyNames));
        }).DisposeWith(CompositeDisposable);
        ;

        EventAggregator.Subscribe<EnableModuleEvent>(_ =>
        {
            OnPropertyChanged(nameof(ModuleDisabledAssemblyNames));
        }).DisposeWith(CompositeDisposable);
        ;

        InitializeComponent();
    }


    private IEventAggregator EventAggregator { get; }

    private ILocalModuleManager LocalModuleManager { get; }

    private ModuleConfig ModuleConfig { get; }

    public string[] ModuleDisabledAssemblyNames => ModuleConfig.DisabledAssemblyNames;

    public string[] ModulePendingDeleteAssemblyNames => LocalModuleManager.GetPendingRemoveDlls();

    public ObservableCollection<IModuleInfo> ModuleInfos { get; } = new();

    protected override void InternalOnLoaded()
    {
        var modules = LocalModuleManager.GetModules();
        foreach (var module in modules)
        {
            ModuleInfos.Add(module);
        }
    }
}