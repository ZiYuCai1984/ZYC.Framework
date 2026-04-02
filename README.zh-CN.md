<p align="center">
  <a href="./README.md">English</a> |
  <a href="./README.ja.md">日本語</a> |
  <a href="./README.zh-CN.md">简体中文</a> |
  <a href="./README.zh-TW.md">繁體中文</a> |
  <a href="./README.ko.md">한국어</a> |
</p>

<p align="center">
  <img src="./docs/images/app.png" alt="ZYC.Framework Logo" width="120" />
</p>

<h1 align="center">ZYC.Framework</h1>

<p align="center">


基于 <b>.NET 10</b> 和 <b>WPF</b> 构建的高性能、多模块、可扩展自动化开发框架。

</p>

<p align="center">
  <a href="https://www.nuget.org/packages/ZYC.Framework.Alpha">
    <img src="https://img.shields.io/nuget/v/ZYC.Framework.Alpha?include_prereleases=true&logo=nuget" alt="NuGet Version" />
  </a>
  <a href="https://www.nuget.org/packages/ZYC.Framework.Alpha">
    <img src="https://img.shields.io/nuget/dt/ZYC.Framework.Alpha?logo=nuget&label=Downloads" alt="NuGet Downloads" />
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
    <img src="https://img.shields.io/github/actions/workflow/status/ZiYuCai1984/ZYC.Framework/build-nightly.yml?branch=main&label=nightly%20build&logo=github" alt="Build nightly workflow" />
  </a>
</p>

---



## 📖  项目简介



**ZYC.Framework** 是一个现代化的桌面自动化解决方案。它结合了 WPF 的强大 UI 表现力和 .NET 10 的最新特性，旨在通过模块化架构简化复杂自动化系统的开发流程。




项目深度集成了 **.NET Aspire** 用于分布式应用编排，并支持 **Blazor** 与 **WebView2** 的混合开发模式，让你可以自由选择 Web 或原生技术栈。


---



## ✨  核心特性

- **多模块架构**：解耦业务逻辑，支持动态加载与独立开发。
- **现代化 UI 支持**：基于 WPF，支持多工作区（Multi-Workspace）与多标签页（Multi-Tab）交互。
- **混合开发模式**：
  - 集成 **WebView2**，轻松嵌入现代 Web 应用。
  - 集成 **Blazor**，实现 Web 组件与桌面端逻辑的无缝复用。
- **云原生就绪**：内置对 **.NET Aspire** 的支持，简化服务发现、治理与部署。
- **企业级内置支持**：
  - **任务管理**：任务调度与生命周期管理。
  - **异常处理**：全局错误捕获与日志追溯机制。
  - **本地化框架**：内置多语言支持方案，适应全球化场景。

---





## 🛠️  技术栈

- **Runtime**: .NET 10 SDK
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Hybrid UI**: WebView2 + Blazor
- **Orchestration**: .NET Aspire
- **Architecture**: Modular Monolith / Plugin-based

---



## 🚀  快速开始


请参阅我们准备好的详细指南：


👉 **[快速开始指南 (quick-start.zh-CN.md)](docs/quick-start.zh-CN.md)**


👉 **[下载 Demo 安装包](https://github.com/ZiYuCai1984/ZYC.Framework/releases/download/v1.2.6/ZYC.Framework.Setup.1.2.6.exe)**

### 安装


你可以通过 NuGet 直接将核心包引入你的项目：

```bash
dotnet add package ZYC.Framework.Alpha --version 1.2.6
```

---






## 功能

### 核心框架

| 功能       | 说明                                              |
| -------- | ----------------------------------------------- |
| 模块化架构    | 支持按模块组织功能，并支持动态加载与扩展。                           |
| 多工作区布局   | 支持工作区分割、合并、方向切换和位置交换。                           |
| 多标签页导航   | 支持基于 URI 的标签页导航、恢复和跨工作区移动。                      |
| 可扩展菜单系统  | 提供主菜单、Hamburger 菜单、窗口标题栏、状态栏等扩展点。               |
| 通知系统     | 内置 Toast、Banner 等通知能力，用于状态和错误提示。                |
| 交互辅助能力   | 提供 BusyWindow、Overlay、拖放处理等桌面交互基础设施。            |
| 混合界面支持   | 支持通过 `WebView2` 嵌入 Web 内容，并可结合 `Blazor` 构建混合应用。 |
| 配置与状态持久化 | 支持配置、应用状态和任务记录的本地持久化。                           |
| 单实例运行    | 支持应用单实例启动控制。                                    |
| MCP 暴露机制 | 可将框架和模块中的公开能力自动暴露为 MCP 工具。                      |

### 内置模块

| 模块            | 说明                                   |
| ------------- | ------------------------------------ |
| About         | 显示应用基本信息，如版本、作者和描述。                  |
| ApiReference  | 内置 API 文档查看页，可在应用内浏览生成的文档。           |
| CLI           | 内嵌终端页面，支持命令执行和参数控制。                  |
| FileExplorer  | 内置文件资源管理器视图，支持目录浏览。                  |
| WebBrowser    | 内置浏览器模块，支持访问网页和本地内容。                 |
| Language      | 提供多语言切换和本地化资源管理。                     |
| Translator    | 提供翻译服务接入能力。                          |
| Settings      | 统一的设置管理界面，支持查看、编辑和重置配置。              |
| Secrets       | 提供敏感配置的安全查看与编辑。                      |
| TaskManager   | 支持任务入队、执行、暂停、恢复、取消和状态追踪。             |
| Update        | 基于 NuGet 的版本检查、下载和更新流程。              |
| NuGet         | 提供 NuGet 包查询、下载、依赖处理和缓存管理。           |
| ModuleManager | 管理本地模块和 NuGet 模块的安装、卸载、启用和禁用。        |
| MCP.Server    | 提供本地 MCP Server，可将应用功能对外暴露为工具服务。     |
| Aspire        | 提供 `Aspire` 工具下载、服务控制和 Dashboard 集成。 |
| Log           | 提供日志功能和日志目录访问入口。                     |
| BlazorDemo    | `Blazor Server` 集成示例模块。              |
| Mock          | 用于测试和演示框架能力的模块。                      |

### 开发与交付

| 功能       | 说明                   |
| -------- | -------------------- |
| CLI 工具   | 提供命令行入口，支持模块级命令扩展。   |
| 模块脚手架    | 支持通过模板快速生成新模块及其抽象项目。 |
| 文档生成     | 支持生成 README 和文档模板内容。 |
| NuGet 打包 | 支持框架和模块的 NuGet 打包流程。 |
| 安装包构建    | 支持生成桌面安装包，方便分发与部署。   |





---



## 📸  界面预览

<table align="center">
  <tr>
    <td>
      <img src="./docs/images/workspace.png" alt="workspace" width="400" />
      <p align="center">工作区展示</p>
    </td>
    <td>
      <img src="./docs/images/multiple-tabs.png" alt="multiple-tabs" width="400" />
      <p align="center">多 Tab 展示</p>
    </td>
  </tr>
  <tr>
    <td>
      <img src="./docs/images/workspace-4.png" alt="workspace-4" width="400" />
      <p align="center">多工作区展示</p>
    </td>
    <td>
      <img src="./docs/images/workspace-4-tabs.gif" alt="workspace-4-tabs" width="400" />
      <p align="center">多工作区 Tab 展示</p>
    </td>
  </tr>
  <tr>
    <td>
      <img src="./docs/images/aspire-dashboard.gif" alt="aspire-dashboard" width="400" />
      <p align="center">Aspire 仪表板</p>
    </td>
    <td>
      <img src="./docs/images/blazor-auth.gif" alt="blazor-auth" width="400" />
      <p align="center">Blazor with Auth</p>
    </td>
  </tr>
  <tr>
    <td>
      <img src="./docs/images/exception.png" alt="exception" width="400" />
      <p align="center">异常处理</p>
    </td>
    <td>
      <img src="./docs/images/taskmanager.png" alt="taskmanager" width="400" />
      <p align="center">任务管理</p>
    </td>
  </tr>
</table>

---



## 📄  开源协议


本项目基于 [MIT License](LICENSE) 开源。

---



## 💖  鸣谢


本项目使用了以下开源库/参考了其部分实现：

* [MahApps.Metro](https://github.com/MahApps/MahApps.Metro): UI 框架支持。
* [MdXaml](https://github.com/whistyun/MdXaml): 文档预览支持。
* [titanium-web-proxy](https://github.com/justcoding121/titanium-web-proxy): 网络代理核心。
* [EasyWindowsTerminalControl](https://github.com/mitchcapper/EasyWindowsTerminalControl): 终端集成方案。

> 以上项目的许可证归其原作者所有；
> 本仓库在遵循对应许可证条款的前提下使用/参考其实现。

---



## 🤝  贡献


如果你有任何建议或发现了 Bug，欢迎提交 Issue 或 Pull Request。
