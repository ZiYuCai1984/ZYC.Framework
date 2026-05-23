<p align="center">
  <a href="./module-development.md">English</a> |
  <a href="./module-development.ja.md">日本語</a> |
  <a href="./module-development.zh-CN.md">简体中文</a> |
  <a href="./module-development.zh-TW.md">繁體中文</a> |
  <a href="./module-development.ko.md">한국어</a> |
</p>


# 모듈 개발

이 문서는 ZYC.Framework Host 런타임 모델에 맞는 모듈을 만드는 방법을 설명합니다. 핵심은 의존성 주입을 진입점으로 사용하고, URI 기반 탭 내비게이션을 제공하며, 필요하면 메인 메뉴에 통합하고, 공개 계약과 WPF 구현을 분리하는 것입니다.

## 모듈을 만드는 시점

기능을 셸에 직접 컴파일하지 않고 ZYC.Framework Host가 발견하고 로드해야 한다면 모듈로 만듭니다. 모듈은 뷰, 탭 팩터리, 메인 메뉴 항목, 구성, 상태, 백그라운드 서비스, 명령줄 옵션을 제공할 수 있습니다.

재사용되는 기능 계약이 있다면 `*.Abstractions` 프로젝트를 만듭니다. 공개 DTO, 상수, 서비스 인터페이스, 메뉴 provider 인터페이스를 여기에 둡니다. WPF 뷰, 탭 항목 구현, 런타임 등록은 구현 프로젝트에 둡니다.

## 권장 프로젝트 구조

| 영역 | 일반적인 파일 | 책임 |
| --- | --- | --- |
| Abstractions | `*ModuleConstants.cs`, `I*MainMenuItemsProvider.cs`, service interfaces | 다른 모듈이 WPF 구현 세부 사항에 의존하지 않고 참조할 수 있는 공개 계약. |
| Implementation | `Module.cs` | 모듈 라이프사이클 진입점. 팩터리, 메뉴 항목, provider, 서비스를 등록합니다. |
| Navigation | `*TabItemFactory.cs`, `*TabItem.cs` | `zyc://` 또는 app URI를 매칭하고 탭 인스턴스를 만듭니다. |
| Menu | `*MainMenuItem.cs`, optional `*MainMenuItemsProvider.cs` | 모듈 탭으로 이동하거나 모듈 동작을 실행하는 사용자 명령을 추가합니다. |
| UI | `UI/*View.xaml`, `UI/*View.xaml.cs` | 탭 항목에서 사용하는 WPF 뷰. |

Abstractions 프로젝트에서는 WPF 컨트롤 타입을 공개하지 마세요. 명령 계약이 필요한 경우 `System.Windows.Input.ICommand`는 Abstractions에서 사용할 수 있습니다.

## 라이프사이클

| 메서드 | 시점 | 용도 |
| --- | --- | --- |
| `RegisterAsync(ContainerBuilder builder)` | Autofac 컨테이너가 만들어지기 전. | 전체 컨테이너에서 사용할 수 있어야 하는 서비스를 등록합니다. |
| `LoadAsync(ILifetimeScope lifetimeScope)` | 컨테이너가 만들어지고 모듈이 활성화된 뒤. | 탭 팩터리, 메인 메뉴 항목, 상태 표시줄 항목, 런타임 hook을 등록합니다. |
| `AfterLoadedAsync(ILifetimeScope lifetimeScope)` | 활성화된 모든 모듈이 로드된 뒤. | 다른 모듈 등록이 끝난 뒤 필요한 교차 모듈 초기화. |

대부분의 UI 모듈은 `LoadAsync`만 구현하면 충분합니다.

## 최소 뷰 모듈

단일 뷰 모듈은 `LoadAsync`에서 simple tab factory를 등록합니다.

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

이것은 `minimal` 프로젝트 템플릿에서 사용하는 패턴입니다. 하나의 WPF `UserControl`을 탭으로 노출하는 정도라면 이 방식으로 충분합니다.

## 라우팅 탭 모듈

안정적인 URI, 라우트 매칭, 파라미터, 사용자 지정 싱글턴 동작, 여러 탭이 필요하다면 `TabItemFactoryBase`를 사용합니다.

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

`TabItemRouteAttribute`는 `Scheme`, `Host`, `Path`, `PathMatch`로 URI를 매칭할 수 있습니다. 같은 URI에 여러 팩터리가 매칭되면 `Priority`가 높은 팩터리가 선택됩니다. `TabItemFactoryBase`의 기본값은 `IsSingle = true`입니다. 여러 인스턴스를 허용하려면 이를 재정의하세요.

## 탭 항목과 뷰

라우팅 탭은 보통 `TabItemInstanceBase<TView>`로 WPF 뷰를 감쌉니다.

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

탭 라이프사이클 동작은 탭 항목에 두고, 시각적인 동작은 뷰에 둡니다. 이렇게 하면 라우팅, 탭 식별, UI 구성을 분리할 수 있습니다.

## 메인 메뉴 항목

메뉴 항목은 일반적으로 모듈 URI로 이동합니다.

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

모듈에서 등록합니다.

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterTabItemFactory<ReportsTabItemFactory>();
    lifetimeScope.RegisterExtensionsMainMenuItem<ReportsMainMenuItem>();

    return Task.CompletedTask;
}
```

기능 위치에 맞는 기존 메뉴 provider를 우선 사용하세요. File, View, Tools, Extensions, About 등이 있습니다. 기능이 여러 하위 명령을 가질 때만 모듈 전용 provider를 추가합니다.

## 구성과 상태

`IConfig` 또는 `IState`를 구현하는 타입은 모듈 등록 중 settings 디렉터리에서 로드되고 Autofac에 자동 등록됩니다. 사용자가 편집하는 설정에는 config를 사용하고, 선택된 경로, 대기 중인 작업, 창 상태 같은 런타임 지속성에는 state를 사용합니다.

큰 비즈니스 데이터를 config/state 클래스에 저장하지 마세요. 작고, 직렬화 가능하며, 버전 변화에 견딜 수 있게 유지합니다.

## 모듈 로딩과 의존성

Host는 `ZYC.Framework.Modules*.dll`처럼 이름이 지정된 표준 모듈 어셈블리를 발견한 뒤 `ModuleConfig.AdditionalAssemblyNames`에 나열된 어셈블리를 추가합니다. `ModuleConfig.DisabledAssemblyNames`에 포함된 어셈블리는 발견되지만 활성 모듈로 로드되지는 않습니다.

의존성은 다른 모듈의 `*.Abstractions.dll` 참조에서 추론됩니다. 모듈 A가 `ZYC.Framework.Modules.B.Abstractions.dll`을 참조하면 런타임은 A가 B에 의존한다고 보고할 수 있으며, A가 B의 WPF 구현을 직접 참조할 필요는 없습니다.

## 체크리스트

- 공개 상수와 계약은 `*.Abstractions`에 둡니다.
- WPF 뷰와 탭 항목은 구현 프로젝트에 둡니다.
- 컨테이너 구성 전에 필요한 서비스만 `RegisterAsync`에서 등록합니다.
- 탭 팩터리와 메뉴 항목은 `LoadAsync`에서 등록합니다.
- 안정적인 URI 라우팅에는 `TabItemRouteAttribute`를 사용합니다.
- 새 메뉴 provider를 추가하기 전에 기존 메인 메뉴 provider를 우선 사용합니다.
- config/state 클래스는 작고 직렬화 가능하게 유지합니다.
