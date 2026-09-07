<p align="center">
  <a href="./project-templates.md">English</a> |
  <a href="./project-templates.ja.md">日本語</a> |
  <a href="./project-templates.zh-CN.md">简体中文</a> |
  <a href="./project-templates.zh-TW.md">繁體中文</a> |
  <a href="./project-templates.ko.md">한국어</a> |
</p>


# 项目模板

ZYC.Framework 通过 `dotnet tool` 命令支持两类常见脚手架任务：创建新的 Host 项目，以及向已有源码树添加新模块。本文说明模板类型、生成结构和命令选项。

## 命令入口

| 命令 | 目的 | 适用场景 |
| --- | --- | --- |
| `zyc new <ProjectName>` | 从项目模板创建一个外部 ZYC.Framework Host 项目。 | 在框架仓库之外启动新应用或示例。 |
| `zyc new-module <ModuleName> [--src-root <SourceRoot>]` | 在已有源码树中创建模块实现项目和对应的 `*.Abstractions` 项目。 | 扩展已有的 ZYC.Framework 风格仓库。 |

以 .NET tool 方式安装或更新 CLI：

```bash
dotnet tool install -g ZYC.Framework.CLI --version 1.4.7
dotnet tool update -g ZYC.Framework.CLI --version 1.4.7
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

## `console` 模板

`console` 会创建一个 `net10.0` 控制台解决方案，其中包含共享构建属性，以及一个引用 `ZYC.CoreToolkit` 4.0.0 的控制台项目。

```bash
zyc new MyCompany.Tools --template console
```

生成结构：

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

当你需要一个保留源模板根级共享构建配置的小型控制台应用时，使用这个模板。

## `wpf` 模板

`wpf` 会创建一个 `net10.0-windows` WPF 解决方案，其中包含基于 Autofac 的应用启动、配置注册、Fody 属性织入，以及对 `ZYC.CoreToolkit` 4.0.0 的引用。

```bash
zyc new MyCompany.Desktop --template wpf
```

生成结构：

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

当你需要一个保留源模板现有 Autofac 和配置启动逻辑的小型 WPF 应用时，使用这个模板。

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
| `--template`, `-t` | 项目模板。支持 `minimal`、`modular`、`console` 和 `wpf`，默认 `minimal`。 |
| `--output`, `-o` | 输出目录。默认 `./<ProjectName>`。 |
| `--package-version` | `ZYC.Framework.Alpha` 包版本。默认使用 CLI 产品版本。 |
| `--overwrite`, `-f` | 覆盖已有文件。不指定时，如果目标文件已存在，生成会失败。 |

包含常用选项的示例：

```bash
zyc new Acme.Tools --template modular --output ./Acme.Tools --package-version 1.4.7
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
| `--src-root`, `-s` | 可选源码根目录，模块项目会创建在这里；省略时默认为当前目录。 |
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
| 你想创建一个引用 `ZYC.CoreToolkit` 的控制台应用。 | `zyc new MyCompany.Tools --template console` |
| 你想创建一个带 Autofac 和配置启动逻辑的 WPF 应用。 | `zyc new MyCompany.Desktop --template wpf` |
| 你想为新应用创建模块化解决方案。 | `zyc new MyCompany.Tools --template modular` |
| 你正在向已有仓库添加模块。 | `zyc new-module Reports --src-root ./src` |
| 你需要把模块项目加入已有 `.slnx`。 | `zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx` |
