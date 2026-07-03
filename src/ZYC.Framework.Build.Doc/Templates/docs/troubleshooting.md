<p align="center">
  <a href="./troubleshooting.md">English</a> |
  <a href="./troubleshooting.ja.md">日本語</a> |
  <a href="./troubleshooting.zh-CN.md">简体中文</a> |
  <a href="./troubleshooting.zh-TW.md">繁體中文</a> |
  <a href="./troubleshooting.ko.md">한국어</a> |
</p>

<!--doc-l10n:begin troubleshooting-content-->
# Troubleshooting

This page lists the most common failure points in a ZYC.Framework host or module project. Start from the visible symptom, then check the layer that owns that behavior.

## Quick Triage

| Symptom | First checks |
| --- | --- |
| `zyc` is not found | Install or update the global `ZYC.Framework.CLI` tool, then open a new shell and run `zyc new --help`. |
| `zyc new` fails | Check the project name, `--template`, `--output`, `--package-version`, and whether the target folder already contains files. Use `--overwrite` only when replacing generated files is intentional. |
| A module is missing | Check that the runtime DLL exists in the app directory, matches `ZYC.Framework.Modules*.dll` or is listed in `ModuleConfig.AdditionalAssemblyNames`, and contains a `ModuleBase` entrypoint. |
| A module is discovered but not loaded | Check `ModuleConfig.DisabledAssemblyNames`. Disabled modules are registered as module info but `LoadAsync` is skipped. |
| The module load error page opens | Inspect the module name, exception, and function name. The failure happened during `LoadAsync` or `AfterLoadedAsync`. |
| A menu item is missing | Check that the owning module loaded, registered the correct menu provider, and did not mark the item hidden. |
| Navigation opens Not Found | No `ITabItemFactory` matched the URI. Check route attributes, factory registration, and the URI passed to `ITabManager.NavigateAsync(...)`. |
| Navigation opens an error tab | A factory, tab item, view, or tab `LoadAsync` failed. Check the exception shown by the error tab and logs. |
| A NuGet-installed module is not active | Reinstall or update it from ModuleManager, confirm `settings/nuget.module.assets.json` was written, then restart the host. |
| Aspire resources do not appear | Check that the module registers `IExtensionResourcesProvider` or `ICommandlineResourcesProvider`, and that Aspire is enabled or started. |
| The embedded terminal fails | Confirm the terminal native DLLs are copied to the expected `runtimes` folders in the output directory. |
| Documentation changes do not appear | Edit files under `src/ZYC.Framework.Build.Doc/Templates`, then regenerate the published docs if needed. |

## CLI and Project Creation

The recommended creation flow uses the global dotnet tool:

```bash
dotnet tool install --global ZYC.Framework.CLI --version $(Version)
dotnet tool update --global ZYC.Framework.CLI --version $(Version)
zyc new MyCompany.Tools --template minimal
```

If `zyc` is not available after installing the tool:

- open a new terminal so the updated tool path is applied;
- run `dotnet tool list --global` and confirm `ZYC.Framework.CLI` is installed;
- run `zyc new --help` to verify that the CLI command resolves.

If project creation fails:

- use a valid dotted C# project name such as `MyCompany.Tools`;
- use `--template minimal` for a host-only project or `--template modular` for a host plus module split;
- use `--output` when the target folder should not be derived from the project name;
- use `--package-version` when the generated project must reference a specific package version;
- use `--overwrite` only when replacing existing generated files is expected.

## Module Discovery

At startup, the host discovers module assemblies from the application directory. Standard built-in module DLLs are matched by file name, and additional modules can be listed in `ModuleConfig.AdditionalAssemblyNames`:

```json
{
  "AdditionalAssemblyNames": [
    "MyCompany.Tools.dll"
  ],
  "DisabledAssemblyNames": []
}
```

If a module is not discovered:

- make sure the DLL is in the application directory;
- make sure the DLL name is either matched by the standard `ZYC.Framework.Modules*.dll` pattern or listed in `AdditionalAssemblyNames`;
- make sure the assembly contains a concrete type derived from `ModuleBase`;
- do not list only an `*.Abstractions` assembly, because abstractions projects define contracts but are not runtime module entrypoints.

If a module appears in module information but never loads:

- check whether its DLL file name is present in `DisabledAssemblyNames`;
- remember that disabled modules are still discovered, but `LoadAsync` is not called;
- remove the file name from `DisabledAssemblyNames` or re-enable it through ModuleManager, then restart if the running host does not reload that module dynamically.

## Module Load Errors

The host records load failures from two phases:

- `LoadAsync`, where the module normally registers menus, tabs, status items, and runtime services;
- `AfterLoadedAsync`, where the module can run work that depends on other loaded modules.

When the module load error page opens, use the displayed module name, function name, and exception as the starting point. `AppConfig.SuppressModuleLoadError` can suppress the page, but it does not fix the underlying failure.

Typical causes:

- a required service was not registered before the module resolved it;
- a view or tab item constructor throws during registration or startup navigation;
- a module assumes another module is enabled, but that dependency is disabled or missing;
- a local file, native DLL, or external process expected by the module is missing.

## Menus, Tabs, and Routing

Menu entries are usually registered from a module's `LoadAsync`. If a menu item is missing:

- confirm the module itself loaded without error;
- register the item under the correct provider, such as File, Tools, Extensions, About, or Settings;
- check whether the item is hidden by state or configuration;
- check ordering only after the item is visible, because priority and anchors do not create the item by themselves.

Tab navigation depends on `ITabItemFactory`. If navigation opens Not Found:

- check that the factory is registered in the loaded assembly;
- check `TabItemRoute` scheme, host, and path against the URI being navigated to;
- check factory priority when a generic route, such as a file preview route, can match before a more specific route;
- check whether a single-instance tab is reusing an existing tab instead of opening a new one.

If navigation opens an error tab, the route matched but creation or loading failed. Check the exception from the error tab and then inspect the factory, tab item constructor, view constructor, and tab `LoadAsync`.

## Workspace and Restore Timing

Startup navigation, protocol-forwarded navigation, and restore-time module actions should wait until the workspace and tab restore pipeline is ready. If a tab is opened in the wrong workspace or disappears after restore:

- run startup navigation after `TabManagerRestoreCompleted`;
- use the focused workspace when the action is user-driven;
- use an explicit workspace id when restoring or forwarding a known workspace target;
- move, create, and close tabs through `ITabManager` instead of mutating UI collections directly.

## Config and State

Concrete `IConfig` and `IState` types are loaded from the settings directory while the module assembly is registered. If settings are not read or saved:

- confirm the config or state type is concrete and lives in a loaded runtime assembly;
- confirm the module assembly is discovered before expecting its config or state type to exist;
- confirm the settings file is under the host settings directory, not the source tree;
- do not put only the contract type in an abstractions assembly and expect runtime state to be created from it.

## Single Instance and Mutex Override

The host derives the single-instance mutex id from product information unless `settings/mutex-id.override` exists. Use Tools > Override Mutex Id to create, update, or delete that file.

After changing the override, restart the host. The mutex and startup URI pipe name are created during startup, so a running process does not switch identities immediately. If side-by-side instances, startup URI forwarding, or foreground-window activation behave unexpectedly, check the current `mutex-id.override` file first.

## NuGet Modules

ModuleManager installs NuGet modules by restoring a temporary project and writing the resolved runtime asset graph to `settings/nuget.module.assets.json`. The host reads that file during the next startup.

If a NuGet module was installed but is not active:

- check that restore succeeded and the assets file exists under `settings`;
- restart the host so startup discovery can load the runtime assemblies;
- confirm the package contains a compatible runtime assembly for the host target, currently `net10.0-windows`;
- check whether the installed module assembly is disabled in `ModuleConfig.DisabledAssemblyNames`;
- reinstall or remove and install again if the assets file points to stale package content.

If a known package does not appear in search results, remember that NuGet search runs before `IncludeRegex`. A package missing from the returned NuGet page never reaches the regex filter. Check `NuGetModuleConfig.SearchTerm`, `SearchSkip`, and `SearchTake`; `SearchTake` is clamped to the NuGet.org single-request limit of 1000, so use `SearchSkip` for later pages.

Install, uninstall, and refresh share the same module-assets pipeline and are serialized by the ModuleManager operation coordinator. If those commands appear disabled, wait for the current restore/search operation to finish before starting another one.

## Aspire and Sidecar Resources

Aspire resources are contributed by modules through extension providers. A module can register `IExtensionResourcesProvider` for direct Aspire builder customization, or `ICommandlineResourcesProvider` for a command-line sidecar resource.

If a resource does not appear:

- confirm the providing module loaded before Aspire builds the resource graph;
- confirm the provider type is registered in the module assembly;
- for command-line resources, confirm the resource name, working directory, and command are valid;
- check whether `AspireConfig.AutoStart` is disabled and start Aspire manually if needed.

If the Aspire dashboard cannot open, check that the Aspire process produced `ASPNETCORE_URLS` and `AppHost:BrowserToken`. The dashboard URI is built from those values.

## CLI Terminal Native Dependencies

The CLI module loads terminal native dependencies from the application output. If the embedded terminal fails early, confirm these files exist:

```text
runtimes\win10-x64\native\conpty.dll
runtimes\win-x64\native\Microsoft.Terminal.Control.dll
```

If they are missing, inspect the package output and copy-local behavior for the CLI module and terminal dependency.

## Documentation Templates

The documentation source used by `ZYC.Framework.Build.Doc` lives under:

```text
src\ZYC.Framework.Build.Doc\Templates
```

If editing a generated root `docs` file appears to work locally but disappears later, move the change into the matching template file and regenerate the docs.

<!--doc-l10n:locale ja-->
# トラブルシューティング

このページは、ZYC.Framework のホストまたはモジュール プロジェクトでよく発生する失敗点をまとめたものです。まず見えている症状から始め、その動作を担当する層を確認してください。

## クイック トリアージ

| 症状 | 最初に確認すること |
| --- | --- |
| `zyc` が見つからない | グローバル `ZYC.Framework.CLI` ツールをインストールまたは更新し、新しいシェルで `zyc new --help` を実行します。 |
| `zyc new` が失敗する | プロジェクト名、`--template`、`--output`、`--package-version`、対象フォルダーに既存ファイルがあるかを確認します。`--overwrite` は置き換えが意図されている場合だけ使います。 |
| モジュールが見つからない | ランタイム DLL が app ディレクトリにあり、`ZYC.Framework.Modules*.dll` に一致するか `ModuleConfig.AdditionalAssemblyNames` に列挙され、`ModuleBase` エントリポイントを含むか確認します。 |
| モジュールは発見されるがロードされない | `ModuleConfig.DisabledAssemblyNames` を確認します。無効化されたモジュールは情報として登録されますが、`LoadAsync` はスキップされます。 |
| モジュール ロード エラー ページが開く | モジュール名、例外、関数名を確認します。失敗は `LoadAsync` または `AfterLoadedAsync` で発生しています。 |
| メニュー項目が表示されない | 所有モジュールがロードされ、正しいメニュー プロバイダーに登録され、項目が hidden になっていないか確認します。 |
| ナビゲーションが Not Found を開く | URI に一致する `ITabItemFactory` がありません。ルート属性、ファクトリ登録、`ITabManager.NavigateAsync(...)` に渡す URI を確認します。 |
| ナビゲーションがエラー タブを開く | ファクトリ、タブ項目、ビュー、またはタブの `LoadAsync` が失敗しています。エラー タブとログの例外を確認します。 |
| NuGet インストール済みモジュールが有効にならない | ModuleManager から再インストールまたは更新し、`settings/nuget.module.assets.json` が書き込まれたことを確認してからホストを再起動します。 |
| Aspire リソースが表示されない | モジュールが `IExtensionResourcesProvider` または `ICommandlineResourcesProvider` を登録しているか、Aspire が有効または開始済みか確認します。 |
| 組み込みターミナルが失敗する | ターミナルの native DLL が出力ディレクトリの想定された `runtimes` フォルダーにコピーされているか確認します。 |
| ドキュメント変更が反映されない | `src/ZYC.Framework.Build.Doc/Templates` 配下を編集し、必要に応じて公開ドキュメントを再生成します。 |

## CLI とプロジェクト作成

推奨される作成フローは、グローバル dotnet ツールを使います。

```bash
dotnet tool install --global ZYC.Framework.CLI --version $(Version)
dotnet tool update --global ZYC.Framework.CLI --version $(Version)
zyc new MyCompany.Tools --template minimal
```

インストール後も `zyc` を使えない場合:

- 新しいターミナルを開き、更新された tool path を反映します。
- `dotnet tool list --global` を実行し、`ZYC.Framework.CLI` がインストール済みであることを確認します。
- `zyc new --help` を実行し、CLI コマンドが解決されることを確認します。

プロジェクト作成が失敗する場合:

- `MyCompany.Tools` のような有効なドット区切りの C# プロジェクト名を使います。
- ホストだけなら `--template minimal`、ホストとモジュール分割なら `--template modular` を使います。
- 対象フォルダーをプロジェクト名から導出したくない場合は `--output` を使います。
- 生成プロジェクトで特定のパッケージ バージョンを参照する場合は `--package-version` を使います。
- 既存の生成ファイルを置き換える意図がある場合だけ `--overwrite` を使います。

## モジュール発見

起動時、ホストはアプリケーション ディレクトリからモジュール アセンブリを発見します。標準の組み込みモジュール DLL はファイル名で照合され、追加モジュールは `ModuleConfig.AdditionalAssemblyNames` に列挙できます。

```json
{
  "AdditionalAssemblyNames": [
    "MyCompany.Tools.dll"
  ],
  "DisabledAssemblyNames": []
}
```

モジュールが発見されない場合:

- DLL がアプリケーション ディレクトリにあることを確認します。
- DLL 名が標準の `ZYC.Framework.Modules*.dll` パターンに一致するか、`AdditionalAssemblyNames` に列挙されていることを確認します。
- アセンブリに `ModuleBase` から派生した具象型が含まれることを確認します。
- `*.Abstractions` アセンブリだけを列挙しないでください。Abstractions プロジェクトは契約を定義するだけで、ランタイム モジュールのエントリポイントではありません。

モジュール情報には表示されるがロードされない場合:

- DLL ファイル名が `DisabledAssemblyNames` に含まれていないか確認します。
- 無効化されたモジュールは発見されますが、`LoadAsync` は呼ばれません。
- `DisabledAssemblyNames` からファイル名を削除するか、ModuleManager で再有効化し、実行中ホストが動的リロードしない場合は再起動します。

## モジュール ロード エラー

ホストは 2 つのフェーズのロード失敗を記録します。

- `LoadAsync`: 通常、メニュー、タブ、ステータス項目、ランタイム サービスを登録します。
- `AfterLoadedAsync`: 他のモジュールのロード後に依存する処理を実行できます。

モジュール ロード エラー ページが開いたら、表示されるモジュール名、関数名、例外から調査します。`AppConfig.SuppressModuleLoadError` はページを抑制できますが、根本原因は修正しません。

典型的な原因:

- 必要なサービスが解決される前に登録されていない。
- ビューまたはタブ項目のコンストラクターが、登録時または起動ナビゲーション時に例外を投げる。
- モジュールが別のモジュールの有効化を前提にしているが、その依存モジュールが無効または欠落している。
- モジュールが期待するローカル ファイル、native DLL、外部プロセスが欠落している。

## メニュー、タブ、ルーティング

メニュー項目は通常、モジュールの `LoadAsync` から登録されます。メニュー項目が表示されない場合:

- モジュール自体がエラーなくロードされたことを確認します。
- File、Tools、Extensions、About、Settings など正しいプロバイダーに登録します。
- 状態または構成で項目が hidden になっていないか確認します。
- priority と anchor は項目を作成しないため、順序確認は項目が見えるようになってから行います。

タブ ナビゲーションは `ITabItemFactory` に依存します。Not Found が開く場合:

- ファクトリがロード済みアセンブリに登録されていることを確認します。
- `TabItemRoute` の scheme、host、path がナビゲーション URI と一致することを確認します。
- ファイル プレビューのような汎用ルートが、より具体的なルートより先に一致し得る場合は factory priority を確認します。
- single-instance タブが新規作成ではなく既存タブを再利用していないか確認します。

エラー タブが開く場合、ルートには一致していますが、作成またはロードが失敗しています。エラー タブの例外を確認し、ファクトリ、タブ項目コンストラクター、ビュー コンストラクター、タブ `LoadAsync` を調べます。

## ワークスペースと復元タイミング

起動ナビゲーション、プロトコル転送ナビゲーション、復元時のモジュール処理は、ワークスペースとタブ復元パイプラインの準備完了を待つ必要があります。タブが別ワークスペースで開く、または復元後に消える場合:

- `TabManagerRestoreCompleted` の後で起動ナビゲーションを実行します。
- ユーザー操作の場合はフォーカス中のワークスペースを使います。
- 復元または既知のワークスペースへの転送では明示的な workspace id を使います。
- タブの移動、作成、クローズは UI コレクションを直接変更せず、`ITabManager` 経由で行います。

## Config と State

具象 `IConfig` と `IState` 型は、モジュール アセンブリ登録時に settings ディレクトリからロードされます。設定が読み書きされない場合:

- config または state 型が具象型で、ロード済みランタイム アセンブリに存在することを確認します。
- config または state 型を期待する前に、モジュール アセンブリが発見されていることを確認します。
- settings ファイルがソース ツリーではなく、ホストの settings ディレクトリにあることを確認します。
- contract 型だけを abstractions アセンブリに置いて、そこからランタイム state が作成されると期待しないでください。

## シングル インスタンスと Mutex override

`settings/mutex-id.override` が存在しない限り、ホストは製品情報から single-instance mutex id を作ります。Tools > Override Mutex Id でこのファイルを作成、更新、削除できます。

Override を変更した後はホストを再起動してください。Mutex と startup URI pipe 名は起動時に作られるため、実行中のプロセスはすぐには identity を切り替えません。Side-by-side instance、startup URI forwarding、foreground-window activation が想定外に動く場合は、まず現在の `mutex-id.override` を確認します。

## NuGet モジュール

ModuleManager は一時プロジェクトを restore し、解決された runtime asset graph を `settings/nuget.module.assets.json` に書き込んで NuGet モジュールをインストールします。ホストは次回起動時にそのファイルを読み取ります。

NuGet モジュールをインストールしたのに有効にならない場合:

- restore が成功し、assets ファイルが `settings` 配下に存在することを確認します。
- 起動時発見で runtime assembly をロードできるよう、ホストを再起動します。
- パッケージが現在のホスト ターゲットである `net10.0-windows` と互換の runtime assembly を含むことを確認します。
- インストール済みモジュール アセンブリが `ModuleConfig.DisabledAssemblyNames` で無効化されていないか確認します。
- assets ファイルが古いパッケージ内容を指す場合は、再インストールまたは削除後に再インストールします。

既知のパッケージが検索結果に出ない場合、NuGet search は `IncludeRegex` より先に実行される点に注意してください。返された NuGet page に含まれないパッケージは regex filter に届きません。`NuGetModuleConfig.SearchTerm`、`SearchSkip`、`SearchTake` を確認します。`SearchTake` は NuGet.org の 1 request 上限である 1000 に clamp されるため、後続ページには `SearchSkip` を使います。

Install、uninstall、refresh は同じ module-assets pipeline を共有し、ModuleManager operation coordinator により直列化されます。これらの command が無効に見える場合は、現在の restore/search operation が完了するまで待ってから次の操作を開始してください。

## Aspire とサイドカー リソース

Aspire リソースは、モジュールの extension provider から提供されます。モジュールは直接 Aspire builder をカスタマイズする `IExtensionResourcesProvider`、またはコマンドライン サイドカー用の `ICommandlineResourcesProvider` を登録できます。

リソースが表示されない場合:

- Aspire が resource graph を構築する前に、提供元モジュールがロードされたことを確認します。
- provider 型がモジュール アセンブリに登録されていることを確認します。
- コマンドライン リソースでは、リソース名、作業ディレクトリ、コマンドが有効であることを確認します。
- `AspireConfig.AutoStart` が無効な場合は、必要に応じて Aspire を手動で開始します。

Aspire dashboard が開けない場合は、Aspire プロセスが `ASPNETCORE_URLS` と `AppHost:BrowserToken` を生成したか確認します。dashboard URI はこれらの値から構築されます。

## CLI ターミナルの native 依存

CLI モジュールは、アプリケーション出力からターミナル native 依存をロードします。組み込みターミナルが早期に失敗する場合、次のファイルが存在することを確認してください。

```text
runtimes\win10-x64\native\conpty.dll
runtimes\win-x64\native\Microsoft.Terminal.Control.dll
```

欠落している場合は、CLI モジュールとターミナル依存の package output および copy-local 動作を確認します。

## ドキュメント テンプレート

`ZYC.Framework.Build.Doc` が使うドキュメント ソースは次の場所です。

```text
src\ZYC.Framework.Build.Doc\Templates
```

生成済みの root `docs` ファイルを編集すると一時的には効いたように見えても、後で消える可能性があります。対応する template file に変更を移し、ドキュメントを再生成してください。

<!--doc-l10n:locale zh-CN-->
# 故障排查

本页汇总 ZYC.Framework Host 或模块项目里最常见的失败点。先从可见症状入手，再检查负责该行为的层。

## 快速定位

| 症状 | 优先检查 |
| --- | --- |
| 找不到 `zyc` | 安装或更新全局 `ZYC.Framework.CLI` 工具，然后打开新 shell 执行 `zyc new --help`。 |
| `zyc new` 失败 | 检查项目名、`--template`、`--output`、`--package-version`，以及目标目录是否已有文件。只有明确要替换生成文件时才使用 `--overwrite`。 |
| 模块缺失 | 检查运行时 DLL 是否在 app 目录中，是否匹配 `ZYC.Framework.Modules*.dll` 或列在 `ModuleConfig.AdditionalAssemblyNames` 中，并且包含 `ModuleBase` 入口。 |
| 模块被发现但未加载 | 检查 `ModuleConfig.DisabledAssemblyNames`。禁用模块会注册为模块信息，但会跳过 `LoadAsync`。 |
| 打开模块加载错误页 | 查看模块名、异常和函数名。失败发生在 `LoadAsync` 或 `AfterLoadedAsync`。 |
| 菜单项缺失 | 检查所属模块是否已加载，是否注册到正确菜单 provider，以及菜单项是否被隐藏。 |
| 导航打开 Not Found | 没有 `ITabItemFactory` 匹配该 URI。检查路由特性、factory 注册，以及传给 `ITabManager.NavigateAsync(...)` 的 URI。 |
| 导航打开错误页 | factory、tab item、view 或 tab 的 `LoadAsync` 失败。查看错误页和日志中的异常。 |
| NuGet 安装的模块未生效 | 在 ModuleManager 中重新安装或更新，确认写入 `settings/nuget.module.assets.json`，然后重启 Host。 |
| Aspire 资源不显示 | 检查模块是否注册 `IExtensionResourcesProvider` 或 `ICommandlineResourcesProvider`，以及 Aspire 是否已启用或启动。 |
| 内嵌终端失败 | 确认终端 native DLL 已复制到输出目录中预期的 `runtimes` 文件夹。 |
| 文档修改不显示 | 修改 `src/ZYC.Framework.Build.Doc/Templates` 下的文件，必要时再重新生成发布文档。 |

## CLI 与项目创建

推荐的创建流程使用全局 dotnet tool：

```bash
dotnet tool install --global ZYC.Framework.CLI --version $(Version)
dotnet tool update --global ZYC.Framework.CLI --version $(Version)
zyc new MyCompany.Tools --template minimal
```

安装后仍然无法使用 `zyc` 时：

- 打开新的终端，让更新后的 tool path 生效；
- 执行 `dotnet tool list --global`，确认已安装 `ZYC.Framework.CLI`；
- 执行 `zyc new --help`，确认 CLI 命令可以解析。

项目创建失败时：

- 使用合法的点分 C# 项目名，例如 `MyCompany.Tools`；
- 只需要 Host 时使用 `--template minimal`，需要 Host + Module 拆分时使用 `--template modular`；
- 不希望目标目录从项目名推导时使用 `--output`；
- 生成项目需要引用指定包版本时使用 `--package-version`；
- 只有明确要替换已有生成文件时才使用 `--overwrite`。

## 模块发现

启动时，Host 会从应用目录发现模块程序集。标准内置模块 DLL 按文件名匹配，额外模块可以列在 `ModuleConfig.AdditionalAssemblyNames` 中：

```json
{
  "AdditionalAssemblyNames": [
    "MyCompany.Tools.dll"
  ],
  "DisabledAssemblyNames": []
}
```

如果模块没有被发现：

- 确认 DLL 位于应用目录；
- 确认 DLL 名称匹配标准 `ZYC.Framework.Modules*.dll` 模式，或已列在 `AdditionalAssemblyNames` 中；
- 确认程序集包含继承自 `ModuleBase` 的具体类型；
- 不要只列出 `*.Abstractions` 程序集，因为 Abstractions 项目只定义契约，不是运行时模块入口。

如果模块出现在模块信息中但一直不加载：

- 检查 DLL 文件名是否存在于 `DisabledAssemblyNames`；
- 记住禁用模块仍会被发现，但不会调用 `LoadAsync`；
- 从 `DisabledAssemblyNames` 中移除文件名，或通过 ModuleManager 重新启用；如果运行中的 Host 不会动态重载该模块，则需要重启。

## 模块加载错误

Host 会记录两个阶段的加载失败：

- `LoadAsync`：模块通常在这里注册菜单、Tab、状态项和运行时服务；
- `AfterLoadedAsync`：模块可以在这里执行依赖其他已加载模块的工作。

模块加载错误页打开时，从显示的模块名、函数名和异常开始排查。`AppConfig.SuppressModuleLoadError` 可以隐藏该页面，但不会修复底层失败。

常见原因：

- 模块解析所需服务之前，该服务还没有注册；
- view 或 tab item 构造函数在注册或启动导航期间抛出异常；
- 模块假定另一个模块已启用，但依赖模块被禁用或缺失；
- 模块所需的本地文件、native DLL 或外部进程不存在。

## 菜单、Tab 与路由

菜单项通常由模块的 `LoadAsync` 注册。菜单项缺失时：

- 确认模块本身已无错误加载；
- 将菜单项注册到正确 provider，例如 File、Tools、Extensions、About 或 Settings；
- 检查菜单项是否因状态或配置被隐藏；
- priority 和 anchor 不会创建菜单项，因此先确认菜单项可见，再排查排序。

Tab 导航依赖 `ITabItemFactory`。如果打开 Not Found：

- 检查 factory 是否已在加载的程序集中注册；
- 检查 `TabItemRoute` 的 scheme、host、path 是否与导航 URI 一致；
- 当文件预览这类通用路由可能先于更具体路由匹配时，检查 factory priority；
- 检查 single-instance tab 是否复用了现有 tab，而不是打开新 tab。

如果打开错误页，说明路由已经匹配，但创建或加载失败。先查看错误页中的异常，再检查 factory、tab item 构造函数、view 构造函数和 tab `LoadAsync`。

## Workspace 与恢复时机

启动导航、协议转发导航、恢复期模块动作都应该等 workspace 与 tab 恢复管线准备完成。如果 tab 打开到错误 workspace，或恢复后消失：

- 在 `TabManagerRestoreCompleted` 之后执行启动导航；
- 用户触发的动作使用当前聚焦 workspace；
- 恢复或转发到已知目标时使用显式 workspace id；
- 通过 `ITabManager` 移动、创建和关闭 tab，不要直接修改 UI 集合。

## Config 与 State

具体的 `IConfig` 和 `IState` 类型会在模块程序集注册期间从 settings 目录加载。设置无法读写时：

- 确认 config 或 state 类型是具体类型，并位于已加载的运行时程序集中；
- 在期望 config 或 state 类型存在之前，确认模块程序集已被发现；
- 确认 settings 文件位于 Host 的 settings 目录，而不是源码树；
- 不要只把契约类型放在 abstractions 程序集中，然后期待它生成运行时 state。

## 单实例与 Mutex Override

如果 `settings/mutex-id.override` 不存在，Host 会根据产品信息派生 single-instance mutex id。可以通过 Tools > Override Mutex Id 创建、更新或删除这个文件。

修改 override 后需要重启 Host。Mutex 和 startup URI pipe name 都在启动时创建，运行中的进程不会立即切换 identity。如果 side-by-side instances、startup URI forwarding 或 foreground-window activation 行为异常，先检查当前 `mutex-id.override` 文件。

## NuGet 模块

ModuleManager 通过 restore 临时项目来安装 NuGet 模块，并把解析后的 runtime asset graph 写入 `settings/nuget.module.assets.json`。Host 会在下一次启动时读取该文件。

如果 NuGet 模块已安装但未生效：

- 检查 restore 是否成功，assets 文件是否存在于 `settings` 下；
- 重启 Host，让启动发现流程加载 runtime assemblies；
- 确认包包含与当前 Host 目标 `net10.0-windows` 兼容的 runtime assembly；
- 检查已安装模块程序集是否被 `ModuleConfig.DisabledAssemblyNames` 禁用；
- 如果 assets 文件指向过期包内容，重新安装，或删除后再安装。

如果已知包没有出现在搜索结果里，注意 NuGet search 会先于 `IncludeRegex` 执行。没有进入返回页的包不会到达 regex filter。检查 `NuGetModuleConfig.SearchTerm`、`SearchSkip` 和 `SearchTake`；`SearchTake` 会被 clamp 到 NuGet.org 单次请求上限 1000，后续页请使用 `SearchSkip`。

Install、uninstall 和 refresh 共用同一条 module-assets pipeline，并由 ModuleManager operation coordinator 串行化。如果这些 command 看起来不可用，先等待当前 restore/search operation 结束，再开始下一次操作。

## Aspire 与 Sidecar 资源

Aspire 资源由模块通过 extension provider 贡献。模块可以注册 `IExtensionResourcesProvider` 来直接定制 Aspire builder，也可以注册 `ICommandlineResourcesProvider` 来提供命令行 sidecar 资源。

资源不显示时：

- 确认提供资源的模块在 Aspire 构建 resource graph 前已加载；
- 确认 provider 类型已在模块程序集中注册；
- 对命令行资源，确认资源名、工作目录和命令有效；
- 如果 `AspireConfig.AutoStart` 被禁用，按需手动启动 Aspire。

如果 Aspire dashboard 无法打开，检查 Aspire 进程是否生成 `ASPNETCORE_URLS` 和 `AppHost:BrowserToken`。dashboard URI 会从这些值构造。

## CLI 终端 native 依赖

CLI 模块会从应用输出目录加载终端 native 依赖。内嵌终端早期失败时，确认这些文件存在：

```text
runtimes\win10-x64\native\conpty.dll
runtimes\win-x64\native\Microsoft.Terminal.Control.dll
```

如果文件缺失，检查 CLI 模块和终端依赖的 package output 与 copy-local 行为。

## 文档模板

`ZYC.Framework.Build.Doc` 使用的文档源在：

```text
src\ZYC.Framework.Build.Doc\Templates
```

如果直接修改生成后的根目录 `docs` 文件，看起来本地有效但后来消失，请把变更移到对应模板文件中，再重新生成文档。

<!--doc-l10n:locale zh-TW-->
# 故障排查

本頁彙整 ZYC.Framework Host 或模組專案裡最常見的失敗點。先從可見症狀入手，再檢查負責該行為的層。

## 快速定位

| 症狀 | 優先檢查 |
| --- | --- |
| 找不到 `zyc` | 安裝或更新全域 `ZYC.Framework.CLI` 工具，然後開啟新的 shell 執行 `zyc new --help`。 |
| `zyc new` 失敗 | 檢查專案名稱、`--template`、`--output`、`--package-version`，以及目標目錄是否已有檔案。只有明確要替換產生檔案時才使用 `--overwrite`。 |
| 模組缺失 | 檢查執行時 DLL 是否在 app 目錄中，是否符合 `ZYC.Framework.Modules*.dll` 或列在 `ModuleConfig.AdditionalAssemblyNames` 中，並且包含 `ModuleBase` 入口。 |
| 模組被發現但未載入 | 檢查 `ModuleConfig.DisabledAssemblyNames`。停用模組會註冊為模組資訊，但會略過 `LoadAsync`。 |
| 開啟模組載入錯誤頁 | 查看模組名稱、例外和函式名稱。失敗發生在 `LoadAsync` 或 `AfterLoadedAsync`。 |
| 選單項缺失 | 檢查所屬模組是否已載入，是否註冊到正確選單 provider，以及選單項是否被隱藏。 |
| 導航開啟 Not Found | 沒有 `ITabItemFactory` 符合該 URI。檢查路由特性、factory 註冊，以及傳給 `ITabManager.NavigateAsync(...)` 的 URI。 |
| 導航開啟錯誤頁 | factory、tab item、view 或 tab 的 `LoadAsync` 失敗。查看錯誤頁和記錄中的例外。 |
| NuGet 安裝的模組未生效 | 在 ModuleManager 中重新安裝或更新，確認寫入 `settings/nuget.module.assets.json`，然後重新啟動 Host。 |
| Aspire 資源不顯示 | 檢查模組是否註冊 `IExtensionResourcesProvider` 或 `ICommandlineResourcesProvider`，以及 Aspire 是否已啟用或啟動。 |
| 內嵌終端失敗 | 確認終端 native DLL 已複製到輸出目錄中預期的 `runtimes` 資料夾。 |
| 文件修改不顯示 | 修改 `src/ZYC.Framework.Build.Doc/Templates` 下的檔案，必要時再重新產生發佈文件。 |

## CLI 與專案建立

推薦的建立流程使用全域 dotnet tool：

```bash
dotnet tool install --global ZYC.Framework.CLI --version $(Version)
dotnet tool update --global ZYC.Framework.CLI --version $(Version)
zyc new MyCompany.Tools --template minimal
```

安裝後仍然無法使用 `zyc` 時：

- 開啟新的終端，讓更新後的 tool path 生效；
- 執行 `dotnet tool list --global`，確認已安裝 `ZYC.Framework.CLI`；
- 執行 `zyc new --help`，確認 CLI 命令可以解析。

專案建立失敗時：

- 使用合法的點分 C# 專案名稱，例如 `MyCompany.Tools`；
- 只需要 Host 時使用 `--template minimal`，需要 Host + Module 拆分時使用 `--template modular`；
- 不希望目標目錄從專案名稱推導時使用 `--output`；
- 產生專案需要參考指定套件版本時使用 `--package-version`；
- 只有明確要替換已有產生檔案時才使用 `--overwrite`。

## 模組發現

啟動時，Host 會從應用目錄發現模組組件。標準內建模組 DLL 按檔名比對，額外模組可以列在 `ModuleConfig.AdditionalAssemblyNames` 中：

```json
{
  "AdditionalAssemblyNames": [
    "MyCompany.Tools.dll"
  ],
  "DisabledAssemblyNames": []
}
```

如果模組沒有被發現：

- 確認 DLL 位於應用目錄；
- 確認 DLL 名稱符合標準 `ZYC.Framework.Modules*.dll` 模式，或已列在 `AdditionalAssemblyNames` 中；
- 確認組件包含繼承自 `ModuleBase` 的具體型別；
- 不要只列出 `*.Abstractions` 組件，因為 Abstractions 專案只定義契約，不是執行時模組入口。

如果模組出現在模組資訊中但一直不載入：

- 檢查 DLL 檔名是否存在於 `DisabledAssemblyNames`；
- 記住停用模組仍會被發現，但不會呼叫 `LoadAsync`；
- 從 `DisabledAssemblyNames` 中移除檔名，或透過 ModuleManager 重新啟用；如果執行中的 Host 不會動態重載該模組，則需要重新啟動。

## 模組載入錯誤

Host 會記錄兩個階段的載入失敗：

- `LoadAsync`：模組通常在這裡註冊選單、Tab、狀態項和執行時服務；
- `AfterLoadedAsync`：模組可以在這裡執行依賴其他已載入模組的工作。

模組載入錯誤頁開啟時，從顯示的模組名稱、函式名稱和例外開始排查。`AppConfig.SuppressModuleLoadError` 可以隱藏該頁面，但不會修復底層失敗。

常見原因：

- 模組解析所需服務之前，該服務還沒有註冊；
- view 或 tab item 建構函式在註冊或啟動導航期間拋出例外；
- 模組假定另一個模組已啟用，但依賴模組被停用或缺失；
- 模組所需的本機檔案、native DLL 或外部程序不存在。

## 選單、Tab 與路由

選單項通常由模組的 `LoadAsync` 註冊。選單項缺失時：

- 確認模組本身已無錯誤載入；
- 將選單項註冊到正確 provider，例如 File、Tools、Extensions、About 或 Settings；
- 檢查選單項是否因狀態或設定被隱藏；
- priority 和 anchor 不會建立選單項，因此先確認選單項可見，再排查排序。

Tab 導航依賴 `ITabItemFactory`。如果開啟 Not Found：

- 檢查 factory 是否已在載入的組件中註冊；
- 檢查 `TabItemRoute` 的 scheme、host、path 是否與導航 URI 一致；
- 當檔案預覽這類通用路由可能先於更具體路由符合時，檢查 factory priority；
- 檢查 single-instance tab 是否複用了既有 tab，而不是開啟新 tab。

如果開啟錯誤頁，表示路由已經符合，但建立或載入失敗。先查看錯誤頁中的例外，再檢查 factory、tab item 建構函式、view 建構函式和 tab `LoadAsync`。

## Workspace 與還原時機

啟動導航、協定轉發導航、還原期模組動作都應該等 workspace 與 tab 還原管線準備完成。如果 tab 開啟到錯誤 workspace，或還原後消失：

- 在 `TabManagerRestoreCompleted` 之後執行啟動導航；
- 使用者觸發的動作使用目前聚焦 workspace；
- 還原或轉發到已知目標時使用明確 workspace id；
- 透過 `ITabManager` 移動、建立和關閉 tab，不要直接修改 UI 集合。

## Config 與 State

具體的 `IConfig` 和 `IState` 型別會在模組組件註冊期間從 settings 目錄載入。設定無法讀寫時：

- 確認 config 或 state 型別是具體型別，並位於已載入的執行時組件中；
- 在期望 config 或 state 型別存在之前，確認模組組件已被發現；
- 確認 settings 檔案位於 Host 的 settings 目錄，而不是原始碼樹；
- 不要只把契約型別放在 abstractions 組件中，然後期待它產生執行時 state。

## 單一實例與 Mutex Override

如果 `settings/mutex-id.override` 不存在，Host 會根據產品資訊派生 single-instance mutex id。可以透過 Tools > Override Mutex Id 建立、更新或刪除這個檔案。

修改 override 後需要重新啟動 Host。Mutex 和 startup URI pipe name 都在啟動時建立，執行中的程序不會立即切換 identity。如果 side-by-side instances、startup URI forwarding 或 foreground-window activation 行為異常，先檢查目前的 `mutex-id.override` 檔案。

## NuGet 模組

ModuleManager 透過 restore 暫時專案來安裝 NuGet 模組，並把解析後的 runtime asset graph 寫入 `settings/nuget.module.assets.json`。Host 會在下一次啟動時讀取該檔案。

如果 NuGet 模組已安裝但未生效：

- 檢查 restore 是否成功，assets 檔案是否存在於 `settings` 下；
- 重新啟動 Host，讓啟動發現流程載入 runtime assemblies；
- 確認套件包含與目前 Host 目標 `net10.0-windows` 相容的 runtime assembly；
- 檢查已安裝模組組件是否被 `ModuleConfig.DisabledAssemblyNames` 停用；
- 如果 assets 檔案指向過期套件內容，重新安裝，或刪除後再安裝。

如果已知套件沒有出現在搜尋結果裡，注意 NuGet search 會先於 `IncludeRegex` 執行。沒有進入返回頁的套件不會到達 regex filter。檢查 `NuGetModuleConfig.SearchTerm`、`SearchSkip` 與 `SearchTake`；`SearchTake` 會被 clamp 到 NuGet.org 單次請求上限 1000，後續頁請使用 `SearchSkip`。

Install、uninstall 與 refresh 共用同一條 module-assets pipeline，並由 ModuleManager operation coordinator 串行化。如果這些 command 看起來不可用，先等待目前 restore/search operation 結束，再開始下一次操作。

## Aspire 與 Sidecar 資源

Aspire 資源由模組透過 extension provider 貢獻。模組可以註冊 `IExtensionResourcesProvider` 來直接自訂 Aspire builder，也可以註冊 `ICommandlineResourcesProvider` 來提供命令列 sidecar 資源。

資源不顯示時：

- 確認提供資源的模組在 Aspire 建構 resource graph 前已載入；
- 確認 provider 型別已在模組組件中註冊；
- 對命令列資源，確認資源名稱、工作目錄和命令有效；
- 如果 `AspireConfig.AutoStart` 被停用，依需要手動啟動 Aspire。

如果 Aspire dashboard 無法開啟，檢查 Aspire 程序是否產生 `ASPNETCORE_URLS` 和 `AppHost:BrowserToken`。dashboard URI 會從這些值建構。

## CLI 終端 native 依賴

CLI 模組會從應用輸出目錄載入終端 native 依賴。內嵌終端早期失敗時，確認這些檔案存在：

```text
runtimes\win10-x64\native\conpty.dll
runtimes\win-x64\native\Microsoft.Terminal.Control.dll
```

如果檔案缺失，檢查 CLI 模組和終端依賴的 package output 與 copy-local 行為。

## 文件模板

`ZYC.Framework.Build.Doc` 使用的文件來源在：

```text
src\ZYC.Framework.Build.Doc\Templates
```

如果直接修改產生後的根目錄 `docs` 檔案，看起來本地有效但後來消失，請把變更移到對應模板檔案中，再重新產生文件。

<!--doc-l10n:locale ko-->
# 문제 해결

이 페이지는 ZYC.Framework 호스트 또는 모듈 프로젝트에서 자주 발생하는 실패 지점을 정리합니다. 보이는 증상에서 시작한 뒤, 그 동작을 담당하는 계층을 확인하세요.

## 빠른 확인

| 증상 | 먼저 확인할 것 |
| --- | --- |
| `zyc`를 찾을 수 없음 | 전역 `ZYC.Framework.CLI` 도구를 설치하거나 업데이트한 뒤, 새 셸에서 `zyc new --help`를 실행합니다. |
| `zyc new` 실패 | 프로젝트 이름, `--template`, `--output`, `--package-version`, 대상 폴더의 기존 파일 여부를 확인합니다. `--overwrite`는 생성 파일을 교체하려는 의도가 있을 때만 사용합니다. |
| 모듈이 보이지 않음 | 런타임 DLL이 app 디렉터리에 있고, `ZYC.Framework.Modules*.dll`과 일치하거나 `ModuleConfig.AdditionalAssemblyNames`에 있으며, `ModuleBase` 진입점을 포함하는지 확인합니다. |
| 모듈은 발견되지만 로드되지 않음 | `ModuleConfig.DisabledAssemblyNames`를 확인합니다. 비활성 모듈은 모듈 정보로 등록되지만 `LoadAsync`는 건너뜁니다. |
| 모듈 로드 오류 페이지가 열림 | 모듈 이름, 예외, 함수 이름을 확인합니다. 실패는 `LoadAsync` 또는 `AfterLoadedAsync`에서 발생했습니다. |
| 메뉴 항목이 없음 | 소유 모듈이 로드되었고, 올바른 메뉴 provider에 등록되었으며, 항목이 hidden 상태가 아닌지 확인합니다. |
| 탐색이 Not Found를 엶 | URI와 일치하는 `ITabItemFactory`가 없습니다. route attribute, factory 등록, `ITabManager.NavigateAsync(...)`에 전달한 URI를 확인합니다. |
| 탐색이 오류 탭을 엶 | factory, tab item, view 또는 tab `LoadAsync`가 실패했습니다. 오류 탭과 로그의 예외를 확인합니다. |
| NuGet으로 설치한 모듈이 활성화되지 않음 | ModuleManager에서 다시 설치하거나 업데이트하고, `settings/nuget.module.assets.json`이 작성되었는지 확인한 뒤 호스트를 재시작합니다. |
| Aspire 리소스가 보이지 않음 | 모듈이 `IExtensionResourcesProvider` 또는 `ICommandlineResourcesProvider`를 등록했는지, Aspire가 활성화 또는 시작되었는지 확인합니다. |
| 내장 터미널 실패 | 터미널 native DLL이 출력 디렉터리의 예상 `runtimes` 폴더에 복사되었는지 확인합니다. |
| 문서 변경이 보이지 않음 | `src/ZYC.Framework.Build.Doc/Templates` 아래의 파일을 수정하고, 필요하면 게시 문서를 다시 생성합니다. |

## CLI와 프로젝트 생성

권장 생성 흐름은 전역 dotnet tool을 사용합니다.

```bash
dotnet tool install --global ZYC.Framework.CLI --version $(Version)
dotnet tool update --global ZYC.Framework.CLI --version $(Version)
zyc new MyCompany.Tools --template minimal
```

설치 후에도 `zyc`를 사용할 수 없다면:

- 새 터미널을 열어 업데이트된 tool path를 반영합니다.
- `dotnet tool list --global`을 실행해 `ZYC.Framework.CLI`가 설치되었는지 확인합니다.
- `zyc new --help`를 실행해 CLI 명령이 해석되는지 확인합니다.

프로젝트 생성이 실패한다면:

- `MyCompany.Tools`처럼 유효한 점 구분 C# 프로젝트 이름을 사용합니다.
- 호스트만 필요하면 `--template minimal`, 호스트와 모듈 분리가 필요하면 `--template modular`을 사용합니다.
- 대상 폴더를 프로젝트 이름에서 유도하지 않으려면 `--output`을 사용합니다.
- 생성 프로젝트가 특정 패키지 버전을 참조해야 하면 `--package-version`을 사용합니다.
- 기존 생성 파일을 교체하려는 경우에만 `--overwrite`를 사용합니다.

## 모듈 발견

시작 시 호스트는 애플리케이션 디렉터리에서 모듈 어셈블리를 발견합니다. 표준 내장 모듈 DLL은 파일 이름으로 매칭되며, 추가 모듈은 `ModuleConfig.AdditionalAssemblyNames`에 나열할 수 있습니다.

```json
{
  "AdditionalAssemblyNames": [
    "MyCompany.Tools.dll"
  ],
  "DisabledAssemblyNames": []
}
```

모듈이 발견되지 않는다면:

- DLL이 애플리케이션 디렉터리에 있는지 확인합니다.
- DLL 이름이 표준 `ZYC.Framework.Modules*.dll` 패턴과 일치하거나 `AdditionalAssemblyNames`에 있는지 확인합니다.
- 어셈블리에 `ModuleBase`에서 파생된 구체 타입이 있는지 확인합니다.
- `*.Abstractions` 어셈블리만 나열하지 마세요. Abstractions 프로젝트는 계약을 정의할 뿐 런타임 모듈 진입점이 아닙니다.

모듈 정보에는 보이지만 로드되지 않는다면:

- DLL 파일 이름이 `DisabledAssemblyNames`에 있는지 확인합니다.
- 비활성 모듈은 발견되지만 `LoadAsync`가 호출되지 않습니다.
- `DisabledAssemblyNames`에서 파일 이름을 제거하거나 ModuleManager로 다시 활성화하고, 실행 중인 호스트가 해당 모듈을 동적으로 다시 로드하지 않는다면 재시작합니다.

## 모듈 로드 오류

호스트는 두 단계의 로드 실패를 기록합니다.

- `LoadAsync`: 모듈은 보통 여기서 메뉴, 탭, 상태 항목, 런타임 서비스를 등록합니다.
- `AfterLoadedAsync`: 모듈은 다른 모듈이 로드된 뒤에 의존 작업을 실행할 수 있습니다.

모듈 로드 오류 페이지가 열리면 표시된 모듈 이름, 함수 이름, 예외에서 시작하세요. `AppConfig.SuppressModuleLoadError`는 페이지를 숨길 수 있지만 근본 실패를 해결하지는 않습니다.

일반적인 원인:

- 필요한 서비스가 해결되기 전에 등록되지 않았습니다.
- view 또는 tab item 생성자가 등록 또는 시작 탐색 중 예외를 던졌습니다.
- 모듈이 다른 모듈이 활성화되어 있다고 가정하지만, 해당 의존 모듈이 비활성화되었거나 없습니다.
- 모듈이 기대하는 로컬 파일, native DLL, 외부 프로세스가 없습니다.

## 메뉴, 탭, 라우팅

메뉴 항목은 보통 모듈의 `LoadAsync`에서 등록됩니다. 메뉴 항목이 없다면:

- 모듈 자체가 오류 없이 로드되었는지 확인합니다.
- File, Tools, Extensions, About, Settings 같은 올바른 provider에 항목을 등록합니다.
- 항목이 상태나 구성 때문에 hidden 상태인지 확인합니다.
- priority와 anchor는 항목을 생성하지 않으므로, 먼저 항목이 보이는지 확인한 뒤 순서를 봅니다.

탭 탐색은 `ITabItemFactory`에 의존합니다. Not Found가 열린다면:

- factory가 로드된 어셈블리에 등록되었는지 확인합니다.
- `TabItemRoute`의 scheme, host, path가 탐색 URI와 일치하는지 확인합니다.
- 파일 미리보기 같은 일반 route가 더 구체적인 route보다 먼저 매칭될 수 있다면 factory priority를 확인합니다.
- single-instance tab이 새 탭을 여는 대신 기존 탭을 재사용하는지 확인합니다.

오류 탭이 열린다면 route는 일치했지만 생성 또는 로드가 실패한 것입니다. 오류 탭의 예외를 확인한 뒤 factory, tab item 생성자, view 생성자, tab `LoadAsync`를 살펴보세요.

## Workspace와 복원 타이밍

시작 탐색, 프로토콜 전달 탐색, 복원 중 모듈 작업은 workspace와 tab 복원 파이프라인이 준비될 때까지 기다려야 합니다. 탭이 잘못된 workspace에서 열리거나 복원 후 사라진다면:

- `TabManagerRestoreCompleted` 이후 시작 탐색을 실행합니다.
- 사용자 동작은 포커스된 workspace를 사용합니다.
- 복원 또는 알려진 대상 workspace로 전달할 때는 명시적인 workspace id를 사용합니다.
- 탭 이동, 생성, 닫기는 UI 컬렉션을 직접 수정하지 말고 `ITabManager`를 통해 수행합니다.

## Config와 State

구체 `IConfig` 및 `IState` 타입은 모듈 어셈블리가 등록될 때 settings 디렉터리에서 로드됩니다. 설정을 읽거나 저장하지 못한다면:

- config 또는 state 타입이 구체 타입이며 로드된 런타임 어셈블리에 있는지 확인합니다.
- config 또는 state 타입을 기대하기 전에 모듈 어셈블리가 발견되었는지 확인합니다.
- settings 파일이 소스 트리가 아니라 호스트 settings 디렉터리에 있는지 확인합니다.
- 계약 타입만 abstractions 어셈블리에 두고 거기서 런타임 state가 생성되리라 기대하지 마세요.

## 단일 인스턴스와 Mutex Override

`settings/mutex-id.override`가 없으면 호스트는 제품 정보에서 single-instance mutex id를 만듭니다. Tools > Override Mutex Id로 이 파일을 만들거나 업데이트하거나 삭제할 수 있습니다.

Override를 변경한 뒤에는 호스트를 재시작하세요. Mutex와 startup URI pipe name은 시작 시 만들어지므로 실행 중인 프로세스가 즉시 identity를 바꾸지는 않습니다. Side-by-side instances, startup URI forwarding, foreground-window activation이 예상과 다르게 동작하면 먼저 현재 `mutex-id.override` 파일을 확인합니다.

## NuGet 모듈

ModuleManager는 임시 프로젝트를 restore하고 해결된 runtime asset graph를 `settings/nuget.module.assets.json`에 작성해 NuGet 모듈을 설치합니다. 호스트는 다음 시작 시 이 파일을 읽습니다.

NuGet 모듈을 설치했지만 활성화되지 않는다면:

- restore가 성공했고 assets 파일이 `settings` 아래에 있는지 확인합니다.
- 시작 발견 과정에서 runtime assemblies를 로드할 수 있도록 호스트를 재시작합니다.
- 패키지가 현재 호스트 대상인 `net10.0-windows`와 호환되는 runtime assembly를 포함하는지 확인합니다.
- 설치된 모듈 어셈블리가 `ModuleConfig.DisabledAssemblyNames`로 비활성화되었는지 확인합니다.
- assets 파일이 오래된 패키지 내용을 가리킨다면 다시 설치하거나 제거 후 설치합니다.

알려진 패키지가 검색 결과에 보이지 않는다면 NuGet search가 `IncludeRegex`보다 먼저 실행된다는 점을 확인하세요. 반환된 NuGet page에 없는 패키지는 regex filter까지 도달하지 않습니다. `NuGetModuleConfig.SearchTerm`, `SearchSkip`, `SearchTake`를 확인합니다. `SearchTake`는 NuGet.org 단일 요청 한도인 1000으로 clamp되므로 이후 페이지에는 `SearchSkip`을 사용합니다.

Install, uninstall, refresh는 같은 module-assets pipeline을 공유하며 ModuleManager operation coordinator에 의해 직렬화됩니다. 이 command들이 비활성화된 것처럼 보이면 현재 restore/search operation이 끝날 때까지 기다린 뒤 다음 작업을 시작하세요.

## Aspire와 Sidecar 리소스

Aspire 리소스는 모듈의 extension provider를 통해 제공됩니다. 모듈은 Aspire builder를 직접 사용자 지정하는 `IExtensionResourcesProvider` 또는 command-line sidecar 리소스용 `ICommandlineResourcesProvider`를 등록할 수 있습니다.

리소스가 보이지 않는다면:

- Aspire가 resource graph를 만들기 전에 제공 모듈이 로드되었는지 확인합니다.
- provider 타입이 모듈 어셈블리에 등록되었는지 확인합니다.
- command-line 리소스는 리소스 이름, 작업 디렉터리, 명령이 유효한지 확인합니다.
- `AspireConfig.AutoStart`가 비활성화되어 있으면 필요에 따라 Aspire를 수동으로 시작합니다.

Aspire dashboard를 열 수 없다면 Aspire 프로세스가 `ASPNETCORE_URLS`와 `AppHost:BrowserToken`을 생성했는지 확인합니다. dashboard URI는 이 값들로 구성됩니다.

## CLI 터미널 native 의존성

CLI 모듈은 애플리케이션 출력에서 터미널 native 의존성을 로드합니다. 내장 터미널이 초기에 실패한다면 다음 파일이 있는지 확인하세요.

```text
runtimes\win10-x64\native\conpty.dll
runtimes\win-x64\native\Microsoft.Terminal.Control.dll
```

파일이 없다면 CLI 모듈과 터미널 의존성의 package output 및 copy-local 동작을 확인합니다.

## 문서 템플릿

`ZYC.Framework.Build.Doc`가 사용하는 문서 소스는 다음 위치에 있습니다.

```text
src\ZYC.Framework.Build.Doc\Templates
```

생성된 root `docs` 파일을 직접 수정하면 로컬에서는 동작하는 것처럼 보여도 나중에 사라질 수 있습니다. 변경을 해당 template file로 옮긴 뒤 문서를 다시 생성하세요.

<!--doc-l10n:end-->
