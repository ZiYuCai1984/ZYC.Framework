<p align="center">
  <a href="./README.md">English</a> |
  <a href="./README.ja.md">日本語</a> |
  <a href="./README.zh-CN.md">简体中文</a> |
  <a href="./README.zh-TW.md">繁體中文</a> |
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
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 10" />
  <img src="https://img.shields.io/badge/Platform-WPF-orange" alt="Platform" />
  <img src="https://img.shields.io/badge/License-MIT-green" alt="License" />
</p>

<p align="center">
  <a href="https://github.com/ZiYuCai1984/ZYC.Framework/actions/workflows/publish-nuget-manual.yml">
    <img src="https://img.shields.io/github/actions/workflow/status/ZiYuCai1984/ZYC.Framework/publish-nuget-manual.yml?branch=main&label=build&logo=github" alt="NuGet manual workflow" />
  </a>
  <a href="https://github.com/ZiYuCai1984/ZYC.Framework/actions/workflows/publish-nuget-nightly.yml">
    <img src="https://img.shields.io/github/actions/workflow/status/ZiYuCai1984/ZYC.Framework/build-nightly.yml?branch=main&label=nightly%20build&logo=github" alt="Build nightly workflow" />
  </a>
</p>

---



## 📖  Overview



**ZYC.Framework** is a modern desktop automation solution that combines the expressive UI capabilities of **WPF** with the latest features of **.NET 10**. It is designed to simplify the development of complex automation systems through a modular architecture.




The project deeply integrates **.NET Aspire** for distributed application orchestration, and supports a hybrid approach with **Blazor** and **WebView2**, so you can choose between a Web-based UI and a native desktop experience as needed.


---



## ✨  Key Features

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





## 🛠️  Tech Stack

- **Runtime**: .NET 10 SDK
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Hybrid UI**: WebView2 + Blazor
- **Orchestration**: .NET Aspire
- **Architecture**: Modular Monolith / Plugin-based

---



## 🚀  Quick Start


Please refer to the detailed guide:


👉 **[Quick Start (quick-start.md)](docs/quick-start.md)**


👉 **[Download Demo Installer](https://github.com/ZiYuCai1984/ZYC.Framework/releases/download/v1.4.7/ZYC.Framework.Setup.1.4.7.exe)**

### Create a Project


The recommended way to start is the global dotnet tool. Install or update the CLI, then create a host project with `zyc new`:

```bash
dotnet tool install --global ZYC.Framework.CLI --version 1.4.7
dotnet tool update --global ZYC.Framework.CLI --version 1.4.7
zyc new MyCompany.Tools --template minimal
```


For manual integration, the core package can still be added directly with NuGet:

```bash
dotnet add package ZYC.Framework.Alpha --version 1.4.7
```

---



## Documentation

| Guide | Purpose |
| --- | --- |
| [Quick Start](docs/quick-start.md) | Create a project and understand the manual fallback setup. |
| [Architecture](docs/architecture.md) | Understand startup, module loading, configuration, navigation, and runtime services. |
| [Navigation and Workspace](docs/navigation-workspace.md) | Work with URI navigation, tabs, workspaces, and restore timing. |
| [Extension Points](docs/extension-points.md) | Find the public places where modules can extend the host. |
| [Built-in Modules](docs/built-in-modules.md) | Review the built-in modules and their main responsibilities. |
| [Module Development](docs/module-development.md) | Build a runtime module with contracts, `ModuleBase`, menus, and tabs. |
| [Project Templates](docs/project-templates.md) | Choose and understand the `minimal` and `modular` CLI templates. |
| [Troubleshooting](docs/troubleshooting.md) | Diagnose CLI, module loading, routing, NuGet module, Aspire, and terminal issues. |



---





## Features

### Core Framework

| Feature                | Description                                                                               |
| ---------------------- | ----------------------------------------------------------------------------------------- |
| Modular Architecture   | Organize features as modules with support for dynamic loading and extension.              |
| Multi-Workspace Layout | Split, merge, reorder, and change workspace layout directions.                            |
| Multi-Tab Navigation   | URI-based tab navigation with switching, restoration, and cross-workspace movement.       |
| Extensible Menu System | Extension points for main menu, hamburger menu, title bar, status bar, and taskbar menus. |
| Notification System    | Built-in Toast and Banner notifications for status and error feedback.                    |
| Interaction Utilities  | Desktop interaction helpers such as BusyWindow, overlays, and drag-and-drop.              |
| Hybrid UI Support      | Embed web content using `WebView2` and build hybrid UI with `Blazor`.                     |
| Configuration & State  | Local persistence for configuration, application state, and task history.                 |
| Single Instance        | Supports single-instance application startup control.                                     |
| MCP Exposure           | Automatically expose public framework and module capabilities as MCP tools.               |

### Built-in Modules

The README keeps only the high-level feature summary. See [Built-in Modules](docs/built-in-modules.md) for the current module list, loading notes, and module responsibilities.

### Development & Delivery

| Feature                  | Description                                                   |
| ------------------------ | ------------------------------------------------------------- |
| CLI Tools                | Standalone CLI entry with module command extension support.   |
| Module Scaffolding       | Generate new modules and abstraction projects from templates. |
| Documentation Generation | Generate README and documentation templates.                  |
| NuGet Packaging          | Build NuGet packages for framework and modules.               |
| Installer Build          | Build desktop installers for distribution.                    |




---



## 📸  UI Preview

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
      <img src="./docs/images/workspace-4-tabs.gif" alt="workspace-4-tabs" width="400" />
      <p align="center">Workspaces + Tabs</p>
    </td>
  </tr>
  <tr>
    <td>
      <img src="./docs/images/aspire-dashboard.gif" alt="aspire-dashboard" width="400" />
      <p align="center">Aspire Dashboard</p>
    </td>
    <td>
      <img src="./docs/images/blazor-auth.gif" alt="blazor-auth" width="400" />
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



## 📄  License


This project is open-sourced under the [MIT License](LICENSE).

---



## 💖  Acknowledgements


This project uses (and/or references parts of implementations from) the following open-source projects:

* [MahApps.Metro](https://github.com/MahApps/MahApps.Metro): UI framework.
* [MdXaml](https://github.com/whistyun/MdXaml): Markdown rendering.
* [titanium-web-proxy](https://github.com/justcoding121/titanium-web-proxy): Proxy core.
* [EasyWindowsTerminalControl](https://github.com/mitchcapper/EasyWindowsTerminalControl): Terminal integration.

> Licenses and copyrights belong to their respective authors.
> This repository uses or references them in compliance with each project's license terms.

---



## 🤝  Contributing


Issues and pull requests are welcome. If you have suggestions or found a bug, please open an issue or submit a PR.
