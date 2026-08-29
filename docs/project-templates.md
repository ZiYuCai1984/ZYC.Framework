<p align="center">
  <a href="./project-templates.md">English</a> |
  <a href="./project-templates.ja.md">日本語</a> |
  <a href="./project-templates.zh-CN.md">简体中文</a> |
  <a href="./project-templates.zh-TW.md">繁體中文</a> |
  <a href="./project-templates.ko.md">한국어</a> |
</p>


# Project Templates

ZYC.Framework provides `dotnet tool` commands for two common scaffolding tasks: creating a new host project and adding a new module to an existing source tree. This page documents the templates, generated structure, and command options.

## Command Surface

| Command | Purpose | Best for |
| --- | --- | --- |
| `zyc new <ProjectName>` | Creates a new external ZYC.Framework Host project from a project template. | Starting a new app or sample outside the framework repository. |
| `zyc new-module <ModuleName> [--src-root <SourceRoot>]` | Creates a module implementation project and matching `*.Abstractions` project inside an existing source tree. | Extending an existing ZYC.Framework-style repository. |

Install or update the CLI as a .NET tool:

```bash
dotnet tool install -g ZYC.Framework.CLI --version 1.4.5
dotnet tool update -g ZYC.Framework.CLI --version 1.4.5
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

## `console` Template

`console` creates a `net10.0` console solution with shared build properties and a console project that references `ZYC.CoreToolkit` 4.0.0.

```bash
zyc new MyCompany.Tools --template console
```

Generated structure:

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

Use this template when you want a small console application that preserves the source template's root-level shared build configuration.

## `wpf` Template

`wpf` creates a `net10.0-windows` WPF solution with Autofac-based application startup, settings registration, Fody property weaving, and `ZYC.CoreToolkit` 4.0.0 references.

```bash
zyc new MyCompany.Desktop --template wpf
```

Generated structure:

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

Use this template when you want a small WPF application with the source template's existing Autofac and settings bootstrap.

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
| `--template`, `-t` | Project template. Supported values are `minimal`, `modular`, `console`, and `wpf`. Defaults to `minimal`. |
| `--output`, `-o` | Output directory. Defaults to `./<ProjectName>`. |
| `--package-version` | `ZYC.Framework.Alpha` package version. Defaults to the CLI product version. |
| `--overwrite`, `-f` | Overwrite existing files. Without this flag, existing target files fail the generation. |

Example with all common options:

```bash
zyc new Acme.Tools --template modular --output ./Acme.Tools --package-version 1.4.5
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
| `--src-root`, `-s` | Optional source root where the module projects will be created. Defaults to the current directory. |
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
| You want a console application that references `ZYC.CoreToolkit`. | `zyc new MyCompany.Tools --template console` |
| You want a WPF application with Autofac and settings bootstrap. | `zyc new MyCompany.Desktop --template wpf` |
| You want a module-style solution for a new app. | `zyc new MyCompany.Tools --template modular` |
| You are adding a module to an existing repository. | `zyc new-module Reports --src-root ./src` |
| You need the module projects added to an existing `.slnx`. | `zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx` |
