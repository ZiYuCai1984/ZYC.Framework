<p align="center">
  <a href="./extension-points.md">English</a> |
  <a href="./extension-points.ja.md">日本語</a> |
  <a href="./extension-points.zh-CN.md">简体中文</a> |
  <a href="./extension-points.zh-TW.md">繁體中文</a> |
  <a href="./extension-points.ko.md">한국어</a> |
</p>


# 擴展點

ZYC.Framework 的擴展大多透過模組與 Autofac 註冊。模組隨 Host 載入，註冊服務或 UI 貢獻；隨後 Shell 從這些註冊中組合選單、Tab、工作區操作、狀態列項目、工作列選單項目、拖放動作與 Aspire 資源。

## 擴展點地圖

| 擴展點 | 註冊位置 | 執行階段消費方 |
| --- | --- | --- |
| 模組生命週期 | `ModuleBase.RegisterAsync`, `LoadAsync`, `AfterLoadedAsync` | Host 啟動與模組載入器。 |
| URI Tab | `ITabItemFactoryManager.RegisterFactory<T>()` | `TabManager.InternalNavigateAsync(...)`。 |
| 簡單 View Tab | `ISimpleTabItemFactoryManager.Register(...)` | 內建 `SimpleTabItemFactory`。 |
| 主選單 | `IMainMenuManager`, `IMainMenuItemsProvider` | `MainMenuManager` 與主選單 View。 |
| 視窗標題列 | `IWindowTitleManager`, `IWindowTitleExtendManager` | `WindowTitleView`。 |
| 工作區選單 | `IWorkspaceMenuManager` | `WorkspaceMenuView`。 |
| 工作區內容選單 Manager | `IWorkspaceContextMenuManager` | Manager 存在並提供排序；在接線內容選單表面時使用。 |
| Tab 標頭右鍵選單 | `ITabItemHeaderContextMenuItemView` | `TabItemHeaderContextMenuItemsResolver`。 |
| 狀態列 | `IStatusBarManager`, `IStatusBarItemsProvider` | `StatusBarManager`。 |
| 工作列選單 | `ITaskbarMenuManager` | `TaskbarContextMenu`。 |
| 設定/狀態 | `IConfig`, `IState` | `ModuleTools.RegisterAllFromAssembly(...)`。 |
| 事件 | `IEventAggregator` | 執行階段發布/訂閱匯流排。 |
| Toast | `IToastManager`, `IToast` | Toast 彈出宿主。 |
| 拖放 | `IDropActionProvider` | `DropOrchestrator`。 |
| Aspire 資源 | `IExtensionResourcesProvider` | `AspireService.Build(...)`。 |
| CLI 選項 | `ModuleBase.RegisterCommandLineOption(...)` | `ZYC.Framework.CLI` 根命令。 |

## 模組生命週期

用模組生命週期決定每類註冊放在哪裡：

| 方法 | 用途 |
| --- | --- |
| `RegisterAsync(ContainerBuilder builder)` | 必須在容器建構前完成的 Autofac 註冊。 |
| `LoadAsync(ILifetimeScope lifetimeScope)` | 執行階段貢獻，例如 Tab factory、選單項目、狀態列 Provider、Aspire 資源註冊。 |
| `AfterLoadedAsync(ILifetimeScope lifetimeScope)` | 需要所有模組都載入完之後再執行的跨模組工作。 |
| `RegisterCommandLineOption(...)` | 模組擁有的 CLI 參數。 |

大多數 UI 模組只需要 `LoadAsync`。

## URI Tab

URI Tab 是暴露功能表面的主要方式。註冊一個 `ITabItemFactory`，通常使用 `ILifetimeScope` 上的輔助方法：

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterTabItemFactory<ReportsTabItemFactory>();
    return Task.CompletedTask;
}
```

當路由可以用 URI 部件表達時，使用 `TabItemRouteAttribute`。當判斷需要服務、檔案類型檢查或更複雜策略時，覆寫 `CheckUriMatchedAsync`。

只有在很小的單 View 場景中，才使用 `RegisterSimpleTabItemFactory(...)`，也就是一個 `UserControl` 可以直接開啟，不需要專門的路由模型。

## 主選單

根選單內建 File、View、Tools、Extensions 與 About Provider。模組選單項目通常註冊到其中一個 Provider 下：

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterExtensionsMainMenuItem<ReportsMainMenuItem>();
    return Task.CompletedTask;
}
```

選單排序先依 `Anchor`，再依同一組內的 `Priority`。`MainMenuManager` 會在 Anchor 組之間插入分隔符，並遞迴排序子項。

只有當模組需要一個包含多個子命令的父選單時，才建立模組自己的 `IMainMenuItemsProvider`。如果只有一個命令，把 `IMainMenuItem` 註冊到既有 Provider 即可。

## 視窗標題列

緊湊的 command-style title-bar buttons 使用 `IWindowTitleManager`；較豐富的標題列內容使用 `IWindowTitleExtendManager`。擴充項實作 `IWindowTitleExtendItem`，並回傳要顯示在標題列中的 view object。

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.Resolve<IWindowTitleExtendManager>()
        .RegisterItem<ReportsWindowTitleItem>();

    return Task.CompletedTask;
}
```

這個 surface 適合需要靠近 window chrome 常駐顯示的 module-owned status 或 account controls。使用者收藏類快捷入口應使用 quick bar items，而不是模組私有標題列 UI。

## 工作區選單

如果命令需要出現在工作區編號旁邊的可見下拉選單中，使用 `IWorkspaceMenuManager`：

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.Resolve<IWorkspaceMenuManager>()
        .RegisterItem<ReportsWorkspaceMenuItem>();

    return Task.CompletedTask;
}
```

`IWorkspaceMenuItem` 支援 `Title`、`Command`、`SubItems`、`Icon`、`Anchor`、`Priority` 與 `Localization`。目前可見的 `WorkspaceMenuView` 讀取 `IWorkspaceMenuManager.GetItems()`。

`IWorkspaceContextMenuManager` 是另一個 Manager，它依 `Anchor` 和 `Priority` 遞迴排序。除非已經有內容選單 View 接入它，否則不要假設它的項目會顯示出來。

## Tab 標頭右鍵選單

Tab 標頭選單項目是註冊為 `ITabItemHeaderContextMenuItemView` 的 WPF 選單項目 View：

```csharp
[RegisterAs(typeof(ITabItemHeaderContextMenuItemView))]
internal partial class ReportsTabHeaderMenuItem :
    ITabItemHeaderContextMenuItemView
{
    public int Order => 20;
}
```

`TabItemHeaderContextMenuItemsResolver` 會解析所有已註冊 View，並依 `Order` 排序。由於 WPF ContextMenu 是 late-bound，如果選單項目需要目前 Tab 實例，優先使用 command parameter 和既有的 `ContextMenuItemBase` 模式。

## 狀態列

狀態列擴展貢獻一個 `IStatusBarItemsProvider`；Provider 回傳一個或多個 `IStatusBarItem`。`StatusBarManager` 彙總所有已註冊 Provider，並依 `Order` 排序。

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.Resolve<IStatusBarManager>()
        .RegisterStatusBarItemsProvider<ReportsStatusBarItemsProvider>();

    return Task.CompletedTask;
}
```

每個 Item 透過 `StatusBarSection.Left` 或 `StatusBarSection.Right` 選擇顯示側。

## 工作列選單

工作列選單項目實作 `ITaskbarMenuItem`，並註冊到 `ITaskbarMenuManager`：

```csharp
lifetimeScope.Resolve<ITaskbarMenuManager>()
    .RegisterMenuItem(lifetimeScope.Resolve<ReportsTaskbarMenuItem>());
```

工作列選單依 `Info.Anchor` 分組，依 `Info.Priority` 排序，並遞迴排序子項。這個表面適合托盤/視窗級命令，不適合本應放進主選單的功能導覽。

## 設定與狀態

任何實作 `IConfig` 或 `IState` 的具體型別，都會在模組組件註冊期間從 settings 目錄載入，並註冊到 Autofac。它適合小型、可 JSON 序列化的設定與狀態。

準則：

- 使用者可編輯選項放在 `IConfig`。
- 執行階段持久化放在 `IState`。
- 型別保持小型，並能承受版本演進。
- 不要把 config/state 當成大型業務資料儲存。

## 事件與 Toast

解耦的執行階段通知使用 `IEventAggregator`：

```csharp
lifetimeScope.PublishEvent(new ReportsChangedEvent());
lifetimeScope.SubscribeEvent<ReportsChangedEvent>(OnReportsChanged, onUiThread: true);
```

使用者可見的暫時回饋使用 `IToastManager`：

```csharp
toastManager.PromptMessage(ToastMessage.Info("Report exported.", localization: false));
toastManager.PromptException(exception);
```

事件用於協調，Toast 用於可見回饋。不要把 Toast 訊息當成控制流程。

## 拖放

拖放動作透過 `IDropActionProvider` 貢獻。Orchestrator 會向所有 Provider 詢問相容的 `DropAction`，用 `CanRun()` 過濾，依 `Id` 去重，然後執行預設動作或顯示選擇器。

當模組需要以工作區感知的方式處理拖入的檔案、路徑或 Tab 負載時，使用這個擴展點。`DropContext` 包含目標物件、工作區 id、修飾鍵、螢幕座標與取消權杖。

## Aspire 資源

Aspire 擴展模組註冊 `IExtensionResourcesProvider`。`AspireService.Build(...)` 會解析所有 Provider，並呼叫每個 Provider 的 `ConfigureResources(builder)`。

對於命令列子服務，透過 `ICommandlineResourcesProvider` 註冊：

```csharp
lifetimeScope.Resolve<ICommandlineResourcesProvider>()
    .Register(new CommandlineServiceOptions
    {
        Name = "reports-worker",
        WorkDirectory = workerDirectory,
        Command = "dotnet run"
    });
```

它適合由 Aspire Host 拉起的 sidecar 服務，而不是大型應用內 UI。

## CLI 選項

CLI 會載入模組，並在最終確定根命令前呼叫 `RegisterCommandLineOption(container, optionRegister)`。當模組需要自己的命令列開關時使用它。

如果是完整鷹架命令，使用類似內建 `zyc new` 和 `zyc new-module` 的明確子命令，不要把無關含義塞進普通 flag。

## 如何選擇擴展面

| 目標 | 使用 |
| --- | --- |
| 開啟功能 View | URI + `ITabItemFactory` |
| 新增一個頂層應用命令 | 既有主選單 Provider |
| 在一個模組父級下新增多個命令 | 模組自有 `IMainMenuItemsProvider` |
| 新增工作區操作 | `IWorkspaceMenuManager` |
| 新增 Tab 標頭動作 | `ITabItemHeaderContextMenuItemView` |
| 顯示輕量執行階段狀態 | Status bar provider |
| 新增托盤/視窗命令 | `ITaskbarMenuManager` |
| 持久化小型設定或狀態 | `IConfig` / `IState` |
| 協調執行階段行為 | `IEventAggregator` |
| 顯示使用者回饋 | `IToastManager` |
| 新增拖入檔案行為 | `IDropActionProvider` |
| 啟動 sidecar 服務 | Aspire resource provider |
