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
| `zyc new-module <ModuleName> --src-root <SourceRoot>` | 기존 소스 트리 안에 모듈 구현 프로젝트와 대응하는 `*.Abstractions` 프로젝트를 만듭니다. | 기존 ZYC.Framework 스타일 저장소를 확장할 때. |

CLI를 .NET tool로 설치하거나 업데이트합니다.

```bash
dotnet tool install -g ZYC.Framework.CLI --version 1.3.3
dotnet tool update -g ZYC.Framework.CLI --version 1.3.3
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
  MyCompany.Tools.csproj
  MyCompany.Tools.slnx
  Module.cs
  ModuleConfig.json
  UI/
    ToolsView.xaml
    ToolsView.xaml.cs
```

하나의 단순한 WPF 뷰만 필요한 실행 가능한 Host를 가장 빠르게 만들고 싶을 때 사용합니다.

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
| `--template`, `-t` | 프로젝트 템플릿. 지원 값은 `minimal`, `modular`입니다. 기본값은 `minimal`입니다. |
| `--output`, `-o` | 출력 디렉터리. 기본값은 `./<ProjectName>`입니다. |
| `--package-version` | `ZYC.Framework.Alpha` 패키지 버전. 기본값은 CLI 제품 버전입니다. |
| `--overwrite`, `-f` | 기존 파일을 덮어씁니다. 이 플래그가 없으면 대상 파일이 있을 때 생성이 실패합니다. |

일반적인 옵션을 모두 지정한 예:

```bash
zyc new Acme.Tools --template modular --output ./Acme.Tools --package-version 1.3.3
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
| `--src-root`, `-s` | 필수 소스 루트. 모듈 프로젝트가 여기에 생성됩니다. |
| `--slnx` | 업데이트할 선택적 solution XML 파일. 솔루션 업데이트가 필요 없으면 생략합니다. |
| `--overwrite`, `-f` | 기존 파일 또는 모듈 디렉터리를 덮어씁니다. 이 플래그가 없으면 대상 디렉터리가 있을 때 생성이 실패합니다. |

## 템플릿 토큰

프로젝트 템플릿은 경로와 텍스트 파일에서 다음 토큰을 치환합니다.

| Token | 값 |
| --- | --- |
| `__PROJECT_NAME__` | 전체 프로젝트 이름. 예: `MyCompany.Tools`. |
| `__PROJECT_SHORT_NAME__` | 마지막 점 구분 세그먼트. 예: `Tools`. |
| `__PROJECT_HOST__` | URI host로 사용하는 소문자 짧은 이름. 예: `tools`. |
| `__PACKAGE_VERSION__` | `--package-version` 또는 CLI 제품 버전으로 선택된 패키지 버전. |

텍스트 템플릿 파일은 UTF-8 with BOM으로 작성되고 CRLF 줄 끝으로 정규화됩니다.

## 템플릿 선택

| 상황 | 권장 명령 |
| --- | --- |
| 하나의 뷰가 있는 가장 빠른 Host가 필요합니다. | `zyc new MyCompany.Tools` |
| 새 앱에 모듈 스타일 솔루션이 필요합니다. | `zyc new MyCompany.Tools --template modular` |
| 기존 저장소에 모듈을 추가합니다. | `zyc new-module Reports --src-root ./src` |
| 모듈 프로젝트를 기존 `.slnx`에 추가해야 합니다. | `zyc new-module Reports --src-root ./src --slnx ./ZYC.Framework.slnx` |
