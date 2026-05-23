<p align="center">
  <a href="./extension-points.md">English</a> |
  <a href="./extension-points.ja.md">日本語</a> |
  <a href="./extension-points.zh-CN.md">简体中文</a> |
  <a href="./extension-points.zh-TW.md">繁體中文</a> |
  <a href="./extension-points.ko.md">한국어</a> |
</p>


# 扩展点

ZYC.Framework 的扩展大多通过模块和 Autofac 注册。模块随 Host 加载，注册服务或 UI 贡献；随后 Shell 从这些注册中组合菜单、Tab、工作区操作、状态栏项、任务栏菜单项、拖放动作和 Aspire 资源。

## 扩展点地图

| 扩展点 | 注册位置 | 运行时消费方 |
| --- | --- | --- |
| 模块生命周期 | `ModuleBase.RegisterAsync`, `LoadAsync`, `AfterLoadedAsync` | Host 启动和模块加载器。 |
| URI Tab | `ITabItemFactoryManager.RegisterFactory<T>()` | `TabManager.InternalNavigateAsync(...)`。 |
| 简单视图 Tab | `ISimpleTabItemFactoryManager.Register(...)` | 内置 `SimpleTabItemFactory`。 |
| 主菜单 | `IMainMenuManager`, `IMainMenuItemsProvider` | `MainMenuManager` 和主菜单 View。 |
| 工作区菜单 | `IWorkspaceMenuManager` | `WorkspaceMenuView`。 |
| 工作区上下文菜单 Manager | `IWorkspaceContextMenuManager` | Manager 存在并提供排序；在接线上下文菜单表面时使用。 |
| Tab 头部右键菜单 | `ITabItemHeaderContextMenuItemView` | `TabItemHeaderContextMenuItemsResolver`。 |
| 状态栏 | `IStatusBarManager`, `IStatusBarItemsProvider` | `StatusBarManager`。 |
| 任务栏菜单 | `ITaskbarMenuManager` | `TaskbarContextMenu`。 |
| 配置/状态 | `IConfig`, `IState` | `ModuleTools.RegisterAllFromAssembly(...)`。 |
| 事件 | `IEventAggregator` | 运行时发布/订阅总线。 |
| Toast | `IToastManager`, `IToast` | Toast 弹出宿主。 |
| 拖放 | `IDropActionProvider` | `DropOrchestrator`。 |
| Aspire 资源 | `IExtensionResourcesProvider` | `AspireService.Build(...)`。 |
| CLI 选项 | `ModuleBase.RegisterCommandLineOption(...)` | `ZYC.Framework.CLI` 根命令。 |

## 模块生命周期

用模块生命周期决定每类注册放在哪里：

| 方法 | 用途 |
| --- | --- |
| `RegisterAsync(ContainerBuilder builder)` | 必须在容器构建前完成的 Autofac 注册。 |
| `LoadAsync(ILifetimeScope lifetimeScope)` | 运行时贡献，例如 Tab 工厂、菜单项、状态栏 Provider、Aspire 资源注册。 |
| `AfterLoadedAsync(ILifetimeScope lifetimeScope)` | 需要所有模块都加载完之后再执行的跨模块工作。 |
| `RegisterCommandLineOption(...)` | 模块拥有的 CLI 参数。 |

大多数 UI 模块只需要 `LoadAsync`。

## URI Tab

URI Tab 是暴露功能表面的主要方式。注册一个 `ITabItemFactory`，通常使用 `ILifetimeScope` 上的辅助方法：

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterTabItemFactory<ReportsTabItemFactory>();
    return Task.CompletedTask;
}
```

当路由可以用 URI 部件表达时，使用 `TabItemRouteAttribute`。当判断需要服务、文件类型检查或更复杂策略时，覆盖 `CheckUriMatchedAsync`。

只有在很小的单视图场景中，才使用 `RegisterSimpleTabItemFactory(...)`，也就是一个 `UserControl` 可以直接打开，不需要专门的路由模型。

## 主菜单

根菜单内置 File、View、Tools、Extensions 和 About Provider。模块菜单项通常注册到其中一个 Provider 下：

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterExtensionsMainMenuItem<ReportsMainMenuItem>();
    return Task.CompletedTask;
}
```

菜单排序先按 `Anchor`，再按同一组内的 `Priority`。`MainMenuManager` 会在 Anchor 组之间插入分隔符，并递归排序子项。

只有当模块需要一个包含多个子命令的父菜单时，才创建模块自己的 `IMainMenuItemsProvider`。如果只有一个命令，把 `IMainMenuItem` 注册到现有 Provider 即可。

## 工作区菜单

如果命令需要出现在工作区编号旁边的可见下拉菜单中，使用 `IWorkspaceMenuManager`：

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.Resolve<IWorkspaceMenuManager>()
        .RegisterItem<ReportsWorkspaceMenuItem>();

    return Task.CompletedTask;
}
```

`IWorkspaceMenuItem` 支持 `Title`、`Command`、`SubItems`、`Icon`、`Anchor`、`Priority` 和 `Localization`。当前可见的 `WorkspaceMenuView` 读取 `IWorkspaceMenuManager.GetItems()`。

`IWorkspaceContextMenuManager` 是另一个 Manager，它按 `Anchor` 和 `Priority` 递归排序。除非已经有上下文菜单 View 接入它，否则不要假设它的项会显示出来。

## Tab 头部右键菜单

Tab 头部菜单项是注册为 `ITabItemHeaderContextMenuItemView` 的 WPF 菜单项 View：

```csharp
[RegisterAs(typeof(ITabItemHeaderContextMenuItemView))]
internal partial class ReportsTabHeaderMenuItem :
    ITabItemHeaderContextMenuItemView
{
    public int Order => 20;
}
```

`TabItemHeaderContextMenuItemsResolver` 会解析所有已注册 View，并按 `Order` 排序。由于 WPF ContextMenu 是 late-bound，如果菜单项需要当前 Tab 实例，优先使用 command parameter 和已有的 `ContextMenuItemBase` 模式。

## 状态栏

状态栏扩展贡献一个 `IStatusBarItemsProvider`；Provider 返回一个或多个 `IStatusBarItem`。`StatusBarManager` 聚合所有已注册 Provider，并按 `Order` 排序。

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.Resolve<IStatusBarManager>()
        .RegisterStatusBarItemsProvider<ReportsStatusBarItemsProvider>();

    return Task.CompletedTask;
}
```

每个 Item 通过 `StatusBarSection.Left` 或 `StatusBarSection.Right` 选择显示侧。

## 任务栏菜单

任务栏菜单项实现 `ITaskbarMenuItem`，并注册到 `ITaskbarMenuManager`：

```csharp
lifetimeScope.Resolve<ITaskbarMenuManager>()
    .RegisterMenuItem(lifetimeScope.Resolve<ReportsTaskbarMenuItem>());
```

任务栏菜单按 `Info.Anchor` 分组，按 `Info.Priority` 排序，并递归排序子项。这个表面适合托盘/窗口级命令，不适合本应放进主菜单的功能导航。

## 配置与状态

任何实现 `IConfig` 或 `IState` 的具体类型，都会在模块程序集注册期间从 settings 目录加载，并注册到 Autofac。它适合小型、可 JSON 序列化的设置和状态。

准则：

- 用户可编辑选项放在 `IConfig`。
- 运行时持久化放在 `IState`。
- 类型保持小型，并能承受版本演进。
- 不要把 config/state 当成大型业务数据存储。

## 事件与 Toast

解耦的运行时通知使用 `IEventAggregator`：

```csharp
lifetimeScope.PublishEvent(new ReportsChangedEvent());
lifetimeScope.SubscribeEvent<ReportsChangedEvent>(OnReportsChanged, onUiThread: true);
```

用户可见的临时反馈使用 `IToastManager`：

```csharp
toastManager.PromptMessage(ToastMessage.Info("Report exported.", localization: false));
toastManager.PromptException(exception);
```

事件用于协调，Toast 用于可见反馈。不要把 Toast 消息当成控制流。

## 拖放

拖放动作通过 `IDropActionProvider` 贡献。Orchestrator 会向所有 Provider 询问兼容的 `DropAction`，用 `CanRun()` 过滤，按 `Id` 去重，然后执行默认动作或显示选择器。

当模块需要以工作区感知的方式处理拖入的文件、路径或 Tab 负载时，使用这个扩展点。`DropContext` 包含目标对象、工作区 id、修饰键、屏幕坐标和取消令牌。

## Aspire 资源

Aspire 扩展模块注册 `IExtensionResourcesProvider`。`AspireService.Build(...)` 会解析所有 Provider，并调用每个 Provider 的 `ConfigureResources(builder)`。

对于命令行子服务，通过 `ICommandlineResourcesProvider` 注册：

```csharp
lifetimeScope.Resolve<ICommandlineResourcesProvider>()
    .Register(new CommandlineServiceOptions
    {
        Name = "reports-worker",
        WorkDirectory = workerDirectory,
        Command = "dotnet run"
    });
```

它适合由 Aspire Host 拉起的 sidecar 服务，而不是大型应用内 UI。

## CLI 选项

CLI 会加载模块，并在最终确定根命令前调用 `RegisterCommandLineOption(container, optionRegister)`。当模块需要自己的命令行开关时使用它。

如果是完整脚手架命令，使用类似内置 `zyc new` 和 `zyc new-module` 的显式子命令，不要把无关含义塞进普通 flag。

## 如何选择扩展面

| 目标 | 使用 |
| --- | --- |
| 打开功能 View | URI + `ITabItemFactory` |
| 添加一个顶层应用命令 | 现有主菜单 Provider |
| 在一个模块父级下添加多个命令 | 模块自有 `IMainMenuItemsProvider` |
| 添加工作区操作 | `IWorkspaceMenuManager` |
| 添加 Tab 头部动作 | `ITabItemHeaderContextMenuItemView` |
| 显示轻量运行时状态 | Status bar provider |
| 添加托盘/窗口命令 | `ITaskbarMenuManager` |
| 持久化小型设置或状态 | `IConfig` / `IState` |
| 协调运行时行为 | `IEventAggregator` |
| 显示用户反馈 | `IToastManager` |
| 添加拖入文件行为 | `IDropActionProvider` |
| 启动 sidecar 服务 | Aspire resource provider |
