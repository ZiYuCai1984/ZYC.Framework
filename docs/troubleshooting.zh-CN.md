<p align="center">
  <a href="./troubleshooting.md">English</a> |
  <a href="./troubleshooting.ja.md">日本語</a> |
  <a href="./troubleshooting.zh-CN.md">简体中文</a> |
  <a href="./troubleshooting.zh-TW.md">繁體中文</a> |
  <a href="./troubleshooting.ko.md">한국어</a> |
</p>


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
dotnet tool install --global ZYC.Framework.CLI --version 1.3.9
dotnet tool update --global ZYC.Framework.CLI --version 1.3.9
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
