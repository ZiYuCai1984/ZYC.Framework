<p align="center">
  <a href="./built-in-modules.md">English</a> |
  <a href="./built-in-modules.ja.md">日本語</a> |
  <a href="./built-in-modules.zh-CN.md">简体中文</a> |
  <a href="./built-in-modules.zh-TW.md">繁體中文</a> |
  <a href="./built-in-modules.ko.md">한국어</a> |
</p>


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
