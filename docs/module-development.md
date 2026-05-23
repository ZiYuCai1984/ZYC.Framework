<p align="center">
  <a href="./module-development.md">English</a> |
  <a href="./module-development.ja.md">日本語</a> |
  <a href="./module-development.zh-CN.md">简体中文</a> |
  <a href="./module-development.zh-TW.md">繁體中文</a> |
  <a href="./module-development.ko.md">한국어</a> |
</p>


# Module Development

This guide explains how to build a ZYC.Framework module that fits the host runtime: dependency injection first, URI-based tab navigation, optional main menu integration, and a clean split between public contracts and WPF implementation.

## When to Create a Module

Create a module when a feature needs to be discovered and loaded by the ZYC.Framework host instead of being compiled directly into the shell. A module can contribute views, tab factories, main menu items, configuration, state, background services, and command-line options.

For reusable feature contracts, create an `*.Abstractions` project. Keep public DTOs, constants, service interfaces, and menu provider interfaces there. Keep WPF views, tab item implementations, and runtime registrations in the implementation project.

## Recommended Project Shape

| Area | Typical files | Responsibility |
| --- | --- | --- |
| Abstractions | `*ModuleConstants.cs`, `I*MainMenuItemsProvider.cs`, service interfaces | Public contracts that other modules can reference without depending on WPF implementation details. |
| Implementation | `Module.cs` | Module lifecycle entrypoint. Registers factories, menu items, providers, and services. |
| Navigation | `*TabItemFactory.cs`, `*TabItem.cs` | Matches `zyc://` or app URIs and creates tab instances. |
| Menu | `*MainMenuItem.cs`, optional `*MainMenuItemsProvider.cs` | Adds user-visible commands that navigate to module tabs or run module actions. |
| UI | `UI/*View.xaml`, `UI/*View.xaml.cs` | WPF view surface used by the tab item. |

Abstractions projects should not expose WPF control types. `System.Windows.Input.ICommand` is acceptable in abstractions when a command contract is needed.

## Lifecycle

| Method | Timing | Use it for |
| --- | --- | --- |
| `RegisterAsync(ContainerBuilder builder)` | Before the Autofac container is built. | Register services that must be available to the whole container. |
| `LoadAsync(ILifetimeScope lifetimeScope)` | After the container is built and the module is enabled. | Register tab factories, main menu items, status bar items, and runtime hooks. |
| `AfterLoadedAsync(ILifetimeScope lifetimeScope)` | After all enabled modules have loaded. | Cross-module initialization that needs other modules to already be registered. |

Most UI modules only need `LoadAsync`.

## Minimal View Module

For a single-view module, register a simple tab factory from `LoadAsync`:

```csharp
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using MyCompany.Tools.UI;

namespace MyCompany.Tools;

internal class Module : ModuleBase
{
    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterSimpleTabItemFactory(
            new SimpleTabItemFactoryInfo(typeof(ToolsView)));

        return Task.CompletedTask;
    }
}
```

This is the pattern used by the `minimal` project template. It is enough when the module only needs to expose one WPF `UserControl` as a tab.

## Routed Tab Module

Use `TabItemFactoryBase` when the module needs a stable URI, route matching, parameters, custom singleton behavior, or multiple tabs.

```csharp
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Reports.Abstractions;

namespace ZYC.Framework.Modules.Reports;

[RegisterSingleInstance]
[TabItemRoute(Host = ReportsModuleConstants.Host)]
internal class ReportsTabItemFactory : TabItemFactoryBase
{
    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(
        TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<ReportsTabItem>(
            new TypedParameter(
                typeof(TabReference),
                new TabReference(context.Uri)));
    }
}
```

`TabItemRouteAttribute` can match by `Scheme`, `Host`, `Path`, and `PathMatch`. When several factories match the same URI, the factory with the higher `Priority` wins. `TabItemFactoryBase` defaults to `IsSingle = true`; override it when the module should allow multiple tab instances.

## Tab Item and View

A routed tab normally wraps a WPF view with `TabItemInstanceBase<TView>`:

```csharp
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.Reports.Abstractions;
using ZYC.Framework.Modules.Reports.UI;

namespace ZYC.Framework.Modules.Reports;

[Register]
[ConstantsSource(typeof(ReportsModuleConstants))]
internal class ReportsTabItem : TabItemInstanceBase<ReportsView>
{
    public ReportsTabItem(
        ILifetimeScope lifetimeScope,
        TabReference tabReference) : base(lifetimeScope, tabReference)
    {
    }
}
```

Keep tab lifetime behavior in the tab item and keep visual behavior in the view. This keeps routing, tab identity, and UI composition separate.

## Main Menu Entry

Menu items usually navigate to the module URI:

```csharp
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Reports.Abstractions;

namespace ZYC.Framework.Modules.Reports;

[RegisterSingleInstance]
internal class ReportsMainMenuItem : MainMenuItem
{
    public ReportsMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = ReportsModuleConstants.Title,
            Icon = ReportsModuleConstants.Icon
        };

        Command = lifetimeScope.CreateNavigateCommand(ReportsModuleConstants.Uri);
    }
}
```

Register it from the module:

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterTabItemFactory<ReportsTabItemFactory>();
    lifetimeScope.RegisterExtensionsMainMenuItem<ReportsMainMenuItem>();

    return Task.CompletedTask;
}
```

Use the existing menu provider that matches the feature location, such as File, View, Tools, Extensions, or About. Add a module-owned provider only when the feature has several child commands.

## Configuration and State

Types implementing `IConfig` or `IState` are loaded from the settings directory during module registration and registered into Autofac automatically. Use config for user-editable settings and state for runtime persistence such as selected paths, pending operations, and window state.

Do not store large business data in config/state classes. Keep them small, serializable, and version-tolerant.

## Module Loading and Dependencies

The host discovers standard module assemblies named like `ZYC.Framework.Modules*.dll`, then adds assemblies listed in `ModuleConfig.AdditionalAssemblyNames`. Assemblies in `ModuleConfig.DisabledAssemblyNames` are discovered but not loaded as enabled modules.

Dependencies are inferred from references to other module `*.Abstractions.dll` assemblies. If module A references `ZYC.Framework.Modules.B.Abstractions.dll`, the runtime can report that A depends on B without forcing A to reference B's WPF implementation.

## Checklist

- Put public constants and contracts in `*.Abstractions`.
- Keep WPF views and tab items in the implementation project.
- Register services in `RegisterAsync` only when they must exist before the container is built.
- Register tab factories and menu items in `LoadAsync`.
- Use `TabItemRouteAttribute` for stable URI routing.
- Prefer existing main menu providers before adding a new provider.
- Keep config/state classes small and serializable.
