<p align="center">
  <a href="./troubleshooting.md">English</a> |
  <a href="./troubleshooting.ja.md">日本語</a> |
  <a href="./troubleshooting.zh-CN.md">简体中文</a> |
  <a href="./troubleshooting.zh-TW.md">繁體中文</a> |
  <a href="./troubleshooting.ko.md">한국어</a> |
</p>


# 문제 해결

이 페이지는 ZYC.Framework 호스트 또는 모듈 프로젝트에서 자주 발생하는 실패 지점을 정리합니다. 보이는 증상에서 시작한 뒤, 그 동작을 담당하는 계층을 확인하세요.

## 빠른 확인

| 증상 | 먼저 확인할 것 |
| --- | --- |
| `zyc`를 찾을 수 없음 | 전역 `ZYC.Framework.CLI` 도구를 설치하거나 업데이트한 뒤, 새 셸에서 `zyc new --help`를 실행합니다. |
| `zyc new` 실패 | 프로젝트 이름, `--template`, `--output`, `--package-version`, 대상 폴더의 기존 파일 여부를 확인합니다. `--overwrite`는 생성 파일을 교체하려는 의도가 있을 때만 사용합니다. |
| 모듈이 보이지 않음 | 런타임 DLL이 app 디렉터리에 있고, `ZYC.Framework.Modules*.dll`과 일치하거나 `ModuleConfig.AdditionalAssemblyNames`에 있으며, `ModuleBase` 진입점을 포함하는지 확인합니다. |
| 모듈은 발견되지만 로드되지 않음 | `ModuleConfig.DisabledAssemblyNames`를 확인합니다. 비활성 모듈은 모듈 정보로 등록되지만 `LoadAsync`는 건너뜁니다. |
| 모듈 로드 오류 페이지가 열림 | 모듈 이름, 예외, 함수 이름을 확인합니다. 실패는 `LoadAsync` 또는 `AfterLoadedAsync`에서 발생했습니다. |
| 메뉴 항목이 없음 | 소유 모듈이 로드되었고, 올바른 메뉴 provider에 등록되었으며, 항목이 hidden 상태가 아닌지 확인합니다. |
| 탐색이 Not Found를 엶 | URI와 일치하는 `ITabItemFactory`가 없습니다. route attribute, factory 등록, `ITabManager.NavigateAsync(...)`에 전달한 URI를 확인합니다. |
| 탐색이 오류 탭을 엶 | factory, tab item, view 또는 tab `LoadAsync`가 실패했습니다. 오류 탭과 로그의 예외를 확인합니다. |
| NuGet으로 설치한 모듈이 활성화되지 않음 | ModuleManager에서 다시 설치하거나 업데이트하고, `settings/nuget.module.assets.json`이 작성되었는지 확인한 뒤 호스트를 재시작합니다. |
| Aspire 리소스가 보이지 않음 | 모듈이 `IExtensionResourcesProvider` 또는 `ICommandlineResourcesProvider`를 등록했는지, Aspire가 활성화 또는 시작되었는지 확인합니다. |
| 내장 터미널 실패 | 터미널 native DLL이 출력 디렉터리의 예상 `runtimes` 폴더에 복사되었는지 확인합니다. |
| 문서 변경이 보이지 않음 | `src/ZYC.Framework.Build.Doc/Templates` 아래의 파일을 수정하고, 필요하면 게시 문서를 다시 생성합니다. |

## CLI와 프로젝트 생성

권장 생성 흐름은 전역 dotnet tool을 사용합니다.

```bash
dotnet tool install --global ZYC.Framework.CLI --version 1.4.6
dotnet tool update --global ZYC.Framework.CLI --version 1.4.6
zyc new MyCompany.Tools --template minimal
```

설치 후에도 `zyc`를 사용할 수 없다면:

- 새 터미널을 열어 업데이트된 tool path를 반영합니다.
- `dotnet tool list --global`을 실행해 `ZYC.Framework.CLI`가 설치되었는지 확인합니다.
- `zyc new --help`를 실행해 CLI 명령이 해석되는지 확인합니다.

프로젝트 생성이 실패한다면:

- `MyCompany.Tools`처럼 유효한 점 구분 C# 프로젝트 이름을 사용합니다.
- 호스트만 필요하면 `--template minimal`, 호스트와 모듈 분리가 필요하면 `--template modular`을 사용합니다.
- 대상 폴더를 프로젝트 이름에서 유도하지 않으려면 `--output`을 사용합니다.
- 생성 프로젝트가 특정 패키지 버전을 참조해야 하면 `--package-version`을 사용합니다.
- 기존 생성 파일을 교체하려는 경우에만 `--overwrite`를 사용합니다.

## 모듈 발견

시작 시 호스트는 애플리케이션 디렉터리에서 모듈 어셈블리를 발견합니다. 표준 내장 모듈 DLL은 파일 이름으로 매칭되며, 추가 모듈은 `ModuleConfig.AdditionalAssemblyNames`에 나열할 수 있습니다.

```json
{
  "AdditionalAssemblyNames": [
    "MyCompany.Tools.dll"
  ],
  "DisabledAssemblyNames": []
}
```

모듈이 발견되지 않는다면:

- DLL이 애플리케이션 디렉터리에 있는지 확인합니다.
- DLL 이름이 표준 `ZYC.Framework.Modules*.dll` 패턴과 일치하거나 `AdditionalAssemblyNames`에 있는지 확인합니다.
- 어셈블리에 `ModuleBase`에서 파생된 구체 타입이 있는지 확인합니다.
- `*.Abstractions` 어셈블리만 나열하지 마세요. Abstractions 프로젝트는 계약을 정의할 뿐 런타임 모듈 진입점이 아닙니다.

모듈 정보에는 보이지만 로드되지 않는다면:

- DLL 파일 이름이 `DisabledAssemblyNames`에 있는지 확인합니다.
- 비활성 모듈은 발견되지만 `LoadAsync`가 호출되지 않습니다.
- `DisabledAssemblyNames`에서 파일 이름을 제거하거나 ModuleManager로 다시 활성화하고, 실행 중인 호스트가 해당 모듈을 동적으로 다시 로드하지 않는다면 재시작합니다.

## 모듈 로드 오류

호스트는 두 단계의 로드 실패를 기록합니다.

- `LoadAsync`: 모듈은 보통 여기서 메뉴, 탭, 상태 항목, 런타임 서비스를 등록합니다.
- `AfterLoadedAsync`: 모듈은 다른 모듈이 로드된 뒤에 의존 작업을 실행할 수 있습니다.

모듈 로드 오류 페이지가 열리면 표시된 모듈 이름, 함수 이름, 예외에서 시작하세요. `AppConfig.SuppressModuleLoadError`는 페이지를 숨길 수 있지만 근본 실패를 해결하지는 않습니다.

일반적인 원인:

- 필요한 서비스가 해결되기 전에 등록되지 않았습니다.
- view 또는 tab item 생성자가 등록 또는 시작 탐색 중 예외를 던졌습니다.
- 모듈이 다른 모듈이 활성화되어 있다고 가정하지만, 해당 의존 모듈이 비활성화되었거나 없습니다.
- 모듈이 기대하는 로컬 파일, native DLL, 외부 프로세스가 없습니다.

## 메뉴, 탭, 라우팅

메뉴 항목은 보통 모듈의 `LoadAsync`에서 등록됩니다. 메뉴 항목이 없다면:

- 모듈 자체가 오류 없이 로드되었는지 확인합니다.
- File, Tools, Extensions, About, Settings 같은 올바른 provider에 항목을 등록합니다.
- 항목이 상태나 구성 때문에 hidden 상태인지 확인합니다.
- priority와 anchor는 항목을 생성하지 않으므로, 먼저 항목이 보이는지 확인한 뒤 순서를 봅니다.

탭 탐색은 `ITabItemFactory`에 의존합니다. Not Found가 열린다면:

- factory가 로드된 어셈블리에 등록되었는지 확인합니다.
- `TabItemRoute`의 scheme, host, path가 탐색 URI와 일치하는지 확인합니다.
- 파일 미리보기 같은 일반 route가 더 구체적인 route보다 먼저 매칭될 수 있다면 factory priority를 확인합니다.
- single-instance tab이 새 탭을 여는 대신 기존 탭을 재사용하는지 확인합니다.

오류 탭이 열린다면 route는 일치했지만 생성 또는 로드가 실패한 것입니다. 오류 탭의 예외를 확인한 뒤 factory, tab item 생성자, view 생성자, tab `LoadAsync`를 살펴보세요.

## Workspace와 복원 타이밍

시작 탐색, 프로토콜 전달 탐색, 복원 중 모듈 작업은 workspace와 tab 복원 파이프라인이 준비될 때까지 기다려야 합니다. 탭이 잘못된 workspace에서 열리거나 복원 후 사라진다면:

- `TabManagerRestoreCompleted` 이후 시작 탐색을 실행합니다.
- 사용자 동작은 포커스된 workspace를 사용합니다.
- 복원 또는 알려진 대상 workspace로 전달할 때는 명시적인 workspace id를 사용합니다.
- 탭 이동, 생성, 닫기는 UI 컬렉션을 직접 수정하지 말고 `ITabManager`를 통해 수행합니다.

## Config와 State

구체 `IConfig` 및 `IState` 타입은 모듈 어셈블리가 등록될 때 settings 디렉터리에서 로드됩니다. 설정을 읽거나 저장하지 못한다면:

- config 또는 state 타입이 구체 타입이며 로드된 런타임 어셈블리에 있는지 확인합니다.
- config 또는 state 타입을 기대하기 전에 모듈 어셈블리가 발견되었는지 확인합니다.
- settings 파일이 소스 트리가 아니라 호스트 settings 디렉터리에 있는지 확인합니다.
- 계약 타입만 abstractions 어셈블리에 두고 거기서 런타임 state가 생성되리라 기대하지 마세요.

## 단일 인스턴스와 Mutex Override

`settings/mutex-id.override`가 없으면 호스트는 제품 정보에서 single-instance mutex id를 만듭니다. Tools > Override Mutex Id로 이 파일을 만들거나 업데이트하거나 삭제할 수 있습니다.

Override를 변경한 뒤에는 호스트를 재시작하세요. Mutex와 startup URI pipe name은 시작 시 만들어지므로 실행 중인 프로세스가 즉시 identity를 바꾸지는 않습니다. Side-by-side instances, startup URI forwarding, foreground-window activation이 예상과 다르게 동작하면 먼저 현재 `mutex-id.override` 파일을 확인합니다.

## NuGet 모듈

ModuleManager는 임시 프로젝트를 restore하고 해결된 runtime asset graph를 `settings/nuget.module.assets.json`에 작성해 NuGet 모듈을 설치합니다. 호스트는 다음 시작 시 이 파일을 읽습니다.

NuGet 모듈을 설치했지만 활성화되지 않는다면:

- restore가 성공했고 assets 파일이 `settings` 아래에 있는지 확인합니다.
- 시작 발견 과정에서 runtime assemblies를 로드할 수 있도록 호스트를 재시작합니다.
- 패키지가 현재 호스트 대상인 `net10.0-windows`와 호환되는 runtime assembly를 포함하는지 확인합니다.
- 설치된 모듈 어셈블리가 `ModuleConfig.DisabledAssemblyNames`로 비활성화되었는지 확인합니다.
- assets 파일이 오래된 패키지 내용을 가리킨다면 다시 설치하거나 제거 후 설치합니다.

알려진 패키지가 검색 결과에 보이지 않는다면 NuGet search가 `IncludeRegex`보다 먼저 실행된다는 점을 확인하세요. 반환된 NuGet page에 없는 패키지는 regex filter까지 도달하지 않습니다. `NuGetModuleConfig.SearchTerm`, `SearchSkip`, `SearchTake`를 확인합니다. `SearchTake`는 NuGet.org 단일 요청 한도인 1000으로 clamp되므로 이후 페이지에는 `SearchSkip`을 사용합니다.

Install, uninstall, refresh는 같은 module-assets pipeline을 공유하며 ModuleManager operation coordinator에 의해 직렬화됩니다. 이 command들이 비활성화된 것처럼 보이면 현재 restore/search operation이 끝날 때까지 기다린 뒤 다음 작업을 시작하세요.

## Aspire와 Sidecar 리소스

Aspire 리소스는 모듈의 extension provider를 통해 제공됩니다. 모듈은 Aspire builder를 직접 사용자 지정하는 `IExtensionResourcesProvider` 또는 command-line sidecar 리소스용 `ICommandlineResourcesProvider`를 등록할 수 있습니다.

리소스가 보이지 않는다면:

- Aspire가 resource graph를 만들기 전에 제공 모듈이 로드되었는지 확인합니다.
- provider 타입이 모듈 어셈블리에 등록되었는지 확인합니다.
- command-line 리소스는 리소스 이름, 작업 디렉터리, 명령이 유효한지 확인합니다.
- `AspireConfig.AutoStart`가 비활성화되어 있으면 필요에 따라 Aspire를 수동으로 시작합니다.

Aspire dashboard를 열 수 없다면 Aspire 프로세스가 `ASPNETCORE_URLS`와 `AppHost:BrowserToken`을 생성했는지 확인합니다. dashboard URI는 이 값들로 구성됩니다.

## CLI 터미널 native 의존성

CLI 모듈은 애플리케이션 출력에서 터미널 native 의존성을 로드합니다. 내장 터미널이 초기에 실패한다면 다음 파일이 있는지 확인하세요.

```text
runtimes\win10-x64\native\conpty.dll
runtimes\win-x64\native\Microsoft.Terminal.Control.dll
```

파일이 없다면 CLI 모듈과 터미널 의존성의 package output 및 copy-local 동작을 확인합니다.

## 문서 템플릿

`ZYC.Framework.Build.Doc`가 사용하는 문서 소스는 다음 위치에 있습니다.

```text
src\ZYC.Framework.Build.Doc\Templates
```

생성된 root `docs` 파일을 직접 수정하면 로컬에서는 동작하는 것처럼 보여도 나중에 사라질 수 있습니다. 변경을 해당 template file로 옮긴 뒤 문서를 다시 생성하세요.
