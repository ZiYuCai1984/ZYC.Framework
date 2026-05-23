<p align="center">
  <a href="./extension-points.md">English</a> |
  <a href="./extension-points.ja.md">日本語</a> |
  <a href="./extension-points.zh-CN.md">简体中文</a> |
  <a href="./extension-points.zh-TW.md">繁體中文</a> |
  <a href="./extension-points.ko.md">한국어</a> |
</p>

<!--doc-l10n:begin extension-points-content-->
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

<!--doc-l10n:locale ja-->
# 拡張ポイント

ZYC.Framework の拡張は、主にモジュールと Autofac を通して登録されます。モジュールは Host と一緒にロードされ、サービスや UI コントリビューションを登録します。その後 Shell は、メニュー、タブ、ワークスペース操作、ステータスバー項目、タスクバー メニュー項目、ドラッグ&ドロップ操作、Aspire リソースをそれらの登録から構成します。

## 拡張ポイント一覧

| 拡張ポイント | 登録元 | 実行時の利用側 |
| --- | --- | --- |
| モジュール ライフサイクル | `ModuleBase.RegisterAsync`, `LoadAsync`, `AfterLoadedAsync` | Host startup と module loader。 |
| URI タブ | `ITabItemFactoryManager.RegisterFactory<T>()` | `TabManager.InternalNavigateAsync(...)`。 |
| Simple view tabs | `ISimpleTabItemFactoryManager.Register(...)` | 組み込み `SimpleTabItemFactory`。 |
| メインメニュー | `IMainMenuManager`, `IMainMenuItemsProvider` | `MainMenuManager` とメインメニュー View。 |
| ワークスペース メニュー | `IWorkspaceMenuManager` | `WorkspaceMenuView`。 |
| ワークスペース コンテキストメニュー manager | `IWorkspaceContextMenuManager` | Manager は存在し、項目をソートします。コンテキストメニュー面を接続するときに使います。 |
| タブ ヘッダー コンテキストメニュー | `ITabItemHeaderContextMenuItemView` | `TabItemHeaderContextMenuItemsResolver`。 |
| ステータスバー | `IStatusBarManager`, `IStatusBarItemsProvider` | `StatusBarManager`。 |
| タスクバー メニュー | `ITaskbarMenuManager` | `TaskbarContextMenu`。 |
| 設定/状態 | `IConfig`, `IState` | `ModuleTools.RegisterAllFromAssembly(...)`。 |
| イベント | `IEventAggregator` | 実行時 publish/subscribe bus。 |
| Toast | `IToastManager`, `IToast` | Toast popup host。 |
| Drag/drop | `IDropActionProvider` | `DropOrchestrator`。 |
| Aspire resources | `IExtensionResourcesProvider` | `AspireService.Build(...)`。 |
| CLI options | `ModuleBase.RegisterCommandLineOption(...)` | `ZYC.Framework.CLI` root command。 |

## モジュール ライフサイクル

各登録をどこに置くかは、モジュール ライフサイクルで決めます。

| メソッド | 用途 |
| --- | --- |
| `RegisterAsync(ContainerBuilder builder)` | コンテナー構築前に必要な Autofac 登録。 |
| `LoadAsync(ILifetimeScope lifetimeScope)` | タブ ファクトリ、メニュー項目、ステータスバー provider、Aspire resource 登録などの実行時コントリビューション。 |
| `AfterLoadedAsync(ILifetimeScope lifetimeScope)` | すべてのモジュールがロードされた後に必要なクロスモジュール処理。 |
| `RegisterCommandLineOption(...)` | モジュールが所有する CLI フラグ。 |

多くの UI モジュールでは `LoadAsync` だけで十分です。

## URI タブ

URI タブは機能面を公開する主要な方法です。`ITabItemFactory` を登録します。通常は `ILifetimeScope` のヘルパーを使います。

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterTabItemFactory<ReportsTabItemFactory>();
    return Task.CompletedTask;
}
```

ルートを URI 部品で表せる場合は `TabItemRouteAttribute` を使います。サービス、ファイル種別判定、より複雑なポリシーが必要な場合は `CheckUriMatchedAsync` をオーバーライドします。

専用のルート モデルを持たず、1 つの `UserControl` を開くだけの小さなケースでは `RegisterSimpleTabItemFactory(...)` を使います。

## メインメニュー

root メニューには File、View、Tools、Extensions、About の組み込み provider があります。モジュール メニュー項目は通常、そのどれかへ登録します。

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterExtensionsMainMenuItem<ReportsMainMenuItem>();
    return Task.CompletedTask;
}
```

メニュー順はまず `Anchor`、同じ anchor 内で `Priority` によって決まります。`MainMenuManager` は anchor グループ間にセパレーターを挿入し、子項目も再帰的にソートします。

複数の子コマンドを持つ親メニューが必要な場合だけ、モジュール専用の `IMainMenuItemsProvider` を作ります。1 つのコマンドなら、既存 provider に `IMainMenuItem` を登録します。

## ワークスペース メニュー

ワークスペース番号の横にある表示中のドロップダウンへコマンドを出す場合は `IWorkspaceMenuManager` を使います。

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.Resolve<IWorkspaceMenuManager>()
        .RegisterItem<ReportsWorkspaceMenuItem>();

    return Task.CompletedTask;
}
```

`IWorkspaceMenuItem` は `Title`、`Command`、`SubItems`、`Icon`、`Anchor`、`Priority`、`Localization` を持ちます。現在の `WorkspaceMenuView` は `IWorkspaceMenuManager.GetItems()` を読みます。

`IWorkspaceContextMenuManager` は別の manager で、`Anchor` と `Priority` による再帰ソートを行います。コンテキストメニュー View が接続されていない限り、その項目が表示されるとは扱わないでください。

## タブ ヘッダー コンテキストメニュー

タブ ヘッダー メニュー項目は `ITabItemHeaderContextMenuItemView` として登録される WPF メニュー項目 View です。

```csharp
[RegisterAs(typeof(ITabItemHeaderContextMenuItemView))]
internal partial class ReportsTabHeaderMenuItem :
    ITabItemHeaderContextMenuItemView
{
    public int Order => 20;
}
```

`TabItemHeaderContextMenuItemsResolver` は登録済み View をすべて解決し、`Order` で並べます。WPF のコンテキストメニューは late-bound なので、現在のタブ インスタンスが必要な項目では command parameter と既存の `ContextMenuItemBase` パターンを優先します。

## ステータスバー

ステータスバー拡張は `IStatusBarItemsProvider` を提供します。Provider は 1 つ以上の `IStatusBarItem` を返します。`StatusBarManager` は登録済み provider を集約し、`Order` で並べます。

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.Resolve<IStatusBarManager>()
        .RegisterStatusBarItemsProvider<ReportsStatusBarItemsProvider>();

    return Task.CompletedTask;
}
```

各 item は `StatusBarSection.Left` または `StatusBarSection.Right` で表示側を選びます。

## タスクバー メニュー

タスクバー メニュー項目は `ITaskbarMenuItem` を実装し、`ITaskbarMenuManager` に登録します。

```csharp
lifetimeScope.Resolve<ITaskbarMenuManager>()
    .RegisterMenuItem(lifetimeScope.Resolve<ReportsTaskbarMenuItem>());
```

タスクバー メニューは `Info.Anchor` でグループ化し、`Info.Priority` で並べ、子項目も再帰的にソートします。この面は tray/window レベルのコマンドに使い、機能ナビゲーションはメインメニューへ置きます。

## 設定と状態

`IConfig` または `IState` を実装する具象型は、モジュール アセンブリ登録時に settings ディレクトリから読み込まれ、Autofac に登録されます。小さな JSON シリアライズ可能な設定と状態に使います。

ガイドライン:

- ユーザーが編集するオプションは `IConfig` に置く。
- 実行時の永続化は `IState` に置く。
- 型は小さく、バージョン変更に強くする。
- config/state を大きな業務データ ストアとして使わない。

## イベントと Toast

疎結合な実行時通知には `IEventAggregator` を使います。

```csharp
lifetimeScope.PublishEvent(new ReportsChangedEvent());
lifetimeScope.SubscribeEvent<ReportsChangedEvent>(OnReportsChanged, onUiThread: true);
```

ユーザー向けの一時的なフィードバックには `IToastManager` を使います。

```csharp
toastManager.PromptMessage(ToastMessage.Info("Report exported.", localization: false));
toastManager.PromptException(exception);
```

イベントは調整用、Toast は表示フィードバック用です。Toast メッセージを制御フローとして使わないでください。

## Drag and Drop

Drag/drop 操作は `IDropActionProvider` から提供します。Orchestrator はすべての provider に対応する `DropAction` を問い合わせ、`CanRun()` で絞り込み、`Id` で重複排除し、既定アクションを実行するか picker を表示します。

モジュールが workspace-aware な形でドロップされたファイル、パス、タブ payload を処理したい場合に使います。`DropContext` には target object、workspace id、modifier keys、screen point、cancellation token が含まれます。

## Aspire Resources

Aspire 拡張モジュールは `IExtensionResourcesProvider` を登録します。`AspireService.Build(...)` はすべての provider を解決し、それぞれの `ConfigureResources(builder)` を呼びます。

コマンドライン子サービスでは `ICommandlineResourcesProvider` から登録します。

```csharp
lifetimeScope.Resolve<ICommandlineResourcesProvider>()
    .Register(new CommandlineServiceOptions
    {
        Name = "reports-worker",
        WorkDirectory = workerDirectory,
        Command = "dotnet run"
    });
```

大きなアプリ内 UI ではなく、Aspire Host に起動させる sidecar service に使います。

## CLI オプション

CLI はモジュールをロードし、root command を確定する前に `RegisterCommandLineOption(container, optionRegister)` を呼びます。モジュールが自分の command-line switch を必要とするときに使います。

完全なスキャフォールド コマンドでは、無関係なフラグに詰め込まず、組み込みの `zyc new` / `zyc new-module` のような明示的 subcommand を使います。

## 選択ガイド

| 目的 | 使うもの |
| --- | --- |
| 機能 View を開く | URI + `ITabItemFactory` |
| アプリ コマンドを 1 つ追加する | 既存 main menu provider |
| 1 つの親の下に複数コマンドを追加する | モジュール専用 `IMainMenuItemsProvider` |
| ワークスペース操作を追加する | `IWorkspaceMenuManager` |
| タブ ヘッダー操作を追加する | `ITabItemHeaderContextMenuItemView` |
| 軽量な実行時状態を表示する | Status bar provider |
| Tray/window コマンドを追加する | `ITaskbarMenuManager` |
| 小さな設定や状態を永続化する | `IConfig` / `IState` |
| 実行時動作を調整する | `IEventAggregator` |
| ユーザーへフィードバックを出す | `IToastManager` |
| ドロップ ファイル動作を追加する | `IDropActionProvider` |
| sidecar service を起動する | Aspire resource provider |

<!--doc-l10n:locale zh-CN-->
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

<!--doc-l10n:locale zh-TW-->
# 擴展點

ZYC.Framework 的擴展大多透過模組與 Autofac 註冊。模組隨 Host 載入，註冊服務或 UI 貢獻；隨後 Shell 從這些註冊中組合選單、Tab、工作區操作、狀態列項目、工作列選單項目、拖放動作與 Aspire 資源。

## 擴展點地圖

| 擴展點 | 註冊位置 | 執行階段消費方 |
| --- | --- | --- |
| 模組生命週期 | `ModuleBase.RegisterAsync`, `LoadAsync`, `AfterLoadedAsync` | Host 啟動與模組載入器。 |
| URI Tab | `ITabItemFactoryManager.RegisterFactory<T>()` | `TabManager.InternalNavigateAsync(...)`。 |
| 簡單 View Tab | `ISimpleTabItemFactoryManager.Register(...)` | 內建 `SimpleTabItemFactory`。 |
| 主選單 | `IMainMenuManager`, `IMainMenuItemsProvider` | `MainMenuManager` 與主選單 View。 |
| 工作區選單 | `IWorkspaceMenuManager` | `WorkspaceMenuView`。 |
| 工作區內容選單 Manager | `IWorkspaceContextMenuManager` | Manager 存在並提供排序；在接線內容選單表面時使用。 |
| Tab 標頭右鍵選單 | `ITabItemHeaderContextMenuItemView` | `TabItemHeaderContextMenuItemsResolver`。 |
| 狀態列 | `IStatusBarManager`, `IStatusBarItemsProvider` | `StatusBarManager`。 |
| 工作列選單 | `ITaskbarMenuManager` | `TaskbarContextMenu`。 |
| 設定/狀態 | `IConfig`, `IState` | `ModuleTools.RegisterAllFromAssembly(...)`。 |
| 事件 | `IEventAggregator` | 執行階段發布/訂閱匯流排。 |
| Toast | `IToastManager`, `IToast` | Toast 彈出宿主。 |
| 拖放 | `IDropActionProvider` | `DropOrchestrator`。 |
| Aspire 資源 | `IExtensionResourcesProvider` | `AspireService.Build(...)`。 |
| CLI 選項 | `ModuleBase.RegisterCommandLineOption(...)` | `ZYC.Framework.CLI` 根命令。 |

## 模組生命週期

用模組生命週期決定每類註冊放在哪裡：

| 方法 | 用途 |
| --- | --- |
| `RegisterAsync(ContainerBuilder builder)` | 必須在容器建構前完成的 Autofac 註冊。 |
| `LoadAsync(ILifetimeScope lifetimeScope)` | 執行階段貢獻，例如 Tab factory、選單項目、狀態列 Provider、Aspire 資源註冊。 |
| `AfterLoadedAsync(ILifetimeScope lifetimeScope)` | 需要所有模組都載入完之後再執行的跨模組工作。 |
| `RegisterCommandLineOption(...)` | 模組擁有的 CLI 參數。 |

大多數 UI 模組只需要 `LoadAsync`。

## URI Tab

URI Tab 是暴露功能表面的主要方式。註冊一個 `ITabItemFactory`，通常使用 `ILifetimeScope` 上的輔助方法：

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterTabItemFactory<ReportsTabItemFactory>();
    return Task.CompletedTask;
}
```

當路由可以用 URI 部件表達時，使用 `TabItemRouteAttribute`。當判斷需要服務、檔案類型檢查或更複雜策略時，覆寫 `CheckUriMatchedAsync`。

只有在很小的單 View 場景中，才使用 `RegisterSimpleTabItemFactory(...)`，也就是一個 `UserControl` 可以直接開啟，不需要專門的路由模型。

## 主選單

根選單內建 File、View、Tools、Extensions 與 About Provider。模組選單項目通常註冊到其中一個 Provider 下：

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterExtensionsMainMenuItem<ReportsMainMenuItem>();
    return Task.CompletedTask;
}
```

選單排序先依 `Anchor`，再依同一組內的 `Priority`。`MainMenuManager` 會在 Anchor 組之間插入分隔符，並遞迴排序子項。

只有當模組需要一個包含多個子命令的父選單時，才建立模組自己的 `IMainMenuItemsProvider`。如果只有一個命令，把 `IMainMenuItem` 註冊到既有 Provider 即可。

## 工作區選單

如果命令需要出現在工作區編號旁邊的可見下拉選單中，使用 `IWorkspaceMenuManager`：

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.Resolve<IWorkspaceMenuManager>()
        .RegisterItem<ReportsWorkspaceMenuItem>();

    return Task.CompletedTask;
}
```

`IWorkspaceMenuItem` 支援 `Title`、`Command`、`SubItems`、`Icon`、`Anchor`、`Priority` 與 `Localization`。目前可見的 `WorkspaceMenuView` 讀取 `IWorkspaceMenuManager.GetItems()`。

`IWorkspaceContextMenuManager` 是另一個 Manager，它依 `Anchor` 和 `Priority` 遞迴排序。除非已經有內容選單 View 接入它，否則不要假設它的項目會顯示出來。

## Tab 標頭右鍵選單

Tab 標頭選單項目是註冊為 `ITabItemHeaderContextMenuItemView` 的 WPF 選單項目 View：

```csharp
[RegisterAs(typeof(ITabItemHeaderContextMenuItemView))]
internal partial class ReportsTabHeaderMenuItem :
    ITabItemHeaderContextMenuItemView
{
    public int Order => 20;
}
```

`TabItemHeaderContextMenuItemsResolver` 會解析所有已註冊 View，並依 `Order` 排序。由於 WPF ContextMenu 是 late-bound，如果選單項目需要目前 Tab 實例，優先使用 command parameter 和既有的 `ContextMenuItemBase` 模式。

## 狀態列

狀態列擴展貢獻一個 `IStatusBarItemsProvider`；Provider 回傳一個或多個 `IStatusBarItem`。`StatusBarManager` 彙總所有已註冊 Provider，並依 `Order` 排序。

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.Resolve<IStatusBarManager>()
        .RegisterStatusBarItemsProvider<ReportsStatusBarItemsProvider>();

    return Task.CompletedTask;
}
```

每個 Item 透過 `StatusBarSection.Left` 或 `StatusBarSection.Right` 選擇顯示側。

## 工作列選單

工作列選單項目實作 `ITaskbarMenuItem`，並註冊到 `ITaskbarMenuManager`：

```csharp
lifetimeScope.Resolve<ITaskbarMenuManager>()
    .RegisterMenuItem(lifetimeScope.Resolve<ReportsTaskbarMenuItem>());
```

工作列選單依 `Info.Anchor` 分組，依 `Info.Priority` 排序，並遞迴排序子項。這個表面適合托盤/視窗級命令，不適合本應放進主選單的功能導覽。

## 設定與狀態

任何實作 `IConfig` 或 `IState` 的具體型別，都會在模組組件註冊期間從 settings 目錄載入，並註冊到 Autofac。它適合小型、可 JSON 序列化的設定與狀態。

準則：

- 使用者可編輯選項放在 `IConfig`。
- 執行階段持久化放在 `IState`。
- 型別保持小型，並能承受版本演進。
- 不要把 config/state 當成大型業務資料儲存。

## 事件與 Toast

解耦的執行階段通知使用 `IEventAggregator`：

```csharp
lifetimeScope.PublishEvent(new ReportsChangedEvent());
lifetimeScope.SubscribeEvent<ReportsChangedEvent>(OnReportsChanged, onUiThread: true);
```

使用者可見的暫時回饋使用 `IToastManager`：

```csharp
toastManager.PromptMessage(ToastMessage.Info("Report exported.", localization: false));
toastManager.PromptException(exception);
```

事件用於協調，Toast 用於可見回饋。不要把 Toast 訊息當成控制流程。

## 拖放

拖放動作透過 `IDropActionProvider` 貢獻。Orchestrator 會向所有 Provider 詢問相容的 `DropAction`，用 `CanRun()` 過濾，依 `Id` 去重，然後執行預設動作或顯示選擇器。

當模組需要以工作區感知的方式處理拖入的檔案、路徑或 Tab 負載時，使用這個擴展點。`DropContext` 包含目標物件、工作區 id、修飾鍵、螢幕座標與取消權杖。

## Aspire 資源

Aspire 擴展模組註冊 `IExtensionResourcesProvider`。`AspireService.Build(...)` 會解析所有 Provider，並呼叫每個 Provider 的 `ConfigureResources(builder)`。

對於命令列子服務，透過 `ICommandlineResourcesProvider` 註冊：

```csharp
lifetimeScope.Resolve<ICommandlineResourcesProvider>()
    .Register(new CommandlineServiceOptions
    {
        Name = "reports-worker",
        WorkDirectory = workerDirectory,
        Command = "dotnet run"
    });
```

它適合由 Aspire Host 拉起的 sidecar 服務，而不是大型應用內 UI。

## CLI 選項

CLI 會載入模組，並在最終確定根命令前呼叫 `RegisterCommandLineOption(container, optionRegister)`。當模組需要自己的命令列開關時使用它。

如果是完整鷹架命令，使用類似內建 `zyc new` 和 `zyc new-module` 的明確子命令，不要把無關含義塞進普通 flag。

## 如何選擇擴展面

| 目標 | 使用 |
| --- | --- |
| 開啟功能 View | URI + `ITabItemFactory` |
| 新增一個頂層應用命令 | 既有主選單 Provider |
| 在一個模組父級下新增多個命令 | 模組自有 `IMainMenuItemsProvider` |
| 新增工作區操作 | `IWorkspaceMenuManager` |
| 新增 Tab 標頭動作 | `ITabItemHeaderContextMenuItemView` |
| 顯示輕量執行階段狀態 | Status bar provider |
| 新增托盤/視窗命令 | `ITaskbarMenuManager` |
| 持久化小型設定或狀態 | `IConfig` / `IState` |
| 協調執行階段行為 | `IEventAggregator` |
| 顯示使用者回饋 | `IToastManager` |
| 新增拖入檔案行為 | `IDropActionProvider` |
| 啟動 sidecar 服務 | Aspire resource provider |

<!--doc-l10n:locale ko-->
# 확장 지점

ZYC.Framework 확장은 대부분 모듈과 Autofac을 통해 등록됩니다. 모듈은 Host와 함께 로드되고 서비스 또는 UI 기여를 등록합니다. 그런 다음 Shell은 해당 등록에서 메뉴, 탭, 워크스페이스 작업, 상태 표시줄 항목, 작업 표시줄 메뉴 항목, 드래그 앤 드롭 동작, Aspire 리소스를 구성합니다.

## 확장 지점 맵

| 확장 지점 | 등록 위치 | 런타임 소비자 |
| --- | --- | --- |
| 모듈 라이프사이클 | `ModuleBase.RegisterAsync`, `LoadAsync`, `AfterLoadedAsync` | Host 시작과 모듈 로더. |
| URI 탭 | `ITabItemFactoryManager.RegisterFactory<T>()` | `TabManager.InternalNavigateAsync(...)`. |
| 단순 뷰 탭 | `ISimpleTabItemFactoryManager.Register(...)` | 내장 `SimpleTabItemFactory`. |
| 메인 메뉴 | `IMainMenuManager`, `IMainMenuItemsProvider` | `MainMenuManager`와 메인 메뉴 뷰. |
| 워크스페이스 메뉴 | `IWorkspaceMenuManager` | `WorkspaceMenuView`. |
| 워크스페이스 컨텍스트 메뉴 manager | `IWorkspaceContextMenuManager` | Manager는 존재하며 항목을 정렬합니다. 컨텍스트 메뉴 표면을 연결할 때 사용합니다. |
| 탭 헤더 컨텍스트 메뉴 | `ITabItemHeaderContextMenuItemView` | `TabItemHeaderContextMenuItemsResolver`. |
| 상태 표시줄 | `IStatusBarManager`, `IStatusBarItemsProvider` | `StatusBarManager`. |
| 작업 표시줄 메뉴 | `ITaskbarMenuManager` | `TaskbarContextMenu`. |
| 구성/상태 | `IConfig`, `IState` | `ModuleTools.RegisterAllFromAssembly(...)`. |
| 이벤트 | `IEventAggregator` | 런타임 publish/subscribe bus. |
| Toast | `IToastManager`, `IToast` | Toast popup host. |
| 드래그 앤 드롭 | `IDropActionProvider` | `DropOrchestrator`. |
| Aspire 리소스 | `IExtensionResourcesProvider` | `AspireService.Build(...)`. |
| CLI 옵션 | `ModuleBase.RegisterCommandLineOption(...)` | `ZYC.Framework.CLI` root command. |

## 모듈 라이프사이클

각 등록 위치는 모듈 라이프사이클에 맞춰 결정합니다.

| 메서드 | 용도 |
| --- | --- |
| `RegisterAsync(ContainerBuilder builder)` | 컨테이너가 만들어지기 전에 필요한 Autofac 등록. |
| `LoadAsync(ILifetimeScope lifetimeScope)` | 탭 팩터리, 메뉴 항목, 상태 표시줄 provider, Aspire 리소스 등록 같은 런타임 기여. |
| `AfterLoadedAsync(ILifetimeScope lifetimeScope)` | 모든 모듈이 로드된 뒤 필요한 교차 모듈 작업. |
| `RegisterCommandLineOption(...)` | 모듈이 소유한 CLI 플래그. |

대부분의 UI 모듈은 `LoadAsync`만 필요합니다.

## URI 탭

URI 탭은 기능 표면을 노출하는 주요 방법입니다. 보통 `ILifetimeScope` helper로 `ITabItemFactory`를 등록합니다.

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterTabItemFactory<ReportsTabItemFactory>();
    return Task.CompletedTask;
}
```

라우트를 URI 부분으로 설명할 수 있으면 `TabItemRouteAttribute`를 사용합니다. 서비스, 파일 형식 검사, 더 복잡한 정책이 필요하면 `CheckUriMatchedAsync`를 재정의합니다.

전용 라우트 모델 없이 하나의 `UserControl`만 열면 되는 작은 단일 뷰 사례에서는 `RegisterSimpleTabItemFactory(...)`를 사용합니다.

## 메인 메뉴

루트 메뉴에는 File, View, Tools, Extensions, About 내장 provider가 있습니다. 모듈 메뉴 항목은 보통 그중 하나 아래에 등록합니다.

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterExtensionsMainMenuItem<ReportsMainMenuItem>();
    return Task.CompletedTask;
}
```

메뉴 정렬은 먼저 `Anchor`, 같은 anchor 그룹 안에서는 `Priority`로 결정됩니다. `MainMenuManager`는 anchor 그룹 사이에 separator를 넣고 하위 항목도 재귀적으로 정렬합니다.

여러 하위 명령을 가진 부모 메뉴가 필요할 때만 모듈 소유 `IMainMenuItemsProvider`를 만듭니다. 명령이 하나라면 기존 provider 아래에 `IMainMenuItem`을 등록합니다.

## 워크스페이스 메뉴

워크스페이스 번호 옆의 표시되는 드롭다운에 명령을 넣으려면 `IWorkspaceMenuManager`를 사용합니다.

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.Resolve<IWorkspaceMenuManager>()
        .RegisterItem<ReportsWorkspaceMenuItem>();

    return Task.CompletedTask;
}
```

`IWorkspaceMenuItem`은 `Title`, `Command`, `SubItems`, `Icon`, `Anchor`, `Priority`, `Localization`을 지원합니다. 현재 표시되는 `WorkspaceMenuView`는 `IWorkspaceMenuManager.GetItems()`를 읽습니다.

`IWorkspaceContextMenuManager`는 별도의 manager이며 `Anchor`와 `Priority`로 재귀 정렬합니다. 컨텍스트 메뉴 뷰가 연결되어 있지 않다면 그 항목이 표시된다고 가정하지 마세요.

## 탭 헤더 컨텍스트 메뉴

탭 헤더 메뉴 항목은 `ITabItemHeaderContextMenuItemView`로 등록되는 WPF 메뉴 항목 뷰입니다.

```csharp
[RegisterAs(typeof(ITabItemHeaderContextMenuItemView))]
internal partial class ReportsTabHeaderMenuItem :
    ITabItemHeaderContextMenuItemView
{
    public int Order => 20;
}
```

`TabItemHeaderContextMenuItemsResolver`는 등록된 모든 뷰를 resolve하고 `Order`로 정렬합니다. WPF ContextMenu는 late-bound이므로 현재 탭 인스턴스가 필요한 항목에서는 command parameter와 기존 `ContextMenuItemBase` 패턴을 우선 사용하세요.

## 상태 표시줄

상태 표시줄 확장은 `IStatusBarItemsProvider`를 제공합니다. Provider는 하나 이상의 `IStatusBarItem`을 반환합니다. `StatusBarManager`는 등록된 모든 provider를 모으고 `Order`로 정렬합니다.

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.Resolve<IStatusBarManager>()
        .RegisterStatusBarItemsProvider<ReportsStatusBarItemsProvider>();

    return Task.CompletedTask;
}
```

각 항목은 `StatusBarSection.Left` 또는 `StatusBarSection.Right`로 표시 위치를 선택합니다.

## 작업 표시줄 메뉴

작업 표시줄 메뉴 항목은 `ITaskbarMenuItem`을 구현하고 `ITaskbarMenuManager`에 등록합니다.

```csharp
lifetimeScope.Resolve<ITaskbarMenuManager>()
    .RegisterMenuItem(lifetimeScope.Resolve<ReportsTaskbarMenuItem>());
```

작업 표시줄 메뉴는 `Info.Anchor`로 그룹화하고 `Info.Priority`로 정렬하며 하위 항목도 재귀적으로 정렬합니다. 이 표면은 tray/window 수준 명령에 사용하고, 기능 내비게이션은 메인 메뉴에 둡니다.

## 구성과 상태

`IConfig` 또는 `IState`를 구현하는 모든 concrete 타입은 모듈 어셈블리 등록 중 settings 디렉터리에서 로드되고 Autofac에 등록됩니다. 작은 JSON 직렬화 가능 설정과 상태에 사용합니다.

지침:

- 사용자가 편집하는 옵션은 `IConfig`에 둡니다.
- 런타임 지속성은 `IState`에 둡니다.
- 타입은 작고 버전 변화에 견딜 수 있게 유지합니다.
- config/state를 큰 비즈니스 데이터 저장소로 사용하지 마세요.

## 이벤트와 Toast

분리된 런타임 알림에는 `IEventAggregator`를 사용합니다.

```csharp
lifetimeScope.PublishEvent(new ReportsChangedEvent());
lifetimeScope.SubscribeEvent<ReportsChangedEvent>(OnReportsChanged, onUiThread: true);
```

사용자에게 보이는 임시 피드백에는 `IToastManager`를 사용합니다.

```csharp
toastManager.PromptMessage(ToastMessage.Info("Report exported.", localization: false));
toastManager.PromptException(exception);
```

이벤트는 조정용이고 Toast는 표시 피드백용입니다. Toast 메시지를 제어 흐름으로 사용하지 마세요.

## 드래그 앤 드롭

드래그 앤 드롭 동작은 `IDropActionProvider`로 기여합니다. Orchestrator는 모든 provider에 호환되는 `DropAction`을 요청하고, `CanRun()`으로 필터링하며, `Id`로 중복을 제거한 뒤 기본 동작을 실행하거나 picker를 표시합니다.

모듈이 dropped file, path, tab payload를 워크스페이스 인식 방식으로 처리해야 할 때 사용합니다. `DropContext`에는 target object, workspace id, modifier keys, screen point, cancellation token이 포함됩니다.

## Aspire 리소스

Aspire 확장 모듈은 `IExtensionResourcesProvider`를 등록합니다. `AspireService.Build(...)`는 모든 provider를 resolve하고 각 provider의 `ConfigureResources(builder)`를 호출합니다.

명령줄 child service는 `ICommandlineResourcesProvider`로 등록합니다.

```csharp
lifetimeScope.Resolve<ICommandlineResourcesProvider>()
    .Register(new CommandlineServiceOptions
    {
        Name = "reports-worker",
        WorkDirectory = workerDirectory,
        Command = "dotnet run"
    });
```

큰 인앱 UI가 아니라 Aspire Host가 시작해야 하는 sidecar service에 사용합니다.

## CLI 옵션

CLI는 모듈을 로드하고 root command를 확정하기 전에 `RegisterCommandLineOption(container, optionRegister)`를 호출합니다. 모듈이 자체 command-line switch를 필요로 할 때 사용합니다.

전체 스캐폴딩 명령은 관련 없는 flag에 끼워 넣지 말고, 내장 `zyc new`, `zyc new-module` 패턴처럼 명시적 subcommand를 사용합니다.

## 올바른 표면 선택

| 목표 | 사용 |
| --- | --- |
| 기능 뷰 열기 | URI + `ITabItemFactory` |
| 앱 명령 하나 추가 | 기존 main menu provider |
| 하나의 모듈 부모 아래 여러 명령 추가 | 모듈 소유 `IMainMenuItemsProvider` |
| 워크스페이스 작업 추가 | `IWorkspaceMenuManager` |
| 탭 헤더 동작 추가 | `ITabItemHeaderContextMenuItemView` |
| 가벼운 런타임 상태 표시 | Status bar provider |
| Tray/window 명령 추가 | `ITaskbarMenuManager` |
| 작은 설정 또는 상태 지속화 | `IConfig` / `IState` |
| 런타임 동작 조정 | `IEventAggregator` |
| 사용자 피드백 표시 | `IToastManager` |
| dropped-file 동작 추가 | `IDropActionProvider` |
| sidecar service 시작 | Aspire resource provider |

<!--doc-l10n:end-->
