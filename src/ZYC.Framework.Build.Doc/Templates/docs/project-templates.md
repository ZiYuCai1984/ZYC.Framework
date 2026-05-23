<p align="center">
  <a href="./project-templates.md">English</a> |
  <a href="./project-templates.ja.md">日本語</a> |
  <a href="./project-templates.zh-CN.md">简体中文</a> |
  <a href="./project-templates.zh-TW.md">繁體中文</a> |
  <a href="./project-templates.ko.md">한국어</a> |
</p>

<!--doc-l10n:begin project-templates-content-->
# Project Templates

ZYC.Framework provides `dotnet tool` commands for two common scaffolding tasks: creating a new host project and adding a new module to an existing source tree. This page documents the templates, generated structure, and command options.

## Command Surface

| Command | Purpose | Best for |
| --- | --- | --- |
| `zyc new <ProjectName>` | Creates a new external ZYC.Framework Host project from a project template. | Starting a new app or sample outside the framework repository. |
| `zyc new-module <ModuleName> --src-root <SourceRoot>` | Creates a module implementation project and matching `*.Abstractions` project inside an existing source tree. | Extending an existing ZYC.Framework-style repository. |

Install or update the CLI as a .NET tool:

```bash
dotnet tool install -g ZYC.Framework.CLI --version $(Version)
dotnet tool update -g ZYC.Framework.CLI --version $(Version)
```

Then verify the command:

```bash
zyc --help
zyc new --help
zyc new-module --help
```

## `minimal` Template

`minimal` is the default template for `zyc new`. It creates a small WPF host project that references `ZYC.Framework.Alpha` and registers one WPF view as a simple tab.

```bash
zyc new MyCompany.Tools
```

Equivalent explicit command:

```bash
zyc new MyCompany.Tools --template minimal
```

Generated structure:

```text
MyCompany.Tools/
  MyCompany.Tools.csproj
  MyCompany.Tools.slnx
  Module.cs
  ModuleConfig.json
  UI/
    ToolsView.xaml
    ToolsView.xaml.cs
```

Use this template when you want the shortest path to a runnable host and the feature surface is one simple WPF view.

## `modular` Template

`modular` creates a small solution with an Entry project, a module implementation project, and a module abstractions project.

```bash
zyc new MyCompany.Tools --template modular
```

Generated structure:

```text
MyCompany.Tools/
  Directory.Build.props
  Directory.Build.targets
  version.props
  MyCompany.Tools.slnx
  Entry/
    Entry.csproj
  ZYC.Framework.Modules.MyCompany.Tools/
    ZYC.Framework.Modules.MyCompany.Tools.csproj
    Module.cs
    ToolsMainMenuItem.cs
    ToolsTabItem.cs
    ToolsTabItemFactory.cs
    UI/
      ToolsView.xaml
      ToolsView.xaml.cs
  ZYC.Framework.Modules.MyCompany.Tools.Abstractions/
    ToolsModuleConstants.cs
    ZYC.Framework.Modules.MyCompany.Tools.Abstractions.csproj
```

Use this template when the feature should look like a real framework module: public constants in Abstractions, a routed tab factory, a main menu item, and a module entrypoint.

## `zyc new` Options

| Option | Description |
| --- | --- |
| `<ProjectName>` | Required project name. It must be a valid dotted C# identifier, for example `Acme.Tools`. |
| `--template`, `-t` | Project template. Supported values are `minimal` and `modular`. Defaults to `minimal`. |
| `--output`, `-o` | Output directory. Defaults to `./<ProjectName>`. |
| `--package-version` | `ZYC.Framework.Alpha` package version. Defaults to the CLI product version. |
| `--overwrite`, `-f` | Overwrite existing files. Without this flag, existing target files fail the generation. |

Example with all common options:

```bash
zyc new Acme.Tools --template modular --output ./Acme.Tools --package-version $(Version)
```

## `new-module` for Existing Source Trees

Use `new-module` when the repository already has a `src` tree and you want to add one module pair.

```bash
zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx
```

The command creates:

```text
src/
  ZYC.Framework.Modules.Reports/
    ZYC.Framework.Modules.Reports.csproj
    Module.cs
    ReportsMainMenuItem.cs
    ReportsTabItem.cs
    ReportsTabItemFactory.cs
    UI/
      ReportsView.xaml
      ReportsView.xaml.cs
  ZYC.Framework.Modules.Reports.Abstractions/
    ReportsModuleConstants.cs
    ZYC.Framework.Modules.Reports.Abstractions.csproj
```

`--slnx` is optional. When provided, the generated projects are added under the `/Modules/` folder in the solution file. Relative `--slnx` paths are resolved from `--src-root`.

`new-module` normalizes the target name. These inputs all generate the same module target:

```bash
zyc new-module Reports --src-root ./src
zyc new-module ZYC.Framework.Modules.Reports --src-root ./src
zyc new-module ZYC.Framework.Modules.Reports.Abstractions --src-root ./src
```

## `new-module` Options

| Option | Description |
| --- | --- |
| `<ModuleName>` | Positional target module name. |
| `--target`, `-t` | Target module name. Use either the positional value or this option, not conflicting values. |
| `--src-root`, `-s` | Required source root where the module projects will be created. |
| `--slnx` | Optional solution XML file to update. Omit it when you do not want solution updates. |
| `--overwrite`, `-f` | Overwrite existing files or module directories. Without this flag, existing target directories fail the generation. |

## Template Tokens

Project templates replace these tokens in paths and text files:

| Token | Value |
| --- | --- |
| `__PROJECT_NAME__` | Full project name, for example `MyCompany.Tools`. |
| `__PROJECT_SHORT_NAME__` | Last dotted segment, for example `Tools`. |
| `__PROJECT_HOST__` | Lowercase short name used as the URI host, for example `tools`. |
| `__PACKAGE_VERSION__` | Package version selected by `--package-version` or the CLI product version. |

Text template files are written as UTF-8 with BOM and normalized to CRLF line endings.

## Choosing a Template

| Situation | Recommended command |
| --- | --- |
| You want the fastest possible host with one view. | `zyc new MyCompany.Tools` |
| You want a module-style solution for a new app. | `zyc new MyCompany.Tools --template modular` |
| You are adding a module to an existing repository. | `zyc new-module Reports --src-root ./src` |
| You need the module projects added to an existing `.slnx`. | `zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx` |

<!--doc-l10n:locale ja-->
# プロジェクト テンプレート

ZYC.Framework は、よく使う 2 つのスキャフォールド作業のために `dotnet tool` コマンドを提供します。新しい Host プロジェクトの作成と、既存ソース ツリーへの新しいモジュール追加です。このページではテンプレート、生成される構成、コマンド オプションを説明します。

## コマンドの範囲

| コマンド | 目的 | 向いている用途 |
| --- | --- | --- |
| `zyc new <ProjectName>` | プロジェクト テンプレートから外部 ZYC.Framework Host プロジェクトを作成する。 | フレームワーク リポジトリ外で新しいアプリやサンプルを始める。 |
| `zyc new-module <ModuleName> --src-root <SourceRoot>` | 既存ソース ツリー内にモジュール実装プロジェクトと対応する `*.Abstractions` プロジェクトを作成する。 | 既存の ZYC.Framework 形式のリポジトリを拡張する。 |

CLI を .NET tool としてインストールまたは更新します。

```bash
dotnet tool install -g ZYC.Framework.CLI --version $(Version)
dotnet tool update -g ZYC.Framework.CLI --version $(Version)
```

コマンドを確認します。

```bash
zyc --help
zyc new --help
zyc new-module --help
```

## `minimal` テンプレート

`minimal` は `zyc new` の既定テンプレートです。`ZYC.Framework.Alpha` を参照する小さな WPF Host プロジェクトを作成し、1 つの WPF ビューを simple tab として登録します。

```bash
zyc new MyCompany.Tools
```

明示的に指定すると次のコマンドと同じです。

```bash
zyc new MyCompany.Tools --template minimal
```

生成される構成:

```text
MyCompany.Tools/
  MyCompany.Tools.csproj
  MyCompany.Tools.slnx
  Module.cs
  ModuleConfig.json
  UI/
    ToolsView.xaml
    ToolsView.xaml.cs
```

最短で実行可能な Host を作り、機能面が 1 つの単純な WPF ビューで足りる場合に使います。

## `modular` テンプレート

`modular` は Entry プロジェクト、モジュール実装プロジェクト、モジュール Abstractions プロジェクトを持つ小さなソリューションを作成します。

```bash
zyc new MyCompany.Tools --template modular
```

生成される構成:

```text
MyCompany.Tools/
  Directory.Build.props
  Directory.Build.targets
  version.props
  MyCompany.Tools.slnx
  Entry/
    Entry.csproj
  ZYC.Framework.Modules.MyCompany.Tools/
    ZYC.Framework.Modules.MyCompany.Tools.csproj
    Module.cs
    ToolsMainMenuItem.cs
    ToolsTabItem.cs
    ToolsTabItemFactory.cs
    UI/
      ToolsView.xaml
      ToolsView.xaml.cs
  ZYC.Framework.Modules.MyCompany.Tools.Abstractions/
    ToolsModuleConstants.cs
    ZYC.Framework.Modules.MyCompany.Tools.Abstractions.csproj
```

公開定数を Abstractions に置き、ルーティング付きタブ ファクトリ、メインメニュー項目、モジュール入口を持つ、実際のフレームワーク モジュールに近い構成が必要な場合に使います。

## `zyc new` オプション

| オプション | 説明 |
| --- | --- |
| `<ProjectName>` | 必須のプロジェクト名。`Acme.Tools` のような有効なドット区切り C# 識別子である必要があります。 |
| `--template`, `-t` | プロジェクト テンプレート。対応値は `minimal` と `modular`。既定は `minimal`。 |
| `--output`, `-o` | 出力ディレクトリ。既定は `./<ProjectName>`。 |
| `--package-version` | `ZYC.Framework.Alpha` のパッケージ バージョン。既定は CLI の製品バージョン。 |
| `--overwrite`, `-f` | 既存ファイルを上書きする。この指定がない場合、対象ファイルが存在すると生成は失敗します。 |

よく使うオプションをすべて指定する例:

```bash
zyc new Acme.Tools --template modular --output ./Acme.Tools --package-version $(Version)
```

## 既存ソース ツリー向けの `new-module`

既に `src` ツリーがあるリポジトリへモジュール ペアを追加する場合は `new-module` を使います。

```bash
zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx
```

このコマンドは次を作成します。

```text
src/
  ZYC.Framework.Modules.Reports/
    ZYC.Framework.Modules.Reports.csproj
    Module.cs
    ReportsMainMenuItem.cs
    ReportsTabItem.cs
    ReportsTabItemFactory.cs
    UI/
      ReportsView.xaml
      ReportsView.xaml.cs
  ZYC.Framework.Modules.Reports.Abstractions/
    ReportsModuleConstants.cs
    ZYC.Framework.Modules.Reports.Abstractions.csproj
```

`--slnx` は任意です。指定すると、生成されたプロジェクトはソリューション ファイルの `/Modules/` フォルダーへ追加されます。相対 `--slnx` パスは `--src-root` から解決されます。

`new-module` はターゲット名を正規化します。次の入力はすべて同じモジュール ターゲットを生成します。

```bash
zyc new-module Reports --src-root ./src
zyc new-module ZYC.Framework.Modules.Reports --src-root ./src
zyc new-module ZYC.Framework.Modules.Reports.Abstractions --src-root ./src
```

## `new-module` オプション

| オプション | 説明 |
| --- | --- |
| `<ModuleName>` | 位置引数のターゲット モジュール名。 |
| `--target`, `-t` | ターゲット モジュール名。位置引数またはこのオプションのどちらかを使い、矛盾する値を指定しないでください。 |
| `--src-root`, `-s` | モジュール プロジェクトを作成する必須のソース ルート。 |
| `--slnx` | 更新する任意の solution XML ファイル。ソリューション更新が不要な場合は省略します。 |
| `--overwrite`, `-f` | 既存ファイルまたはモジュール ディレクトリを上書きする。この指定がない場合、対象ディレクトリが存在すると生成は失敗します。 |

## テンプレート トークン

プロジェクト テンプレートは、パスとテキスト ファイル内の次のトークンを置換します。

| トークン | 値 |
| --- | --- |
| `__PROJECT_NAME__` | 完全なプロジェクト名。例: `MyCompany.Tools`。 |
| `__PROJECT_SHORT_NAME__` | 最後のドット区切りセグメント。例: `Tools`。 |
| `__PROJECT_HOST__` | URI host として使う小文字の短い名前。例: `tools`。 |
| `__PACKAGE_VERSION__` | `--package-version` または CLI 製品バージョンで選択されたパッケージ バージョン。 |

テキスト テンプレート ファイルは UTF-8 with BOM で書き込まれ、CRLF 改行に正規化されます。

## テンプレートの選び方

| 状況 | 推奨コマンド |
| --- | --- |
| 1 つのビューを持つ最速の Host が欲しい。 | `zyc new MyCompany.Tools` |
| 新しいアプリでモジュール形式のソリューションが欲しい。 | `zyc new MyCompany.Tools --template modular` |
| 既存リポジトリへモジュールを追加したい。 | `zyc new-module Reports --src-root ./src` |
| 生成したモジュール プロジェクトを既存 `.slnx` に追加したい。 | `zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx` |

<!--doc-l10n:locale zh-CN-->
# 项目模板

ZYC.Framework 通过 `dotnet tool` 命令支持两类常见脚手架任务：创建新的 Host 项目，以及向已有源码树添加新模块。本文说明模板类型、生成结构和命令选项。

## 命令入口

| 命令 | 目的 | 适用场景 |
| --- | --- | --- |
| `zyc new <ProjectName>` | 从项目模板创建一个外部 ZYC.Framework Host 项目。 | 在框架仓库之外启动新应用或示例。 |
| `zyc new-module <ModuleName> --src-root <SourceRoot>` | 在已有源码树中创建模块实现项目和对应的 `*.Abstractions` 项目。 | 扩展已有的 ZYC.Framework 风格仓库。 |

以 .NET tool 方式安装或更新 CLI：

```bash
dotnet tool install -g ZYC.Framework.CLI --version $(Version)
dotnet tool update -g ZYC.Framework.CLI --version $(Version)
```

然后确认命令可用：

```bash
zyc --help
zyc new --help
zyc new-module --help
```

## `minimal` 模板

`minimal` 是 `zyc new` 的默认模板。它会创建一个小型 WPF Host 项目，引用 `ZYC.Framework.Alpha`，并把一个 WPF View 注册为 simple tab。

```bash
zyc new MyCompany.Tools
```

等价的显式命令：

```bash
zyc new MyCompany.Tools --template minimal
```

生成结构：

```text
MyCompany.Tools/
  MyCompany.Tools.csproj
  MyCompany.Tools.slnx
  Module.cs
  ModuleConfig.json
  UI/
    ToolsView.xaml
    ToolsView.xaml.cs
```

当你想用最短路径得到可运行 Host，并且功能表面只是一个简单 WPF View 时，使用这个模板。

## `modular` 模板

`modular` 会创建一个小型解决方案，包含 Entry 项目、模块实现项目和模块 Abstractions 项目。

```bash
zyc new MyCompany.Tools --template modular
```

生成结构：

```text
MyCompany.Tools/
  Directory.Build.props
  Directory.Build.targets
  version.props
  MyCompany.Tools.slnx
  Entry/
    Entry.csproj
  ZYC.Framework.Modules.MyCompany.Tools/
    ZYC.Framework.Modules.MyCompany.Tools.csproj
    Module.cs
    ToolsMainMenuItem.cs
    ToolsTabItem.cs
    ToolsTabItemFactory.cs
    UI/
      ToolsView.xaml
      ToolsView.xaml.cs
  ZYC.Framework.Modules.MyCompany.Tools.Abstractions/
    ToolsModuleConstants.cs
    ZYC.Framework.Modules.MyCompany.Tools.Abstractions.csproj
```

当功能应该像真实框架模块一样组织时使用它：公开常量在 Abstractions，模块提供带路由的 Tab 工厂、主菜单项和模块入口。

## `zyc new` 选项

| 选项 | 说明 |
| --- | --- |
| `<ProjectName>` | 必填项目名。必须是有效的点分隔 C# 标识符，例如 `Acme.Tools`。 |
| `--template`, `-t` | 项目模板。支持 `minimal` 和 `modular`，默认 `minimal`。 |
| `--output`, `-o` | 输出目录。默认 `./<ProjectName>`。 |
| `--package-version` | `ZYC.Framework.Alpha` 包版本。默认使用 CLI 产品版本。 |
| `--overwrite`, `-f` | 覆盖已有文件。不指定时，如果目标文件已存在，生成会失败。 |

包含常用选项的示例：

```bash
zyc new Acme.Tools --template modular --output ./Acme.Tools --package-version $(Version)
```

## 面向已有源码树的 `new-module`

当仓库已经有 `src` 树，并且你想添加一组模块项目时，使用 `new-module`。

```bash
zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx
```

该命令会创建：

```text
src/
  ZYC.Framework.Modules.Reports/
    ZYC.Framework.Modules.Reports.csproj
    Module.cs
    ReportsMainMenuItem.cs
    ReportsTabItem.cs
    ReportsTabItemFactory.cs
    UI/
      ReportsView.xaml
      ReportsView.xaml.cs
  ZYC.Framework.Modules.Reports.Abstractions/
    ReportsModuleConstants.cs
    ZYC.Framework.Modules.Reports.Abstractions.csproj
```

`--slnx` 是可选项。提供它时，生成的项目会被加入解决方案文件的 `/Modules/` 文件夹。相对 `--slnx` 路径会从 `--src-root` 解析。

`new-module` 会规范化目标名。下面这些输入都会生成同一个模块目标：

```bash
zyc new-module Reports --src-root ./src
zyc new-module ZYC.Framework.Modules.Reports --src-root ./src
zyc new-module ZYC.Framework.Modules.Reports.Abstractions --src-root ./src
```

## `new-module` 选项

| 选项 | 说明 |
| --- | --- |
| `<ModuleName>` | 位置参数形式的目标模块名。 |
| `--target`, `-t` | 目标模块名。请使用位置参数或此选项之一，不要提供冲突值。 |
| `--src-root`, `-s` | 必填源码根目录，模块项目会创建在这里。 |
| `--slnx` | 可选的 solution XML 文件路径。无需更新解决方案时省略。 |
| `--overwrite`, `-f` | 覆盖已有文件或模块目录。不指定时，如果目标目录已存在，生成会失败。 |

## 模板 Token

项目模板会替换路径和文本文件中的这些 Token：

| Token | 值 |
| --- | --- |
| `__PROJECT_NAME__` | 完整项目名，例如 `MyCompany.Tools`。 |
| `__PROJECT_SHORT_NAME__` | 最后一个点分隔片段，例如 `Tools`。 |
| `__PROJECT_HOST__` | 用作 URI Host 的小写短名称，例如 `tools`。 |
| `__PACKAGE_VERSION__` | 由 `--package-version` 或 CLI 产品版本决定的包版本。 |

文本模板文件会以 UTF-8 with BOM 写入，并统一为 CRLF 换行。

## 如何选择模板

| 情况 | 推荐命令 |
| --- | --- |
| 你想最快得到一个只有一个 View 的 Host。 | `zyc new MyCompany.Tools` |
| 你想为新应用创建模块化解决方案。 | `zyc new MyCompany.Tools --template modular` |
| 你正在向已有仓库添加模块。 | `zyc new-module Reports --src-root ./src` |
| 你需要把模块项目加入已有 `.slnx`。 | `zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx` |

<!--doc-l10n:locale zh-TW-->
# 專案範本

ZYC.Framework 透過 `dotnet tool` 命令支援兩類常見鷹架任務：建立新的 Host 專案，以及向既有原始碼樹新增新模組。本文說明範本類型、生成結構與命令選項。

## 命令入口

| 命令 | 目的 | 適用場景 |
| --- | --- | --- |
| `zyc new <ProjectName>` | 從專案範本建立一個外部 ZYC.Framework Host 專案。 | 在框架儲存庫之外啟動新應用或範例。 |
| `zyc new-module <ModuleName> --src-root <SourceRoot>` | 在既有原始碼樹中建立模組實作專案與對應的 `*.Abstractions` 專案。 | 擴展既有的 ZYC.Framework 風格儲存庫。 |

以 .NET tool 方式安裝或更新 CLI：

```bash
dotnet tool install -g ZYC.Framework.CLI --version $(Version)
dotnet tool update -g ZYC.Framework.CLI --version $(Version)
```

然後確認命令可用：

```bash
zyc --help
zyc new --help
zyc new-module --help
```

## `minimal` 範本

`minimal` 是 `zyc new` 的預設範本。它會建立一個小型 WPF Host 專案，引用 `ZYC.Framework.Alpha`，並把一個 WPF View 註冊為 simple tab。

```bash
zyc new MyCompany.Tools
```

等價的明確命令：

```bash
zyc new MyCompany.Tools --template minimal
```

生成結構：

```text
MyCompany.Tools/
  MyCompany.Tools.csproj
  MyCompany.Tools.slnx
  Module.cs
  ModuleConfig.json
  UI/
    ToolsView.xaml
    ToolsView.xaml.cs
```

當你想用最短路徑得到可執行 Host，且功能表面只是一個簡單 WPF View 時，使用這個範本。

## `modular` 範本

`modular` 會建立一個小型解決方案，包含 Entry 專案、模組實作專案與模組 Abstractions 專案。

```bash
zyc new MyCompany.Tools --template modular
```

生成結構：

```text
MyCompany.Tools/
  Directory.Build.props
  Directory.Build.targets
  version.props
  MyCompany.Tools.slnx
  Entry/
    Entry.csproj
  ZYC.Framework.Modules.MyCompany.Tools/
    ZYC.Framework.Modules.MyCompany.Tools.csproj
    Module.cs
    ToolsMainMenuItem.cs
    ToolsTabItem.cs
    ToolsTabItemFactory.cs
    UI/
      ToolsView.xaml
      ToolsView.xaml.cs
  ZYC.Framework.Modules.MyCompany.Tools.Abstractions/
    ToolsModuleConstants.cs
    ZYC.Framework.Modules.MyCompany.Tools.Abstractions.csproj
```

當功能應該像真實框架模組一樣組織時使用它：公開常數在 Abstractions，模組提供帶路由的 Tab factory、主選單項目與模組入口。

## `zyc new` 選項

| 選項 | 說明 |
| --- | --- |
| `<ProjectName>` | 必填專案名。必須是有效的點分隔 C# 識別碼，例如 `Acme.Tools`。 |
| `--template`, `-t` | 專案範本。支援 `minimal` 與 `modular`，預設 `minimal`。 |
| `--output`, `-o` | 輸出目錄。預設 `./<ProjectName>`。 |
| `--package-version` | `ZYC.Framework.Alpha` 套件版本。預設使用 CLI 產品版本。 |
| `--overwrite`, `-f` | 覆蓋既有檔案。不指定時，如果目標檔案已存在，生成會失敗。 |

包含常用選項的範例：

```bash
zyc new Acme.Tools --template modular --output ./Acme.Tools --package-version $(Version)
```

## 面向既有原始碼樹的 `new-module`

當儲存庫已經有 `src` 樹，且你想新增一組模組專案時，使用 `new-module`。

```bash
zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx
```

該命令會建立：

```text
src/
  ZYC.Framework.Modules.Reports/
    ZYC.Framework.Modules.Reports.csproj
    Module.cs
    ReportsMainMenuItem.cs
    ReportsTabItem.cs
    ReportsTabItemFactory.cs
    UI/
      ReportsView.xaml
      ReportsView.xaml.cs
  ZYC.Framework.Modules.Reports.Abstractions/
    ReportsModuleConstants.cs
    ZYC.Framework.Modules.Reports.Abstractions.csproj
```

`--slnx` 是可選項。提供它時，生成的專案會被加入解決方案檔案的 `/Modules/` 資料夾。相對 `--slnx` 路徑會從 `--src-root` 解析。

`new-module` 會正規化目標名。下面這些輸入都會生成同一個模組目標：

```bash
zyc new-module Reports --src-root ./src
zyc new-module ZYC.Framework.Modules.Reports --src-root ./src
zyc new-module ZYC.Framework.Modules.Reports.Abstractions --src-root ./src
```

## `new-module` 選項

| 選項 | 說明 |
| --- | --- |
| `<ModuleName>` | 位置參數形式的目標模組名。 |
| `--target`, `-t` | 目標模組名。請使用位置參數或此選項之一，不要提供衝突值。 |
| `--src-root`, `-s` | 必填原始碼根目錄，模組專案會建立在這裡。 |
| `--slnx` | 可選的 solution XML 檔案路徑。無需更新解決方案時省略。 |
| `--overwrite`, `-f` | 覆蓋既有檔案或模組目錄。不指定時，如果目標目錄已存在，生成會失敗。 |

## 範本 Token

專案範本會替換路徑與文字檔案中的這些 Token：

| Token | 值 |
| --- | --- |
| `__PROJECT_NAME__` | 完整專案名，例如 `MyCompany.Tools`。 |
| `__PROJECT_SHORT_NAME__` | 最後一個點分隔片段，例如 `Tools`。 |
| `__PROJECT_HOST__` | 用作 URI Host 的小寫短名稱，例如 `tools`。 |
| `__PACKAGE_VERSION__` | 由 `--package-version` 或 CLI 產品版本決定的套件版本。 |

文字範本檔案會以 UTF-8 with BOM 寫入，並統一為 CRLF 換行。

## 如何選擇範本

| 情況 | 建議命令 |
| --- | --- |
| 你想最快得到一個只有一個 View 的 Host。 | `zyc new MyCompany.Tools` |
| 你想為新應用建立模組化解決方案。 | `zyc new MyCompany.Tools --template modular` |
| 你正在向既有儲存庫新增模組。 | `zyc new-module Reports --src-root ./src` |
| 你需要把模組專案加入既有 `.slnx`。 | `zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx` |

<!--doc-l10n:locale ko-->
# 프로젝트 템플릿

ZYC.Framework는 두 가지 일반적인 스캐폴딩 작업을 위한 `dotnet tool` 명령을 제공합니다. 새 Host 프로젝트 만들기와 기존 소스 트리에 새 모듈 추가입니다. 이 문서는 템플릿, 생성 구조, 명령 옵션을 설명합니다.

## 명령 범위

| 명령 | 목적 | 적합한 경우 |
| --- | --- | --- |
| `zyc new <ProjectName>` | 프로젝트 템플릿에서 외부 ZYC.Framework Host 프로젝트를 만듭니다. | 프레임워크 저장소 밖에서 새 앱이나 샘플을 시작할 때. |
| `zyc new-module <ModuleName> --src-root <SourceRoot>` | 기존 소스 트리 안에 모듈 구현 프로젝트와 대응하는 `*.Abstractions` 프로젝트를 만듭니다. | 기존 ZYC.Framework 스타일 저장소를 확장할 때. |

CLI를 .NET tool로 설치하거나 업데이트합니다.

```bash
dotnet tool install -g ZYC.Framework.CLI --version $(Version)
dotnet tool update -g ZYC.Framework.CLI --version $(Version)
```

명령을 확인합니다.

```bash
zyc --help
zyc new --help
zyc new-module --help
```

## `minimal` 템플릿

`minimal`은 `zyc new`의 기본 템플릿입니다. `ZYC.Framework.Alpha`를 참조하는 작은 WPF Host 프로젝트를 만들고, 하나의 WPF 뷰를 simple tab으로 등록합니다.

```bash
zyc new MyCompany.Tools
```

명시적으로 쓰면 다음과 같습니다.

```bash
zyc new MyCompany.Tools --template minimal
```

생성 구조:

```text
MyCompany.Tools/
  MyCompany.Tools.csproj
  MyCompany.Tools.slnx
  Module.cs
  ModuleConfig.json
  UI/
    ToolsView.xaml
    ToolsView.xaml.cs
```

하나의 단순한 WPF 뷰만 필요한 실행 가능한 Host를 가장 빠르게 만들고 싶을 때 사용합니다.

## `modular` 템플릿

`modular`는 Entry 프로젝트, 모듈 구현 프로젝트, 모듈 Abstractions 프로젝트가 있는 작은 솔루션을 만듭니다.

```bash
zyc new MyCompany.Tools --template modular
```

생성 구조:

```text
MyCompany.Tools/
  Directory.Build.props
  Directory.Build.targets
  version.props
  MyCompany.Tools.slnx
  Entry/
    Entry.csproj
  ZYC.Framework.Modules.MyCompany.Tools/
    ZYC.Framework.Modules.MyCompany.Tools.csproj
    Module.cs
    ToolsMainMenuItem.cs
    ToolsTabItem.cs
    ToolsTabItemFactory.cs
    UI/
      ToolsView.xaml
      ToolsView.xaml.cs
  ZYC.Framework.Modules.MyCompany.Tools.Abstractions/
    ToolsModuleConstants.cs
    ZYC.Framework.Modules.MyCompany.Tools.Abstractions.csproj
```

공개 상수는 Abstractions에 두고, 라우팅 탭 팩터리, 메인 메뉴 항목, 모듈 진입점을 갖춘 실제 프레임워크 모듈 형태가 필요할 때 사용합니다.

## `zyc new` 옵션

| 옵션 | 설명 |
| --- | --- |
| `<ProjectName>` | 필수 프로젝트 이름. `Acme.Tools`처럼 유효한 점 구분 C# 식별자여야 합니다. |
| `--template`, `-t` | 프로젝트 템플릿. 지원 값은 `minimal`, `modular`입니다. 기본값은 `minimal`입니다. |
| `--output`, `-o` | 출력 디렉터리. 기본값은 `./<ProjectName>`입니다. |
| `--package-version` | `ZYC.Framework.Alpha` 패키지 버전. 기본값은 CLI 제품 버전입니다. |
| `--overwrite`, `-f` | 기존 파일을 덮어씁니다. 이 플래그가 없으면 대상 파일이 있을 때 생성이 실패합니다. |

일반적인 옵션을 모두 지정한 예:

```bash
zyc new Acme.Tools --template modular --output ./Acme.Tools --package-version $(Version)
```

## 기존 소스 트리를 위한 `new-module`

저장소에 이미 `src` 트리가 있고 하나의 모듈 쌍을 추가하려면 `new-module`을 사용합니다.

```bash
zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx
```

이 명령은 다음을 만듭니다.

```text
src/
  ZYC.Framework.Modules.Reports/
    ZYC.Framework.Modules.Reports.csproj
    Module.cs
    ReportsMainMenuItem.cs
    ReportsTabItem.cs
    ReportsTabItemFactory.cs
    UI/
      ReportsView.xaml
      ReportsView.xaml.cs
  ZYC.Framework.Modules.Reports.Abstractions/
    ReportsModuleConstants.cs
    ZYC.Framework.Modules.Reports.Abstractions.csproj
```

`--slnx`는 선택 사항입니다. 제공하면 생성된 프로젝트가 솔루션 파일의 `/Modules/` 폴더 아래에 추가됩니다. 상대 `--slnx` 경로는 `--src-root`를 기준으로 해석됩니다.

`new-module`은 target 이름을 정규화합니다. 다음 입력은 모두 같은 모듈 target을 생성합니다.

```bash
zyc new-module Reports --src-root ./src
zyc new-module ZYC.Framework.Modules.Reports --src-root ./src
zyc new-module ZYC.Framework.Modules.Reports.Abstractions --src-root ./src
```

## `new-module` 옵션

| 옵션 | 설명 |
| --- | --- |
| `<ModuleName>` | 위치 인수 형태의 target 모듈 이름. |
| `--target`, `-t` | target 모듈 이름. 위치 값 또는 이 옵션 중 하나를 사용하고, 서로 다른 값을 동시에 지정하지 마세요. |
| `--src-root`, `-s` | 필수 소스 루트. 모듈 프로젝트가 여기에 생성됩니다. |
| `--slnx` | 업데이트할 선택적 solution XML 파일. 솔루션 업데이트가 필요 없으면 생략합니다. |
| `--overwrite`, `-f` | 기존 파일 또는 모듈 디렉터리를 덮어씁니다. 이 플래그가 없으면 대상 디렉터리가 있을 때 생성이 실패합니다. |

## 템플릿 토큰

프로젝트 템플릿은 경로와 텍스트 파일에서 다음 토큰을 치환합니다.

| Token | 값 |
| --- | --- |
| `__PROJECT_NAME__` | 전체 프로젝트 이름. 예: `MyCompany.Tools`. |
| `__PROJECT_SHORT_NAME__` | 마지막 점 구분 세그먼트. 예: `Tools`. |
| `__PROJECT_HOST__` | URI host로 사용하는 소문자 짧은 이름. 예: `tools`. |
| `__PACKAGE_VERSION__` | `--package-version` 또는 CLI 제품 버전으로 선택된 패키지 버전. |

텍스트 템플릿 파일은 UTF-8 with BOM으로 작성되고 CRLF 줄 끝으로 정규화됩니다.

## 템플릿 선택

| 상황 | 권장 명령 |
| --- | --- |
| 하나의 뷰가 있는 가장 빠른 Host가 필요합니다. | `zyc new MyCompany.Tools` |
| 새 앱에 모듈 스타일 솔루션이 필요합니다. | `zyc new MyCompany.Tools --template modular` |
| 기존 저장소에 모듈을 추가합니다. | `zyc new-module Reports --src-root ./src` |
| 모듈 프로젝트를 기존 `.slnx`에 추가해야 합니다. | `zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx` |

<!--doc-l10n:end-->
