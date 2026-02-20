<p align="center">
  <a href="./README.md">English</a> |
  <a href="./README.ja.md">日本語</a> |
  <a href="./README.zh-CN.md">简体中文</a> |
  <a href="./README.zh-TW.md">简體中文</a> |
  <a href="./README.ko.md">한국어</a> |
</p>

<p align="center">
  <img src="./docs/images/app.png" alt="ZYC.Framework Logo" width="120" />
</p>

<h1 align="center">ZYC.Framework</h1>

<p align="center">
  <b>.NET 10</b>과 <b>WPF</b>를 기반으로 구축된 고성능, 모듈화, 확장 가능한 데스크톱 자동화 프레임워크입니다.
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/ZYC.Framework.Alpha">
    <img src="https://img.shields.io/nuget/v/ZYC.Framework.Alpha?include_prereleases=true&logo=nuget" alt="NuGet Version" />
  </a>
  <a href="https://www.nuget.org/packages/ZYC.Framework.Alpha">
    <img src="https://img.shields.io/nuget/dt/ZYC.Framework.Alpha?logo=nuget&label=Downloads" alt="NuGet Downloads" />
  </a>
  <a href="https://raw.githubusercontent.com/ZiYuCai1984/Temp/refs/heads/main/ZYC.Framework.Setup.exe">
    <img src="https://img.shields.io/badge/Download-Setup-blue?logo=windows&logoColor=white&label=Download%20Demo%20Installer" alt="Download Demo Installer" />
  </a>
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 10" />
  <img src="https://img.shields.io/badge/Platform-WPF-orange" alt="Platform" />
  <img src="https://img.shields.io/badge/License-MIT-green" alt="License" />
</p>

<p align="center">
  <a href="https://github.com/ZiYuCai1984/ZYC.Framework/actions/workflows/publish-nuget-manual.yml">
    <img src="https://img.shields.io/github/actions/workflow/status/ZiYuCai1984/ZYC.Framework/publish-nuget-manual.yml?branch=main&label=build&logo=github" alt="NuGet manual workflow" />
  </a>
  <a href="https://github.com/ZiYuCai1984/ZYC.Framework/actions/workflows/publish-nuget-nightly.yml">
    <img src="https://img.shields.io/github/actions/workflow/status/ZiYuCai1984/ZYC.Framework/publish-nuget-nightly.yml?branch=main&label=nightly%20build&logo=github" alt="NuGet nightly workflow" />
  </a>
</p>

---

## 📖 개요

**ZYC.Framework**은 **WPF**의 표현력 있는 UI 기능과 **.NET 10**의 최신 기능을 결합한 현대적인 데스크톱 자동화 솔루션입니다.  
모듈화된 아키텍처를 통해 복잡한 자동화 시스템의 개발과 유지보수를 단순화하는 것을 목표로 합니다.

본 프로젝트는 분산 애플리케이션 오케스트레이션을 위해 **.NET Aspire**를 깊이 통합하고 있으며,  
**Blazor** 및 **WebView2** 기반의 하이브리드 접근 방식을 지원하여 Web UI와 네이티브 데스크톱 경험 중 필요에 따라 선택할 수 있습니다.

---

## ✨ 주요 기능

- **모듈화 아키텍처**: 비즈니스 로직을 분리하여 동적 로딩과 독립적인 개발을 지원합니다.
- **현대적인 UI 경험**: WPF 기반으로 **다중 워크스페이스** 및 **다중 탭** 인터랙션을 지원합니다.
- **하이브리드 개발**:
  - **WebView2**를 통한 최신 Web 애플리케이션 임베딩
  - **Blazor**를 활용한 데스크톱 환경에서의 Web 컴포넌트 재사용
- **클라우드 네이티브 대응**: **.NET Aspire** 내장 지원으로 서비스 탐색, 거버넌스 및 배포를 단순화
- **엔터프라이즈 지향 기능 내장 (Batteries Included)**:
  - **작업 관리**: 작업 스케줄링 및 전체 라이프사이클 관리
  - **예외 처리**: 강력한 전역 오류 수집 및 진단 메커니즘
  - **다국어 지원**: 글로벌 애플리케이션을 위한 내장 로컬라이제이션 기능

---

## 📸 UI 미리보기

<table align="center">
  <tr>
    <td>
      <img src="./docs/images/workspace.png" alt="workspace" width="400" />
      <p align="center">워크스페이스 뷰</p>
    </td>
    <td>
      <img src="./docs/images/multiple-tabs.png" alt="multiple-tabs" width="400" />
      <p align="center">다중 탭</p>
    </td>
  </tr>

  <tr>
    <td>
      <img src="./docs/images/workspace-4.png" alt="workspace-4" width="400" />
      <p align="center">다중 워크스페이스</p>
    </td>
    <td>
      <img src="./docs/images/workspace-4-tabs.png" alt="workspace-4-tabs" width="400" />
      <p align="center">워크스페이스 + 탭</p>
    </td>
  </tr>

  <tr>
    <td>
      <img src="./docs/images/aspire-dashboard.png" alt="aspire-dashboard" width="400" />
      <p align="center">Aspire 대시보드</p>
    </td>
    <td>
      <img src="./docs/images/blazor-auth.png" alt="blazor-auth" width="400" />
      <p align="center">Blazor (인증 포함)</p>
    </td>
  </tr>

  <tr>
    <td>
      <img src="./docs/images/exception.png" alt="exception" width="400" />
      <p align="center">예외 처리</p>
    </td>
    <td>
      <img src="./docs/images/taskmanager.png" alt="taskmanager" width="400" />
      <p align="center">작업 관리자</p>
    </td>
  </tr>
</table>

---

## 🛠️ 기술 스택

- **런타임**: .NET 10 SDK
- **UI 프레임워크**: WPF (Windows Presentation Foundation)
- **하이브리드 UI**: WebView2 + Blazor (Web UI)
- **오케스트레이션**: .NET Aspire
- **아키텍처**: 모듈형 모놀리식 / 플러그인 기반

---

## 🚀 빠른 시작

자세한 가이드는 아래 문서를 참고하세요:

👉 **[Quick Start (quick-start.md)](docs/quick-start.md)**

### 설치

NuGet을 통해 핵심 패키지를 설치합니다:

```bash
dotnet add package ZYC.Framework.Alpha --version [version]
````

---

## 🏗️ 프로젝트 구조

```text
ZYC.Framework
├── src
│   ├── ZYC.Framework                         # WPF desktop host/entry: main window, workspaces, menus, UI lifecycle
│   ├── ZYC.Framework.Abstractions            # Shared contracts: interfaces, states, configs used across host/modules
│   ├── ZYC.Framework.Core                    # Core infrastructure: commands, bindings, converters, base UI components, i18n
│   ├── ZYC.Framework.MetroWindow             # Metro-style window shell (alternative window implementation)
│   ├── ZYC.Framework.WebView2                # WebView2 hosting layer: navigation, menu bar, interop, page hosting
│   ├── ZYC.Framework.CLI                     # CLI tool: developer utilities, module/file helpers, automation entrypoints
│   ├── ZYC.Framework.Build.*                 # Build & packaging toolchain
│   │   ├── ZYC.Framework.Build.NuGet         # NuGet packaging tool: build/.props/.targets, README, PatchNote, outputs
│   │   ├── ZYC.Framework.Build.InnoSetup     # Inno Setup builder: produces the Windows installer (setup)
│   │   └── ZYC.Framework.Build.NewModule     # Module scaffolder: templates for Module + Abstractions projects
│   ├── ZYC.Framework.Modules.*               # Feature modules
│   │   ├── ZYC.Framework.Modules.About                  # About / version info page (UI + tab)
│   │   ├── ZYC.Framework.Modules.Aspire                 # .NET Aspire AppHost/orchestration integration + dashboard
│   │   ├── ZYC.Framework.Modules.BlazorDemo             # Blazor Server demo: web UI + auth/integration showcase
│   │   ├── ZYC.Framework.Modules.CLI                    # In-app CLI module (terminal-like tools page)
│   │   ├── ZYC.Framework.Modules.FileExplorer           # File Explorer module (Explorer-like browsing in tabs)
│   │   ├── ZYC.Framework.Modules.Language               # Language/i18n module: language switching + resources config
│   │   ├── ZYC.Framework.Modules.Log                    # Logging module: view logs, open log folder, log plumbing
│   │   ├── ZYC.Framework.Modules.Mock                   # Test/demo module for validating UI/notifications/tasks/workspaces
│   │   ├── ZYC.Framework.Modules.ModuleManager          # Module manager: enable/disable/install/uninstall (local + NuGet)
│   │   ├── ZYC.Framework.Modules.NuGet                  # NuGet access layer: sources, metadata, version utilities
│   │   ├── ZYC.Framework.Modules.Secrets                # Secrets utilities: password generator, Wi-Fi password, secrets UI
│   │   ├── ZYC.Framework.Modules.Settings               # Settings module: settings UI, grouping, reset actions
│   │   ├── ZYC.Framework.Modules.TaskManager            # Task manager: queue/progress/pause/cancel/cleanup framework
│   │   ├── ZYC.Framework.Modules.Translator             # Translator module: integrates translation services/local runner
│   │   ├── ZYC.Framework.Modules.Update                 # Update module: check/download/apply+restart, fault handling UI
│   │   └── ZYC.Framework.Modules.WebBrowser             # Built-in web browser module (tab-hosted browsing)
│   └── Thirdparty                            # Integrated third-party components (vendored/forked)
│       ├── ZYC.Terminal                      # Terminal/ConPTY integration: pseudo console, PTY, process + pipes
│       ├── ZYC.MdXaml                        # Markdown renderer + extensions
│       └── ZYC.Titanium.Web.Proxy            # HTTP(S) proxy core (integrated Titanium Web Proxy fork)
```

---

## 📄 라이선스

본 프로젝트는 [MIT License](LICENSE) 하에 오픈소스로 제공됩니다.

---

## 💖 감사의 말

본 프로젝트는 다음 오픈소스 프로젝트를 사용하거나(또는 일부 구현을 참고하였습니다):

* [MahApps.Metro](https://github.com/MahApps/MahApps.Metro): UI 프레임워크
* [MdXaml](https://github.com/whistyun/MdXaml): Markdown 렌더링
* [titanium-web-proxy](https://github.com/justcoding121/titanium-web-proxy): 프록시 코어
* [EasyWindowsTerminalControl](https://github.com/mitchcapper/EasyWindowsTerminalControl): 터미널 통합

> 라이선스 및 저작권은 각 프로젝트의 원저작자에게 귀속됩니다.
> 본 저장소는 각 프로젝트의 라이선스 조건을 준수하여 사용 및 참조합니다.

---

## 🤝 기여하기

Issue 및 Pull Request를 환영합니다.
제안 사항이나 버그를 발견하셨다면 언제든지 Issue를 열거나 PR을 제출해 주세요.

```

