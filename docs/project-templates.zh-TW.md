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
| `zyc new-module <ModuleName> --src-root <SourceRoot>` | 在既有原始碼樹中建立模組實作專案與對應的 `*.Abstractions` 專案。 | 擴展既有的 ZYC.Framework 風格儲存庫。 |

以 .NET tool 方式安裝或更新 CLI：

```bash
dotnet tool install -g ZYC.Framework.CLI --version 1.4.1
dotnet tool update -g ZYC.Framework.CLI --version 1.4.1
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
  MyCompany.Tools.csproj
  MyCompany.Tools.slnx
  Module.cs
  ModuleConfig.json
  UI/
    ToolsView.xaml
    ToolsView.xaml.cs
```

當你想用最短路徑得到可執行 Host，且功能表面只是一個簡單 WPF View 時，使用這個範本。

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
| `--template`, `-t` | 專案範本。支援 `minimal` 與 `modular`，預設 `minimal`。 |
| `--output`, `-o` | 輸出目錄。預設 `./<ProjectName>`。 |
| `--package-version` | `ZYC.Framework.Alpha` 套件版本。預設使用 CLI 產品版本。 |
| `--overwrite`, `-f` | 覆蓋既有檔案。不指定時，如果目標檔案已存在，生成會失敗。 |

包含常用選項的範例：

```bash
zyc new Acme.Tools --template modular --output ./Acme.Tools --package-version 1.4.1
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
| `--src-root`, `-s` | 必填原始碼根目錄，模組專案會建立在這裡。 |
| `--slnx` | 可選的 solution XML 檔案路徑。無需更新解決方案時省略。 |
| `--overwrite`, `-f` | 覆蓋既有檔案或模組目錄。不指定時，如果目標目錄已存在，生成會失敗。 |

## 範本 Token

專案範本會替換路徑與文字檔案中的這些 Token：

| Token | 值 |
| --- | --- |
| `__PROJECT_NAME__` | 完整專案名，例如 `MyCompany.Tools`。 |
| `__PROJECT_SHORT_NAME__` | 最後一個點分隔片段，例如 `Tools`。 |
| `__PROJECT_HOST__` | 用作 URI Host 的小寫短名稱，例如 `tools`。 |
| `__PACKAGE_VERSION__` | 由 `--package-version` 或 CLI 產品版本決定的套件版本。 |

文字範本檔案會以 UTF-8 with BOM 寫入，並統一為 CRLF 換行。

## 如何選擇範本

| 情況 | 建議命令 |
| --- | --- |
| 你想最快得到一個只有一個 View 的 Host。 | `zyc new MyCompany.Tools` |
| 你想為新應用建立模組化解決方案。 | `zyc new MyCompany.Tools --template modular` |
| 你正在向既有儲存庫新增模組。 | `zyc new-module Reports --src-root ./src` |
| 你需要把模組專案加入既有 `.slnx`。 | `zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx` |
