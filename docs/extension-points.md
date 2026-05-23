<p align="center">
  <a href="./extension-points.md">English</a> |
  <a href="./extension-points.ja.md">日本語</a> |
  <a href="./extension-points.zh-CN.md">简体中文</a> |
  <a href="./extension-points.zh-TW.md">繁體中文</a> |
  <a href="./extension-points.ko.md">한국어</a> |
</p>


# Extension Points

ZYC.Framework extensions are mostly registered through modules and Autofac. A module loads with the host, registers services or UI contributions, and then the shell composes menus, tabs, workspace operations, status bar items, taskbar menu items, drag/drop actions, and Aspire resources from those registrations.

## Extension Point Map

| Extension point | Register from | Runtime consumer |
| --- | --- | --- |
| Module lifecycle | `ModuleBase.RegisterAsync`, `LoadAsync`, `AfterLoadedAsync` | Host startup and module loader. |
| URI tabs | `ITabItemFactoryManager.RegisterFactory<T>()` | `TabManager.InternalNavigateAsync(...)`. |
| Simple view tabs | `ISimpleTabItemFactoryManager.Register(...)` | Built-in `SimpleTabItemFactory`. |
| Main menu | `IMainMenuManager`, `IMainMenuItemsProvider` | `MainMenuManager` and main menu views. |
| Workspace menu | `IWorkspaceMenuManager` | `WorkspaceMenuView`. |
| Workspace context menu manager | `IWorkspaceContextMenuManager` | Manager exists and sorts items; use it when wiring a context-menu surface. |
| Tab header context menu | `ITabItemHeaderContextMenuItemView` | `TabItemHeaderContextMenuItemsResolver`. |
| Status bar | `IStatusBarManager`, `IStatusBarItemsProvider` | `StatusBarManager`. |
| Taskbar menu | `ITaskbarMenuManager` | `TaskbarContextMenu`. |
| Config/state | `IConfig`, `IState` | `ModuleTools.RegisterAllFromAssembly(...)`. |
| Events | `IEventAggregator` | Runtime publish/subscribe bus. |
| Toasts | `IToastManager`, `IToast` | Toast popup host. |
| Drag/drop | `IDropActionProvider` | `DropOrchestrator`. |
| Aspire resources | `IExtensionResourcesProvider` | `AspireService.Build(...)`. |
| CLI options | `ModuleBase.RegisterCommandLineOption(...)` | `ZYC.Framework.CLI` root command. |

## Module Lifecycle

Use the module lifecycle to decide where each registration belongs:

| Method | Use for |
| --- | --- |
| `RegisterAsync(ContainerBuilder builder)` | Autofac registrations that must exist before the container is built. |
| `LoadAsync(ILifetimeScope lifetimeScope)` | Runtime contributions such as tab factories, menu items, status bar providers, and Aspire resource registration. |
| `AfterLoadedAsync(ILifetimeScope lifetimeScope)` | Cross-module work that needs all modules to already be loaded. |
| `RegisterCommandLineOption(...)` | CLI flags owned by the module. |

Most UI modules only need `LoadAsync`.

## URI Tabs

URI tabs are the primary way to expose a feature surface. Register an `ITabItemFactory`, usually by using the helper on `ILifetimeScope`:

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterTabItemFactory<ReportsTabItemFactory>();
    return Task.CompletedTask;
}
```

Use `TabItemRouteAttribute` when the route can be described by URI parts. Override `CheckUriMatchedAsync` when the decision needs a service, file type check, or more complex policy.

Use `RegisterSimpleTabItemFactory(...)` only for the small single-view case where a `UserControl` can be opened without a dedicated route model.

## Main Menu

The root menu contains built-in providers for File, View, Tools, Extensions, and About. Module menu items normally register under one of those providers:

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterExtensionsMainMenuItem<ReportsMainMenuItem>();
    return Task.CompletedTask;
}
```

Menu ordering uses `Anchor` first and `Priority` inside the anchor group. `MainMenuManager` inserts separators between anchor groups and sorts sub-items recursively.

Create a module-owned `IMainMenuItemsProvider` only when the module needs a parent menu with multiple child commands. For one command, register an `IMainMenuItem` under an existing provider.

## Workspace Menus

Use `IWorkspaceMenuManager` for commands that should appear in the visible workspace drop-down beside the workspace index:

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.Resolve<IWorkspaceMenuManager>()
        .RegisterItem<ReportsWorkspaceMenuItem>();

    return Task.CompletedTask;
}
```

`IWorkspaceMenuItem` supports `Title`, `Command`, `SubItems`, `Icon`, `Anchor`, `Priority`, and `Localization`. The visible `WorkspaceMenuView` currently reads `IWorkspaceMenuManager.GetItems()`.

`IWorkspaceContextMenuManager` is a separate manager that sorts items by `Anchor` and `Priority` recursively. Do not assume its items are visible unless a context-menu view has been wired to it.

## Tab Header Context Menu

Tab header menu items are WPF menu item views registered as `ITabItemHeaderContextMenuItemView`:

```csharp
[RegisterAs(typeof(ITabItemHeaderContextMenuItemView))]
internal partial class ReportsTabHeaderMenuItem :
    ITabItemHeaderContextMenuItemView
{
    public int Order => 20;
}
```

`TabItemHeaderContextMenuItemsResolver` resolves all registered views and orders them by `Order`. Because WPF context menus are late-bound, prefer command parameters and the existing `ContextMenuItemBase` pattern when the item needs the current tab instance.

## Status Bar

A status bar extension contributes an `IStatusBarItemsProvider`; the provider returns one or more `IStatusBarItem` instances. `StatusBarManager` aggregates all registered providers and orders items by `Order`.

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.Resolve<IStatusBarManager>()
        .RegisterStatusBarItemsProvider<ReportsStatusBarItemsProvider>();

    return Task.CompletedTask;
}
```

Each item chooses its side with `StatusBarSection.Left` or `StatusBarSection.Right`.

## Taskbar Menu

Taskbar menu items implement `ITaskbarMenuItem` and are registered with `ITaskbarMenuManager`:

```csharp
lifetimeScope.Resolve<ITaskbarMenuManager>()
    .RegisterMenuItem(lifetimeScope.Resolve<ReportsTaskbarMenuItem>());
```

The taskbar menu groups by `Info.Anchor`, orders by `Info.Priority`, and recursively sorts sub-items. Use this surface for tray/window-level commands, not for feature navigation that belongs in the main menu.

## Config and State

Any concrete type implementing `IConfig` or `IState` is loaded from the settings directory during module assembly registration and registered into Autofac. Use this for small JSON-serializable settings and state.

Guidelines:

- Put user-editable options in `IConfig`.
- Put runtime persistence in `IState`.
- Keep the type small and version-tolerant.
- Do not use config/state as a large business data store.

## Events and Toasts

Use `IEventAggregator` for decoupled runtime notifications:

```csharp
lifetimeScope.PublishEvent(new ReportsChangedEvent());
lifetimeScope.SubscribeEvent<ReportsChangedEvent>(OnReportsChanged, onUiThread: true);
```

Use `IToastManager` for user-facing transient feedback:

```csharp
toastManager.PromptMessage(ToastMessage.Info("Report exported.", localization: false));
toastManager.PromptException(exception);
```

Events are for coordination. Toasts are for visible feedback. Avoid using toast messages as control flow.

## Drag and Drop

Drag/drop actions are contributed through `IDropActionProvider`. The orchestrator asks every provider for compatible `DropAction` items, filters by `CanRun()`, de-duplicates by `Id`, and then either executes the default action or shows a picker.

Use this extension point when a module wants to handle dropped files, paths, or tab payloads in a workspace-aware way. The `DropContext` includes the target object, workspace id, modifier keys, screen point, and cancellation token.

## Aspire Resources

Aspire extension modules register `IExtensionResourcesProvider`. `AspireService.Build(...)` resolves all providers and calls `ConfigureResources(builder)` on each one.

For command-line child services, register through `ICommandlineResourcesProvider`:

```csharp
lifetimeScope.Resolve<ICommandlineResourcesProvider>()
    .Register(new CommandlineServiceOptions
    {
        Name = "reports-worker",
        WorkDirectory = workerDirectory,
        Command = "dotnet run"
    });
```

Use this for sidecar services that should be launched by the Aspire host instead of by a large in-app UI.

## CLI Options

The CLI loads modules and calls `RegisterCommandLineOption(container, optionRegister)` before finalizing the root command. Use this when the module needs its own command-line switch.

For full scaffolding commands, use explicit subcommands such as the built-in `zyc new` and `zyc new-module` pattern instead of overloading unrelated flags.

## Choosing the Right Surface

| Goal | Use |
| --- | --- |
| Open a feature view | URI + `ITabItemFactory` |
| Add one top-level app command | Existing main menu provider |
| Add several commands under one module parent | Module-owned `IMainMenuItemsProvider` |
| Add a workspace operation | `IWorkspaceMenuManager` |
| Add a tab header action | `ITabItemHeaderContextMenuItemView` |
| Show lightweight runtime status | Status bar provider |
| Add tray/window command | `ITaskbarMenuManager` |
| Persist small settings or state | `IConfig` / `IState` |
| Coordinate runtime behavior | `IEventAggregator` |
| Show user feedback | `IToastManager` |
| Add dropped-file behavior | `IDropActionProvider` |
| Launch a sidecar service | Aspire resource provider |
