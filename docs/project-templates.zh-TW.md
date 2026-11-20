<p align="center">
  <a href="./project-templates.md">English</a> |
  <a href="./project-templates.ja.md">日本語</a> |
  <a href="./project-templates.zh-CN.md">简体中文</a> |
  <a href="./project-templates.zh-TW.md">繁體中文</a> |
  <a href="./project-templates.ko.md">한국어</a> |
</p>


# 專案範本

ZYC.Framework 透過 `dotnet tool` 命令支援兩類常見鷹架任務：建立新的 Host 專案，以及向既有原始碼樹新增新模組。本文說明範本類型、生成結構與命令選項。

## 命令入口

| 命令 | 目的 | 適用場景 |
| --- | --- | --- |
| `zyc new <ProjectName>` | 從專案範本建立一個外部 ZYC.Framework Host 專案。 | 在框架儲存庫之外啟動新應用或範例。 |
| `zyc new-module <ModuleName> [--src-root <SourceRoot>]` | 在既有原始碼樹中建立模組實作專案與對應的 `*.Abstractions` 專案。 | 擴展既有的 ZYC.Framework 風格儲存庫。 |

以 .NET tool 方式安裝或更新 CLI：

```bash
dotnet tool install -g ZYC.Framework.CLI --version 1.6.3
dotnet tool update -g ZYC.Framework.CLI --version 1.6.3
```

然後確認命令可用：

```bash
zyc --help
zyc new --help
zyc new-module --help
```

## `minimal` 範本

`minimal` 是 `zyc new` 的預設範本。它會建立一個小型 WPF Host 專案，引用 `ZYC.Framework.Alpha`，並把一個 WPF View 註冊為 simple tab。

```bash
zyc new MyCompany.Tools
```

等價的明確命令：

```bash
zyc new MyCompany.Tools --template minimal
```

生成結構：

```text
MyCompany.Tools/
  Directory.Build.props
  Directory.Build.targets
  MyCompany.Tools.csproj
  MyCompany.Tools.slnx
  Module.cs
  ModuleConfig.json
  UI/
    ToolsView.xaml
    ToolsView.xaml.cs
```

`Directory.Build.props` 存放共用建置屬性，並把建置輸出放到 `_bin/app/`，因此 Host 的 `settings/` 與 `logs/` 目錄會產生在 `_bin/` 下。`Directory.Build.targets` 設定 `ZYC.Framework.Alpha` 的版本，並在套件還原後匯入它的相依版本，之後新增的 `ZYC.CoreToolkit.*` 參考可以省略 `Version`。

當你想用最短路徑得到可執行 Host，且功能表面只是一個簡單 WPF View 時，使用這個範本。

## `react` 範本

`react` 會建立與 `minimal` 相同的單一專案 Host，但它的 Tab 是一個 WebView2 View，顯示以 Vite 與 TypeScript 建置的 React 前端。建置此專案需要 Node.js 20.19+ 或 22.12+。

```bash
zyc new MyCompany.Tools --template react
```

生成結構：

```text
MyCompany.Tools/
  Directory.Build.props
  Directory.Build.targets
  MyCompany.Tools.csproj
  MyCompany.Tools.slnx
  Module.cs
  ModuleConfig.json
  README.md
  UI/
    ToolsView.cs
  ClientApp/
    index.html
    package.json
    tsconfig.json
    vite.config.ts
    src/
      App.tsx
      index.css
      main.tsx
      webview.ts
```

- `ToolsView` 繼承 `WebViewHostBase`，透過虛擬主機 `https://<web-name>.example/` 提供建置好的前端，其中 `<web-name>` 是 `__PROJECT_WEB_NAME__` 的值，例如 `mycompany-tools`。
- 建置專案時，`ClientApp/package.json` 變更後會執行 `npm install`，前端原始碼變更後會執行 `npm run build`，然後把 `ClientApp/dist` 複製到輸出目錄下的 `ClientApp/`。傳入 `-p:SkipClientAppBuild=true` 可略過這一步。
- 在 Debug 建置中，只要 `npm run dev` 正在執行，View 就會載入 Vite 開發伺服器 `http://localhost:5173`，前端變更不需重新建置即可生效。
- `ClientApp/src/webview.ts` 封裝了 `window.chrome.webview` 橋接。產生的頁面會傳送 `ping` 訊息，View 中的 `OnWebMessageReceived` 會回覆 `pong`。

當你想用 React 而不是 XAML 撰寫 Tab 介面時，使用這個範本。

## `console` 範本

`console` 會建立一個 `net10.0` 主控台解決方案，其中包含共用建置屬性，以及一個引用 `ZYC.CoreToolkit` 4.0.0 的主控台專案。

```bash
zyc new MyCompany.Tools --template console
```

生成結構：

```text
MyCompany.Tools/
  .editorconfig
  .gitignore
  Directory.Build.props
  Directory.Build.targets
  MyCompany.Tools.slnx
  MyCompany.Tools/
    MyCompany.Tools.csproj
    Program.cs
```

當你需要一個保留來源範本根層級共用建置設定的小型主控台應用時，使用這個範本。

## `build` 範本

`build` 會建立獨立的 `<ProjectName>.Build` 專案，負責版本產生、編譯和 NuGet 封裝，並附帶一個簡單的主專案。

在 Windows 上使用 .NET 10 SDK 執行：

```powershell
zyc new MyCompany.Tools --template build
cd MyCompany.Tools
dotnet run --project MyCompany.Tools.Build/MyCompany.Tools.Build.csproj -c Release
```

在 `MyCompany.Tools.Build/BuildEnvironment.cs` 中設定套件版本。Build 專案會產生 `version.props`，編譯參與發佈的專案並將 NuGet 套件輸出至 `_bin/`，自身輸出放在 `_bin_build/`。產生的 README 說明了手動發佈工作流程和儲存庫變數 `NUGET_USER` 的設定方式。

## `wpf` 範本

`wpf` 會建立一個 `net10.0-windows` WPF 解決方案，其中包含以 Autofac 為基礎的應用程式啟動、設定註冊、Fody 屬性織入，以及對 `ZYC.CoreToolkit` 4.0.0 的參考。

```bash
zyc new MyCompany.Desktop --template wpf
```

生成結構：

```text
MyCompany.Desktop/
  .editorconfig
  .gitignore
  Directory.Build.props
  Directory.Build.targets
  FodyWeavers.xml
  MyCompany.Desktop.slnx
  MyCompany.Desktop/
    App.xaml
    App.xaml.cs
    MainWindow.xaml
    MainWindow.xaml.cs
    Program.cs
    MyCompany.Desktop.csproj
```

當你需要一個保留來源範本既有 Autofac 與設定啟動邏輯的小型 WPF 應用時，使用這個範本。

## `modular` 範本

`modular` 會建立一個小型解決方案，包含 Entry 專案、模組實作專案與模組 Abstractions 專案。

```bash
zyc new MyCompany.Tools --template modular
```

生成結構：

```text
MyCompany.Tools/
  Directory.Build.props
  Directory.Build.targets
  version.props
  MyCompany.Tools.slnx
  Entry/
    Entry.csproj
  ZYC.Framework.Modules.MyCompany.Tools/
    ZYC.Framework.Modules.MyCompany.Tools.csproj
    Module.cs
    ToolsMainMenuItem.cs
    ToolsTabItem.cs
    ToolsTabItemFactory.cs
    UI/
      ToolsView.xaml
      ToolsView.xaml.cs
  ZYC.Framework.Modules.MyCompany.Tools.Abstractions/
    ToolsModuleConstants.cs
    ZYC.Framework.Modules.MyCompany.Tools.Abstractions.csproj
```

當功能應該像真實框架模組一樣組織時使用它：公開常數在 Abstractions，模組提供帶路由的 Tab factory、主選單項目與模組入口。

## `zyc new` 選項

| 選項 | 說明 |
| --- | --- |
| `<ProjectName>` | 必填專案名。必須是有效的點分隔 C# 識別碼，例如 `Acme.Tools`。 |
| `--template`, `-t` | 專案範本。支援 `minimal`、`modular`、`console`、`build`、`wpf` 與 `react`，預設 `minimal`。 |
| `--output`, `-o` | 輸出目錄。預設 `./<ProjectName>`。 |
| `--package-version` | `ZYC.Framework.Alpha` 套件版本。預設使用 CLI 產品版本。 |
| `--overwrite`, `-f` | 覆蓋既有檔案。不指定時，如果目標檔案已存在，生成會失敗。 |

包含常用選項的範例：

```bash
zyc new Acme.Tools --template modular --output ./Acme.Tools --package-version 1.6.3
```

## 面向既有原始碼樹的 `new-module`

當儲存庫已經有 `src` 樹，且你想新增一組模組專案時，使用 `new-module`。

```bash
zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx
```

該命令會建立：

```text
src/
  ZYC.Framework.Modules.Reports/
    ZYC.Framework.Modules.Reports.csproj
    Module.cs
    ReportsMainMenuItem.cs
    ReportsTabItem.cs
    ReportsTabItemFactory.cs
    UI/
      ReportsView.xaml
      ReportsView.xaml.cs
  ZYC.Framework.Modules.Reports.Abstractions/
    ReportsModuleConstants.cs
    ZYC.Framework.Modules.Reports.Abstractions.csproj
```

`--slnx` 是可選項。提供它時，生成的專案會被加入解決方案檔案的 `/Modules/` 資料夾。相對 `--slnx` 路徑會從 `--src-root` 解析。

`new-module` 會正規化目標名。下面這些輸入都會生成同一個模組目標：

```bash
zyc new-module Reports --src-root ./src
zyc new-module ZYC.Framework.Modules.Reports --src-root ./src
zyc new-module ZYC.Framework.Modules.Reports.Abstractions --src-root ./src
```

## `new-module` 選項

| 選項 | 說明 |
| --- | --- |
| `<ModuleName>` | 位置參數形式的目標模組名。 |
| `--target`, `-t` | 目標模組名。請使用位置參數或此選項之一，不要提供衝突值。 |
| `--src-root`, `-s` | 可選原始碼根目錄，模組專案會建立在這裡；省略時預設為目前目錄。 |
| `--slnx` | 可選的 solution XML 檔案路徑。無需更新解決方案時省略。 |
| `--overwrite`, `-f` | 覆蓋既有檔案或模組目錄。不指定時，如果目標目錄已存在，生成會失敗。 |

## 範本 Token

專案範本會替換路徑與文字檔案中的這些 Token：

| Token | 值 |
| --- | --- |
| `__PROJECT_NAME__` | 完整專案名，例如 `MyCompany.Tools`。 |
| `__PROJECT_SHORT_NAME__` | 最後一個點分隔片段，例如 `Tools`。 |
| `__PROJECT_HOST__` | 用作 URI Host 的小寫短名稱，例如 `tools`。 |
| `__PROJECT_WEB_NAME__` | 小寫、以連字號分隔的完整名稱，用作 npm 套件名稱與 WebView2 虛擬主機，例如 `mycompany-tools`。 |
| `__V_ZYC_FRAMEWORK_ALPHA__` | 由 `--package-version` 或 CLI 產品版本決定的套件版本。 |

文字範本檔案會統一為 CRLF 換行，並以 UTF-8 with BOM 寫入。前端檔案（`package.json`、`tsconfig.json`、`.ts`、`.tsx`、`.html`、`.css`）由 Node.js 工具讀取，因此寫入時不帶 BOM。

## 如何選擇範本

| 情況 | 建議命令 |
| --- | --- |
| 你想最快得到一個只有一個 View 的 Host。 | `zyc new MyCompany.Tools` |
| 你想用 React 撰寫 Tab 介面。 | `zyc new MyCompany.Tools --template react` |
| 你想建立一個引用 `ZYC.CoreToolkit` 的主控台應用。 | `zyc new MyCompany.Tools --template console` |
| 你需要獨立的編譯專案來管理版本、編譯和封裝。 | `zyc new MyCompany.Tools --template build` |
| 你想建立一個帶有 Autofac 與設定啟動邏輯的 WPF 應用程式。 | `zyc new MyCompany.Desktop --template wpf` |
| 你想為新應用建立模組化解決方案。 | `zyc new MyCompany.Tools --template modular` |
| 你正在向既有儲存庫新增模組。 | `zyc new-module Reports --src-root ./src` |
| 你需要把模組專案加入既有 `.slnx`。 | `zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx` |
