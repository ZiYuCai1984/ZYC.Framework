<p align="center">
  <a href="./built-in-modules.md">English</a> |
  <a href="./built-in-modules.ja.md">日本語</a> |
  <a href="./built-in-modules.zh-CN.md">简体中文</a> |
  <a href="./built-in-modules.zh-TW.md">繁體中文</a> |
  <a href="./built-in-modules.ko.md">한국어</a> |
</p>


# 內建模組

本文彙總目前 ZYC.Framework 原始碼樹中的內建模組。這裡的內建模組指包含 `Module : ModuleBase` 入口，並 intended 由模組載入器發現的 `ZYC.Framework.Modules.*` 專案。

`ZYC.Framework.Modules.*.Abstractions` 這類 Abstractions 專案是契約組件，本身不是執行階段模組。

## 內建模組如何載入

啟動時，模組載入器會在應用程式目錄掃描命名類似 `ZYC.Framework.Modules*.dll` 的組件。對於每個模組組件，它會：

- 註冊組件裡的 Autofac 服務；
- 從 settings 目錄載入具體的 `IConfig` 和 `IState` 型別；
- 找到第一個繼承 `ModuleBase` 的型別；
- 建立模組實例並呼叫 `RegisterAsync`；
- 稍後只對啟用的模組呼叫 `LoadAsync`。

`ModuleConfig.DisabledAssemblyNames` 依檔名停用已發現的模組組件。`ModuleConfig.AdditionalAssemblyNames` 從應用目錄追加額外組件。

## 模組清單

| Module | 主要表面 | 說明 |
| --- | --- | --- |
| `About` | About 選單與路由 Tab | 顯示產品/about 資訊。 |
| `ApiReference` | About 選單與 WebView2 Tab | 承載 API reference 內容。 |
| `Aspire` | Tools 選單、路由 Tab、狀態列 | 啟動與監控 Aspire resources；解析 `IExtensionResourcesProvider` 貢獻。 |
| `BlazorDemo` | Tools 選單與路由 Tab | 示範桌面 Host 內的 Blazor 整合。 |
| `CLI` | Tools 選單與終端 Tab | 承載嵌入式終端，並載入終端 native dependencies。 |
| `FileExplorer` | File 選單與路由 Tab | 開啟檔案系統瀏覽表面。 |
| `FileExplorer.Features` | File menu sub-provider | 在 FileExplorer 契約之上新增 recent-path 類 File 選單能力。 |
| `Language` | Settings 選單與路由 Tab | 提供語言選擇與在地化資源管理。 |
| `Log` | File 選單與 logging provider | 註冊 log4net-backed logger provider，並暴露日誌檢視。 |
| `MCP.Server` | Tools menu provider | 暴露 MCP server 操作。 |
| `Mock` | Root mock menu 與 demo tabs | 面向對話框、通知、任務、CLI 與範例 View 的開發/測試模組。 |
| `ModuleManager` | Extensions 選單與路由 Tab | 管理本地模組與 NuGet-installed modules。 |
| `NuGet` | File 選單 | 提供 NuGet cache tooling。 |
| `Secrets` | Settings 選單與路由 Tab | 透過 `ISecrets` 管理 secret-like settings。 |
| `Settings` | Root Settings 選單與路由 Tab | 承載其他模組使用的 settings shell。 |
| `TaskManager` | Tools 選單、路由 Tab、狀態列 | 初始化 task management，並暴露任務狀態/操作。 |
| `TextEditor` | File/Open 選單與路由 Tab | 提供 text preview/edit 表面，包括 generic `file://` preview handling。 |
| `Translator` | Aspire command-line resource | 在 Aspire 可用時註冊 LibreTranslate sidecar。 |
| `Update` | About 選單與路由 Tab | 提供更新檢查；可在 tab/workspace restore 後執行啟動檢查。 |
| `WebBrowser` | Tools 選單與 WebView2 Tab | 在 Host 內開啟瀏覽器 Tab。 |

## Shell 與診斷模組

`Settings`、`Language`、`Secrets`、`Log`、`TaskManager`、`ModuleManager`、`Update`、`About`、`ApiReference` 主要是 Shell 或維運類模組。它們讓框架更容易檢查、設定與維護。

這些模組通常從 `LoadAsync` 註冊選單項目與路由 Tab。有些模組也會更早註冊服務：

- `Log` 在 `RegisterAsync` 中註冊 logging providers。
- `Language` 註冊 language-resource adapters，並載入 default language resources。
- `Secrets` 註冊從 config objects 到 `ISecrets` 的 adapter。
- `TaskManager` 在暴露 UI 前初始化 `ITaskManager`。
- `Update` 在所有模組載入後訂閱事件，並在啟動檢查前等待 `TabManagerRestoreCompleted`。

## 導覽與內容模組

`WebBrowser`、`FileExplorer`、`TextEditor`、`CLI`、`BlazorDemo` 暴露面向使用者的內容表面。它們都依賴 Tab routing，而不是由 Shell 直接建構 View。

如果這些模組開啟了錯誤 Tab 或 Not Found Tab，請檢查已註冊的 `ITabItemFactory`、route attributes、factory priority，以及傳給 `ITabManager.NavigateAsync(...)` 的 URI。

## Aspire 與 Sidecar 模組

`Aspire` 是執行 Aspire resources 的 Host 側模組。它註冊 Aspire dashboard tab、Tools menu entry 與 status bar item。如果 `AspireConfig.AutoStart` 為 true，它會在模組載入期間啟動 Aspire service。

`Translator` 是 sidecar 風格模組。它不暴露大型 UI，而是向 `ICommandlineResourcesProvider` 註冊 command-line resource，讓 Aspire 模組可以啟動 LibreTranslate。

## 開發與 Demo 模組

`Mock` 與 `BlazorDemo` 適合開發或驗證框架行為。`Mock` 註冊用於通知、對話框、task manager behavior 與 CLI integration 的 demo tabs 與 sample views。除非明確是診斷或範例用途，不要把生產功能放進 Mock。

## 不完整或非模組目錄

只有同時具備真實 module project 與 `Module.cs` 的目錄，才應該被視為 active built-in module。只有 `obj`、生成檔案或 `UI` 子目錄的資料夾，不足以作為 runtime discovery 的依據。

文件化或排查模組載入時，請從 compiled output 與 `Module.cs` 開始，不要只看資料夾名稱。
