<p align="center">
  <a href="./module-development.md">English</a> |
  <a href="./module-development.ja.md">日本語</a> |
  <a href="./module-development.zh-CN.md">简体中文</a> |
  <a href="./module-development.zh-TW.md">繁體中文</a> |
  <a href="./module-development.ko.md">한국어</a> |
</p>


# 模組開發

這篇文件說明如何撰寫符合 ZYC.Framework Host 執行階段模型的模組：以依賴注入為入口，使用 URI 驅動 Tab 導覽，按需接入主選單，並清楚分離公開契約與 WPF 實作。

## 什麼時候建立模組

當一個功能需要由 ZYC.Framework Host 發現並載入，而不是直接編譯進 Shell 時，就應該做成模組。模組可以提供 View、Tab factory、主選單項目、設定、狀態、背景服務與命令列選項。

如果功能契約需要被其他模組重用，請建立 `*.Abstractions` 專案。公開 DTO、常數、服務介面、選單 Provider 介面放在這裡。WPF View、TabItem 實作與執行階段註冊邏輯放在實作專案。

## 建議專案結構

| 區域 | 常見檔案 | 職責 |
| --- | --- | --- |
| Abstractions | `*ModuleConstants.cs`, `I*MainMenuItemsProvider.cs`, service interfaces | 其他模組可引用的公開契約，不依賴 WPF 實作細節。 |
| Implementation | `Module.cs` | 模組生命週期入口。註冊 factory、選單項目、Provider 與服務。 |
| Navigation | `*TabItemFactory.cs`, `*TabItem.cs` | 匹配 `zyc://` 或 app URI，並建立 Tab 實例。 |
| Menu | `*MainMenuItem.cs`, optional `*MainMenuItemsProvider.cs` | 加入使用者可見命令，用於導覽到模組 Tab 或執行模組動作。 |
| UI | `UI/*View.xaml`, `UI/*View.xaml.cs` | TabItem 使用的 WPF View。 |

Abstractions 專案不要暴露 WPF 控制項型別。如果需要命令契約，可以在 Abstractions 中使用 `System.Windows.Input.ICommand`。

## 生命週期

| 方法 | 時機 | 用途 |
| --- | --- | --- |
| `RegisterAsync(ContainerBuilder builder)` | Autofac 容器建構之前。 | 註冊必須在整個容器中可用的服務。 |
| `LoadAsync(ILifetimeScope lifetimeScope)` | 容器建構之後，且模組處於啟用狀態時。 | 註冊 Tab factory、主選單項目、狀態列項目與執行階段 Hook。 |
| `AfterLoadedAsync(ILifetimeScope lifetimeScope)` | 所有啟用模組載入完成之後。 | 需要依賴其他模組已完成註冊的跨模組初始化。 |

大多數 UI 模組只需要實作 `LoadAsync`。

## 最小 View 模組

對於單一 View 模組，可以在 `LoadAsync` 中註冊 simple tab factory：

```csharp
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using MyCompany.Tools.UI;

namespace MyCompany.Tools;

internal class Module : ModuleBase
{
    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterSimpleTabItemFactory(
            new SimpleTabItemFactoryInfo(typeof(ToolsView)));

        return Task.CompletedTask;
    }
}
```

這是 `minimal` 專案範本使用的模式。當模組只需要把一個 WPF `UserControl` 暴露為 Tab 時，這種方式就足夠。

## 帶路由的 Tab 模組

如果模組需要穩定 URI、路由匹配、參數、客製單例行為或多個 Tab，請使用 `TabItemFactoryBase`。

```csharp
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Reports.Abstractions;

namespace ZYC.Framework.Modules.Reports;

[RegisterSingleInstance]
[TabItemRoute(Host = ReportsModuleConstants.Host)]
internal class ReportsTabItemFactory : TabItemFactoryBase
{
    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(
        TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<ReportsTabItem>(
            new TypedParameter(
                typeof(TabReference),
                new TabReference(context.Uri)));
    }
}
```

`TabItemRouteAttribute` 可以依 `Scheme`、`Host`、`Path` 與 `PathMatch` 匹配 URI。如果多個 factory 匹配同一個 URI，`Priority` 較高的 factory 會優先。`TabItemFactoryBase` 預設 `IsSingle = true`；如果模組允許開啟多個實例，請覆寫它。

## TabItem 與 View

帶路由的 Tab 通常用 `TabItemInstanceBase<TView>` 包裝 WPF View：

```csharp
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.Reports.Abstractions;
using ZYC.Framework.Modules.Reports.UI;

namespace ZYC.Framework.Modules.Reports;

[Register]
[ConstantsSource(typeof(ReportsModuleConstants))]
internal class ReportsTabItem : TabItemInstanceBase<ReportsView>
{
    public ReportsTabItem(
        ILifetimeScope lifetimeScope,
        TabReference tabReference) : base(lifetimeScope, tabReference)
    {
    }
}
```

Tab 生命週期行為放在 TabItem 中，視覺行為放在 View 中。這樣可以把路由、Tab 識別與 UI 組合拆開。

## 主選單入口

選單項目通常導覽到模組 URI：

```csharp
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Reports.Abstractions;

namespace ZYC.Framework.Modules.Reports;

[RegisterSingleInstance]
internal class ReportsMainMenuItem : MainMenuItem
{
    public ReportsMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = ReportsModuleConstants.Title,
            Icon = ReportsModuleConstants.Icon
        };

        Command = lifetimeScope.CreateNavigateCommand(ReportsModuleConstants.Uri);
    }
}
```

然後從模組註冊：

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterTabItemFactory<ReportsTabItemFactory>();
    lifetimeScope.RegisterExtensionsMainMenuItem<ReportsMainMenuItem>();

    return Task.CompletedTask;
}
```

優先使用符合功能位置的既有選單 Provider，例如 File、View、Tools、Extensions 或 About。只有當功能擁有多個子命令時，才增加模組自己的 Provider。

## 設定與狀態

實作 `IConfig` 或 `IState` 的型別會在模組註冊期間從 settings 目錄讀取，並自動註冊到 Autofac。使用者可編輯設定使用 config；執行階段持久化內容，例如選中路徑、待處理操作、視窗狀態，使用 state。

不要把大型業務資料放進 config/state 類別。它們應保持小型、可序列化，並且能承受版本演進。

## 模組載入與依賴

Host 會發現命名類似 `ZYC.Framework.Modules*.dll` 的標準模組組件，然後追加 `ModuleConfig.AdditionalAssemblyNames` 中列出的組件。`ModuleConfig.DisabledAssemblyNames` 中的組件會被發現，但不會作為啟用模組載入。

依賴關係會透過對其他模組 `*.Abstractions.dll` 的引用推斷。如果模組 A 引用了 `ZYC.Framework.Modules.B.Abstractions.dll`，執行階段就可以報告 A 依賴 B，而不需要 A 直接引用 B 的 WPF 實作。

## 檢查清單

- 公開常數與契約放在 `*.Abstractions`。
- WPF View 與 TabItem 放在實作專案。
- 只有必須在容器建構前存在的服務才放進 `RegisterAsync`。
- Tab factory 與選單項目在 `LoadAsync` 中註冊。
- 穩定 URI 路由使用 `TabItemRouteAttribute`。
- 新增選單 Provider 前，優先使用既有主選單 Provider。
- config/state 類別保持小型且可序列化。
