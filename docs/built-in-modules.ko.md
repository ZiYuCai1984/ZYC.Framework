<p align="center">
  <a href="./built-in-modules.md">English</a> |
  <a href="./built-in-modules.ja.md">日本語</a> |
  <a href="./built-in-modules.zh-CN.md">简体中文</a> |
  <a href="./built-in-modules.zh-TW.md">繁體中文</a> |
  <a href="./built-in-modules.ko.md">한국어</a> |
</p>


# 내장 모듈

이 문서는 현재 ZYC.Framework 소스 트리에 포함된 내장 모듈을 요약합니다. 여기서 내장 모듈은 `Module : ModuleBase` 진입점을 가지고 모듈 로더가 발견하도록 의도된 `ZYC.Framework.Modules.*` 프로젝트를 의미합니다.

`ZYC.Framework.Modules.*.Abstractions` 같은 Abstractions 프로젝트는 계약 어셈블리이며, 그 자체로 런타임 모듈은 아닙니다.

## 내장 모듈 로딩 방식

시작 시 모듈 로더는 애플리케이션 디렉터리에서 `ZYC.Framework.Modules*.dll` 형식의 어셈블리를 찾습니다. 각 모듈 어셈블리에 대해 다음을 수행합니다.

- 어셈블리의 Autofac 서비스를 등록합니다.
- settings 디렉터리에서 concrete `IConfig`와 `IState` 타입을 로드합니다.
- `ModuleBase`를 상속한 첫 번째 타입을 찾습니다.
- 모듈 인스턴스를 만들고 `RegisterAsync`를 호출합니다.
- 이후 활성화된 모듈에만 `LoadAsync`를 호출합니다.

`ModuleConfig.DisabledAssemblyNames`는 발견된 모듈 어셈블리를 파일 이름으로 비활성화합니다. `ModuleConfig.AdditionalAssemblyNames`는 애플리케이션 디렉터리에서 추가 어셈블리를 읽습니다.

## 모듈 목록

| Module | 주요 표면 | 설명 |
| --- | --- | --- |
| `About` | About 메뉴와 라우팅 탭 | 제품/about 정보를 표시합니다. |
| `Accounts` | 창 제목 표시줄 확장과 계정 서비스 | Provider 기반 account session을 초기화하고 sign-in/sign-out 작업을 노출합니다. |
| `Accounts.GitHub` | GitHub OAuth WebView2 탭 | GitHub account provider와 sign-in callback 처리를 제공합니다. |
| `ApiReference` | About 메뉴와 WebView2 탭 | API reference content를 호스트합니다. |
| `Aspire` | Tools 메뉴, 라우팅 탭, 상태 표시줄 | Aspire resources를 시작하고 모니터링하며 `IExtensionResourcesProvider` 기여를 resolve합니다. |
| `BlazorDemo` | Tools 메뉴와 라우팅 탭 | 데스크톱 Host 안의 Blazor 통합을 시연합니다. |
| `ChromeExtensions` | Extensions 메뉴와 라우팅 탭 | WebBrowser가 사용할 로컬 Chrome Web Store extension packages를 관리합니다. |
| `CLI` | Tools 메뉴와 터미널 탭 | 내장 터미널을 호스트하고 terminal native dependencies를 로드합니다. |
| `FileExplorer` | File 메뉴와 라우팅 탭 | 파일 시스템 탐색 표면을 엽니다. |
| `FileExplorer.Features` | File menu sub-provider | FileExplorer contracts 위에 recent-path 계열 File 메뉴 기능을 추가합니다. |
| `Language` | Settings 메뉴와 라우팅 탭 | 언어 선택과 로컬라이제이션 리소스 관리를 제공합니다. |
| `Log` | File 메뉴와 logging provider | log4net 기반 logger provider를 등록하고 로그 보기를 제공합니다. |
| `MCP.Server` | Tools menu provider | MCP server 작업을 노출합니다. |
| `Mock` | Root mock menu와 demo tabs | 대화상자, 알림, 작업, CLI, 샘플 View용 개발/테스트 모듈입니다. |
| `ModuleManager` | Extensions 메뉴와 라우팅 탭 | 로컬 모듈과 NuGet-installed modules를 관리합니다. |
| `NuGet` | File 메뉴 | NuGet cache tooling을 제공합니다. |
| `Secrets` | Settings 메뉴와 라우팅 탭 | `ISecrets`를 통해 secret-like settings를 관리합니다. |
| `Settings` | Root Settings 메뉴와 라우팅 탭 | 다른 모듈이 사용하는 settings shell을 호스트합니다. |
| `TaskManager` | Tools 메뉴, 라우팅 탭, 상태 표시줄 | task management를 초기화하고 작업 상태/동작을 노출합니다. |
| `TextEditor` | File/Open 메뉴와 라우팅 탭 | text preview/edit 표면을 제공하며 generic `file://` preview handling을 포함합니다. |
| `Translator` | Aspire command-line resource | Aspire가 사용 가능할 때 LibreTranslate sidecar를 등록합니다. |
| `Update` | About 메뉴와 라우팅 탭 | 업데이트 확인을 제공하며 tab/workspace restore 뒤 시작 시 확인할 수 있습니다. |
| `WebBrowser` | Tools 메뉴와 WebView2 탭 | Host 안에서 browser tab을 엽니다. |

## Shell 및 Diagnostics 모듈

`Settings`, `Language`, `Secrets`, `Log`, `TaskManager`, `ModuleManager`, `Update`, `About`, `Accounts`, `ChromeExtensions`, `ApiReference`는 주로 Shell 또는 운영 모듈입니다. Framework를 검사, 설정, 유지관리하기 쉽게 합니다.

이 모듈들은 보통 `LoadAsync`에서 메뉴 항목과 라우팅 탭을 등록합니다. 일부는 더 이른 단계에서 서비스도 등록합니다.

- `Log`는 `RegisterAsync`에서 logging providers를 등록합니다.
- `Language`는 language-resource adapters를 등록하고 default language resources를 로드합니다.
- `Secrets`는 config objects에서 `ISecrets`로 가는 adapter를 등록합니다.
- `TaskManager`는 UI를 노출하기 전에 `ITaskManager`를 초기화합니다.
- `Accounts`는 `IAccountManager`를 초기화하고 창 제목 표시줄 계정 표면을 등록합니다.
- `ChromeExtensions`는 Extensions 아래에 extension package manager 탭을 등록합니다.
- `Update`는 모든 모듈이 로드된 뒤 구독하고 시작 확인 전에 `TabManagerRestoreCompleted`를 기다립니다.

## Navigation 및 Content 모듈

`WebBrowser`, `FileExplorer`, `TextEditor`, `CLI`, `BlazorDemo`는 사용자 대상 content surface를 노출합니다. 이들은 Shell에서 View를 직접 만들지 않고 모두 tab routing에 의존합니다.

`Accounts.GitHub`와 `ChromeExtensions`도 provider sign-in과 Chrome Web Store package discovery를 위해 WebView2 기반 탭을 사용합니다. 둘 다 일반 모듈로 로드되며 browser-specific behavior는 WebView2 infrastructure와 module contracts를 통해 처리합니다.

이 모듈들이 잘못된 탭이나 Not Found 탭을 열면 등록된 `ITabItemFactory`, route attributes, factory priority, `ITabManager.NavigateAsync(...)`에 전달되는 URI를 확인하세요.

## Aspire 및 Sidecar 모듈

`Aspire`는 Aspire resources를 실행하는 Host 측 모듈입니다. Aspire dashboard tab, Tools menu entry, status bar item을 등록합니다. `AspireConfig.AutoStart`가 true이면 모듈 로드 중 Aspire service를 시작합니다.

`Translator`는 sidecar 스타일 모듈입니다. 큰 UI를 노출하지 않고 `ICommandlineResourcesProvider`에 command-line resource를 등록하여 Aspire 모듈이 LibreTranslate를 시작할 수 있게 합니다.

## Development 및 Demo 모듈

`Mock`과 `BlazorDemo`는 framework behavior 개발과 검증에 유용합니다. `Mock`은 알림, 대화상자, task manager behavior, CLI integration을 위한 demo tabs와 sample views를 등록합니다. 명확히 진단 또는 샘플 용도가 아니라면 production feature를 Mock에 두지 마세요.

## 불완전하거나 모듈이 아닌 디렉터리

실제 module project와 `Module.cs`를 가진 디렉터리만 active built-in module로 취급해야 합니다. `obj`, 생성 파일, `UI` 하위 폴더만 있는 폴더는 runtime discovery의 근거가 되지 않습니다.

모듈 로딩을 문서화하거나 문제를 조사할 때는 폴더 이름만 보지 말고 compiled output과 `Module.cs`에서 시작하세요.
