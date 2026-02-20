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
  <b>.NET 10</b> と <b>WPF</b> で構築した、高性能・モジュール型・拡張可能なデスクトップ自動化フレームワーク。
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

## 📖 概要

**ZYC.Framework** は、**WPF** の表現力と **.NET 10** の最新機能を活かした、モダンなデスクトップ自動化ソリューションです。モジュール指向のアーキテクチャにより、複雑な自動化システムの開発をシンプルにします。

また、本プロジェクトは分散アプリケーションのオーケストレーションのために **.NET Aspire** を深く統合しています。さらに **Blazor** と **WebView2** を利用したハイブリッド構成にも対応しており、Web / ネイティブのどちらの技術スタックも柔軟に選択できます。

---

## ✨ 主な特長

- **モジュール型アーキテクチャ**：ビジネスロジックを疎結合化し、動的ロードや独立開発を支援します。
- **モダン UI**：WPF をベースに、**マルチワークスペース**（Multi-Workspace）および **マルチタブ**（Multi-Tab）をサポートします。
- **ハイブリッド開発**：
  - **WebView2** を統合し、モダンな Web アプリをデスクトップに埋め込み可能。
  - **Blazor** を統合し、Web コンポーネントとデスクトップ側ロジックをシームレスに再利用。
- **クラウドネイティブ対応**：**.NET Aspire** を内蔵し、サービス発見・ガバナンス・デプロイを簡素化します。
- **エンタープライズ向け内蔵機能**：
  - **タスク管理**：タスクのスケジューリングとライフサイクル管理。
  - **例外処理**：グローバル例外の捕捉と診断のための仕組み。
  - **ローカライズ**：多言語対応のためのフレームワークを内蔵。

---

## 📸 UI プレビュー

<table align="center">
  <tr>
    <td>
      <img src="./docs/images/workspace.png" alt="workspace" width="400" />
      <p align="center">ワークスペース表示</p>
    </td>
    <td>
      <img src="./docs/images/multiple-tabs.png" alt="multiple-tabs" width="400" />
      <p align="center">マルチタブ表示</p>
    </td>
  </tr>

  <tr>
    <td>
      <img src="./docs/images/workspace-4.png" alt="workspace-4" width="400" />
      <p align="center">複数ワークスペース</p>
    </td>
    <td>
      <img src="./docs/images/workspace-4-tabs.png" alt="workspace-4-tabs" width="400" />
      <p align="center">複数ワークスペース + タブ</p>
    </td>
  </tr>

  <tr>
    <td>
      <img src="./docs/images/aspire-dashboard.png" alt="aspire-dashboard" width="400" />
      <p align="center">Aspire ダッシュボード</p>
    </td>
    <td>
      <img src="./docs/images/blazor-auth.png" alt="blazor-auth" width="400" />
      <p align="center">Blazor（認証付き）</p>
    </td>
  </tr>

  <tr>
    <td>
      <img src="./docs/images/exception.png" alt="exception" width="400" />
      <p align="center">例外処理</p>
    </td>
    <td>
      <img src="./docs/images/taskmanager.png" alt="taskmanager" width="400" />
      <p align="center">タスク管理</p>
    </td>
  </tr>
</table>

---

## 🛠️ 技術スタック

- **Runtime**: .NET 10 SDK
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Hybrid UI**: WebView2 + Blazor（Web UI）
- **Orchestration**: .NET Aspire
- **Architecture**: Modular Monolith / Plugin-based

---

## 🚀 クイックスタート

詳細な手順はこちら：

👉 **[クイックスタート (quick-start.md)](docs/quick-start.md)**

### インストール

NuGet でコアパッケージを追加できます：

```bash
dotnet add package ZYC.Framework.Alpha --version [バージョン]
````

---

## 🏗️ プロジェクト構成

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

## 📄 ライセンス

本プロジェクトは [MIT License](LICENSE) のもとで公開されています。

---

## 💖 謝辞

本プロジェクトは以下の OSS を利用し、また一部実装を参考にしています：

* [MahApps.Metro](https://github.com/MahApps/MahApps.Metro): UI フレームワーク。
* [MdXaml](https://github.com/whistyun/MdXaml): Markdown 表示。
* [titanium-web-proxy](https://github.com/justcoding121/titanium-web-proxy): プロキシのコア。
* [EasyWindowsTerminalControl](https://github.com/mitchcapper/EasyWindowsTerminalControl): ターミナル統合。

> ライセンスおよび著作権は各プロジェクトの作者に帰属します。
> 本リポジトリは各ライセンス条項に従って利用・参照しています。

---

## 🤝 コントリビューション

Issue / Pull Request は歓迎です。改善提案やバグ報告があれば、お気軽に Issue を立ててください。
