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
  一個基於 <b>.NET 10</b> 與 <b>WPF</b> 構建的高效能、模組化、可擴充的桌面自動化框架。
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

## 📖 專案概覽

**ZYC.Framework** 是一個現代化的桌面自動化解決方案，結合了 **WPF** 的高表現力 UI 能力與 **.NET 10** 的最新技術特性。  
其核心目標是透過模組化架構，降低複雜自動化系統的開發與維護成本。

本專案深度整合 **.NET Aspire** 以實現分散式應用程式的協調與管理，同時支援 **Blazor** 與 **WebView2** 的混合架構，讓你可以依需求在 Web UI 與原生桌面體驗之間自由取捨。

---

## ✨ 主要特性

- **模組化架構**：業務邏輯高度解耦，支援動態載入與獨立開發。
- **現代化 UI 體驗**：基於 WPF，支援 **多工作區** 與 **多分頁** 操作模式。
- **混合式開發**：
  - 整合 **WebView2**，可嵌入現代 Web 應用程式。
  - 整合 **Blazor**，在桌面應用中重用 Web 元件。
- **雲原生就緒**：內建 **.NET Aspire** 支援，簡化服務探索、治理與部署流程。
- **內建企業級能力（Batteries Included）**：
  - **任務管理**：任務排程與完整的生命週期管理。
  - **例外處理**：健全的全域錯誤捕捉與診斷機制。
  - **在地化支援**：內建多語系架構，支援全球化應用需求。

---

## 📸 UI 預覽

<table align="center">
  <tr>
    <td>
      <img src="./docs/images/workspace.png" alt="workspace" width="400" />
      <p align="center">工作區檢視</p>
    </td>
    <td>
      <img src="./docs/images/multiple-tabs.png" alt="multiple-tabs" width="400" />
      <p align="center">多分頁</p>
    </td>
  </tr>

  <tr>
    <td>
      <img src="./docs/images/workspace-4.png" alt="workspace-4" width="400" />
      <p align="center">多工作區</p>
    </td>
    <td>
      <img src="./docs/images/workspace-4-tabs.png" alt="workspace-4-tabs" width="400" />
      <p align="center">工作區 + 分頁</p>
    </td>
  </tr>

  <tr>
    <td>
      <img src="./docs/images/aspire-dashboard.png" alt="aspire-dashboard" width="400" />
      <p align="center">Aspire 儀表板</p>
    </td>
    <td>
      <img src="./docs/images/blazor-auth.png" alt="blazor-auth" width="400" />
      <p align="center">Blazor（含身分驗證）</p>
    </td>
  </tr>

  <tr>
    <td>
      <img src="./docs/images/exception.png" alt="exception" width="400" />
      <p align="center">例外處理</p>
    </td>
    <td>
      <img src="./docs/images/taskmanager.png" alt="taskmanager" width="400" />
      <p align="center">任務管理器</p>
    </td>
  </tr>
</table>

---

## 🛠️ 技術棧

- **執行環境**：.NET 10 SDK
- **UI 框架**：WPF（Windows Presentation Foundation）
- **混合 UI**：WebView2 + Blazor（Web UI）
- **應用協調**：.NET Aspire
- **架構風格**：模組化單體（Modular Monolith）／插件式架構

---

## 🚀 快速開始

請參考完整說明文件：

👉 **[Quick Start（quick-start.md）](docs/quick-start.md)**

### 安裝方式

透過 NuGet 安裝核心套件：

```bash
dotnet add package ZYC.Framework.Alpha --version [version]
````

---

## 🏗️ 專案結構

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

## 📄 授權條款

本專案以 [MIT License](LICENSE) 授權並開源。

---

## 💖 致謝

本專案使用（或參考了部分實作）以下開源專案：

* [MahApps.Metro](https://github.com/MahApps/MahApps.Metro)：UI 框架
* [MdXaml](https://github.com/whistyun/MdXaml)：Markdown 呈現
* [titanium-web-proxy](https://github.com/justcoding121/titanium-web-proxy)：代理核心
* [EasyWindowsTerminalControl](https://github.com/mitchcapper/EasyWindowsTerminalControl)：終端機整合

> 授權與著作權歸各專案原作者所有。
> 本倉庫對其使用與引用皆遵循各自的授權條款。

---

## 🤝 參與貢獻

歡迎提交 Issue 與 Pull Request。
如果你有任何建議或發現問題，請隨時提出。

