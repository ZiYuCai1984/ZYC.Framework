<p align="center">
  <a href="./module-development.md">English</a> |
  <a href="./module-development.ja.md">日本語</a> |
  <a href="./module-development.zh-CN.md">简体中文</a> |
  <a href="./module-development.zh-TW.md">繁體中文</a> |
  <a href="./module-development.ko.md">한국어</a> |
</p>


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
