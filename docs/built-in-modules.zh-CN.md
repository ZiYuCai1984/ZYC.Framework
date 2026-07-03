<p align="center">
  <a href="./built-in-modules.md">English</a> |
  <a href="./built-in-modules.ja.md">日本語</a> |
  <a href="./built-in-modules.zh-CN.md">简体中文</a> |
  <a href="./built-in-modules.zh-TW.md">繁體中文</a> |
  <a href="./built-in-modules.ko.md">한국어</a> |
</p>


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
| `Accounts` | 窗口标题栏扩展和账号服务 | 初始化 provider-based account session，并暴露登录/退出操作。 |
| `Accounts.GitHub` | GitHub OAuth WebView2 Tab | 提供 GitHub account provider 和登录回调处理。 |
| `ApiReference` | About 菜单和 WebView2 Tab | 承载 API reference 内容。 |
| `Aspire` | Tools 菜单、路由 Tab、状态栏 | 启动和监控 Aspire resources；解析 `IExtensionResourcesProvider` 贡献。 |
| `BlazorDemo` | Tools 菜单和路由 Tab | 演示桌面 Host 内的 Blazor 集成。 |
| `ChromeExtensions` | Extensions 菜单和路由 Tab | 管理 WebBrowser 使用的本地 Chrome Web Store extension packages。 |
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

`Settings`、`Language`、`Secrets`、`Log`、`TaskManager`、`ModuleManager`、`Update`、`About`、`Accounts`、`ChromeExtensions`、`ApiReference` 主要是 Shell 或运维类模块。它们让框架更容易检查、配置和维护。

这些模块通常从 `LoadAsync` 注册菜单项和路由 Tab。有些模块也会更早注册服务：

- `Log` 在 `RegisterAsync` 中注册 logging providers。
- `Language` 注册 language-resource adapters，并加载 default language resources。
- `Secrets` 注册从 config objects 到 `ISecrets` 的 adapter。
- `TaskManager` 在暴露 UI 前初始化 `ITaskManager`。
- `Accounts` 初始化 `IAccountManager`，并注册窗口标题栏账号表面。
- `ChromeExtensions` 在 Extensions 下注册 extension package manager Tab。
- `Update` 在所有模块加载后订阅事件，并在启动检查前等待 `TabManagerRestoreCompleted`。

## 导航与内容模块

`WebBrowser`、`FileExplorer`、`TextEditor`、`CLI`、`BlazorDemo` 暴露面向用户的内容表面。它们都依赖 Tab routing，而不是由 Shell 直接构造 View。

`Accounts.GitHub` 和 `ChromeExtensions` 也使用 WebView2-backed tabs，分别用于 provider 登录和 Chrome Web Store package discovery。它们仍然作为普通模块加载；浏览器相关行为由 WebView2 infrastructure 与 module contracts 承载。

如果这些模块打开了错误 Tab 或 Not Found Tab，请检查已注册的 `ITabItemFactory`、route attributes、factory priority，以及传给 `ITabManager.NavigateAsync(...)` 的 URI。

## Aspire 与 Sidecar 模块

`Aspire` 是运行 Aspire resources 的 Host 侧模块。它注册 Aspire dashboard tab、Tools menu entry 和 status bar item。如果 `AspireConfig.AutoStart` 为 true，它会在模块加载期间启动 Aspire service。

`Translator` 是 sidecar 风格模块。它不暴露大型 UI，而是向 `ICommandlineResourcesProvider` 注册 command-line resource，让 Aspire 模块可以启动 LibreTranslate。

## 开发与 Demo 模块

`Mock` 和 `BlazorDemo` 适合开发或验证框架行为。`Mock` 注册用于通知、对话框、task manager behavior 和 CLI integration 的 demo tabs 与 sample views。除非明确是诊断或示例用途，不要把生产功能放进 Mock。

## 不完整或非模块目录

只有同时具备真实 module project 和 `Module.cs` 的目录，才应该被视为 active built-in module。只有 `obj`、生成文件或 `UI` 子目录的文件夹，不足以作为 runtime discovery 的依据。

文档化或排查模块加载时，请从 compiled output 和 `Module.cs` 开始，不要只看文件夹名称。
