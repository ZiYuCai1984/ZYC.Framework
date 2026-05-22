<p align="center">
  <a href="./quick-start.md">English</a> |
  <a href="./quick-start.ja.md">日本語</a> |
  <a href="./quick-start.zh-CN.md">简体中文</a> |
  <a href="./quick-start.zh-TW.md">繁體中文</a> |
  <a href="./quick-start.ko.md">한국어</a> |
</p>


# 🚀 快速開始：建立你的第一個 ZYC.Framework Host

本指南將帶你從零開始建立一個 **ZYC.Framework Host** 專案。你將學會如何透過 NuGet 整合框架，並使用模組化機制（Module + UserControl）將自訂 UI 掛載到宿主環境中。🛠️

---

## 建議方式：透過 dotnet tool 建立

安裝或更新 ZYC.Framework CLI 工具：

```bash
dotnet tool install --global ZYC.Framework.CLI --version 1.3.2
# 如果已經安裝過：
dotnet tool update --global ZYC.Framework.CLI --version 1.3.2
```

建立一個最小 Host 專案：

```bash
zyc new WpfApp1
```

預設專案範本是 `minimal`，會產生與下方「手動建立方法」等價的 Host 結構。你也可以明確指定範本：

```bash
zyc new WpfApp1 --template minimal
```

如果你需要一個包含 `Entry` 專案、模組專案與 `Abstractions` 專案的解決方案，可以使用 `modular`：

```bash
zyc new MyCompany.Tools --template modular
```

常用選項：

```bash
zyc new MyCompany.Tools --output ./MyCompany.Tools --package-version 1.3.2
```

開啟產生的解決方案或專案，將其設為啟動專案，然後開始偵錯。產生結果已包含套件參考、`Module.cs`、`ModuleConfig.json` 與初始 View。

---

## 手動建立方法

如果你希望手動建立 Host 專案，可以依照下方的等價步驟操作。

### 1. 🧱 專案準備與前置條件

1. **建立專案**：建立一個以 **.NET 10** 為目標框架的 **WPF Application**（例如命名為 `WpfApp1`）。✨
2. **加入 NuGet 套件**：透過 NuGet 套件管理器安裝核心套件 `ZYC.Framework.Alpha`。📦

```xml
<ItemGroup>
  <PackageReference Include="ZYC.Framework.Alpha" Version="1.3.2" />
</ItemGroup>
```

3. **清理預設進入點**：🧹
框架提供了自己的統一進入點（`Entry.cs`）。你 **必須刪除** 範本預設產生的下列檔案：
* `App.xaml`
* `App.xaml.cs`

> [!IMPORTANT]
> ⚠️ **關鍵步驟**：你必須刪除 `App.xaml`，否則會發生全域進入點衝突。應用程式的啟動流程將完全由框架接管。

---

### 2. ⚙️ 設定組件參考

為了讓宿主正確識別並載入抽象介面，請在 `.csproj` 檔案中手動加入對 `Abstractions` 組件的參考。🔗

```xml
<ItemGroup>
  <Reference Include="ZYC.Framework.Abstractions">
    <HintPath>$(OutputPath)ZYC.Framework.Abstractions.dll</HintPath>
  </Reference>
</ItemGroup>
```

---

### 3. 🛠️ 實作業務模組 (`Module.cs`)

在專案根目錄建立一個 `Module.cs` 檔案。這個類別相當於模組的「大腦」，負責定義載入邏輯，並向宿主註冊 UI 頁面。🧠

```csharp
using Autofac;
using ZYC.Framework.Abstractions.Tab;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac;

namespace WpfApp1;

internal class Module : ModuleBase
{
    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        // 選用：掛載內建偵錯工具
        DebuggerTools.Attach();

        // 解析 Tab Manager 並註冊你的 UI 元件
        var simpleTabItemFactoryManager = lifetimeScope.Resolve<ISimpleTabItemFactoryManager>();
        simpleTabItemFactoryManager.Register(new SimpleTabItemFactoryInfo(typeof(UserControl1)));

        return base.LoadAsync(lifetimeScope);
    }
}
```

---

### 4. 🎨 建立 UI 元件

建立一個新的 `UserControl1`（WPF User Control），並加入 `[Register]` 屬性。這樣框架的相依性注入（DI）容器就能自動識別並管理它。🖥️

```csharp
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace WpfApp1;

[Register] // 自動註冊到 DI 容器
public partial class UserControl1
{
    public UserControl1()
    {
        InitializeComponent();
    }
}
```

---

### 5. 📄 新增模組設定檔

在專案根目錄建立一個 `ModuleConfig.json` 檔案。這個檔案相當於宿主的「地圖」，用來告訴宿主要動態載入哪些組件。同時需要將它設定為相對於主程式輸出到 `../settings/ModuleConfig.json`。⚙️

1. **檔案內容**：
```json
{
  "AdditionalAssemblyNames": [
    "WpfApp1.dll"
  ],
  "DisabledAssemblyNames": []
}
```

2. **專案項目設定**：📌
在 `.csproj` 檔案中加入以下設定，讓 `ModuleConfig.json` 在建置時輸出到 `../settings/ModuleConfig.json`：

```xml
<ItemGroup>
  <None Update="ModuleConfig.json">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    <Link>../settings/ModuleConfig.json</Link>
  </None>
</ItemGroup>
```

> 💡 **提示**：`AdditionalAssemblyNames` 必須包含 `Module.cs` 所在組件的名稱。

---

### 6. ▶️ 執行與偵錯

1. **設定啟動專案**：將這個 WPF 專案設為方案的 **Startup Project**。
2. **開始偵錯**：按下 `F5`。

🎉 **預期結果**：
宿主將啟動、掃描 `ModuleConfig.json`，並載入 `WpfApp1` 模組。你註冊的 `UserControl1` 頁面會自動以新分頁的形式出現在主介面中。

---

![quick-start-ui.png](./images/quick-start-ui.png)
