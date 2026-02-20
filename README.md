<p align="center">
  <a href="./README.md">English</a> |
  <a href="./README.ja.md">日本語</a> |
  <a href="./README.zh-CN.md">简体中文</a> |
  <a href="./README.zh-TW.md">简體中文</a> |
  <a href="./README.ko.md">한국어</a> |
</p>

<p align="center">
  <img src="./docs/images/app.png" alt="ZYC.Framework Logo" width="120" />
</p>

<h1 align="center">ZYC.Framework</h1>

<p align="center">
  A high-performance, modular, and extensible desktop automation framework built with <b>.NET 10</b> and <b>WPF</b>.
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/ZYC.Framework.Alpha">
    <img src="https://img.shields.io/nuget/v/ZYC.Framework.Alpha?include_prereleases=true&logo=nuget" alt="NuGet Version" />
  </a>
  <a href="https://www.nuget.org/packages/ZYC.Framework.Alpha">
    <img src="https://img.shields.io/nuget/dt/ZYC.Framework.Alpha?logo=nuget&label=Downloads" alt="NuGet Downloads" />
  </a>
  <a href="https://raw.githubusercontent.com/ZiYuCai1984/Temp/refs/heads/main/ZYC.Framework.Setup.exe">
    <img src="https://img.shields.io/badge/Download-Setup-blue?logo=windows&logoColor=white&label=Download%20Demo%20Installer" alt="Download Demo Installer" />
  </a>
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 10" />
  <img src="https://img.shields.io/badge/Platform-WPF-orange" alt="Platform" />
  <img src="https://img.shields.io/badge/License-MIT-green" alt="License" />
</p>

<p align="center">
  <a href="https://github.com/ZiYuCai1984/ZYC.Framework/actions/workflows/publish-nuget-manual.yml">
    <img src="https://img.shields.io/github/actions/workflow/status/ZiYuCai1984/ZYC.Framework/publish-nuget-manual.yml?branch=main&label=build&logo=github" alt="NuGet manual workflow" />
  </a>
  <a href="https://github.com/ZiYuCai1984/ZYC.Framework/actions/workflows/publish-nuget-nightly.yml">
    <img src="https://img.shields.io/github/actions/workflow/status/ZiYuCai1984/ZYC.Framework/publish-nuget-nightly.yml?branch=main&label=nightly%20build&logo=github" alt="NuGet nightly workflow" />
  </a>
</p>

---

## 📖 Overview

**ZYC.Framework** is a modern desktop automation solution that combines the expressive UI capabilities of **WPF** with the latest features of **.NET 10**. It is designed to simplify the development of complex automation systems through a modular architecture.

The project deeply integrates **.NET Aspire** for distributed application orchestration, and supports a hybrid approach with **Blazor** and **WebView2**, so you can choose between a Web-based UI and a native desktop experience as needed.

---

## ✨ Key Features

- **Modular Architecture**: Decoupled business logic with dynamic loading and independent development.
- **Modern UI Experience**: Built on WPF with support for **multi-workspace** and **multi-tab** interactions.
- **Hybrid Development**:
  - **WebView2** integration for embedding modern Web applications.
  - **Blazor** integration to reuse Web components seamlessly in desktop scenarios.
- **Cloud-Native Ready**: Built-in **.NET Aspire** support to simplify service discovery, governance, and deployment.
- **Batteries Included (Enterprise-Oriented)**:
  - **Task Management**: Task scheduling and lifecycle management.
  - **Exception Handling**: Robust global error capture and diagnostics.
  - **Localization**: Built-in multi-language support for global-ready apps.

---

## 📸 UI Preview

<table align="center">
  <tr>
    <td>
      <img src="./docs/images/workspace.png" alt="workspace" width="400" />
      <p align="center">Workspace View</p>
    </td>
    <td>
      <img src="./docs/images/multiple-tabs.png" alt="multiple-tabs" width="400" />
      <p align="center">Multiple Tabs</p>
    </td>
  </tr>

  <tr>
    <td>
      <img src="./docs/images/workspace-4.png" alt="workspace-4" width="400" />
      <p align="center">Multiple Workspaces</p>
    </td>
    <td>
      <img src="./docs/images/workspace-4-tabs.png" alt="workspace-4-tabs" width="400" />
      <p align="center">Workspaces + Tabs</p>
    </td>
  </tr>

  <tr>
    <td>
      <img src="./docs/images/aspire-dashboard.png" alt="aspire-dashboard" width="400" />
      <p align="center">Aspire Dashboard</p>
    </td>
    <td>
      <img src="./docs/images/blazor-auth.png" alt="blazor-auth" width="400" />
      <p align="center">Blazor (with Auth)</p>
    </td>
  </tr>

  <tr>
    <td>
      <img src="./docs/images/exception.png" alt="exception" width="400" />
      <p align="center">Exception Handling</p>
    </td>
    <td>
      <img src="./docs/images/taskmanager.png" alt="taskmanager" width="400" />
      <p align="center">Task Manager</p>
    </td>
  </tr>
</table>

---

## 🛠️ Tech Stack

- **Runtime**: .NET 10 SDK
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Hybrid UI**: WebView2 + Blazor (Web UI)
- **Orchestration**: .NET Aspire
- **Architecture**: Modular Monolith / Plugin-based

---

## 🚀 Quick Start

Please refer to the detailed guide:

👉 **[Quick Start (quick-start.md)](docs/quick-start.md)**

### Installation

Install the core package via NuGet:

```bash
dotnet add package ZYC.Framework.Alpha --version [version]
````

---

## 🏗️ Project Structure

```text
ZYC.Framework
├── src
│   ├── ZYC.Framework                         # WPF desktop host/entry: main window, workspaces, menus, UI lifecycle
│   ├── ZYC.Framework.Abstractions            # Shared contracts: interfaces, states, configs used across host/modules
│   ├── ZYC.Framework.Core                    # Core infrastructure: commands, bindings, converters, base UI components, i18n
│   ├── ZYC.Framework.MetroWindow             # Metro-style window shell (alternative window implementation)
│   ├── ZYC.Framework.WebView2                # WebView2 hosting layer: navigation, menu bar, interop, page hosting
│   ├── ZYC.Framework.CLI                     # CLI tool: developer utilities, module/file helpers, automation entrypoints
│   ├── ZYC.Framework.Build.*                 # Build & packaging toolchain
│   │   ├── ZYC.Framework.Build.NuGet         # NuGet packaging tool: build/.props/.targets, README, PatchNote, outputs
│   │   ├── ZYC.Framework.Build.InnoSetup     # Inno Setup builder: produces the Windows installer (setup)
│   │   └── ZYC.Framework.Build.NewModule     # Module scaffolder: templates for Module + Abstractions projects
│   ├── ZYC.Framework.Modules.*               # Feature modules
│   │   ├── ZYC.Framework.Modules.About                  # About / version info page (UI + tab)
│   │   ├── ZYC.Framework.Modules.Aspire                 # .NET Aspire AppHost/orchestration integration + dashboard
│   │   ├── ZYC.Framework.Modules.BlazorDemo             # Blazor Server demo: web UI + auth/integration showcase
│   │   ├── ZYC.Framework.Modules.CLI                    # In-app CLI module (terminal-like tools page)
│   │   ├── ZYC.Framework.Modules.FileExplorer           # File Explorer module (Explorer-like browsing in tabs)
│   │   ├── ZYC.Framework.Modules.Language               # Language/i18n module: language switching + resources config
│   │   ├── ZYC.Framework.Modules.Log                    # Logging module: view logs, open log folder, log plumbing
│   │   ├── ZYC.Framework.Modules.Mock                   # Test/demo module for validating UI/notifications/tasks/workspaces
│   │   ├── ZYC.Framework.Modules.ModuleManager          # Module manager: enable/disable/install/uninstall (local + NuGet)
│   │   ├── ZYC.Framework.Modules.NuGet                  # NuGet access layer: sources, metadata, version utilities
│   │   ├── ZYC.Framework.Modules.Secrets                # Secrets utilities: password generator, Wi-Fi password, secrets UI
│   │   ├── ZYC.Framework.Modules.Settings               # Settings module: settings UI, grouping, reset actions
│   │   ├── ZYC.Framework.Modules.TaskManager            # Task manager: queue/progress/pause/cancel/cleanup framework
│   │   ├── ZYC.Framework.Modules.Translator             # Translator module: integrates translation services/local runner
│   │   ├── ZYC.Framework.Modules.Update                 # Update module: check/download/apply+restart, fault handling UI
│   │   └── ZYC.Framework.Modules.WebBrowser             # Built-in web browser module (tab-hosted browsing)
│   └── Thirdparty                            # Integrated third-party components (vendored/forked)
│       ├── ZYC.Terminal                      # Terminal/ConPTY integration: pseudo console, PTY, process + pipes
│       ├── ZYC.MdXaml                        # Markdown renderer + extensions
│       └── ZYC.Titanium.Web.Proxy            # HTTP(S) proxy core (integrated Titanium Web Proxy fork)
```

---

## 📄 License

This project is open-sourced under the [MIT License](LICENSE).

---

## 💖 Acknowledgements

This project uses (and/or references parts of implementations from) the following open-source projects:

* [MahApps.Metro](https://github.com/MahApps/MahApps.Metro): UI framework.
* [MdXaml](https://github.com/whistyun/MdXaml): Markdown rendering.
* [titanium-web-proxy](https://github.com/justcoding121/titanium-web-proxy): Proxy core.
* [EasyWindowsTerminalControl](https://github.com/mitchcapper/EasyWindowsTerminalControl): Terminal integration.

> Licenses and copyrights belong to their respective authors.
> This repository uses or references them in compliance with each project's license terms.

---

## 🤝 Contributing

Issues and pull requests are welcome. If you have suggestions or found a bug, please open an issue or submit a PR.
