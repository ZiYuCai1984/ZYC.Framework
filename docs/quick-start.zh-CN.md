<p align="center">
  <a href="./quick-start.md">English</a> |
  <a href="./quick-start.ja.md">日本語</a> |
  <a href="./quick-start.zh-CN.md">简体中文</a> |
  <a href="./quick-start.zh-TW.md">繁體中文</a> |
  <a href="./quick-start.ko.md">한국어</a> |
</p>


# 🚀 快速开始：创建你的第一个 ZYC.Framework Host

本指南将带你从零开始创建一个 **ZYC.Framework Host** 项目。你将学习如何通过 NuGet 集成框架，并使用模块化机制（Module + UserControl）将自定义 UI 挂载到宿主环境中。🛠️

---

## 推荐方式：通过 dotnet tool 创建

安装或更新 ZYC.Framework CLI 工具：

```bash
dotnet tool install --global ZYC.Framework.CLI --version 1.4.1
# 如果已经安装过：
dotnet tool update --global ZYC.Framework.CLI --version 1.4.1
```

创建一个最小 Host 项目：

```bash
zyc new WpfApp1
```

默认项目模板是 `minimal`，它会生成与下面“手动创建方法”等价的 Host 结构。你也可以显式指定模板：

```bash
zyc new WpfApp1 --template minimal
```

如果你需要一个包含 `Entry` 项目、模块项目和 `Abstractions` 项目的解决方案，可以使用 `modular`：

```bash
zyc new MyCompany.Tools --template modular
```

常用选项：

```bash
zyc new MyCompany.Tools --output ./MyCompany.Tools --package-version 1.4.1
```

打开生成的解决方案或项目，将其设置为启动项目，然后开始调试。生成结果已经包含包引用、`Module.cs`、`ModuleConfig.json` 和初始视图。

---

## 手动创建方法

如果你希望手动创建 Host 项目，可以按下面的等价步骤操作。

### 1. 🧱 项目准备与前置条件

1. **创建项目**：创建一个以 **.NET 10** 为目标框架的 **WPF Application**（例如命名为 `WpfApp1`）。✨
2. **添加 NuGet 包**：通过 NuGet 包管理器安装核心包 `ZYC.Framework.Alpha`。📦

```xml
<ItemGroup>
  <PackageReference Include="ZYC.Framework.Alpha" Version="1.4.1" />
</ItemGroup>
```

3. **清理默认入口点**：🧹
框架提供了自己的统一入口点（`Entry.cs`）。你 **必须删除** 模板默认生成的以下文件：
* `App.xaml`
* `App.xaml.cs`

> [!IMPORTANT]
> ⚠️ **关键步骤**：你必须删除 `App.xaml`，否则会产生全局入口点冲突。应用程序的启动逻辑将完全由框架接管。

---

### 2. ⚙️ 配置程序集引用

为了让宿主正确识别并加载抽象接口，请在 `.csproj` 文件中手动添加对 `Abstractions` 程序集的引用。🔗

```xml
<ItemGroup>
  <Reference Include="ZYC.Framework.Abstractions">
    <HintPath>$(OutputPath)ZYC.Framework.Abstractions.dll</HintPath>
  </Reference>
</ItemGroup>
```

---

### 3. 🛠️ 实现业务模块 (`Module.cs`)

在项目根目录下创建一个 `Module.cs` 文件。这个类相当于模块的“大脑”，负责定义加载逻辑，并向宿主注册 UI 页面。🧠

```csharp
using Autofac;
using ZYC.Framework.Abstractions.Tab;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac;

namespace WpfApp1;

internal class Module : ModuleBase
{
    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        // 可选：挂载内置调试工具
        DebuggerTools.Attach();

        // 解析 Tab Manager 并注册你的 UI 组件
        var simpleTabItemFactoryManager = lifetimeScope.Resolve<ISimpleTabItemFactoryManager>();
        simpleTabItemFactoryManager.Register(new SimpleTabItemFactoryInfo(typeof(UserControl1)));

        return base.LoadAsync(lifetimeScope);
    }
}
```

---

### 4. 🎨 创建 UI 组件

创建一个新的 `UserControl1`（WPF User Control），并添加 `[Register]` 特性。这样框架的依赖注入（DI）容器就能自动识别并管理它。🖥️

```csharp
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace WpfApp1;

[Register] // 自动注册到 DI 容器
public partial class UserControl1
{
    public UserControl1()
    {
        InitializeComponent();
    }
}
```

---

### 5. 📄 添加模块配置文件

在项目根目录创建一个 `ModuleConfig.json` 文件。这个文件相当于宿主的“地图”，用于告诉宿主要动态加载哪些程序集。同时需要将它配置为相对于主程序生成到 `../settings/ModuleConfig.json`。⚙️

1. **文件内容**：
```json
{
  "AdditionalAssemblyNames": [
    "WpfApp1.dll"
  ],
  "DisabledAssemblyNames": []
}
```

2. **项目项设置**：📌
在 `.csproj` 文件中加入以下配置，让 `ModuleConfig.json` 在构建时生成到 `../settings/ModuleConfig.json`：

```xml
<ItemGroup>
  <None Update="ModuleConfig.json">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    <Link>../settings/ModuleConfig.json</Link>
  </None>
</ItemGroup>
```

> 💡 **提示**：`AdditionalAssemblyNames` 必须包含 `Module.cs` 所在程序集的名称。

---

### 6. ▶️ 运行与调试

1. **设置启动项目**：将这个 WPF 项目设置为解决方案的 **Startup Project**。
2. **开始调试**：按下 `F5`。

🎉 **预期结果**：
宿主将启动，扫描 `ModuleConfig.json`，并加载 `WpfApp1` 模块。你注册的 `UserControl1` 页面会自动作为一个新标签页出现在主界面中。

---

![quick-start-ui.png](./images/quick-start-ui.png)
