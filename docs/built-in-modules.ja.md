<p align="center">
  <a href="./built-in-modules.md">English</a> |
  <a href="./built-in-modules.ja.md">日本語</a> |
  <a href="./built-in-modules.zh-CN.md">简体中文</a> |
  <a href="./built-in-modules.zh-TW.md">繁體中文</a> |
  <a href="./built-in-modules.ko.md">한국어</a> |
</p>


# 組み込みモジュール

このページは、現在の ZYC.Framework ソース ツリーに含まれる組み込みモジュールをまとめます。ここでの組み込みモジュールとは、`Module : ModuleBase` の入口を持ち、モジュール ローダーに発見されることを意図した `ZYC.Framework.Modules.*` プロジェクトです。

`ZYC.Framework.Modules.*.Abstractions` のような Abstractions プロジェクトはコントラクト アセンブリであり、それだけでは実行時モジュールではありません。

## 組み込みモジュールのロード方法

起動時、モジュール ローダーはアプリケーション ディレクトリから `ZYC.Framework.Modules*.dll` 形式のアセンブリを探します。各モジュール アセンブリについて、次を行います。

- アセンブリ内の Autofac サービスを登録する。
- settings ディレクトリから具象 `IConfig` / `IState` 型を読み込む。
- `ModuleBase` を継承した最初の型を探す。
- モジュール インスタンスを作成し、`RegisterAsync` を呼ぶ。
- 後で、有効なモジュールだけ `LoadAsync` を呼ぶ。

`ModuleConfig.DisabledAssemblyNames` は発見されたモジュール アセンブリをファイル名で無効化します。`ModuleConfig.AdditionalAssemblyNames` はアプリケーション ディレクトリから追加アセンブリを読み込みます。

## モジュール一覧

| Module | 主な面 | 補足 |
| --- | --- | --- |
| `About` | About メニューとルーティング タブ | 製品/about 情報を表示する。 |
| `Accounts` | タイトルバー拡張とアカウント サービス | Provider ベースの account session を初期化し、sign-in/sign-out 操作を公開する。 |
| `Accounts.GitHub` | GitHub OAuth WebView2 タブ | GitHub account provider と sign-in callback 処理を提供する。 |
| `ApiReference` | About メニューと WebView2 タブ | API reference content をホストする。 |
| `Aspire` | Tools メニュー、ルーティング タブ、ステータスバー | Aspire resources を開始・監視し、`IExtensionResourcesProvider` の寄与を解決する。 |
| `BlazorDemo` | Tools メニューとルーティング タブ | デスクトップ Host 内の Blazor 統合を示す。 |
| `ChromeExtensions` | Extensions メニューとルーティング タブ | WebBrowser 用のローカル Chrome Web Store extension packages を管理する。 |
| `CLI` | Tools メニューと terminal タブ | 組み込み terminal をホストし、terminal native dependencies をロードする。 |
| `FileExplorer` | File メニューとルーティング タブ | ファイル システム閲覧面を開く。 |
| `FileExplorer.Features` | File menu sub-provider | FileExplorer contracts の上に recent-path 系機能を追加する。 |
| `Language` | Settings メニューとルーティング タブ | 言語選択と localization resource 管理を提供する。 |
| `Log` | File メニューと logging provider | log4net ベースの logger provider を登録し、ログ表示を提供する。 |
| `MCP.Server` | Tools menu provider | MCP server 操作を公開する。 |
| `Mock` | Root mock menu と demo tabs | ダイアログ、通知、タスク、CLI、サンプル View 用の開発/テスト モジュール。 |
| `ModuleManager` | Extensions メニューとルーティング タブ | ローカル モジュールと NuGet-installed modules を管理する。 |
| `NuGet` | File メニュー | NuGet cache tooling を提供する。 |
| `Secrets` | Settings メニューとルーティング タブ | `ISecrets` 経由で secret-like settings を管理する。 |
| `Settings` | Root Settings メニューとルーティング タブ | 他モジュールが使う settings shell をホストする。 |
| `TaskManager` | Tools メニュー、ルーティング タブ、ステータスバー | Task management を初期化し、タスク状態/操作を公開する。 |
| `TextEditor` | File/Open メニューとルーティング タブ | text preview/edit 面を提供し、generic `file://` preview を扱う。 |
| `Translator` | Aspire command-line resource | Aspire が利用可能な場合、LibreTranslate sidecar を登録する。 |
| `Update` | About メニューとルーティング タブ | update check を提供し、tab/workspace restore 後に起動時チェックできる。 |
| `WebBrowser` | Tools メニューと WebView2 タブ | Host 内で browser tab を開く。 |

## Shell と Diagnostics モジュール

`Settings`、`Language`、`Secrets`、`Log`、`TaskManager`、`ModuleManager`、`Update`、`About`、`Accounts`、`ChromeExtensions`、`ApiReference` は主に Shell/運用系モジュールです。Framework の確認、設定、保守をしやすくします。

これらのモジュールは通常 `LoadAsync` からメニュー項目とルーティング タブを登録します。一部はより早い段階でサービスも登録します。

- `Log` は `RegisterAsync` で logging providers を登録します。
- `Language` は language-resource adapters を登録し、default language resources を読み込みます。
- `Secrets` は config objects から `ISecrets` への adapter を登録します。
- `TaskManager` は UI 公開前に `ITaskManager` を初期化します。
- `Accounts` は `IAccountManager` を初期化し、タイトルバーの account surface を登録します。
- `ChromeExtensions` は extension package manager タブを Extensions 配下に登録します。
- `Update` はすべてのモジュールがロードされた後に購読し、起動時チェック前に `TabManagerRestoreCompleted` を待ちます。

## Navigation と Content モジュール

`WebBrowser`、`FileExplorer`、`TextEditor`、`CLI`、`BlazorDemo` はユーザー向け content surface を公開します。Shell から View を直接作るのではなく、いずれも tab routing に依存します。

`Accounts.GitHub` と `ChromeExtensions` も provider sign-in と Chrome Web Store package discovery のために WebView2 ベースのタブを使います。どちらも通常のモジュールとしてロードされ、browser-specific behavior は WebView2 infrastructure と module contracts を通じて扱います。

これらのモジュールが誤ったタブや Not Found タブを開く場合は、登録済み `ITabItemFactory`、route attributes、factory priority、`ITabManager.NavigateAsync(...)` に渡している URI を確認してください。

## Aspire と Sidecar モジュール

`Aspire` は Aspire resources を実行する Host 側モジュールです。Aspire dashboard tab、Tools menu entry、status bar item を登録します。`AspireConfig.AutoStart` が true の場合、module load 中に Aspire service を開始します。

`Translator` は sidecar 型モジュールです。大きな UI は持たず、`ICommandlineResourcesProvider` に command-line resource を登録し、Aspire module が LibreTranslate を起動できるようにします。

## Development と Demo モジュール

`Mock` と `BlazorDemo` は framework behavior の開発・検証に役立ちます。`Mock` は通知、ダイアログ、task manager behavior、CLI integration のための demo tabs と sample views を登録します。Production feature は、明示的に診断/サンプル用途でない限り Mock に入れないでください。

## 不完全または非モジュールのディレクトリ

実際の module project と `Module.cs` を持つディレクトリだけを active built-in module と扱います。`obj`、生成ファイル、`UI` サブフォルダーだけのフォルダーは runtime discovery の根拠になりません。

モジュール ロードを文書化または調査するときは、フォルダー名だけでなく、compiled output と `Module.cs` から始めてください。
