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
<!--doc-meta: l10n intro-tagline -->
<!--doc-l10n:begin intro-tagline-->
A high-performance, modular, and extensible desktop automation framework built with <b>.NET 10</b> and <b>WPF</b>.
<!--doc-l10n:locale ja-->
<b>.NET 10</b> と <b>WPF</b> で構築した、高性能・モジュール型・拡張可能なデスクトップ自動化フレームワーク。
<!--doc-l10n:locale zh-CN-->
基于 <b>.NET 10</b> 和 <b>WPF</b> 构建的高性能、多模块、可扩展自动化开发框架。
<!--doc-l10n:locale zh-TW-->
一個基於 <b>.NET 10</b> 與 <b>WPF</b> 構建的高效能、模組化、可擴充的桌面自動化框架。
<!--doc-l10n:locale ko-->
<b>.NET 10</b>과 <b>WPF</b>를 기반으로 구축된 고성능, 모듈화, 확장 가능한 데스크톱 자동화 프레임워크입니다.
<!--doc-l10n:end-->
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/$(PackageId)">
    <img src="https://img.shields.io/nuget/v/$(PackageId)?include_prereleases=true&logo=nuget" alt="NuGet Version" />
  </a>
  <a href="https://www.nuget.org/packages/$(PackageId)">
    <img src="https://img.shields.io/nuget/dt/$(PackageId)?logo=nuget&label=Downloads" alt="NuGet Downloads" />
  </a>
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 10" />
  <img src="https://img.shields.io/badge/Platform-WPF-orange" alt="Platform" />
  <img src="https://img.shields.io/badge/License-MIT-green" alt="License" />
</p>

<p align="center">
  <a href="$(ProjectUrl)/actions/workflows/publish-nuget-manual.yml">
    <img src="https://img.shields.io/github/actions/workflow/status/ZiYuCai1984/ZYC.Framework/publish-nuget-manual.yml?branch=main&label=build&logo=github" alt="NuGet manual workflow" />
  </a>
  <a href="$(ProjectUrl)/actions/workflows/publish-nuget-nightly.yml">
    <img src="https://img.shields.io/github/actions/workflow/status/ZiYuCai1984/ZYC.Framework/build-nightly.yml?branch=main&label=nightly%20build&logo=github" alt="Build nightly workflow" />
  </a>
</p>

---


<!--doc-meta: section overview -->
## 📖 <!--doc-meta: l10n overview-title --> <!--doc-l10n:begin overview-title-->Overview<!--doc-l10n:locale ja-->概要<!--doc-l10n:locale zh-CN-->项目简介<!--doc-l10n:locale zh-TW-->專案概覽<!--doc-l10n:locale ko-->개요<!--doc-l10n:end-->

<!--doc-meta: l10n overview-p1 -->
<!--doc-l10n:begin overview-p1-->
**ZYC.Framework** is a modern desktop automation solution that combines the expressive UI capabilities of **WPF** with the latest features of **.NET 10**. It is designed to simplify the development of complex automation systems through a modular architecture.
<!--doc-l10n:locale ja-->
**ZYC.Framework** は、**WPF** の表現力と **.NET 10** の最新機能を活かした、モダンなデスクトップ自動化ソリューションです。モジュール指向のアーキテクチャにより、複雑な自動化システムの開発をシンプルにします。
<!--doc-l10n:locale zh-CN-->
**ZYC.Framework** 是一个现代化的桌面自动化解决方案。它结合了 WPF 的强大 UI 表现力和 .NET 10 的最新特性，旨在通过模块化架构简化复杂自动化系统的开发流程。
<!--doc-l10n:locale zh-TW-->
**ZYC.Framework** 是一個現代化的桌面自動化解決方案，結合了 **WPF** 的高表現力 UI 能力與 **.NET 10** 的最新技術特性。其核心目標是透過模組化架構，降低複雜自動化系統的開發與維護成本。
<!--doc-l10n:locale ko-->
**ZYC.Framework**은 **WPF**의 표현력 있는 UI 기능과 **.NET 10**의 최신 기능을 결합한 현대적인 데스크톱 자동화 솔루션입니다. 모듈화된 아키텍처를 통해 복잡한 자동화 시스템의 개발과 유지보수를 단순화하는 것을 목표로 합니다.
<!--doc-l10n:end-->

<!--doc-meta: l10n overview-p2 -->
<!--doc-l10n:begin overview-p2-->
The project deeply integrates **.NET Aspire** for distributed application orchestration, and supports a hybrid approach with **Blazor** and **WebView2**, so you can choose between a Web-based UI and a native desktop experience as needed.
<!--doc-l10n:locale ja-->
また、本プロジェクトは分散アプリケーションのオーケストレーションのために **.NET Aspire** を深く統合しています。さらに **Blazor** と **WebView2** を利用したハイブリッド構成にも対応しており、Web / ネイティブのどちらの技術スタックも柔軟に選択できます。
<!--doc-l10n:locale zh-CN-->
项目深度集成了 **.NET Aspire** 用于分布式应用编排，并支持 **Blazor** 与 **WebView2** 的混合开发模式，让你可以自由选择 Web 或原生技术栈。
<!--doc-l10n:locale zh-TW-->
本專案深度整合 **.NET Aspire** 以實現分散式應用程式的協調與管理，同時支援 **Blazor** 與 **WebView2** 的混合架構，讓你可以依需求在 Web UI 與原生桌面體驗之間自由取捨。
<!--doc-l10n:locale ko-->
본 프로젝트는 분산 애플리케이션 오케스트레이션을 위해 **.NET Aspire**를 깊이 통합하고 있으며, **Blazor** 및 **WebView2** 기반의 하이브리드 접근 방식을 지원하여 Web UI와 네이티브 데스크톱 경험 중 필요에 따라 선택할 수 있습니다.
<!--doc-l10n:end-->

---


<!--doc-meta: section features -->
## ✨ <!--doc-meta: l10n features-title --> <!--doc-l10n:begin features-title-->Key Features<!--doc-l10n:locale ja-->主な特長<!--doc-l10n:locale zh-CN-->核心特性<!--doc-l10n:locale zh-TW-->主要特性<!--doc-l10n:locale ko-->주요 기능<!--doc-l10n:end-->

- <!--doc-l10n:begin feature-modular-->**Modular Architecture**: Decoupled business logic with dynamic loading and independent development.<!--doc-l10n:locale ja-->**モジュール型アーキテクチャ**：ビジネスロジックを疎結合化し、動的ロードや独立開発を支援します。<!--doc-l10n:locale zh-CN-->**多模块架构**：解耦业务逻辑，支持动态加载与独立开发。<!--doc-l10n:locale zh-TW-->**模組化架構**：業務邏輯高度解耦，支援動態載入與獨立開發。<!--doc-l10n:locale ko-->**모듈화 아키텍처**: 비즈니스 로직을 분리하여 동적 로딩과 독립적인 개발을 지원합니다.<!--doc-l10n:end-->
- <!--doc-l10n:begin feature-ui-->**Modern UI Experience**: Built on WPF with support for **multi-workspace** and **multi-tab** interactions.<!--doc-l10n:locale ja-->**モダン UI**：WPF をベースに、**マルチワークスペース**（Multi-Workspace）および **マルチタブ**（Multi-Tab）をサポートします。<!--doc-l10n:locale zh-CN-->**现代化 UI 支持**：基于 WPF，支持多工作区（Multi-Workspace）与多标签页（Multi-Tab）交互。<!--doc-l10n:locale zh-TW-->**現代化 UI 體驗**：基於 WPF，支援 **多工作區** 與 **多分頁** 操作模式。<!--doc-l10n:locale ko-->**현대적인 UI 경험**: WPF 기반으로 **다중 워크스페이스** 및 **다중 탭** 인터랙션을 지원합니다.<!--doc-l10n:end-->
- <!--doc-l10n:begin feature-hybrid-title-->**Hybrid Development**:<!--doc-l10n:locale ja-->**ハイブリッド開発**：<!--doc-l10n:locale zh-CN-->**混合开发模式**：<!--doc-l10n:locale zh-TW-->**混合式開發**：<!--doc-l10n:locale ko-->**하이브리드 개발**:<!--doc-l10n:end-->
  - <!--doc-l10n:begin feature-webview2-->**WebView2** integration for embedding modern Web applications.<!--doc-l10n:locale ja-->**WebView2** を統合し、モダンな Web アプリをデスクトップに埋め込み可能。<!--doc-l10n:locale zh-CN-->集成 **WebView2**，轻松嵌入现代 Web 应用。<!--doc-l10n:locale zh-TW-->整合 **WebView2**，可嵌入現代 Web 應用程式。<!--doc-l10n:locale ko-->**WebView2**를 통한 최신 Web 애플리케이션 임베딩.<!--doc-l10n:end-->
  - <!--doc-l10n:begin feature-blazor-->**Blazor** integration to reuse Web components seamlessly in desktop scenarios.<!--doc-l10n:locale ja-->**Blazor** を統合し、Web コンポーネントとデスクトップ側ロジックをシームレスに再利用。<!--doc-l10n:locale zh-CN-->集成 **Blazor**，实现 Web 组件与桌面端逻辑的无缝复用。<!--doc-l10n:locale zh-TW-->整合 **Blazor**，在桌面應用中重用 Web 元件。<!--doc-l10n:locale ko-->**Blazor**를 활용한 데스크톱 환경에서의 Web 컴포넌트 재사용.<!--doc-l10n:end-->
- <!--doc-l10n:begin feature-cloud-->**Cloud-Native Ready**: Built-in **.NET Aspire** support to simplify service discovery, governance, and deployment.<!--doc-l10n:locale ja-->**クラウドネイティブ対応**：**.NET Aspire** を内蔵し、サービス発見・ガバナンス・デプロイを簡素化します。<!--doc-l10n:locale zh-CN-->**云原生就绪**：内置对 **.NET Aspire** 的支持，简化服务发现、治理与部署。<!--doc-l10n:locale zh-TW-->**雲原生就緒**：內建 **.NET Aspire** 支援，簡化服務探索、治理與部署流程。<!--doc-l10n:locale ko-->**클라우드 네이티브 대응**: **.NET Aspire** 내장 지원으로 서비스 탐색, 거버넌스 및 배포를 단순화합니다.<!--doc-l10n:end-->
- <!--doc-l10n:begin feature-batteries-title-->**Batteries Included (Enterprise-Oriented)**:<!--doc-l10n:locale ja-->**エンタープライズ向け内蔵機能**：<!--doc-l10n:locale zh-CN-->**企业级内置支持**：<!--doc-l10n:locale zh-TW-->**內建企業級能力（Batteries Included）**：<!--doc-l10n:locale ko-->**엔터프라이즈 지향 기능 내장 (Batteries Included)**:<!--doc-l10n:end-->
  - <!--doc-l10n:begin feature-task-->**Task Management**: Task scheduling and lifecycle management.<!--doc-l10n:locale ja-->**タスク管理**：タスクのスケジューリングとライフサイクル管理。<!--doc-l10n:locale zh-CN-->**任务管理**：任务调度与生命周期管理。<!--doc-l10n:locale zh-TW-->**任務管理**：任務排程與完整的生命週期管理。<!--doc-l10n:locale ko-->**작업 관리**: 작업 스케줄링 및 전체 라이프사이클 관리.<!--doc-l10n:end-->
  - <!--doc-l10n:begin feature-exception-->**Exception Handling**: Robust global error capture and diagnostics.<!--doc-l10n:locale ja-->**例外処理**：グローバル例外の捕捉と診断のための仕組み。<!--doc-l10n:locale zh-CN-->**异常处理**：全局错误捕获与日志追溯机制。<!--doc-l10n:locale zh-TW-->**例外處理**：健全的全域錯誤捕捉與診斷機制。<!--doc-l10n:locale ko-->**예외 처리**: 강력한 전역 오류 수집 및 진단 메커니즘.<!--doc-l10n:end-->
  - <!--doc-l10n:begin feature-localization-->**Localization**: Built-in multi-language support for global-ready apps.<!--doc-l10n:locale ja-->**ローカライズ**：多言語対応のためのフレームワークを内蔵。<!--doc-l10n:locale zh-CN-->**本地化框架**：内置多语言支持方案，适应全球化场景。<!--doc-l10n:locale zh-TW-->**在地化支援**：內建多語系架構，支援全球化應用需求。<!--doc-l10n:locale ko-->**다국어 지원**: 글로벌 애플리케이션을 위한 내장 로컬라이제이션 기능.<!--doc-l10n:end-->

---




<!--doc-meta: section tech-stack -->
## 🛠️ <!--doc-meta: l10n stack-title --> <!--doc-l10n:begin stack-title-->Tech Stack<!--doc-l10n:locale ja-->技術スタック<!--doc-l10n:locale zh-CN-->技术栈<!--doc-l10n:locale zh-TW-->技術棧<!--doc-l10n:locale ko-->기술 스택<!--doc-l10n:end-->

- **Runtime**: .NET 10 SDK
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Hybrid UI**: WebView2 + Blazor
- **Orchestration**: .NET Aspire
- **Architecture**: Modular Monolith / Plugin-based

---


<!--doc-meta: section quick-start -->
## 🚀 <!--doc-meta: l10n quickstart-title --> <!--doc-l10n:begin quickstart-title-->Quick Start<!--doc-l10n:locale ja-->クイックスタート<!--doc-l10n:locale zh-CN-->快速开始<!--doc-l10n:locale zh-TW-->快速開始<!--doc-l10n:locale ko-->빠른 시작<!--doc-l10n:end-->

<!--doc-meta: l10n quickstart-lead -->
<!--doc-l10n:begin quickstart-lead-->Please refer to the detailed guide:<!--doc-l10n:locale ja-->詳細な手順はこちら：<!--doc-l10n:locale zh-CN-->请参阅我们准备好的详细指南：<!--doc-l10n:locale zh-TW-->請參閱完整指南：<!--doc-l10n:locale ko-->자세한 내용은 다음 가이드를 참고하세요:<!--doc-l10n:end-->

<!--doc-l10n:begin quickstart-link-line-->
👉 **[Quick Start (quick-start.md)](docs/quick-start.md)**
<!--doc-l10n:locale ja-->
👉 **[クイックスタート (quick-start.ja.md)](docs/quick-start.ja.md)**
<!--doc-l10n:locale zh-CN-->
👉 **[快速开始指南 (quick-start.zh-CN.md)](docs/quick-start.zh-CN.md)**
<!--doc-l10n:locale zh-TW-->
👉 **[快速開始指南 (quick-start.zh-TW.md)](docs/quick-start.zh-TW.md)**
<!--doc-l10n:locale ko-->
👉 **[빠른 시작 가이드 (quick-start.ko.md)](docs/quick-start.ko.md)**
<!--doc-l10n:end-->

👉 **[<!--doc-l10n:begin quickstart-demo-installer-link-->Download Demo Installer<!--doc-l10n:locale ja-->デモ インストーラーをダウンロード<!--doc-l10n:locale zh-CN-->下载 Demo 安装包<!--doc-l10n:locale zh-TW-->下載 Demo 安裝程式<!--doc-l10n:locale ko-->데모 설치 프로그램 다운로드<!--doc-l10n:end-->](https://github.com/ZiYuCai1984/ZYC.Framework/releases/download/v$(Version)/ZYC.Framework.Setup.$(Version).exe)**

### <!--doc-l10n:begin install-title-->Installation<!--doc-l10n:locale ja-->インストール<!--doc-l10n:locale zh-CN-->安装<!--doc-l10n:locale zh-TW-->安裝<!--doc-l10n:locale ko-->설치<!--doc-l10n:end-->

<!--doc-meta: l10n install-lead -->
<!--doc-l10n:begin install-lead-->Install the core package via NuGet:<!--doc-l10n:locale ja-->NuGet でコアパッケージを追加できます：<!--doc-l10n:locale zh-CN-->你可以通过 NuGet 直接将核心包引入你的项目：<!--doc-l10n:locale zh-TW-->你可以透過 NuGet 將核心套件加入專案：<!--doc-l10n:locale ko-->NuGet을 통해 코어 패키지를 설치할 수 있습니다:<!--doc-l10n:end-->

```bash
dotnet add package $(PackageId) --version $(Version)
```

---

<!--doc-meta: section features -->

<!--doc-l10n:begin features-->

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

| Module        | Description                                                           |
| ------------- | --------------------------------------------------------------------- |
| About         | Application metadata (version, author, description, copyright).       |
| ApiReference  | Built-in API documentation viewer.                                    |
| CLI           | Embedded terminal with command execution and parameter support.       |
| FileExplorer  | Embedded file explorer for directory browsing.                        |
| WebBrowser    | Embedded browser for web and local content.                           |
| Language      | Language switching and localization resource management.              |
| Translator    | Translation service integration.                                      |
| Settings      | Unified configuration management UI.                                  |
| Secrets       | Secure configuration editing and display.                             |
| TaskManager   | Task queue, execution, pause/resume, cancel, and status tracking.     |
| Update        | NuGet-based version checking and update workflow.                     |
| NuGet         | Package search, download, dependency handling, and caching.           |
| ModuleManager | Install, enable, disable, and manage local or NuGet modules.          |
| MCP.Server    | Local MCP server exposing application capabilities.                   |
| Aspire        | `Aspire` tool integration with service control and dashboard support. |
| Log           | Logging infrastructure and log directory access.                      |
| BlazorDemo    | Demonstrates `Blazor Server` integration.                             |
| Mock          | Test module for validating framework capabilities.                    |

### Development & Delivery

| Feature                  | Description                                                   |
| ------------------------ | ------------------------------------------------------------- |
| CLI Tools                | Standalone CLI entry with module command extension support.   |
| Module Scaffolding       | Generate new modules and abstraction projects from templates. |
| Documentation Generation | Generate README and documentation templates.                  |
| NuGet Packaging          | Build NuGet packages for framework and modules.               |
| Installer Build          | Build desktop installers for distribution.                    |

<!--doc-l10n:locale ja-->

## 主な機能

### コアフレームワーク

| 機能           | 説明                                     |
| ------------ | -------------------------------------- |
| モジュールアーキテクチャ | 機能をモジュール単位で構成し、動的ロードと拡張をサポート。          |
| マルチワークスペース   | ワークスペースの分割・結合・並び替え・方向変更に対応。            |
| マルチタブナビゲーション | URI ベースのタブナビゲーション、復元、ワークスペース間移動。       |
| 拡張可能メニュー     | メインメニュー、ハンバーガーメニュー、タイトルバーなどの拡張ポイント。    |
| 通知システム       | Toast / Banner による通知表示。                |
| UI インタラクション  | BusyWindow、Overlay、ドラッグ＆ドロップ処理などを提供。   |
| ハイブリッド UI    | `WebView2` と `Blazor` によるハイブリッド UI 構築。 |
| 設定・状態管理      | 設定や状態、タスク履歴のローカル保存。                    |
| シングルインスタンス   | アプリケーションの単一インスタンス起動制御。                 |
| MCP 公開       | フレームワーク機能を MCP ツールとして公開可能。             |

### 組み込みモジュール

| モジュール         | 説明                    |
| ------------- | --------------------- |
| About         | バージョンや作者などの基本情報表示。    |
| ApiReference  | アプリ内で API ドキュメントを閲覧。  |
| CLI           | コマンド実行可能な内蔵ターミナル。     |
| FileExplorer  | ディレクトリ閲覧用ファイルエクスプローラ。 |
| WebBrowser    | Web / ローカルページ閲覧用ブラウザ。 |
| Language      | 多言語切替とローカライズ管理。       |
| Translator    | 翻訳サービス統合。             |
| Settings      | 統合設定管理画面。             |
| Secrets       | 機密設定の安全な管理。           |
| TaskManager   | タスク管理と状態追跡。           |
| Update        | NuGet ベースの更新機能。       |
| NuGet         | NuGet パッケージ検索と管理。     |
| ModuleManager | モジュールのインストール・有効化・管理。  |
| MCP.Server    | ローカル MCP Server。      |
| Aspire        | `Aspire` ツール統合。       |
| Log           | ログ機能とログディレクトリ。        |
| BlazorDemo    | Blazor Server 統合デモ。   |
| Mock          | テスト用モジュール。            |

### 開発・配布

| 機能          | 説明                 |
| ----------- | ------------------ |
| CLI ツール     | コマンドラインインターフェース。   |
| モジュールテンプレート | 新規モジュール生成テンプレート。   |
| ドキュメント生成    | README / ドキュメント生成。 |
| NuGet パッケージ | NuGet パッケージ作成。     |
| インストーラ      | デスクトップインストーラ生成。    |


<!--doc-l10n:locale zh-CN-->


## 功能

### 核心框架

| 功能       | 说明                                              |
| -------- | ----------------------------------------------- |
| 模块化架构    | 支持按模块组织功能，并支持动态加载与扩展。                           |
| 多工作区布局   | 支持工作区分割、合并、方向切换和位置交换。                           |
| 多标签页导航   | 支持基于 URI 的标签页导航、恢复和跨工作区移动。                      |
| 可扩展菜单系统  | 提供主菜单、Hamburger 菜单、窗口标题栏、状态栏等扩展点。               |
| 通知系统     | 内置 Toast、Banner 等通知能力，用于状态和错误提示。                |
| 交互辅助能力   | 提供 BusyWindow、Overlay、拖放处理等桌面交互基础设施。            |
| 混合界面支持   | 支持通过 `WebView2` 嵌入 Web 内容，并可结合 `Blazor` 构建混合应用。 |
| 配置与状态持久化 | 支持配置、应用状态和任务记录的本地持久化。                           |
| 单实例运行    | 支持应用单实例启动控制。                                    |
| MCP 暴露机制 | 可将框架和模块中的公开能力自动暴露为 MCP 工具。                      |

### 内置模块

| 模块            | 说明                                   |
| ------------- | ------------------------------------ |
| About         | 显示应用基本信息，如版本、作者和描述。                  |
| ApiReference  | 内置 API 文档查看页，可在应用内浏览生成的文档。           |
| CLI           | 内嵌终端页面，支持命令执行和参数控制。                  |
| FileExplorer  | 内置文件资源管理器视图，支持目录浏览。                  |
| WebBrowser    | 内置浏览器模块，支持访问网页和本地内容。                 |
| Language      | 提供多语言切换和本地化资源管理。                     |
| Translator    | 提供翻译服务接入能力。                          |
| Settings      | 统一的设置管理界面，支持查看、编辑和重置配置。              |
| Secrets       | 提供敏感配置的安全查看与编辑。                      |
| TaskManager   | 支持任务入队、执行、暂停、恢复、取消和状态追踪。             |
| Update        | 基于 NuGet 的版本检查、下载和更新流程。              |
| NuGet         | 提供 NuGet 包查询、下载、依赖处理和缓存管理。           |
| ModuleManager | 管理本地模块和 NuGet 模块的安装、卸载、启用和禁用。        |
| MCP.Server    | 提供本地 MCP Server，可将应用功能对外暴露为工具服务。     |
| Aspire        | 提供 `Aspire` 工具下载、服务控制和 Dashboard 集成。 |
| Log           | 提供日志功能和日志目录访问入口。                     |
| BlazorDemo    | `Blazor Server` 集成示例模块。              |
| Mock          | 用于测试和演示框架能力的模块。                      |

### 开发与交付

| 功能       | 说明                   |
| -------- | -------------------- |
| CLI 工具   | 提供命令行入口，支持模块级命令扩展。   |
| 模块脚手架    | 支持通过模板快速生成新模块及其抽象项目。 |
| 文档生成     | 支持生成 README 和文档模板内容。 |
| NuGet 打包 | 支持框架和模块的 NuGet 打包流程。 |
| 安装包构建    | 支持生成桌面安装包，方便分发与部署。   |


<!--doc-l10n:locale zh-TW-->



## 功能

### 核心框架

| 功能     | 說明                            |
| ------ | ----------------------------- |
| 模組化架構  | 支援模組化功能組織與動態載入。               |
| 多工作區佈局 | 支援工作區分割、合併與重新排列。              |
| 多分頁導航  | 基於 URI 的分頁導航與恢復。              |
| 可擴展選單  | 主選單、Hamburger 選單等擴展點。         |
| 通知系統   | 內建 Toast 與 Banner 通知。         |
| 互動輔助   | BusyWindow、Overlay、拖放處理。      |
| 混合 UI  | `WebView2` + `Blazor` 混合桌面應用。 |
| 設定與狀態  | 本地持久化設定與應用狀態。                 |
| 單例執行   | 支援單實例應用啟動。                    |
| MCP 暴露 | 可將功能暴露為 MCP 工具。               |

### 內建模組

| 模組            | 說明           |
| ------------- | ------------ |
| About         | 應用基本資訊。      |
| ApiReference  | 內建 API 文件瀏覽。 |
| CLI           | 內建終端。        |
| FileExplorer  | 檔案瀏覽器。       |
| WebBrowser    | 內建瀏覽器。       |
| Language      | 多語言管理。       |
| Translator    | 翻譯服務整合。      |
| Settings      | 設定管理。        |
| Secrets       | 敏感設定管理。      |
| TaskManager   | 任務管理與追蹤。     |
| Update        | NuGet 更新。    |
| NuGet         | 套件管理。        |
| ModuleManager | 模組管理。        |
| MCP.Server    | MCP 服務。      |
| Aspire        | Aspire 工具整合。 |
| Log           | 日誌功能。        |
| BlazorDemo    | Blazor 整合示例。 |
| Mock          | 測試模組。        |

### 開發與交付

| 功能       | 說明                |
| -------- | ----------------- |
| CLI 工具   | 命令列入口。            |
| 模組模板     | 快速建立新模組。          |
| 文件生成     | README / Docs 生成。 |
| NuGet 打包 | NuGet 套件生成。       |
| 安裝包      | 桌面安裝程式建構。         |


<!--doc-l10n:locale ko-->


## 기능

### 핵심 프레임워크

| 기능        | 설명                                   |
| --------- | ------------------------------------ |
| 모듈 아키텍처   | 기능을 모듈 단위로 구성하고 동적 확장을 지원합니다.        |
| 멀티 워크스페이스 | 워크스페이스 분할, 병합, 재배치 지원.               |
| 멀티 탭 탐색   | URI 기반 탭 탐색 및 복원.                    |
| 확장 가능한 메뉴 | 메인 메뉴 및 다양한 UI 확장 지점 제공.             |
| 알림 시스템    | Toast 및 Banner 알림 제공.                |
| UI 인터랙션   | BusyWindow, Overlay, Drag & Drop 지원. |
| 하이브리드 UI  | `WebView2` + `Blazor` 기반 UI.         |
| 설정 및 상태   | 설정과 상태 로컬 저장.                        |
| 단일 인스턴스   | 단일 인스턴스 실행 지원.                       |
| MCP 노출    | MCP 도구로 기능 노출 가능.                    |

### 내장 모듈

| 모듈            | 설명             |
| ------------- | -------------- |
| About         | 애플리케이션 정보 표시.  |
| ApiReference  | API 문서 뷰어.     |
| CLI           | 내장 터미널.        |
| FileExplorer  | 파일 탐색기.        |
| WebBrowser    | 내장 브라우저.       |
| Language      | 다국어 관리.        |
| Translator    | 번역 서비스 통합.     |
| Settings      | 설정 관리 UI.      |
| Secrets       | 민감 설정 관리.      |
| TaskManager   | 작업 관리 및 추적.    |
| Update        | NuGet 기반 업데이트. |
| NuGet         | 패키지 관리.        |
| ModuleManager | 모듈 관리.         |
| MCP.Server    | MCP 서버.        |
| Aspire        | Aspire 도구 통합.  |
| Log           | 로그 기능.         |
| BlazorDemo    | Blazor 통합 데모.  |
| Mock          | 테스트 모듈.        |

### 개발 및 배포

| 기능        | 설명               |
| --------- | ---------------- |
| CLI 도구    | 명령줄 인터페이스.       |
| 모듈 템플릿    | 새 모듈 생성 템플릿.     |
| 문서 생성     | README / 문서 생성.  |
| NuGet 패키징 | NuGet 패키지 빌드.    |
| 설치 프로그램   | 데스크톱 설치 프로그램 빌드. |


<!--doc-l10n:end-->


---


<!--doc-meta: section preview -->
## 📸 <!--doc-meta: l10n preview-title --> <!--doc-l10n:begin preview-title-->UI Preview<!--doc-l10n:locale ja-->UI プレビュー<!--doc-l10n:locale zh-CN-->界面预览<!--doc-l10n:locale zh-TW-->UI 預覽<!--doc-l10n:locale ko-->UI 미리보기<!--doc-l10n:end-->

<table align="center">
  <tr>
    <td>
      <img src="./docs/images/workspace.png" alt="workspace" width="400" />
      <p align="center"><!--doc-l10n:begin preview-workspace-->Workspace View<!--doc-l10n:locale ja-->ワークスペース表示<!--doc-l10n:locale zh-CN-->工作区展示<!--doc-l10n:locale zh-TW-->工作區展示<!--doc-l10n:locale ko-->워크스페이스 보기<!--doc-l10n:end--></p>
    </td>
    <td>
      <img src="./docs/images/multiple-tabs.png" alt="multiple-tabs" width="400" />
      <p align="center"><!--doc-l10n:begin preview-tabs-->Multiple Tabs<!--doc-l10n:locale ja-->マルチタブ表示<!--doc-l10n:locale zh-CN-->多 Tab 展示<!--doc-l10n:locale zh-TW-->多分頁展示<!--doc-l10n:locale ko-->다중 탭 표시<!--doc-l10n:end--></p>
    </td>
  </tr>
  <tr>
    <td>
      <img src="./docs/images/workspace-4.png" alt="workspace-4" width="400" />
      <p align="center"><!--doc-l10n:begin preview-workspaces-->Multiple Workspaces<!--doc-l10n:locale ja-->複数ワークスペース<!--doc-l10n:locale zh-CN-->多工作区展示<!--doc-l10n:locale zh-TW-->多工作區展示<!--doc-l10n:locale ko-->다중 워크스페이스<!--doc-l10n:end--></p>
    </td>
    <td>
      <img src="./docs/images/workspace-4-tabs.gif" alt="workspace-4-tabs" width="400" />
      <p align="center"><!--doc-l10n:begin preview-workspaces-tabs-->Workspaces + Tabs<!--doc-l10n:locale ja-->複数ワークスペース + タブ<!--doc-l10n:locale zh-CN-->多工作区 Tab 展示<!--doc-l10n:locale zh-TW-->工作區 + 分頁<!--doc-l10n:locale ko-->워크스페이스 + 탭<!--doc-l10n:end--></p>
    </td>
  </tr>
  <tr>
    <td>
      <img src="./docs/images/aspire-dashboard.gif" alt="aspire-dashboard" width="400" />
      <p align="center"><!--doc-l10n:begin preview-aspire-->Aspire Dashboard<!--doc-l10n:locale ja-->Aspire ダッシュボード<!--doc-l10n:locale zh-CN-->Aspire 仪表板<!--doc-l10n:locale zh-TW-->Aspire 儀表板<!--doc-l10n:locale ko-->Aspire 대시보드<!--doc-l10n:end--></p>
    </td>
    <td>
      <img src="./docs/images/blazor-auth.gif" alt="blazor-auth" width="400" />
      <p align="center"><!--doc-l10n:begin preview-blazor-->Blazor (with Auth)<!--doc-l10n:locale ja-->Blazor（認証付き）<!--doc-l10n:locale zh-CN-->Blazor with Auth<!--doc-l10n:locale zh-TW-->Blazor（含驗證）<!--doc-l10n:locale ko-->Blazor (인증 포함)<!--doc-l10n:end--></p>
    </td>
  </tr>
  <tr>
    <td>
      <img src="./docs/images/exception.png" alt="exception" width="400" />
      <p align="center"><!--doc-l10n:begin preview-exception-->Exception Handling<!--doc-l10n:locale ja-->例外処理<!--doc-l10n:locale zh-CN-->异常处理<!--doc-l10n:locale zh-TW-->例外處理<!--doc-l10n:locale ko-->예외 처리<!--doc-l10n:end--></p>
    </td>
    <td>
      <img src="./docs/images/taskmanager.png" alt="taskmanager" width="400" />
      <p align="center"><!--doc-l10n:begin preview-taskmanager-->Task Manager<!--doc-l10n:locale ja-->タスク管理<!--doc-l10n:locale zh-CN-->任务管理<!--doc-l10n:locale zh-TW-->任務管理<!--doc-l10n:locale ko-->작업 관리자<!--doc-l10n:end--></p>
    </td>
  </tr>
</table>

---


<!--doc-meta: section license -->
## 📄 <!--doc-meta: l10n license-title --> <!--doc-l10n:begin license-title-->License<!--doc-l10n:locale ja-->ライセンス<!--doc-l10n:locale zh-CN-->开源协议<!--doc-l10n:locale zh-TW-->授權條款<!--doc-l10n:locale ko-->라이선스<!--doc-l10n:end-->

<!--doc-meta: l10n license-body -->
<!--doc-l10n:begin license-body-->This project is open-sourced under the [MIT License](LICENSE).<!--doc-l10n:locale ja-->本プロジェクトは [MIT License](LICENSE) のもとで公開されています。<!--doc-l10n:locale zh-CN-->本项目基于 [MIT License](LICENSE) 开源。<!--doc-l10n:locale zh-TW-->本專案以 [MIT License](LICENSE) 授權並開源。<!--doc-l10n:locale ko-->본 프로젝트는 [MIT License](LICENSE) 하에 오픈소스로 제공됩니다.<!--doc-l10n:end-->

---


<!--doc-meta: section acknowledgements -->
## 💖 <!--doc-meta: l10n thanks-title --> <!--doc-l10n:begin thanks-title-->Acknowledgements<!--doc-l10n:locale ja-->謝辞<!--doc-l10n:locale zh-CN-->鸣谢<!--doc-l10n:locale zh-TW-->致謝<!--doc-l10n:locale ko-->감사의 말<!--doc-l10n:end-->

<!--doc-meta: l10n thanks-lead -->
<!--doc-l10n:begin thanks-lead-->This project uses (and/or references parts of implementations from) the following open-source projects:<!--doc-l10n:locale ja-->本プロジェクトは以下の OSS を利用し、また一部実装を参考にしています：<!--doc-l10n:locale zh-CN-->本项目使用了以下开源库/参考了其部分实现：<!--doc-l10n:locale zh-TW-->本專案使用（或參考了部分實作）以下開源專案：<!--doc-l10n:locale ko-->본 프로젝트는 다음 오픈소스 프로젝트를 사용하거나(또는 일부 구현을 참고하였습니다):<!--doc-l10n:end-->

* [MahApps.Metro](https://github.com/MahApps/MahApps.Metro): <!--doc-l10n:begin thanks-mahapps-->UI framework.<!--doc-l10n:locale ja-->UI フレームワーク。<!--doc-l10n:locale zh-CN-->UI 框架支持。<!--doc-l10n:locale zh-TW-->UI 框架。<!--doc-l10n:locale ko-->UI 프레임워크.<!--doc-l10n:end-->
* [MdXaml](https://github.com/whistyun/MdXaml): <!--doc-l10n:begin thanks-mdxaml-->Markdown rendering.<!--doc-l10n:locale ja-->Markdown 表示。<!--doc-l10n:locale zh-CN-->文档预览支持。<!--doc-l10n:locale zh-TW-->Markdown 呈現。<!--doc-l10n:locale ko-->Markdown 렌더링.<!--doc-l10n:end-->
* [titanium-web-proxy](https://github.com/justcoding121/titanium-web-proxy): <!--doc-l10n:begin thanks-proxy-->Proxy core.<!--doc-l10n:locale ja-->プロキシのコア。<!--doc-l10n:locale zh-CN-->网络代理核心。<!--doc-l10n:locale zh-TW-->代理核心。<!--doc-l10n:locale ko-->프록시 코어.<!--doc-l10n:end-->
* [EasyWindowsTerminalControl](https://github.com/mitchcapper/EasyWindowsTerminalControl): <!--doc-l10n:begin thanks-terminal-->Terminal integration.<!--doc-l10n:locale ja-->ターミナル統合。<!--doc-l10n:locale zh-CN-->终端集成方案。<!--doc-l10n:locale zh-TW-->終端機整合。<!--doc-l10n:locale ko-->터미널 통합.<!--doc-l10n:end-->

> <!--doc-l10n:begin thanks-note-1-->Licenses and copyrights belong to their respective authors.<!--doc-l10n:locale ja-->ライセンスおよび著作権は各プロジェクトの作者に帰属します。<!--doc-l10n:locale zh-CN-->以上项目的许可证归其原作者所有；<!--doc-l10n:locale zh-TW-->授權與著作權歸各專案原作者所有。<!--doc-l10n:locale ko-->라이선스 및 저작권은 각 프로젝트의 원저작자에게 귀속됩니다.<!--doc-l10n:end-->
> <!--doc-l10n:begin thanks-note-2-->This repository uses or references them in compliance with each project's license terms.<!--doc-l10n:locale ja-->本リポジトリは各ライセンス条項に従って利用・参照しています。<!--doc-l10n:locale zh-CN-->本仓库在遵循对应许可证条款的前提下使用/参考其实现。<!--doc-l10n:locale zh-TW-->本倉庫對其使用與引用皆遵循各自的授權條款。<!--doc-l10n:locale ko-->본 저장소는 각 프로젝트의 라이선스 조건을 준수하여 사용 및 참조합니다.<!--doc-l10n:end-->

---


<!--doc-meta: section contributing -->
## 🤝 <!--doc-meta: l10n contributing-title --> <!--doc-l10n:begin contributing-title-->Contributing<!--doc-l10n:locale ja-->コントリビューション<!--doc-l10n:locale zh-CN-->贡献<!--doc-l10n:locale zh-TW-->參與貢獻<!--doc-l10n:locale ko-->기여하기<!--doc-l10n:end-->

<!--doc-meta: l10n contributing-body -->
<!--doc-l10n:begin contributing-body-->Issues and pull requests are welcome. If you have suggestions or found a bug, please open an issue or submit a PR.<!--doc-l10n:locale ja-->Issue / Pull Request は歓迎です。改善提案やバグ報告があれば、お気軽に Issue を立ててください。<!--doc-l10n:locale zh-CN-->如果你有任何建议或发现了 Bug，欢迎提交 Issue 或 Pull Request。<!--doc-l10n:locale zh-TW-->歡迎提交 Issue 與 Pull Request。如果你有任何建議或發現問題，請隨時提出。<!--doc-l10n:locale ko-->Issue 및 Pull Request를 환영합니다. 제안 사항이나 버그를 발견하셨다면 언제든지 Issue를 열거나 PR을 제출해 주세요.<!--doc-l10n:end-->
