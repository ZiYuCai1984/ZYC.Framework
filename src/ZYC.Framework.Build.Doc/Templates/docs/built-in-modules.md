<p align="center">
  <a href="./built-in-modules.md">English</a> |
  <a href="./built-in-modules.ja.md">日本語</a> |
  <a href="./built-in-modules.zh-CN.md">简体中文</a> |
  <a href="./built-in-modules.zh-TW.md">繁體中文</a> |
  <a href="./built-in-modules.ko.md">한국어</a> |
</p>

<!--doc-l10n:begin built-in-modules-content-->
# Built-in Modules

This page summarizes the built-in modules that are part of the current ZYC.Framework source tree. A built-in module here means a `ZYC.Framework.Modules.*` project that contains a `Module : ModuleBase` entrypoint and is intended to be discovered by the module loader.

Abstractions projects such as `ZYC.Framework.Modules.*.Abstractions` are contract assemblies, not runtime modules by themselves.

## How Built-in Modules Load

At startup, the module loader scans the application directory for assemblies named like `ZYC.Framework.Modules*.dll`. For each module assembly, it:

- registers Autofac services from the assembly;
- loads concrete `IConfig` and `IState` types from the settings directory;
- finds the first type derived from `ModuleBase`;
- creates the module instance and calls `RegisterAsync`;
- later calls `LoadAsync` only for enabled modules.

`ModuleConfig.DisabledAssemblyNames` disables discovered module assemblies by file name. `ModuleConfig.AdditionalAssemblyNames` adds extra assemblies from the application directory.

## Module List

| Module | Main surface | Notes |
| --- | --- | --- |
| `About` | About menu and routed tab | Shows product/about information. |
| `ApiReference` | About menu and WebView2 tab | Hosts API reference content. |
| `Aspire` | Tools menu, routed tab, status bar | Starts and monitors Aspire resources; resolves `IExtensionResourcesProvider` contributions. |
| `BlazorDemo` | Tools menu and routed tab | Demonstrates Blazor integration inside the desktop host. |
| `CLI` | Tools menu and terminal tab | Hosts the embedded terminal and loads terminal native dependencies. |
| `FileExplorer` | File menu and routed tab | Opens file-system browsing surfaces. |
| `FileExplorer.Features` | File menu sub-provider | Adds recent-path style File menu features on top of FileExplorer contracts. |
| `Language` | Settings menu and routed tabs | Provides language selection and localization resource management. |
| `Log` | File menu and logging provider | Registers the log4net-backed logger provider and exposes log viewing. |
| `MCP.Server` | Tools menu provider | Exposes MCP server operations. |
| `Mock` | Root mock menu and demo tabs | Development/testing module for dialogs, notifications, tasks, CLI, and sample views. |
| `ModuleManager` | Extensions menu and routed tabs | Manages local modules and NuGet-installed modules. |
| `NuGet` | File menu | Provides NuGet cache tooling. |
| `Secrets` | Settings menu and routed tab | Provides secret-like settings management through `ISecrets`. |
| `Settings` | Root Settings menu and routed tab | Hosts the settings shell used by other modules. |
| `TaskManager` | Tools menu, routed tab, status bar | Initializes task management and exposes task status/actions. |
| `TextEditor` | File/Open menu and routed tabs | Provides text preview and edit surfaces, including generic `file://` preview handling. |
| `Translator` | Aspire command-line resource | Registers LibreTranslate as an Aspire-launched sidecar when Aspire support is available. |
| `Update` | About menu and routed tab | Provides update checking; can check after tab/workspace restore. |
| `WebBrowser` | Tools menu and WebView2 tab | Opens browser tabs inside the host. |

## Shell and Diagnostics Modules

`Settings`, `Language`, `Secrets`, `Log`, `TaskManager`, `ModuleManager`, `Update`, `About`, and `ApiReference` are mostly shell or operational modules. They make the framework easier to inspect, configure, and maintain.

These modules usually register menu items and routed tabs from `LoadAsync`. Some also register services earlier:

- `Log` registers logging providers in `RegisterAsync`.
- `Language` registers language-resource adapters and loads default language resources.
- `Secrets` registers an adapter from config objects to `ISecrets`.
- `TaskManager` initializes `ITaskManager` before exposing its UI.
- `Update` subscribes after all modules load and waits for `TabManagerRestoreCompleted` before startup checks.

## Navigation and Content Modules

`WebBrowser`, `FileExplorer`, `TextEditor`, `CLI`, and `BlazorDemo` expose user-facing content surfaces. They all rely on tab routing rather than direct view construction from the shell.

If one of these modules opens the wrong tab or a Not Found tab, check the registered `ITabItemFactory`, route attributes, factory priority, and the URI being passed to `ITabManager.NavigateAsync(...)`.

## Aspire and Sidecar Modules

`Aspire` is the host-side module for running Aspire resources. It registers an Aspire dashboard tab, a Tools menu entry, and a status bar item. If `AspireConfig.AutoStart` is true, it starts the Aspire service during module load.

`Translator` is a sidecar-style module. It does not expose a large UI; instead, it registers a command-line resource with `ICommandlineResourcesProvider` so the Aspire module can launch LibreTranslate.

## Development and Demo Modules

`Mock` and `BlazorDemo` are useful when developing or validating framework behavior. `Mock` registers demo tabs and sample views for notifications, dialogs, task manager behavior, and CLI integration. Keep production features out of Mock unless they are intentionally diagnostic or sample-only.

## Incomplete or Non-module Directories

Only directories with a real module project and `Module.cs` should be treated as active built-in modules. A folder that only contains `obj`, generated files, or a `UI` subfolder is not enough for runtime discovery.

When documenting or troubleshooting module loading, start from the compiled output and `Module.cs`, not from folder names alone.

<!--doc-l10n:locale ja-->
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
| `ApiReference` | About メニューと WebView2 タブ | API reference content をホストする。 |
| `Aspire` | Tools メニュー、ルーティング タブ、ステータスバー | Aspire resources を開始・監視し、`IExtensionResourcesProvider` の寄与を解決する。 |
| `BlazorDemo` | Tools メニューとルーティング タブ | デスクトップ Host 内の Blazor 統合を示す。 |
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

`Settings`、`Language`、`Secrets`、`Log`、`TaskManager`、`ModuleManager`、`Update`、`About`、`ApiReference` は主に Shell/運用系モジュールです。Framework の確認、設定、保守をしやすくします。

これらのモジュールは通常 `LoadAsync` からメニュー項目とルーティング タブを登録します。一部はより早い段階でサービスも登録します。

- `Log` は `RegisterAsync` で logging providers を登録します。
- `Language` は language-resource adapters を登録し、default language resources を読み込みます。
- `Secrets` は config objects から `ISecrets` への adapter を登録します。
- `TaskManager` は UI 公開前に `ITaskManager` を初期化します。
- `Update` はすべてのモジュールがロードされた後に購読し、起動時チェック前に `TabManagerRestoreCompleted` を待ちます。

## Navigation と Content モジュール

`WebBrowser`、`FileExplorer`、`TextEditor`、`CLI`、`BlazorDemo` はユーザー向け content surface を公開します。Shell から View を直接作るのではなく、いずれも tab routing に依存します。

これらのモジュールが誤ったタブや Not Found タブを開く場合は、登録済み `ITabItemFactory`、route attributes、factory priority、`ITabManager.NavigateAsync(...)` に渡している URI を確認してください。

## Aspire と Sidecar モジュール

`Aspire` は Aspire resources を実行する Host 側モジュールです。Aspire dashboard tab、Tools menu entry、status bar item を登録します。`AspireConfig.AutoStart` が true の場合、module load 中に Aspire service を開始します。

`Translator` は sidecar 型モジュールです。大きな UI は持たず、`ICommandlineResourcesProvider` に command-line resource を登録し、Aspire module が LibreTranslate を起動できるようにします。

## Development と Demo モジュール

`Mock` と `BlazorDemo` は framework behavior の開発・検証に役立ちます。`Mock` は通知、ダイアログ、task manager behavior、CLI integration のための demo tabs と sample views を登録します。Production feature は、明示的に診断/サンプル用途でない限り Mock に入れないでください。

## 不完全または非モジュールのディレクトリ

実際の module project と `Module.cs` を持つディレクトリだけを active built-in module と扱います。`obj`、生成ファイル、`UI` サブフォルダーだけのフォルダーは runtime discovery の根拠になりません。

モジュール ロードを文書化または調査するときは、フォルダー名だけでなく、compiled output と `Module.cs` から始めてください。

<!--doc-l10n:locale zh-CN-->
# 内置模块

本文汇总当前 ZYC.Framework 源码树中的内置模块。这里的内置模块指包含 `Module : ModuleBase` 入口、并 intended 由模块加载器发现的 `ZYC.Framework.Modules.*` 项目。

`ZYC.Framework.Modules.*.Abstractions` 这类 Abstractions 项目是契约程序集，本身不是运行时模块。

## 内置模块如何加载

启动时，模块加载器会在应用程序目录扫描命名类似 `ZYC.Framework.Modules*.dll` 的程序集。对于每个模块程序集，它会：

- 注册程序集里的 Autofac 服务；
- 从 settings 目录加载具体的 `IConfig` 和 `IState` 类型；
- 找到第一个继承 `ModuleBase` 的类型；
- 创建模块实例并调用 `RegisterAsync`；
- 稍后只对启用的模块调用 `LoadAsync`。

`ModuleConfig.DisabledAssemblyNames` 按文件名禁用已发现的模块程序集。`ModuleConfig.AdditionalAssemblyNames` 从应用目录追加额外程序集。

## 模块清单

| Module | 主要表面 | 说明 |
| --- | --- | --- |
| `About` | About 菜单和路由 Tab | 显示产品/about 信息。 |
| `ApiReference` | About 菜单和 WebView2 Tab | 承载 API reference 内容。 |
| `Aspire` | Tools 菜单、路由 Tab、状态栏 | 启动和监控 Aspire resources；解析 `IExtensionResourcesProvider` 贡献。 |
| `BlazorDemo` | Tools 菜单和路由 Tab | 演示桌面 Host 内的 Blazor 集成。 |
| `CLI` | Tools 菜单和终端 Tab | 承载嵌入式终端，并加载终端 native dependencies。 |
| `FileExplorer` | File 菜单和路由 Tab | 打开文件系统浏览表面。 |
| `FileExplorer.Features` | File menu sub-provider | 在 FileExplorer 契约之上添加 recent-path 类 File 菜单能力。 |
| `Language` | Settings 菜单和路由 Tab | 提供语言选择与本地化资源管理。 |
| `Log` | File 菜单和 logging provider | 注册 log4net-backed logger provider，并暴露日志查看。 |
| `MCP.Server` | Tools menu provider | 暴露 MCP server 操作。 |
| `Mock` | Root mock menu 和 demo tabs | 面向对话框、通知、任务、CLI 和示例 View 的开发/测试模块。 |
| `ModuleManager` | Extensions 菜单和路由 Tab | 管理本地模块和 NuGet-installed modules。 |
| `NuGet` | File 菜单 | 提供 NuGet cache tooling。 |
| `Secrets` | Settings 菜单和路由 Tab | 通过 `ISecrets` 管理 secret-like settings。 |
| `Settings` | Root Settings 菜单和路由 Tab | 承载其他模块使用的 settings shell。 |
| `TaskManager` | Tools 菜单、路由 Tab、状态栏 | 初始化 task management，并暴露任务状态/操作。 |
| `TextEditor` | File/Open 菜单和路由 Tab | 提供 text preview/edit 表面，包括 generic `file://` preview handling。 |
| `Translator` | Aspire command-line resource | 在 Aspire 可用时注册 LibreTranslate sidecar。 |
| `Update` | About 菜单和路由 Tab | 提供更新检查；可在 tab/workspace restore 后执行启动检查。 |
| `WebBrowser` | Tools 菜单和 WebView2 Tab | 在 Host 内打开浏览器 Tab。 |

## Shell 与诊断模块

`Settings`、`Language`、`Secrets`、`Log`、`TaskManager`、`ModuleManager`、`Update`、`About`、`ApiReference` 主要是 Shell 或运维类模块。它们让框架更容易检查、配置和维护。

这些模块通常从 `LoadAsync` 注册菜单项和路由 Tab。有些模块也会更早注册服务：

- `Log` 在 `RegisterAsync` 中注册 logging providers。
- `Language` 注册 language-resource adapters，并加载 default language resources。
- `Secrets` 注册从 config objects 到 `ISecrets` 的 adapter。
- `TaskManager` 在暴露 UI 前初始化 `ITaskManager`。
- `Update` 在所有模块加载后订阅事件，并在启动检查前等待 `TabManagerRestoreCompleted`。

## 导航与内容模块

`WebBrowser`、`FileExplorer`、`TextEditor`、`CLI`、`BlazorDemo` 暴露面向用户的内容表面。它们都依赖 Tab routing，而不是由 Shell 直接构造 View。

如果这些模块打开了错误 Tab 或 Not Found Tab，请检查已注册的 `ITabItemFactory`、route attributes、factory priority，以及传给 `ITabManager.NavigateAsync(...)` 的 URI。

## Aspire 与 Sidecar 模块

`Aspire` 是运行 Aspire resources 的 Host 侧模块。它注册 Aspire dashboard tab、Tools menu entry 和 status bar item。如果 `AspireConfig.AutoStart` 为 true，它会在模块加载期间启动 Aspire service。

`Translator` 是 sidecar 风格模块。它不暴露大型 UI，而是向 `ICommandlineResourcesProvider` 注册 command-line resource，让 Aspire 模块可以启动 LibreTranslate。

## 开发与 Demo 模块

`Mock` 和 `BlazorDemo` 适合开发或验证框架行为。`Mock` 注册用于通知、对话框、task manager behavior 和 CLI integration 的 demo tabs 与 sample views。除非明确是诊断或示例用途，不要把生产功能放进 Mock。

## 不完整或非模块目录

只有同时具备真实 module project 和 `Module.cs` 的目录，才应该被视为 active built-in module。只有 `obj`、生成文件或 `UI` 子目录的文件夹，不足以作为 runtime discovery 的依据。

文档化或排查模块加载时，请从 compiled output 和 `Module.cs` 开始，不要只看文件夹名称。

<!--doc-l10n:locale zh-TW-->
# 內建模組

本文彙總目前 ZYC.Framework 原始碼樹中的內建模組。這裡的內建模組指包含 `Module : ModuleBase` 入口，並 intended 由模組載入器發現的 `ZYC.Framework.Modules.*` 專案。

`ZYC.Framework.Modules.*.Abstractions` 這類 Abstractions 專案是契約組件，本身不是執行階段模組。

## 內建模組如何載入

啟動時，模組載入器會在應用程式目錄掃描命名類似 `ZYC.Framework.Modules*.dll` 的組件。對於每個模組組件，它會：

- 註冊組件裡的 Autofac 服務；
- 從 settings 目錄載入具體的 `IConfig` 和 `IState` 型別；
- 找到第一個繼承 `ModuleBase` 的型別；
- 建立模組實例並呼叫 `RegisterAsync`；
- 稍後只對啟用的模組呼叫 `LoadAsync`。

`ModuleConfig.DisabledAssemblyNames` 依檔名停用已發現的模組組件。`ModuleConfig.AdditionalAssemblyNames` 從應用目錄追加額外組件。

## 模組清單

| Module | 主要表面 | 說明 |
| --- | --- | --- |
| `About` | About 選單與路由 Tab | 顯示產品/about 資訊。 |
| `ApiReference` | About 選單與 WebView2 Tab | 承載 API reference 內容。 |
| `Aspire` | Tools 選單、路由 Tab、狀態列 | 啟動與監控 Aspire resources；解析 `IExtensionResourcesProvider` 貢獻。 |
| `BlazorDemo` | Tools 選單與路由 Tab | 示範桌面 Host 內的 Blazor 整合。 |
| `CLI` | Tools 選單與終端 Tab | 承載嵌入式終端，並載入終端 native dependencies。 |
| `FileExplorer` | File 選單與路由 Tab | 開啟檔案系統瀏覽表面。 |
| `FileExplorer.Features` | File menu sub-provider | 在 FileExplorer 契約之上新增 recent-path 類 File 選單能力。 |
| `Language` | Settings 選單與路由 Tab | 提供語言選擇與在地化資源管理。 |
| `Log` | File 選單與 logging provider | 註冊 log4net-backed logger provider，並暴露日誌檢視。 |
| `MCP.Server` | Tools menu provider | 暴露 MCP server 操作。 |
| `Mock` | Root mock menu 與 demo tabs | 面向對話框、通知、任務、CLI 與範例 View 的開發/測試模組。 |
| `ModuleManager` | Extensions 選單與路由 Tab | 管理本地模組與 NuGet-installed modules。 |
| `NuGet` | File 選單 | 提供 NuGet cache tooling。 |
| `Secrets` | Settings 選單與路由 Tab | 透過 `ISecrets` 管理 secret-like settings。 |
| `Settings` | Root Settings 選單與路由 Tab | 承載其他模組使用的 settings shell。 |
| `TaskManager` | Tools 選單、路由 Tab、狀態列 | 初始化 task management，並暴露任務狀態/操作。 |
| `TextEditor` | File/Open 選單與路由 Tab | 提供 text preview/edit 表面，包括 generic `file://` preview handling。 |
| `Translator` | Aspire command-line resource | 在 Aspire 可用時註冊 LibreTranslate sidecar。 |
| `Update` | About 選單與路由 Tab | 提供更新檢查；可在 tab/workspace restore 後執行啟動檢查。 |
| `WebBrowser` | Tools 選單與 WebView2 Tab | 在 Host 內開啟瀏覽器 Tab。 |

## Shell 與診斷模組

`Settings`、`Language`、`Secrets`、`Log`、`TaskManager`、`ModuleManager`、`Update`、`About`、`ApiReference` 主要是 Shell 或維運類模組。它們讓框架更容易檢查、設定與維護。

這些模組通常從 `LoadAsync` 註冊選單項目與路由 Tab。有些模組也會更早註冊服務：

- `Log` 在 `RegisterAsync` 中註冊 logging providers。
- `Language` 註冊 language-resource adapters，並載入 default language resources。
- `Secrets` 註冊從 config objects 到 `ISecrets` 的 adapter。
- `TaskManager` 在暴露 UI 前初始化 `ITaskManager`。
- `Update` 在所有模組載入後訂閱事件，並在啟動檢查前等待 `TabManagerRestoreCompleted`。

## 導覽與內容模組

`WebBrowser`、`FileExplorer`、`TextEditor`、`CLI`、`BlazorDemo` 暴露面向使用者的內容表面。它們都依賴 Tab routing，而不是由 Shell 直接建構 View。

如果這些模組開啟了錯誤 Tab 或 Not Found Tab，請檢查已註冊的 `ITabItemFactory`、route attributes、factory priority，以及傳給 `ITabManager.NavigateAsync(...)` 的 URI。

## Aspire 與 Sidecar 模組

`Aspire` 是執行 Aspire resources 的 Host 側模組。它註冊 Aspire dashboard tab、Tools menu entry 與 status bar item。如果 `AspireConfig.AutoStart` 為 true，它會在模組載入期間啟動 Aspire service。

`Translator` 是 sidecar 風格模組。它不暴露大型 UI，而是向 `ICommandlineResourcesProvider` 註冊 command-line resource，讓 Aspire 模組可以啟動 LibreTranslate。

## 開發與 Demo 模組

`Mock` 與 `BlazorDemo` 適合開發或驗證框架行為。`Mock` 註冊用於通知、對話框、task manager behavior 與 CLI integration 的 demo tabs 與 sample views。除非明確是診斷或範例用途，不要把生產功能放進 Mock。

## 不完整或非模組目錄

只有同時具備真實 module project 與 `Module.cs` 的目錄，才應該被視為 active built-in module。只有 `obj`、生成檔案或 `UI` 子目錄的資料夾，不足以作為 runtime discovery 的依據。

文件化或排查模組載入時，請從 compiled output 與 `Module.cs` 開始，不要只看資料夾名稱。

<!--doc-l10n:locale ko-->
# 내장 모듈

이 문서는 현재 ZYC.Framework 소스 트리에 포함된 내장 모듈을 요약합니다. 여기서 내장 모듈은 `Module : ModuleBase` 진입점을 가지고 모듈 로더가 발견하도록 의도된 `ZYC.Framework.Modules.*` 프로젝트를 의미합니다.

`ZYC.Framework.Modules.*.Abstractions` 같은 Abstractions 프로젝트는 계약 어셈블리이며, 그 자체로 런타임 모듈은 아닙니다.

## 내장 모듈 로딩 방식

시작 시 모듈 로더는 애플리케이션 디렉터리에서 `ZYC.Framework.Modules*.dll` 형식의 어셈블리를 찾습니다. 각 모듈 어셈블리에 대해 다음을 수행합니다.

- 어셈블리의 Autofac 서비스를 등록합니다.
- settings 디렉터리에서 concrete `IConfig`와 `IState` 타입을 로드합니다.
- `ModuleBase`를 상속한 첫 번째 타입을 찾습니다.
- 모듈 인스턴스를 만들고 `RegisterAsync`를 호출합니다.
- 이후 활성화된 모듈에만 `LoadAsync`를 호출합니다.

`ModuleConfig.DisabledAssemblyNames`는 발견된 모듈 어셈블리를 파일 이름으로 비활성화합니다. `ModuleConfig.AdditionalAssemblyNames`는 애플리케이션 디렉터리에서 추가 어셈블리를 읽습니다.

## 모듈 목록

| Module | 주요 표면 | 설명 |
| --- | --- | --- |
| `About` | About 메뉴와 라우팅 탭 | 제품/about 정보를 표시합니다. |
| `ApiReference` | About 메뉴와 WebView2 탭 | API reference content를 호스트합니다. |
| `Aspire` | Tools 메뉴, 라우팅 탭, 상태 표시줄 | Aspire resources를 시작하고 모니터링하며 `IExtensionResourcesProvider` 기여를 resolve합니다. |
| `BlazorDemo` | Tools 메뉴와 라우팅 탭 | 데스크톱 Host 안의 Blazor 통합을 시연합니다. |
| `CLI` | Tools 메뉴와 터미널 탭 | 내장 터미널을 호스트하고 terminal native dependencies를 로드합니다. |
| `FileExplorer` | File 메뉴와 라우팅 탭 | 파일 시스템 탐색 표면을 엽니다. |
| `FileExplorer.Features` | File menu sub-provider | FileExplorer contracts 위에 recent-path 계열 File 메뉴 기능을 추가합니다. |
| `Language` | Settings 메뉴와 라우팅 탭 | 언어 선택과 로컬라이제이션 리소스 관리를 제공합니다. |
| `Log` | File 메뉴와 logging provider | log4net 기반 logger provider를 등록하고 로그 보기를 제공합니다. |
| `MCP.Server` | Tools menu provider | MCP server 작업을 노출합니다. |
| `Mock` | Root mock menu와 demo tabs | 대화상자, 알림, 작업, CLI, 샘플 View용 개발/테스트 모듈입니다. |
| `ModuleManager` | Extensions 메뉴와 라우팅 탭 | 로컬 모듈과 NuGet-installed modules를 관리합니다. |
| `NuGet` | File 메뉴 | NuGet cache tooling을 제공합니다. |
| `Secrets` | Settings 메뉴와 라우팅 탭 | `ISecrets`를 통해 secret-like settings를 관리합니다. |
| `Settings` | Root Settings 메뉴와 라우팅 탭 | 다른 모듈이 사용하는 settings shell을 호스트합니다. |
| `TaskManager` | Tools 메뉴, 라우팅 탭, 상태 표시줄 | task management를 초기화하고 작업 상태/동작을 노출합니다. |
| `TextEditor` | File/Open 메뉴와 라우팅 탭 | text preview/edit 표면을 제공하며 generic `file://` preview handling을 포함합니다. |
| `Translator` | Aspire command-line resource | Aspire가 사용 가능할 때 LibreTranslate sidecar를 등록합니다. |
| `Update` | About 메뉴와 라우팅 탭 | 업데이트 확인을 제공하며 tab/workspace restore 뒤 시작 시 확인할 수 있습니다. |
| `WebBrowser` | Tools 메뉴와 WebView2 탭 | Host 안에서 browser tab을 엽니다. |

## Shell 및 Diagnostics 모듈

`Settings`, `Language`, `Secrets`, `Log`, `TaskManager`, `ModuleManager`, `Update`, `About`, `ApiReference`는 주로 Shell 또는 운영 모듈입니다. Framework를 검사, 설정, 유지관리하기 쉽게 합니다.

이 모듈들은 보통 `LoadAsync`에서 메뉴 항목과 라우팅 탭을 등록합니다. 일부는 더 이른 단계에서 서비스도 등록합니다.

- `Log`는 `RegisterAsync`에서 logging providers를 등록합니다.
- `Language`는 language-resource adapters를 등록하고 default language resources를 로드합니다.
- `Secrets`는 config objects에서 `ISecrets`로 가는 adapter를 등록합니다.
- `TaskManager`는 UI를 노출하기 전에 `ITaskManager`를 초기화합니다.
- `Update`는 모든 모듈이 로드된 뒤 구독하고 시작 확인 전에 `TabManagerRestoreCompleted`를 기다립니다.

## Navigation 및 Content 모듈

`WebBrowser`, `FileExplorer`, `TextEditor`, `CLI`, `BlazorDemo`는 사용자 대상 content surface를 노출합니다. 이들은 Shell에서 View를 직접 만들지 않고 모두 tab routing에 의존합니다.

이 모듈들이 잘못된 탭이나 Not Found 탭을 열면 등록된 `ITabItemFactory`, route attributes, factory priority, `ITabManager.NavigateAsync(...)`에 전달되는 URI를 확인하세요.

## Aspire 및 Sidecar 모듈

`Aspire`는 Aspire resources를 실행하는 Host 측 모듈입니다. Aspire dashboard tab, Tools menu entry, status bar item을 등록합니다. `AspireConfig.AutoStart`가 true이면 모듈 로드 중 Aspire service를 시작합니다.

`Translator`는 sidecar 스타일 모듈입니다. 큰 UI를 노출하지 않고 `ICommandlineResourcesProvider`에 command-line resource를 등록하여 Aspire 모듈이 LibreTranslate를 시작할 수 있게 합니다.

## Development 및 Demo 모듈

`Mock`과 `BlazorDemo`는 framework behavior 개발과 검증에 유용합니다. `Mock`은 알림, 대화상자, task manager behavior, CLI integration을 위한 demo tabs와 sample views를 등록합니다. 명확히 진단 또는 샘플 용도가 아니라면 production feature를 Mock에 두지 마세요.

## 불완전하거나 모듈이 아닌 디렉터리

실제 module project와 `Module.cs`를 가진 디렉터리만 active built-in module로 취급해야 합니다. `obj`, 생성 파일, `UI` 하위 폴더만 있는 폴더는 runtime discovery의 근거가 되지 않습니다.

모듈 로딩을 문서화하거나 문제를 조사할 때는 폴더 이름만 보지 말고 compiled output과 `Module.cs`에서 시작하세요.

<!--doc-l10n:end-->
