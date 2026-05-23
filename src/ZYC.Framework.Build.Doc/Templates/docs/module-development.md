<p align="center">
  <a href="./module-development.md">English</a> |
  <a href="./module-development.ja.md">日本語</a> |
  <a href="./module-development.zh-CN.md">简体中文</a> |
  <a href="./module-development.zh-TW.md">繁體中文</a> |
  <a href="./module-development.ko.md">한국어</a> |
</p>

<!--doc-l10n:begin module-development-content-->
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

<!--doc-l10n:locale ja-->
# モジュール開発

このガイドでは、ZYC.Framework Host の実行時モデルに合うモジュールの作り方を説明します。基本は、依存性注入を入口にし、URI ベースでタブへ遷移し、必要に応じてメインメニューへ統合し、公開コントラクトと WPF 実装を分離することです。

## モジュールを作るタイミング

シェルに直接組み込むのではなく、ZYC.Framework Host に発見・ロードさせたい機能はモジュールにします。モジュールはビュー、タブ ファクトリ、メインメニュー項目、設定、状態、バックグラウンド サービス、コマンドライン オプションを提供できます。

再利用されるコントラクトがある場合は `*.Abstractions` プロジェクトを作成します。公開 DTO、定数、サービス インターフェイス、メニュー プロバイダー インターフェイスはそこに置きます。WPF ビュー、タブ項目、実行時登録は実装プロジェクトに置きます。

## 推奨プロジェクト構成

| 領域 | 代表的なファイル | 役割 |
| --- | --- | --- |
| Abstractions | `*ModuleConstants.cs`, `I*MainMenuItemsProvider.cs`, service interfaces | 他モジュールが WPF 実装へ依存せず参照できる公開コントラクト。 |
| Implementation | `Module.cs` | モジュールのライフサイクル入口。ファクトリ、メニュー項目、プロバイダー、サービスを登録する。 |
| Navigation | `*TabItemFactory.cs`, `*TabItem.cs` | `zyc://` または app URI を判定し、タブ インスタンスを作成する。 |
| Menu | `*MainMenuItem.cs`, optional `*MainMenuItemsProvider.cs` | モジュールのタブへ遷移する、または処理を実行するユーザー向けコマンドを追加する。 |
| UI | `UI/*View.xaml`, `UI/*View.xaml.cs` | タブ項目で使う WPF ビュー。 |

Abstractions プロジェクトでは WPF コントロール型を公開しないでください。コマンド契約が必要な場合、`System.Windows.Input.ICommand` は Abstractions で使えます。

## ライフサイクル

| メソッド | タイミング | 主な用途 |
| --- | --- | --- |
| `RegisterAsync(ContainerBuilder builder)` | Autofac コンテナー構築前。 | コンテナー全体で必要になるサービス登録。 |
| `LoadAsync(ILifetimeScope lifetimeScope)` | コンテナー構築後、かつモジュールが有効なとき。 | タブ ファクトリ、メインメニュー項目、ステータスバー項目、実行時フックの登録。 |
| `AfterLoadedAsync(ILifetimeScope lifetimeScope)` | 有効なすべてのモジュールがロードされた後。 | 他モジュールの登録が完了していることを前提にした初期化。 |

多くの UI モジュールでは `LoadAsync` だけで十分です。

## 最小構成のビュー モジュール

単一ビューのモジュールでは、`LoadAsync` で simple tab factory を登録します。

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

これは `minimal` プロジェクト テンプレートで使われる形です。1 つの WPF `UserControl` をタブとして公開するだけなら、この形で足ります。

## ルーティング付きタブ モジュール

安定した URI、ルート判定、パラメーター、独自のシングルトン制御、複数タブが必要な場合は `TabItemFactoryBase` を使います。

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

`TabItemRouteAttribute` は `Scheme`、`Host`、`Path`、`PathMatch` で URI を判定できます。同じ URI に複数のファクトリが一致した場合は、`Priority` が高いファクトリが選ばれます。`TabItemFactoryBase` の既定は `IsSingle = true` です。複数インスタンスを許可したい場合はオーバーライドしてください。

## タブ項目とビュー

ルーティング付きタブでは、通常 `TabItemInstanceBase<TView>` で WPF ビューをラップします。

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

タブのライフタイムはタブ項目に、見た目の振る舞いはビューに置きます。これにより、ルーティング、タブ ID、UI 構成を分離できます。

## メインメニュー項目

メニュー項目は通常、モジュール URI へ遷移します。

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

モジュールから登録します。

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterTabItemFactory<ReportsTabItemFactory>();
    lifetimeScope.RegisterExtensionsMainMenuItem<ReportsMainMenuItem>();

    return Task.CompletedTask;
}
```

機能の配置に合う既存のメニュー プロバイダーを使ってください。File、View、Tools、Extensions、About などがあります。複数の子コマンドを持つ機能だけ、モジュール専用のプロバイダーを追加します。

## 設定と状態

`IConfig` または `IState` を実装した型は、モジュール登録中に settings ディレクトリから読み込まれ、Autofac に自動登録されます。ユーザーが編集する設定には config を、選択パス、保留中の操作、ウィンドウ状態などの実行時永続化には state を使います。

大きな業務データを config/state クラスへ入れないでください。小さく、シリアライズ可能で、バージョン変更に耐えやすい形にします。

## モジュール ロードと依存関係

Host は `ZYC.Framework.Modules*.dll` のような標準モジュール アセンブリを発見し、さらに `ModuleConfig.AdditionalAssemblyNames` に列挙されたアセンブリを追加します。`ModuleConfig.DisabledAssemblyNames` に含まれるアセンブリは発見されますが、有効なモジュールとしてはロードされません。

依存関係は、他モジュールの `*.Abstractions.dll` への参照から推定されます。モジュール A が `ZYC.Framework.Modules.B.Abstractions.dll` を参照していれば、A が B に依存することを実行時に報告できます。その際、A が B の WPF 実装へ直接依存する必要はありません。

## チェックリスト

- 公開定数とコントラクトは `*.Abstractions` に置く。
- WPF ビューとタブ項目は実装プロジェクトに置く。
- コンテナー構築前に必要なサービスだけ `RegisterAsync` で登録する。
- タブ ファクトリとメニュー項目は `LoadAsync` で登録する。
- 安定した URI ルーティングには `TabItemRouteAttribute` を使う。
- 新しいプロバイダーを追加する前に、既存のメインメニュー プロバイダーを優先する。
- config/state クラスは小さく、シリアライズ可能に保つ。

<!--doc-l10n:locale zh-CN-->
# 模块开发

这篇文档说明如何编写一个符合 ZYC.Framework Host 运行时模型的模块：以依赖注入为入口，使用 URI 驱动 Tab 导航，按需接入主菜单，并清晰分离公开契约与 WPF 实现。

## 什么时候创建模块

当一个功能需要由 ZYC.Framework Host 发现并加载，而不是直接编译进 Shell 时，就应该做成模块。模块可以贡献视图、Tab 工厂、主菜单项、配置、状态、后台服务和命令行选项。

如果功能契约需要被其他模块复用，请创建 `*.Abstractions` 项目。公开 DTO、常量、服务接口、菜单 Provider 接口放在这里。WPF 视图、TabItem 实现和运行时注册逻辑放在实现项目。

## 推荐项目结构

| 区域 | 常见文件 | 职责 |
| --- | --- | --- |
| Abstractions | `*ModuleConstants.cs`, `I*MainMenuItemsProvider.cs`, service interfaces | 其他模块可以引用的公开契约，不依赖 WPF 实现细节。 |
| Implementation | `Module.cs` | 模块生命周期入口。注册工厂、菜单项、Provider 和服务。 |
| Navigation | `*TabItemFactory.cs`, `*TabItem.cs` | 匹配 `zyc://` 或 app URI，并创建 Tab 实例。 |
| Menu | `*MainMenuItem.cs`, optional `*MainMenuItemsProvider.cs` | 添加用户可见命令，用于导航到模块 Tab 或执行模块动作。 |
| UI | `UI/*View.xaml`, `UI/*View.xaml.cs` | TabItem 使用的 WPF 视图。 |

Abstractions 项目不要暴露 WPF 控件类型。如果需要命令契约，可以在 Abstractions 中使用 `System.Windows.Input.ICommand`。

## 生命周期

| 方法 | 时机 | 用途 |
| --- | --- | --- |
| `RegisterAsync(ContainerBuilder builder)` | Autofac 容器构建之前。 | 注册必须在整个容器中可用的服务。 |
| `LoadAsync(ILifetimeScope lifetimeScope)` | 容器构建之后，并且模块处于启用状态时。 | 注册 Tab 工厂、主菜单项、状态栏项和运行时 Hook。 |
| `AfterLoadedAsync(ILifetimeScope lifetimeScope)` | 所有启用模块加载完成之后。 | 需要依赖其他模块已经完成注册的跨模块初始化。 |

大多数 UI 模块只需要实现 `LoadAsync`。

## 最小视图模块

对于单视图模块，可以在 `LoadAsync` 中注册 simple tab factory：

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

这是 `minimal` 项目模板使用的模式。当模块只需要把一个 WPF `UserControl` 暴露为 Tab 时，这种方式就足够了。

## 带路由的 Tab 模块

如果模块需要稳定 URI、路由匹配、参数、定制单例行为或多个 Tab，请使用 `TabItemFactoryBase`。

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

`TabItemRouteAttribute` 可以按 `Scheme`、`Host`、`Path` 和 `PathMatch` 匹配 URI。如果多个工厂匹配同一个 URI，`Priority` 更高的工厂会优先。`TabItemFactoryBase` 默认 `IsSingle = true`；如果模块允许打开多个实例，请覆盖它。

## TabItem 与 View

带路由的 Tab 通常用 `TabItemInstanceBase<TView>` 包装 WPF 视图：

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

Tab 生命周期行为放在 TabItem 中，视觉行为放在 View 中。这样可以把路由、Tab 标识和 UI 组合拆开。

## 主菜单入口

菜单项通常导航到模块 URI：

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

然后从模块注册：

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterTabItemFactory<ReportsTabItemFactory>();
    lifetimeScope.RegisterExtensionsMainMenuItem<ReportsMainMenuItem>();

    return Task.CompletedTask;
}
```

优先使用符合功能位置的现有菜单 Provider，例如 File、View、Tools、Extensions 或 About。只有当功能拥有多个子命令时，才增加模块自己的 Provider。

## 配置与状态

实现 `IConfig` 或 `IState` 的类型会在模块注册期间从 settings 目录读取，并自动注册到 Autofac。用户可编辑设置使用 config；运行时持久化内容，例如选中路径、待处理操作、窗口状态，使用 state。

不要把大型业务数据放进 config/state 类。它们应保持小型、可序列化，并且能承受版本演进。

## 模块加载与依赖

Host 会发现命名类似 `ZYC.Framework.Modules*.dll` 的标准模块程序集，然后追加 `ModuleConfig.AdditionalAssemblyNames` 中列出的程序集。`ModuleConfig.DisabledAssemblyNames` 中的程序集会被发现，但不会作为启用模块加载。

依赖关系通过对其他模块 `*.Abstractions.dll` 的引用推断。如果模块 A 引用了 `ZYC.Framework.Modules.B.Abstractions.dll`，运行时就可以报告 A 依赖 B，而不需要 A 直接引用 B 的 WPF 实现。

## 检查清单

- 公开常量和契约放在 `*.Abstractions`。
- WPF 视图和 TabItem 放在实现项目。
- 只有必须在容器构建前存在的服务才放进 `RegisterAsync`。
- Tab 工厂和菜单项在 `LoadAsync` 中注册。
- 稳定 URI 路由使用 `TabItemRouteAttribute`。
- 添加新菜单 Provider 前，优先使用现有主菜单 Provider。
- config/state 类保持小型且可序列化。

<!--doc-l10n:locale zh-TW-->
# 模組開發

這篇文件說明如何撰寫符合 ZYC.Framework Host 執行階段模型的模組：以依賴注入為入口，使用 URI 驅動 Tab 導覽，按需接入主選單，並清楚分離公開契約與 WPF 實作。

## 什麼時候建立模組

當一個功能需要由 ZYC.Framework Host 發現並載入，而不是直接編譯進 Shell 時，就應該做成模組。模組可以提供 View、Tab factory、主選單項目、設定、狀態、背景服務與命令列選項。

如果功能契約需要被其他模組重用，請建立 `*.Abstractions` 專案。公開 DTO、常數、服務介面、選單 Provider 介面放在這裡。WPF View、TabItem 實作與執行階段註冊邏輯放在實作專案。

## 建議專案結構

| 區域 | 常見檔案 | 職責 |
| --- | --- | --- |
| Abstractions | `*ModuleConstants.cs`, `I*MainMenuItemsProvider.cs`, service interfaces | 其他模組可引用的公開契約，不依賴 WPF 實作細節。 |
| Implementation | `Module.cs` | 模組生命週期入口。註冊 factory、選單項目、Provider 與服務。 |
| Navigation | `*TabItemFactory.cs`, `*TabItem.cs` | 匹配 `zyc://` 或 app URI，並建立 Tab 實例。 |
| Menu | `*MainMenuItem.cs`, optional `*MainMenuItemsProvider.cs` | 加入使用者可見命令，用於導覽到模組 Tab 或執行模組動作。 |
| UI | `UI/*View.xaml`, `UI/*View.xaml.cs` | TabItem 使用的 WPF View。 |

Abstractions 專案不要暴露 WPF 控制項型別。如果需要命令契約，可以在 Abstractions 中使用 `System.Windows.Input.ICommand`。

## 生命週期

| 方法 | 時機 | 用途 |
| --- | --- | --- |
| `RegisterAsync(ContainerBuilder builder)` | Autofac 容器建構之前。 | 註冊必須在整個容器中可用的服務。 |
| `LoadAsync(ILifetimeScope lifetimeScope)` | 容器建構之後，且模組處於啟用狀態時。 | 註冊 Tab factory、主選單項目、狀態列項目與執行階段 Hook。 |
| `AfterLoadedAsync(ILifetimeScope lifetimeScope)` | 所有啟用模組載入完成之後。 | 需要依賴其他模組已完成註冊的跨模組初始化。 |

大多數 UI 模組只需要實作 `LoadAsync`。

## 最小 View 模組

對於單一 View 模組，可以在 `LoadAsync` 中註冊 simple tab factory：

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

這是 `minimal` 專案範本使用的模式。當模組只需要把一個 WPF `UserControl` 暴露為 Tab 時，這種方式就足夠。

## 帶路由的 Tab 模組

如果模組需要穩定 URI、路由匹配、參數、客製單例行為或多個 Tab，請使用 `TabItemFactoryBase`。

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

`TabItemRouteAttribute` 可以依 `Scheme`、`Host`、`Path` 與 `PathMatch` 匹配 URI。如果多個 factory 匹配同一個 URI，`Priority` 較高的 factory 會優先。`TabItemFactoryBase` 預設 `IsSingle = true`；如果模組允許開啟多個實例，請覆寫它。

## TabItem 與 View

帶路由的 Tab 通常用 `TabItemInstanceBase<TView>` 包裝 WPF View：

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

Tab 生命週期行為放在 TabItem 中，視覺行為放在 View 中。這樣可以把路由、Tab 識別與 UI 組合拆開。

## 主選單入口

選單項目通常導覽到模組 URI：

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

然後從模組註冊：

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterTabItemFactory<ReportsTabItemFactory>();
    lifetimeScope.RegisterExtensionsMainMenuItem<ReportsMainMenuItem>();

    return Task.CompletedTask;
}
```

優先使用符合功能位置的既有選單 Provider，例如 File、View、Tools、Extensions 或 About。只有當功能擁有多個子命令時，才增加模組自己的 Provider。

## 設定與狀態

實作 `IConfig` 或 `IState` 的型別會在模組註冊期間從 settings 目錄讀取，並自動註冊到 Autofac。使用者可編輯設定使用 config；執行階段持久化內容，例如選中路徑、待處理操作、視窗狀態，使用 state。

不要把大型業務資料放進 config/state 類別。它們應保持小型、可序列化，並且能承受版本演進。

## 模組載入與依賴

Host 會發現命名類似 `ZYC.Framework.Modules*.dll` 的標準模組組件，然後追加 `ModuleConfig.AdditionalAssemblyNames` 中列出的組件。`ModuleConfig.DisabledAssemblyNames` 中的組件會被發現，但不會作為啟用模組載入。

依賴關係會透過對其他模組 `*.Abstractions.dll` 的引用推斷。如果模組 A 引用了 `ZYC.Framework.Modules.B.Abstractions.dll`，執行階段就可以報告 A 依賴 B，而不需要 A 直接引用 B 的 WPF 實作。

## 檢查清單

- 公開常數與契約放在 `*.Abstractions`。
- WPF View 與 TabItem 放在實作專案。
- 只有必須在容器建構前存在的服務才放進 `RegisterAsync`。
- Tab factory 與選單項目在 `LoadAsync` 中註冊。
- 穩定 URI 路由使用 `TabItemRouteAttribute`。
- 新增選單 Provider 前，優先使用既有主選單 Provider。
- config/state 類別保持小型且可序列化。

<!--doc-l10n:locale ko-->
# 모듈 개발

이 문서는 ZYC.Framework Host 런타임 모델에 맞는 모듈을 만드는 방법을 설명합니다. 핵심은 의존성 주입을 진입점으로 사용하고, URI 기반 탭 내비게이션을 제공하며, 필요하면 메인 메뉴에 통합하고, 공개 계약과 WPF 구현을 분리하는 것입니다.

## 모듈을 만드는 시점

기능을 셸에 직접 컴파일하지 않고 ZYC.Framework Host가 발견하고 로드해야 한다면 모듈로 만듭니다. 모듈은 뷰, 탭 팩터리, 메인 메뉴 항목, 구성, 상태, 백그라운드 서비스, 명령줄 옵션을 제공할 수 있습니다.

재사용되는 기능 계약이 있다면 `*.Abstractions` 프로젝트를 만듭니다. 공개 DTO, 상수, 서비스 인터페이스, 메뉴 provider 인터페이스를 여기에 둡니다. WPF 뷰, 탭 항목 구현, 런타임 등록은 구현 프로젝트에 둡니다.

## 권장 프로젝트 구조

| 영역 | 일반적인 파일 | 책임 |
| --- | --- | --- |
| Abstractions | `*ModuleConstants.cs`, `I*MainMenuItemsProvider.cs`, service interfaces | 다른 모듈이 WPF 구현 세부 사항에 의존하지 않고 참조할 수 있는 공개 계약. |
| Implementation | `Module.cs` | 모듈 라이프사이클 진입점. 팩터리, 메뉴 항목, provider, 서비스를 등록합니다. |
| Navigation | `*TabItemFactory.cs`, `*TabItem.cs` | `zyc://` 또는 app URI를 매칭하고 탭 인스턴스를 만듭니다. |
| Menu | `*MainMenuItem.cs`, optional `*MainMenuItemsProvider.cs` | 모듈 탭으로 이동하거나 모듈 동작을 실행하는 사용자 명령을 추가합니다. |
| UI | `UI/*View.xaml`, `UI/*View.xaml.cs` | 탭 항목에서 사용하는 WPF 뷰. |

Abstractions 프로젝트에서는 WPF 컨트롤 타입을 공개하지 마세요. 명령 계약이 필요한 경우 `System.Windows.Input.ICommand`는 Abstractions에서 사용할 수 있습니다.

## 라이프사이클

| 메서드 | 시점 | 용도 |
| --- | --- | --- |
| `RegisterAsync(ContainerBuilder builder)` | Autofac 컨테이너가 만들어지기 전. | 전체 컨테이너에서 사용할 수 있어야 하는 서비스를 등록합니다. |
| `LoadAsync(ILifetimeScope lifetimeScope)` | 컨테이너가 만들어지고 모듈이 활성화된 뒤. | 탭 팩터리, 메인 메뉴 항목, 상태 표시줄 항목, 런타임 hook을 등록합니다. |
| `AfterLoadedAsync(ILifetimeScope lifetimeScope)` | 활성화된 모든 모듈이 로드된 뒤. | 다른 모듈 등록이 끝난 뒤 필요한 교차 모듈 초기화. |

대부분의 UI 모듈은 `LoadAsync`만 구현하면 충분합니다.

## 최소 뷰 모듈

단일 뷰 모듈은 `LoadAsync`에서 simple tab factory를 등록합니다.

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

이것은 `minimal` 프로젝트 템플릿에서 사용하는 패턴입니다. 하나의 WPF `UserControl`을 탭으로 노출하는 정도라면 이 방식으로 충분합니다.

## 라우팅 탭 모듈

안정적인 URI, 라우트 매칭, 파라미터, 사용자 지정 싱글턴 동작, 여러 탭이 필요하다면 `TabItemFactoryBase`를 사용합니다.

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

`TabItemRouteAttribute`는 `Scheme`, `Host`, `Path`, `PathMatch`로 URI를 매칭할 수 있습니다. 같은 URI에 여러 팩터리가 매칭되면 `Priority`가 높은 팩터리가 선택됩니다. `TabItemFactoryBase`의 기본값은 `IsSingle = true`입니다. 여러 인스턴스를 허용하려면 이를 재정의하세요.

## 탭 항목과 뷰

라우팅 탭은 보통 `TabItemInstanceBase<TView>`로 WPF 뷰를 감쌉니다.

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

탭 라이프사이클 동작은 탭 항목에 두고, 시각적인 동작은 뷰에 둡니다. 이렇게 하면 라우팅, 탭 식별, UI 구성을 분리할 수 있습니다.

## 메인 메뉴 항목

메뉴 항목은 일반적으로 모듈 URI로 이동합니다.

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

모듈에서 등록합니다.

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterTabItemFactory<ReportsTabItemFactory>();
    lifetimeScope.RegisterExtensionsMainMenuItem<ReportsMainMenuItem>();

    return Task.CompletedTask;
}
```

기능 위치에 맞는 기존 메뉴 provider를 우선 사용하세요. File, View, Tools, Extensions, About 등이 있습니다. 기능이 여러 하위 명령을 가질 때만 모듈 전용 provider를 추가합니다.

## 구성과 상태

`IConfig` 또는 `IState`를 구현하는 타입은 모듈 등록 중 settings 디렉터리에서 로드되고 Autofac에 자동 등록됩니다. 사용자가 편집하는 설정에는 config를 사용하고, 선택된 경로, 대기 중인 작업, 창 상태 같은 런타임 지속성에는 state를 사용합니다.

큰 비즈니스 데이터를 config/state 클래스에 저장하지 마세요. 작고, 직렬화 가능하며, 버전 변화에 견딜 수 있게 유지합니다.

## 모듈 로딩과 의존성

Host는 `ZYC.Framework.Modules*.dll`처럼 이름이 지정된 표준 모듈 어셈블리를 발견한 뒤 `ModuleConfig.AdditionalAssemblyNames`에 나열된 어셈블리를 추가합니다. `ModuleConfig.DisabledAssemblyNames`에 포함된 어셈블리는 발견되지만 활성 모듈로 로드되지는 않습니다.

의존성은 다른 모듈의 `*.Abstractions.dll` 참조에서 추론됩니다. 모듈 A가 `ZYC.Framework.Modules.B.Abstractions.dll`을 참조하면 런타임은 A가 B에 의존한다고 보고할 수 있으며, A가 B의 WPF 구현을 직접 참조할 필요는 없습니다.

## 체크리스트

- 공개 상수와 계약은 `*.Abstractions`에 둡니다.
- WPF 뷰와 탭 항목은 구현 프로젝트에 둡니다.
- 컨테이너 구성 전에 필요한 서비스만 `RegisterAsync`에서 등록합니다.
- 탭 팩터리와 메뉴 항목은 `LoadAsync`에서 등록합니다.
- 안정적인 URI 라우팅에는 `TabItemRouteAttribute`를 사용합니다.
- 새 메뉴 provider를 추가하기 전에 기존 메인 메뉴 provider를 우선 사용합니다.
- config/state 클래스는 작고 직렬화 가능하게 유지합니다.

<!--doc-l10n:end-->
