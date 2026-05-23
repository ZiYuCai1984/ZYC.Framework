<p align="center">
  <a href="./extension-points.md">English</a> |
  <a href="./extension-points.ja.md">日本語</a> |
  <a href="./extension-points.zh-CN.md">简体中文</a> |
  <a href="./extension-points.zh-TW.md">繁體中文</a> |
  <a href="./extension-points.ko.md">한국어</a> |
</p>


# 확장 지점

ZYC.Framework 확장은 대부분 모듈과 Autofac을 통해 등록됩니다. 모듈은 Host와 함께 로드되고 서비스 또는 UI 기여를 등록합니다. 그런 다음 Shell은 해당 등록에서 메뉴, 탭, 워크스페이스 작업, 상태 표시줄 항목, 작업 표시줄 메뉴 항목, 드래그 앤 드롭 동작, Aspire 리소스를 구성합니다.

## 확장 지점 맵

| 확장 지점 | 등록 위치 | 런타임 소비자 |
| --- | --- | --- |
| 모듈 라이프사이클 | `ModuleBase.RegisterAsync`, `LoadAsync`, `AfterLoadedAsync` | Host 시작과 모듈 로더. |
| URI 탭 | `ITabItemFactoryManager.RegisterFactory<T>()` | `TabManager.InternalNavigateAsync(...)`. |
| 단순 뷰 탭 | `ISimpleTabItemFactoryManager.Register(...)` | 내장 `SimpleTabItemFactory`. |
| 메인 메뉴 | `IMainMenuManager`, `IMainMenuItemsProvider` | `MainMenuManager`와 메인 메뉴 뷰. |
| 워크스페이스 메뉴 | `IWorkspaceMenuManager` | `WorkspaceMenuView`. |
| 워크스페이스 컨텍스트 메뉴 manager | `IWorkspaceContextMenuManager` | Manager는 존재하며 항목을 정렬합니다. 컨텍스트 메뉴 표면을 연결할 때 사용합니다. |
| 탭 헤더 컨텍스트 메뉴 | `ITabItemHeaderContextMenuItemView` | `TabItemHeaderContextMenuItemsResolver`. |
| 상태 표시줄 | `IStatusBarManager`, `IStatusBarItemsProvider` | `StatusBarManager`. |
| 작업 표시줄 메뉴 | `ITaskbarMenuManager` | `TaskbarContextMenu`. |
| 구성/상태 | `IConfig`, `IState` | `ModuleTools.RegisterAllFromAssembly(...)`. |
| 이벤트 | `IEventAggregator` | 런타임 publish/subscribe bus. |
| Toast | `IToastManager`, `IToast` | Toast popup host. |
| 드래그 앤 드롭 | `IDropActionProvider` | `DropOrchestrator`. |
| Aspire 리소스 | `IExtensionResourcesProvider` | `AspireService.Build(...)`. |
| CLI 옵션 | `ModuleBase.RegisterCommandLineOption(...)` | `ZYC.Framework.CLI` root command. |

## 모듈 라이프사이클

각 등록 위치는 모듈 라이프사이클에 맞춰 결정합니다.

| 메서드 | 용도 |
| --- | --- |
| `RegisterAsync(ContainerBuilder builder)` | 컨테이너가 만들어지기 전에 필요한 Autofac 등록. |
| `LoadAsync(ILifetimeScope lifetimeScope)` | 탭 팩터리, 메뉴 항목, 상태 표시줄 provider, Aspire 리소스 등록 같은 런타임 기여. |
| `AfterLoadedAsync(ILifetimeScope lifetimeScope)` | 모든 모듈이 로드된 뒤 필요한 교차 모듈 작업. |
| `RegisterCommandLineOption(...)` | 모듈이 소유한 CLI 플래그. |

대부분의 UI 모듈은 `LoadAsync`만 필요합니다.

## URI 탭

URI 탭은 기능 표면을 노출하는 주요 방법입니다. 보통 `ILifetimeScope` helper로 `ITabItemFactory`를 등록합니다.

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterTabItemFactory<ReportsTabItemFactory>();
    return Task.CompletedTask;
}
```

라우트를 URI 부분으로 설명할 수 있으면 `TabItemRouteAttribute`를 사용합니다. 서비스, 파일 형식 검사, 더 복잡한 정책이 필요하면 `CheckUriMatchedAsync`를 재정의합니다.

전용 라우트 모델 없이 하나의 `UserControl`만 열면 되는 작은 단일 뷰 사례에서는 `RegisterSimpleTabItemFactory(...)`를 사용합니다.

## 메인 메뉴

루트 메뉴에는 File, View, Tools, Extensions, About 내장 provider가 있습니다. 모듈 메뉴 항목은 보통 그중 하나 아래에 등록합니다.

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterExtensionsMainMenuItem<ReportsMainMenuItem>();
    return Task.CompletedTask;
}
```

메뉴 정렬은 먼저 `Anchor`, 같은 anchor 그룹 안에서는 `Priority`로 결정됩니다. `MainMenuManager`는 anchor 그룹 사이에 separator를 넣고 하위 항목도 재귀적으로 정렬합니다.

여러 하위 명령을 가진 부모 메뉴가 필요할 때만 모듈 소유 `IMainMenuItemsProvider`를 만듭니다. 명령이 하나라면 기존 provider 아래에 `IMainMenuItem`을 등록합니다.

## 워크스페이스 메뉴

워크스페이스 번호 옆의 표시되는 드롭다운에 명령을 넣으려면 `IWorkspaceMenuManager`를 사용합니다.

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.Resolve<IWorkspaceMenuManager>()
        .RegisterItem<ReportsWorkspaceMenuItem>();

    return Task.CompletedTask;
}
```

`IWorkspaceMenuItem`은 `Title`, `Command`, `SubItems`, `Icon`, `Anchor`, `Priority`, `Localization`을 지원합니다. 현재 표시되는 `WorkspaceMenuView`는 `IWorkspaceMenuManager.GetItems()`를 읽습니다.

`IWorkspaceContextMenuManager`는 별도의 manager이며 `Anchor`와 `Priority`로 재귀 정렬합니다. 컨텍스트 메뉴 뷰가 연결되어 있지 않다면 그 항목이 표시된다고 가정하지 마세요.

## 탭 헤더 컨텍스트 메뉴

탭 헤더 메뉴 항목은 `ITabItemHeaderContextMenuItemView`로 등록되는 WPF 메뉴 항목 뷰입니다.

```csharp
[RegisterAs(typeof(ITabItemHeaderContextMenuItemView))]
internal partial class ReportsTabHeaderMenuItem :
    ITabItemHeaderContextMenuItemView
{
    public int Order => 20;
}
```

`TabItemHeaderContextMenuItemsResolver`는 등록된 모든 뷰를 resolve하고 `Order`로 정렬합니다. WPF ContextMenu는 late-bound이므로 현재 탭 인스턴스가 필요한 항목에서는 command parameter와 기존 `ContextMenuItemBase` 패턴을 우선 사용하세요.

## 상태 표시줄

상태 표시줄 확장은 `IStatusBarItemsProvider`를 제공합니다. Provider는 하나 이상의 `IStatusBarItem`을 반환합니다. `StatusBarManager`는 등록된 모든 provider를 모으고 `Order`로 정렬합니다.

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.Resolve<IStatusBarManager>()
        .RegisterStatusBarItemsProvider<ReportsStatusBarItemsProvider>();

    return Task.CompletedTask;
}
```

각 항목은 `StatusBarSection.Left` 또는 `StatusBarSection.Right`로 표시 위치를 선택합니다.

## 작업 표시줄 메뉴

작업 표시줄 메뉴 항목은 `ITaskbarMenuItem`을 구현하고 `ITaskbarMenuManager`에 등록합니다.

```csharp
lifetimeScope.Resolve<ITaskbarMenuManager>()
    .RegisterMenuItem(lifetimeScope.Resolve<ReportsTaskbarMenuItem>());
```

작업 표시줄 메뉴는 `Info.Anchor`로 그룹화하고 `Info.Priority`로 정렬하며 하위 항목도 재귀적으로 정렬합니다. 이 표면은 tray/window 수준 명령에 사용하고, 기능 내비게이션은 메인 메뉴에 둡니다.

## 구성과 상태

`IConfig` 또는 `IState`를 구현하는 모든 concrete 타입은 모듈 어셈블리 등록 중 settings 디렉터리에서 로드되고 Autofac에 등록됩니다. 작은 JSON 직렬화 가능 설정과 상태에 사용합니다.

지침:

- 사용자가 편집하는 옵션은 `IConfig`에 둡니다.
- 런타임 지속성은 `IState`에 둡니다.
- 타입은 작고 버전 변화에 견딜 수 있게 유지합니다.
- config/state를 큰 비즈니스 데이터 저장소로 사용하지 마세요.

## 이벤트와 Toast

분리된 런타임 알림에는 `IEventAggregator`를 사용합니다.

```csharp
lifetimeScope.PublishEvent(new ReportsChangedEvent());
lifetimeScope.SubscribeEvent<ReportsChangedEvent>(OnReportsChanged, onUiThread: true);
```

사용자에게 보이는 임시 피드백에는 `IToastManager`를 사용합니다.

```csharp
toastManager.PromptMessage(ToastMessage.Info("Report exported.", localization: false));
toastManager.PromptException(exception);
```

이벤트는 조정용이고 Toast는 표시 피드백용입니다. Toast 메시지를 제어 흐름으로 사용하지 마세요.

## 드래그 앤 드롭

드래그 앤 드롭 동작은 `IDropActionProvider`로 기여합니다. Orchestrator는 모든 provider에 호환되는 `DropAction`을 요청하고, `CanRun()`으로 필터링하며, `Id`로 중복을 제거한 뒤 기본 동작을 실행하거나 picker를 표시합니다.

모듈이 dropped file, path, tab payload를 워크스페이스 인식 방식으로 처리해야 할 때 사용합니다. `DropContext`에는 target object, workspace id, modifier keys, screen point, cancellation token이 포함됩니다.

## Aspire 리소스

Aspire 확장 모듈은 `IExtensionResourcesProvider`를 등록합니다. `AspireService.Build(...)`는 모든 provider를 resolve하고 각 provider의 `ConfigureResources(builder)`를 호출합니다.

명령줄 child service는 `ICommandlineResourcesProvider`로 등록합니다.

```csharp
lifetimeScope.Resolve<ICommandlineResourcesProvider>()
    .Register(new CommandlineServiceOptions
    {
        Name = "reports-worker",
        WorkDirectory = workerDirectory,
        Command = "dotnet run"
    });
```

큰 인앱 UI가 아니라 Aspire Host가 시작해야 하는 sidecar service에 사용합니다.

## CLI 옵션

CLI는 모듈을 로드하고 root command를 확정하기 전에 `RegisterCommandLineOption(container, optionRegister)`를 호출합니다. 모듈이 자체 command-line switch를 필요로 할 때 사용합니다.

전체 스캐폴딩 명령은 관련 없는 flag에 끼워 넣지 말고, 내장 `zyc new`, `zyc new-module` 패턴처럼 명시적 subcommand를 사용합니다.

## 올바른 표면 선택

| 목표 | 사용 |
| --- | --- |
| 기능 뷰 열기 | URI + `ITabItemFactory` |
| 앱 명령 하나 추가 | 기존 main menu provider |
| 하나의 모듈 부모 아래 여러 명령 추가 | 모듈 소유 `IMainMenuItemsProvider` |
| 워크스페이스 작업 추가 | `IWorkspaceMenuManager` |
| 탭 헤더 동작 추가 | `ITabItemHeaderContextMenuItemView` |
| 가벼운 런타임 상태 표시 | Status bar provider |
| Tray/window 명령 추가 | `ITaskbarMenuManager` |
| 작은 설정 또는 상태 지속화 | `IConfig` / `IState` |
| 런타임 동작 조정 | `IEventAggregator` |
| 사용자 피드백 표시 | `IToastManager` |
| dropped-file 동작 추가 | `IDropActionProvider` |
| sidecar service 시작 | Aspire resource provider |
