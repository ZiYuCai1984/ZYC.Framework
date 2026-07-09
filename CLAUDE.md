# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.
`AGENTS.md` is the repository-level rule source. If this file and `AGENTS.md` differ, follow `AGENTS.md`.

## Project Overview

ZYC.Framework is a modular desktop automation framework built on **.NET 10 + WPF**. It ships as a NuGet package (`ZYC.Framework.Alpha`) and a dotnet CLI tool (`zyc`). Consumers create a WPF host project, install the package, delete `App.xaml`, and register modules via `ModuleConfig.json`.

## Build Commands

All output goes to `src/_bin/{Version}/` (a single shared output directory across projects).

| Task | Command |
|------|---------|
| Pack NuGet packages | `pack-nuget.cmd` |
| Generate API docs | `update-doc.cmd` |
| Build installer | `pack-setup.cmd` |
| Scaffold new module | `new-module.cmd` |

Each `.cmd` script runs `dotnet run --project src/ZYC.Framework.Build.*/`. Do not run builds unless the user explicitly asks.

The solution file is `src/ZYC.Framework.slnx` (new `.slnx` format).

## Architecture

### Host & Module System

```
ZYC.Framework (WinExe host)
├── ZYC.Framework.Abstractions      ← public contracts (net10.0, no WPF)
├── ZYC.Framework.Core              ← XAML controls, converters, utilities
├── ZYC.Framework.MetroWindow       ← Metro/MahApps window styling
├── ZYC.Framework.WebView2          ← embedded browser host
├── ZYC.Framework.CLI               ← dotnet tool (zyc)
├── ZYC.Framework.Modules.*         ← built-in feature modules
├── Thirdparty/                     ← vendored: MdXaml, Terminal, WebProxy
└── ZYC.Framework.Build.*/          ← build automation tools
```

Modules are discovered at runtime via `settings/ModuleConfig.json` (`AdditionalAssemblyNames`). Each module implements `ModuleBase` and calls `LoadAsync` to register tabs, menu items, and services.

### DI Container

Autofac is the DI container. Types are registered automatically by applying `[Register]` (from `ZYC.CoreToolkit.Extensions.Autofac.Attributes`). Module entry points extend `ModuleBase` and receive an `ILifetimeScope` in `LoadAsync`.

### UI Extension Points

- **Tabs** — register via `ISimpleTabItemFactoryManager` or `ITabItemFactory`
- **Menus** — register via `IMenuManager` and menu factory interfaces
- **Notifications** — Toast and Banner through the notification service
- **Tasks** — background work via the TaskManager infrastructure

### Target Frameworks

- `*.Abstractions` projects → `net10.0`, `UseWPF=false`, XML docs generated automatically
- WPF host, UI, and module projects generally use `net10.0-windows`, `UseWPF=true`
- Non-WPF tools and libraries such as `ZYC.Framework.CLI` and `Thirdparty/ZYC.Titanium.Web.Proxy` use `net10.0`, `UseWPF=false`

`System.Windows.Input.ICommand` may be referenced from Abstractions; it does not require the Windows TFM.

## Conventions Summary

This section summarizes `AGENTS.md`; keep `AGENTS.md` authoritative for exact repository rules.

**Naming**
- Interfaces: `I` prefix, PascalCase (e.g., `IUpdateManager`)
- Async methods must end with `Async` when returning `Task`/`ValueTask`
- TaskManager event DTOs follow `ManagedTask*Event` (e.g., `ManagedTaskCompletedEvent`)
- Namespaces: file-scoped, matching folder structure
- Avoid adding `sealed` unless it is required for correctness, API contract, or an explicit user request

**Files**
- New files must use **CRLF** line endings and **UTF-8 with BOM** encoding
- XML docs are required for all public APIs in `*.Abstractions` projects; optional elsewhere

**Change Policy**
- Prefer minimal, surgical changes aligned with existing architecture
- Prefer additive and backward-compatible changes; confirm before introducing breaking changes
- When renaming types, update file names and all references across the solution
- Preserve existing target frameworks, output paths, and packaging structure
- Do NOT suggest enabling/changing code analyzers, StyleCop/FXCop/Roslyn rules, linting tools, or `RunAnalyzersDuringBuild`
- Do NOT propose changes to the build pipeline, CI, or solution-wide props/targets unless asked
- Do NOT introduce large new dependencies without an explicit request

## Key Configuration Files

- `src/Directory.Build.props` — global TFM, WPF, Nullable, output path settings
- `src/nuget.props` / `src/nuget.targets` — centralized package versions
- `src/version.props` — shared version
- `src/global.json` — SDK and Aspire version pins
- `docfx.json` — generates API docs from `*.Abstractions` projects into `_site/`

## Diagrams

When providing module dependency graphs, use Mermaid `graph TD` format.
