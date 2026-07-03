<p align="center">
  <a href="./troubleshooting.md">English</a> |
  <a href="./troubleshooting.ja.md">日本語</a> |
  <a href="./troubleshooting.zh-CN.md">简体中文</a> |
  <a href="./troubleshooting.zh-TW.md">繁體中文</a> |
  <a href="./troubleshooting.ko.md">한국어</a> |
</p>


# 故障排查

本頁彙整 ZYC.Framework Host 或模組專案裡最常見的失敗點。先從可見症狀入手，再檢查負責該行為的層。

## 快速定位

| 症狀 | 優先檢查 |
| --- | --- |
| 找不到 `zyc` | 安裝或更新全域 `ZYC.Framework.CLI` 工具，然後開啟新的 shell 執行 `zyc new --help`。 |
| `zyc new` 失敗 | 檢查專案名稱、`--template`、`--output`、`--package-version`，以及目標目錄是否已有檔案。只有明確要替換產生檔案時才使用 `--overwrite`。 |
| 模組缺失 | 檢查執行時 DLL 是否在 app 目錄中，是否符合 `ZYC.Framework.Modules*.dll` 或列在 `ModuleConfig.AdditionalAssemblyNames` 中，並且包含 `ModuleBase` 入口。 |
| 模組被發現但未載入 | 檢查 `ModuleConfig.DisabledAssemblyNames`。停用模組會註冊為模組資訊，但會略過 `LoadAsync`。 |
| 開啟模組載入錯誤頁 | 查看模組名稱、例外和函式名稱。失敗發生在 `LoadAsync` 或 `AfterLoadedAsync`。 |
| 選單項缺失 | 檢查所屬模組是否已載入，是否註冊到正確選單 provider，以及選單項是否被隱藏。 |
| 導航開啟 Not Found | 沒有 `ITabItemFactory` 符合該 URI。檢查路由特性、factory 註冊，以及傳給 `ITabManager.NavigateAsync(...)` 的 URI。 |
| 導航開啟錯誤頁 | factory、tab item、view 或 tab 的 `LoadAsync` 失敗。查看錯誤頁和記錄中的例外。 |
| NuGet 安裝的模組未生效 | 在 ModuleManager 中重新安裝或更新，確認寫入 `settings/nuget.module.assets.json`，然後重新啟動 Host。 |
| Aspire 資源不顯示 | 檢查模組是否註冊 `IExtensionResourcesProvider` 或 `ICommandlineResourcesProvider`，以及 Aspire 是否已啟用或啟動。 |
| 內嵌終端失敗 | 確認終端 native DLL 已複製到輸出目錄中預期的 `runtimes` 資料夾。 |
| 文件修改不顯示 | 修改 `src/ZYC.Framework.Build.Doc/Templates` 下的檔案，必要時再重新產生發佈文件。 |

## CLI 與專案建立

推薦的建立流程使用全域 dotnet tool：

```bash
dotnet tool install --global ZYC.Framework.CLI --version 1.3.8
dotnet tool update --global ZYC.Framework.CLI --version 1.3.8
zyc new MyCompany.Tools --template minimal
```

安裝後仍然無法使用 `zyc` 時：

- 開啟新的終端，讓更新後的 tool path 生效；
- 執行 `dotnet tool list --global`，確認已安裝 `ZYC.Framework.CLI`；
- 執行 `zyc new --help`，確認 CLI 命令可以解析。

專案建立失敗時：

- 使用合法的點分 C# 專案名稱，例如 `MyCompany.Tools`；
- 只需要 Host 時使用 `--template minimal`，需要 Host + Module 拆分時使用 `--template modular`；
- 不希望目標目錄從專案名稱推導時使用 `--output`；
- 產生專案需要參考指定套件版本時使用 `--package-version`；
- 只有明確要替換已有產生檔案時才使用 `--overwrite`。

## 模組發現

啟動時，Host 會從應用目錄發現模組組件。標準內建模組 DLL 按檔名比對，額外模組可以列在 `ModuleConfig.AdditionalAssemblyNames` 中：

```json
{
  "AdditionalAssemblyNames": [
    "MyCompany.Tools.dll"
  ],
  "DisabledAssemblyNames": []
}
```

如果模組沒有被發現：

- 確認 DLL 位於應用目錄；
- 確認 DLL 名稱符合標準 `ZYC.Framework.Modules*.dll` 模式，或已列在 `AdditionalAssemblyNames` 中；
- 確認組件包含繼承自 `ModuleBase` 的具體型別；
- 不要只列出 `*.Abstractions` 組件，因為 Abstractions 專案只定義契約，不是執行時模組入口。

如果模組出現在模組資訊中但一直不載入：

- 檢查 DLL 檔名是否存在於 `DisabledAssemblyNames`；
- 記住停用模組仍會被發現，但不會呼叫 `LoadAsync`；
- 從 `DisabledAssemblyNames` 中移除檔名，或透過 ModuleManager 重新啟用；如果執行中的 Host 不會動態重載該模組，則需要重新啟動。

## 模組載入錯誤

Host 會記錄兩個階段的載入失敗：

- `LoadAsync`：模組通常在這裡註冊選單、Tab、狀態項和執行時服務；
- `AfterLoadedAsync`：模組可以在這裡執行依賴其他已載入模組的工作。

模組載入錯誤頁開啟時，從顯示的模組名稱、函式名稱和例外開始排查。`AppConfig.SuppressModuleLoadError` 可以隱藏該頁面，但不會修復底層失敗。

常見原因：

- 模組解析所需服務之前，該服務還沒有註冊；
- view 或 tab item 建構函式在註冊或啟動導航期間拋出例外；
- 模組假定另一個模組已啟用，但依賴模組被停用或缺失；
- 模組所需的本機檔案、native DLL 或外部程序不存在。

## 選單、Tab 與路由

選單項通常由模組的 `LoadAsync` 註冊。選單項缺失時：

- 確認模組本身已無錯誤載入；
- 將選單項註冊到正確 provider，例如 File、Tools、Extensions、About 或 Settings；
- 檢查選單項是否因狀態或設定被隱藏；
- priority 和 anchor 不會建立選單項，因此先確認選單項可見，再排查排序。

Tab 導航依賴 `ITabItemFactory`。如果開啟 Not Found：

- 檢查 factory 是否已在載入的組件中註冊；
- 檢查 `TabItemRoute` 的 scheme、host、path 是否與導航 URI 一致；
- 當檔案預覽這類通用路由可能先於更具體路由符合時，檢查 factory priority；
- 檢查 single-instance tab 是否複用了既有 tab，而不是開啟新 tab。

如果開啟錯誤頁，表示路由已經符合，但建立或載入失敗。先查看錯誤頁中的例外，再檢查 factory、tab item 建構函式、view 建構函式和 tab `LoadAsync`。

## Workspace 與還原時機

啟動導航、協定轉發導航、還原期模組動作都應該等 workspace 與 tab 還原管線準備完成。如果 tab 開啟到錯誤 workspace，或還原後消失：

- 在 `TabManagerRestoreCompleted` 之後執行啟動導航；
- 使用者觸發的動作使用目前聚焦 workspace；
- 還原或轉發到已知目標時使用明確 workspace id；
- 透過 `ITabManager` 移動、建立和關閉 tab，不要直接修改 UI 集合。

## Config 與 State

具體的 `IConfig` 和 `IState` 型別會在模組組件註冊期間從 settings 目錄載入。設定無法讀寫時：

- 確認 config 或 state 型別是具體型別，並位於已載入的執行時組件中；
- 在期望 config 或 state 型別存在之前，確認模組組件已被發現；
- 確認 settings 檔案位於 Host 的 settings 目錄，而不是原始碼樹；
- 不要只把契約型別放在 abstractions 組件中，然後期待它產生執行時 state。

## 單一實例與 Mutex Override

如果 `settings/mutex-id.override` 不存在，Host 會根據產品資訊派生 single-instance mutex id。可以透過 Tools > Override Mutex Id 建立、更新或刪除這個檔案。

修改 override 後需要重新啟動 Host。Mutex 和 startup URI pipe name 都在啟動時建立，執行中的程序不會立即切換 identity。如果 side-by-side instances、startup URI forwarding 或 foreground-window activation 行為異常，先檢查目前的 `mutex-id.override` 檔案。

## NuGet 模組

ModuleManager 透過 restore 暫時專案來安裝 NuGet 模組，並把解析後的 runtime asset graph 寫入 `settings/nuget.module.assets.json`。Host 會在下一次啟動時讀取該檔案。

如果 NuGet 模組已安裝但未生效：

- 檢查 restore 是否成功，assets 檔案是否存在於 `settings` 下；
- 重新啟動 Host，讓啟動發現流程載入 runtime assemblies；
- 確認套件包含與目前 Host 目標 `net10.0-windows` 相容的 runtime assembly；
- 檢查已安裝模組組件是否被 `ModuleConfig.DisabledAssemblyNames` 停用；
- 如果 assets 檔案指向過期套件內容，重新安裝，或刪除後再安裝。

如果已知套件沒有出現在搜尋結果裡，注意 NuGet search 會先於 `IncludeRegex` 執行。沒有進入返回頁的套件不會到達 regex filter。檢查 `NuGetModuleConfig.SearchTerm`、`SearchSkip` 與 `SearchTake`；`SearchTake` 會被 clamp 到 NuGet.org 單次請求上限 1000，後續頁請使用 `SearchSkip`。

Install、uninstall 與 refresh 共用同一條 module-assets pipeline，並由 ModuleManager operation coordinator 串行化。如果這些 command 看起來不可用，先等待目前 restore/search operation 結束，再開始下一次操作。

## Aspire 與 Sidecar 資源

Aspire 資源由模組透過 extension provider 貢獻。模組可以註冊 `IExtensionResourcesProvider` 來直接自訂 Aspire builder，也可以註冊 `ICommandlineResourcesProvider` 來提供命令列 sidecar 資源。

資源不顯示時：

- 確認提供資源的模組在 Aspire 建構 resource graph 前已載入；
- 確認 provider 型別已在模組組件中註冊；
- 對命令列資源，確認資源名稱、工作目錄和命令有效；
- 如果 `AspireConfig.AutoStart` 被停用，依需要手動啟動 Aspire。

如果 Aspire dashboard 無法開啟，檢查 Aspire 程序是否產生 `ASPNETCORE_URLS` 和 `AppHost:BrowserToken`。dashboard URI 會從這些值建構。

## CLI 終端 native 依賴

CLI 模組會從應用輸出目錄載入終端 native 依賴。內嵌終端早期失敗時，確認這些檔案存在：

```text
runtimes\win10-x64\native\conpty.dll
runtimes\win-x64\native\Microsoft.Terminal.Control.dll
```

如果檔案缺失，檢查 CLI 模組和終端依賴的 package output 與 copy-local 行為。

## 文件模板

`ZYC.Framework.Build.Doc` 使用的文件來源在：

```text
src\ZYC.Framework.Build.Doc\Templates
```

如果直接修改產生後的根目錄 `docs` 檔案，看起來本地有效但後來消失，請把變更移到對應模板檔案中，再重新產生文件。
