<p align="center">
  <a href="./project-templates.md">English</a> |
  <a href="./project-templates.ja.md">日本語</a> |
  <a href="./project-templates.zh-CN.md">简体中文</a> |
  <a href="./project-templates.zh-TW.md">繁體中文</a> |
  <a href="./project-templates.ko.md">한국어</a> |
</p>


# プロジェクト テンプレート

ZYC.Framework は、よく使う 2 つのスキャフォールド作業のために `dotnet tool` コマンドを提供します。新しい Host プロジェクトの作成と、既存ソース ツリーへの新しいモジュール追加です。このページではテンプレート、生成される構成、コマンド オプションを説明します。

## コマンドの範囲

| コマンド | 目的 | 向いている用途 |
| --- | --- | --- |
| `zyc new <ProjectName>` | プロジェクト テンプレートから外部 ZYC.Framework Host プロジェクトを作成する。 | フレームワーク リポジトリ外で新しいアプリやサンプルを始める。 |
| `zyc new-module <ModuleName> [--src-root <SourceRoot>]` | 既存ソース ツリー内にモジュール実装プロジェクトと対応する `*.Abstractions` プロジェクトを作成する。 | 既存の ZYC.Framework 形式のリポジトリを拡張する。 |

CLI を .NET tool としてインストールまたは更新します。

```bash
dotnet tool install -g ZYC.Framework.CLI --version 1.4.6
dotnet tool update -g ZYC.Framework.CLI --version 1.4.6
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

## `console` テンプレート

`console` は、共有ビルド プロパティと `ZYC.CoreToolkit` 4.0.0 を参照するコンソール プロジェクトを含む `net10.0` コンソール ソリューションを作成します。

```bash
zyc new MyCompany.Tools --template console
```

生成される構成:

```text
MyCompany.Tools/
  .editorconfig
  .gitignore
  Directory.Build.props
  Directory.Build.targets
  MyCompany.Tools.slnx
  MyCompany.Tools/
    MyCompany.Tools.csproj
    Program.cs
```

ソース テンプレートのルート レベル共有ビルド構成を保持した小さなコンソール アプリケーションが必要な場合に使います。

## `wpf` テンプレート

`wpf` は、Autofac ベースのアプリケーション起動、設定登録、Fody プロパティ ウィービング、および `ZYC.CoreToolkit` 4.0.0 への参照を備えた `net10.0-windows` WPF ソリューションを作成します。

```bash
zyc new MyCompany.Desktop --template wpf
```

生成される構成:

```text
MyCompany.Desktop/
  .editorconfig
  .gitignore
  Directory.Build.props
  Directory.Build.targets
  FodyWeavers.xml
  MyCompany.Desktop.slnx
  MyCompany.Desktop/
    App.xaml
    App.xaml.cs
    MainWindow.xaml
    MainWindow.xaml.cs
    Program.cs
    MyCompany.Desktop.csproj
```

ソース テンプレート既存の Autofac と設定のブートストラップを備えた小さな WPF アプリケーションが必要な場合に使います。

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
| `--template`, `-t` | プロジェクト テンプレート。対応値は `minimal`、`modular`、`console`、`wpf`。既定は `minimal`。 |
| `--output`, `-o` | 出力ディレクトリ。既定は `./<ProjectName>`。 |
| `--package-version` | `ZYC.Framework.Alpha` のパッケージ バージョン。既定は CLI の製品バージョン。 |
| `--overwrite`, `-f` | 既存ファイルを上書きする。この指定がない場合、対象ファイルが存在すると生成は失敗します。 |

よく使うオプションをすべて指定する例:

```bash
zyc new Acme.Tools --template modular --output ./Acme.Tools --package-version 1.4.6
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
| `--src-root`, `-s` | モジュール プロジェクトを作成する任意のソース ルート。省略時は現在のディレクトリを使用します。 |
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
| `ZYC.CoreToolkit` を参照するコンソール アプリケーションが欲しい。 | `zyc new MyCompany.Tools --template console` |
| Autofac と設定のブートストラップを備えた WPF アプリケーションが欲しい。 | `zyc new MyCompany.Desktop --template wpf` |
| 新しいアプリでモジュール形式のソリューションが欲しい。 | `zyc new MyCompany.Tools --template modular` |
| 既存リポジトリへモジュールを追加したい。 | `zyc new-module Reports --src-root ./src` |
| 生成したモジュール プロジェクトを既存 `.slnx` に追加したい。 | `zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx` |
