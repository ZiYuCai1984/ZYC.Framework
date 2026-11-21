<p align="center">
  <a href="./project-templates.md">English</a> |
  <a href="./project-templates.ja.md">日本語</a> |
  <a href="./project-templates.zh-CN.md">简体中文</a> |
  <a href="./project-templates.zh-TW.md">繁體中文</a> |
  <a href="./project-templates.ko.md">한국어</a> |
</p>


# 프로젝트 템플릿

ZYC.Framework는 두 가지 일반적인 스캐폴딩 작업을 위한 `dotnet tool` 명령을 제공합니다. 새 Host 프로젝트 만들기와 기존 소스 트리에 새 모듈 추가입니다. 이 문서는 템플릿, 생성 구조, 명령 옵션을 설명합니다.

## 명령 범위

| 명령 | 목적 | 적합한 경우 |
| --- | --- | --- |
| `zyc new <ProjectName>` | 프로젝트 템플릿에서 외부 ZYC.Framework Host 프로젝트를 만듭니다. | 프레임워크 저장소 밖에서 새 앱이나 샘플을 시작할 때. |
| `zyc new-module <ModuleName> [--src-root <SourceRoot>]` | 기존 소스 트리 안에 모듈 구현 프로젝트와 대응하는 `*.Abstractions` 프로젝트를 만듭니다. | 기존 ZYC.Framework 스타일 저장소를 확장할 때. |

CLI를 .NET tool로 설치하거나 업데이트합니다.

```bash
dotnet tool install -g ZYC.Framework.CLI --version 1.6.4
dotnet tool update -g ZYC.Framework.CLI --version 1.6.4
```

명령을 확인합니다.

```bash
zyc --help
zyc new --help
zyc new-module --help
```

## `minimal` 템플릿

`minimal`은 `zyc new`의 기본 템플릿입니다. `ZYC.Framework.Alpha`를 참조하는 작은 WPF Host 프로젝트를 만들고, 하나의 WPF 뷰를 simple tab으로 등록합니다.

```bash
zyc new MyCompany.Tools
```

명시적으로 쓰면 다음과 같습니다.

```bash
zyc new MyCompany.Tools --template minimal
```

생성 구조:

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

`Directory.Build.props`는 공통 빌드 속성을 정의하고 빌드 출력을 `_bin/app/`에 둡니다. 따라서 Host의 `settings/`와 `logs/` 폴더는 `_bin/`에 만들어집니다. `Directory.Build.targets`는 `ZYC.Framework.Alpha` 버전을 지정하고, 패키지가 복원된 뒤에는 그 종속성 버전을 가져오므로 나중에 추가하는 `ZYC.CoreToolkit.*` 참조에서는 `Version`을 생략할 수 있습니다.

하나의 단순한 WPF 뷰만 필요한 실행 가능한 Host를 가장 빠르게 만들고 싶을 때 사용합니다.

## `react` 템플릿

`react`는 `minimal`과 같은 단일 프로젝트 Host를 만들지만, 탭은 Vite와 TypeScript로 빌드한 React 프런트엔드를 표시하는 WebView2 뷰입니다. 프로젝트를 빌드하려면 Node.js 20.19 이상 또는 22.12 이상이 필요합니다.

```bash
zyc new MyCompany.Tools --template react
```

생성 구조:

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
  Services/
    IGreetingService.cs
    GreetingService.cs
  ClientApp/
    index.html
    package.json
    tsconfig.json
    vite.config.ts
    src/
      App.tsx
      host-services.d.ts
      index.css
      main.tsx
      webview.ts
```

- `ToolsView`는 `WebViewHostBase`를 상속하며, 빌드된 프런트엔드를 가상 호스트 `https://<web-name>.example/`에서 제공합니다. `<web-name>`은 `__PROJECT_WEB_NAME__` 값입니다. 예: `mycompany-tools`.
- 프로젝트를 빌드하면 `ClientApp/package.json`이 바뀌었을 때 `npm install`을, 프런트엔드 소스가 바뀌었을 때 `npm run build`를 실행한 뒤 `ClientApp/dist`를 출력 디렉터리의 `ClientApp/`으로 복사합니다. 이 단계를 건너뛰려면 `-p:SkipClientAppBuild=true`를 전달합니다.
- Debug 빌드에서는 `npm run dev`가 실행 중이면 뷰가 Vite 개발 서버 `http://localhost:5173`을 로드하므로, 다시 빌드하지 않아도 프런트엔드 변경이 반영됩니다.
- `ClientApp/src/webview.ts`는 페이지에서 C# 서비스를 타입 있게 호출하게 해 줍니다: `await hostService('GreetingService').greetAsync('React')`. `Services/IGreetingService.cs`에는 `[ExposeToWeb]`(와 `[ExposeToMCP]`)가 붙어 있고, 뷰의 `WebBridge`가 호출을 처리합니다. 빌드할 때마다 `ZYC.Framework.CLI.exe web-types`가 실행되어 `[ExposeToWeb]` 인터페이스로부터 `ClientApp/src/host-services.d.ts`를 다시 생성합니다.

탭 UI를 XAML 대신 React로 만들고 싶을 때 사용합니다.

## `console` 템플릿

`console`은 공유 빌드 속성과 `ZYC.CoreToolkit` 4.0.0을 참조하는 콘솔 프로젝트를 포함한 `net10.0` 콘솔 솔루션을 만듭니다.

```bash
zyc new MyCompany.Tools --template console
```

생성 구조:

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

원본 템플릿의 루트 수준 공유 빌드 구성을 유지하는 작은 콘솔 애플리케이션이 필요할 때 사용합니다.

## `build` 템플릿

`build`는 버전 생성, 컴파일 및 NuGet 패키징을 담당하는 독립적인 `<ProjectName>.Build` 프로젝트와 간단한 애플리케이션 프로젝트를 만듭니다.

Windows에서 .NET 10 SDK로 실행합니다:

```powershell
zyc new MyCompany.Tools --template build
cd MyCompany.Tools
dotnet run --project MyCompany.Tools.Build/MyCompany.Tools.Build.csproj -c Release
```

패키지 버전은 `MyCompany.Tools.Build/BuildEnvironment.cs`에서 설정합니다. 빌드 프로젝트는 `version.props`를 생성하고 배포 대상 프로젝트를 빌드하여 `_bin/`에 패키지를 만듭니다. 빌드 프로젝트 자체의 출력은 `_bin_build/`에 저장됩니다. 수동 게시 워크플로와 저장소 변수 `NUGET_USER`의 설정 방법은 생성된 README에 설명되어 있습니다.

## `wpf` 템플릿

`wpf`는 Autofac 기반 애플리케이션 시작, 설정 등록, Fody 속성 위빙 및 `ZYC.CoreToolkit` 4.0.0 참조를 포함한 `net10.0-windows` WPF 솔루션을 만듭니다.

```bash
zyc new MyCompany.Desktop --template wpf
```

생성 구조:

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

원본 템플릿의 기존 Autofac 및 설정 부트스트랩을 유지하는 작은 WPF 애플리케이션이 필요할 때 사용합니다.

## `modular` 템플릿

`modular`는 Entry 프로젝트, 모듈 구현 프로젝트, 모듈 Abstractions 프로젝트가 있는 작은 솔루션을 만듭니다.

```bash
zyc new MyCompany.Tools --template modular
```

생성 구조:

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

공개 상수는 Abstractions에 두고, 라우팅 탭 팩터리, 메인 메뉴 항목, 모듈 진입점을 갖춘 실제 프레임워크 모듈 형태가 필요할 때 사용합니다.

## `zyc new` 옵션

| 옵션 | 설명 |
| --- | --- |
| `<ProjectName>` | 필수 프로젝트 이름. `Acme.Tools`처럼 유효한 점 구분 C# 식별자여야 합니다. |
| `--template`, `-t` | 프로젝트 템플릿. 지원 값은 `minimal`, `modular`, `console`, `build`, `wpf`, `react`입니다. 기본값은 `minimal`입니다. |
| `--output`, `-o` | 출력 디렉터리. 기본값은 `./<ProjectName>`입니다. |
| `--package-version` | `ZYC.Framework.Alpha` 패키지 버전. 기본값은 CLI 제품 버전입니다. |
| `--overwrite`, `-f` | 기존 파일을 덮어씁니다. 이 플래그가 없으면 대상 파일이 있을 때 생성이 실패합니다. |

일반적인 옵션을 모두 지정한 예:

```bash
zyc new Acme.Tools --template modular --output ./Acme.Tools --package-version 1.6.4
```

## 기존 소스 트리를 위한 `new-module`

저장소에 이미 `src` 트리가 있고 하나의 모듈 쌍을 추가하려면 `new-module`을 사용합니다.

```bash
zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx
```

이 명령은 다음을 만듭니다.

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

`--slnx`는 선택 사항입니다. 제공하면 생성된 프로젝트가 솔루션 파일의 `/Modules/` 폴더 아래에 추가됩니다. 상대 `--slnx` 경로는 `--src-root`를 기준으로 해석됩니다.

`new-module`은 target 이름을 정규화합니다. 다음 입력은 모두 같은 모듈 target을 생성합니다.

```bash
zyc new-module Reports --src-root ./src
zyc new-module ZYC.Framework.Modules.Reports --src-root ./src
zyc new-module ZYC.Framework.Modules.Reports.Abstractions --src-root ./src
```

## `new-module` 옵션

| 옵션 | 설명 |
| --- | --- |
| `<ModuleName>` | 위치 인수 형태의 target 모듈 이름. |
| `--target`, `-t` | target 모듈 이름. 위치 값 또는 이 옵션 중 하나를 사용하고, 서로 다른 값을 동시에 지정하지 마세요. |
| `--src-root`, `-s` | 선택적 소스 루트. 모듈 프로젝트가 여기에 생성되며, 생략하면 현재 디렉터리를 사용합니다. |
| `--slnx` | 업데이트할 선택적 solution XML 파일. 솔루션 업데이트가 필요 없으면 생략합니다. |
| `--overwrite`, `-f` | 기존 파일 또는 모듈 디렉터리를 덮어씁니다. 이 플래그가 없으면 대상 디렉터리가 있을 때 생성이 실패합니다. |

## 템플릿 토큰

프로젝트 템플릿은 경로와 텍스트 파일에서 다음 토큰을 치환합니다.

| Token | 값 |
| --- | --- |
| `__PROJECT_NAME__` | 전체 프로젝트 이름. 예: `MyCompany.Tools`. |
| `__PROJECT_SHORT_NAME__` | 마지막 점 구분 세그먼트. 예: `Tools`. |
| `__PROJECT_HOST__` | URI host로 사용하는 소문자 짧은 이름. 예: `tools`. |
| `__PROJECT_WEB_NAME__` | npm 패키지 이름과 WebView2 가상 호스트로 사용하는, 하이픈으로 구분된 소문자 전체 이름. 예: `mycompany-tools`. |
| `__V_ZYC_FRAMEWORK_ALPHA__` | `--package-version` 또는 CLI 제품 버전으로 선택된 패키지 버전. |

텍스트 템플릿 파일은 CRLF 줄 끝으로 정규화되고 UTF-8 with BOM으로 작성됩니다. 프런트엔드 파일(`package.json`, `tsconfig.json`, `.ts`, `.tsx`, `.html`, `.css`)은 Node.js 도구가 읽기 때문에 BOM 없이 작성됩니다.

## 템플릿 선택

| 상황 | 권장 명령 |
| --- | --- |
| 하나의 뷰가 있는 가장 빠른 Host가 필요합니다. | `zyc new MyCompany.Tools` |
| 탭 UI를 React로 만들고 싶습니다. | `zyc new MyCompany.Tools --template react` |
| `ZYC.CoreToolkit`을 참조하는 콘솔 애플리케이션이 필요합니다. | `zyc new MyCompany.Tools --template console` |
| 버전 관리, 컴파일 및 패키징을 위한 독립적인 빌드 프로젝트가 필요합니다. | `zyc new MyCompany.Tools --template build` |
| Autofac 및 설정 부트스트랩이 포함된 WPF 애플리케이션이 필요합니다. | `zyc new MyCompany.Desktop --template wpf` |
| 새 앱에 모듈 스타일 솔루션이 필요합니다. | `zyc new MyCompany.Tools --template modular` |
| 기존 저장소에 모듈을 추가합니다. | `zyc new-module Reports --src-root ./src` |
| 모듈 프로젝트를 기존 `.slnx`에 추가해야 합니다. | `zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx` |
