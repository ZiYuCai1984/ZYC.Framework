<p align="center">
  <a href="./extension-points.md">English</a> |
  <a href="./extension-points.ja.md">日本語</a> |
  <a href="./extension-points.zh-CN.md">简体中文</a> |
  <a href="./extension-points.zh-TW.md">繁體中文</a> |
  <a href="./extension-points.ko.md">한국어</a> |
</p>


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
