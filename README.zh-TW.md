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


一個基於 <b>.NET 10</b> 與 <b>WPF</b> 構建的高效能、模組化、可擴充的桌面自動化框架。

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



## 📖  專案概覽



**ZYC.Framework** 是一個現代化的桌面自動化解決方案，結合了 **WPF** 的高表現力 UI 能力與 **.NET 10** 的最新技術特性。其核心目標是透過模組化架構，降低複雜自動化系統的開發與維護成本。




本專案深度整合 **.NET Aspire** 以實現分散式應用程式的協調與管理，同時支援 **Blazor** 與 **WebView2** 的混合架構，讓你可以依需求在 Web UI 與原生桌面體驗之間自由取捨。


---



## ✨  主要特性

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





## 🛠️  技術棧

- **Runtime**: .NET 10 SDK
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Hybrid UI**: WebView2 + Blazor
- **Orchestration**: .NET Aspire
- **Architecture**: Modular Monolith / Plugin-based

---



## 🚀  快速開始


請參閱完整指南：


👉 **[快速開始指南 (quick-start.zh-TW.md)](docs/quick-start.zh-TW.md)**


👉 **[下載 Demo 安裝程式](https://github.com/ZiYuCai1984/ZYC.Framework/releases/download/v1.2.4/ZYC.Framework.Setup.1.2.4.exe)**

### 安裝


你可以透過 NuGet 將核心套件加入專案：

```bash
dotnet add package ZYC.Framework.Alpha --version 1.2.4
```

---







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





---



## 📸  UI 預覽

<table align="center">
  <tr>
    <td>
      <img src="./docs/images/workspace.png" alt="workspace" width="400" />
      <p align="center">工作區展示</p>
    </td>
    <td>
      <img src="./docs/images/multiple-tabs.png" alt="multiple-tabs" width="400" />
      <p align="center">多分頁展示</p>
    </td>
  </tr>
  <tr>
    <td>
      <img src="./docs/images/workspace-4.png" alt="workspace-4" width="400" />
      <p align="center">多工作區展示</p>
    </td>
    <td>
      <img src="./docs/images/workspace-4-tabs.gif" alt="workspace-4-tabs" width="400" />
      <p align="center">工作區 + 分頁</p>
    </td>
  </tr>
  <tr>
    <td>
      <img src="./docs/images/aspire-dashboard.gif" alt="aspire-dashboard" width="400" />
      <p align="center">Aspire 儀表板</p>
    </td>
    <td>
      <img src="./docs/images/blazor-auth.gif" alt="blazor-auth" width="400" />
      <p align="center">Blazor（含驗證）</p>
    </td>
  </tr>
  <tr>
    <td>
      <img src="./docs/images/exception.png" alt="exception" width="400" />
      <p align="center">例外處理</p>
    </td>
    <td>
      <img src="./docs/images/taskmanager.png" alt="taskmanager" width="400" />
      <p align="center">任務管理</p>
    </td>
  </tr>
</table>

---



## 📄  授權條款


本專案以 [MIT License](LICENSE) 授權並開源。

---



## 💖  致謝


本專案使用（或參考了部分實作）以下開源專案：

* [MahApps.Metro](https://github.com/MahApps/MahApps.Metro): UI 框架。
* [MdXaml](https://github.com/whistyun/MdXaml): Markdown 呈現。
* [titanium-web-proxy](https://github.com/justcoding121/titanium-web-proxy): 代理核心。
* [EasyWindowsTerminalControl](https://github.com/mitchcapper/EasyWindowsTerminalControl): 終端機整合。

> 授權與著作權歸各專案原作者所有。
> 本倉庫對其使用與引用皆遵循各自的授權條款。

---



## 🤝  參與貢獻


歡迎提交 Issue 與 Pull Request。如果你有任何建議或發現問題，請隨時提出。
