<p align="center">
  <a href="./module-development.md">English</a> |
  <a href="./module-development.ja.md">日本語</a> |
  <a href="./module-development.zh-CN.md">简体中文</a> |
  <a href="./module-development.zh-TW.md">繁體中文</a> |
  <a href="./module-development.ko.md">한국어</a> |
</p>


# 模块开发

这篇文档说明如何编写一个符合 ZYC.Framework Host 运行时模型的模块：以依赖注入为入口，使用 URI 驱动 Tab 导航，按需接入主菜单，并清晰分离公开契约与 WPF 实现。

## 什么时候创建模块

当一个功能需要由 ZYC.Framework Host 发现并加载，而不是直接编译进 Shell 时，就应该做成模块。模块可以贡献视图、Tab 工厂、主菜单项、配置、状态、后台服务和命令行选项。

如果功能契约需要被其他模块复用，请创建 `*.Abstractions` 项目。公开 DTO、常量、服务接口、菜单 Provider 接口放在这里。WPF 视图、TabItem 实现和运行时注册逻辑放在实现项目。

## 推荐项目结构

| 区域 | 常见文件 | 职责 |
| --- | --- | --- |
| Abstractions | `*ModuleConstants.cs`, `I*MainMenuItemsProvider.cs`, service interfaces | 其他模块可以引用的公开契约，不依赖 WPF 实现细节。 |
| Implementation | `Module.cs` | 模块生命周期入口。注册工厂、菜单项、Provider 和服务。 |
| Navigation | `*TabItemFactory.cs`, `*TabItem.cs` | 匹配 `zyc://` 或 app URI，并创建 Tab 实例。 |
| Menu | `*MainMenuItem.cs`, optional `*MainMenuItemsProvider.cs` | 添加用户可见命令，用于导航到模块 Tab 或执行模块动作。 |
| UI | `UI/*View.xaml`, `UI/*View.xaml.cs` | TabItem 使用的 WPF 视图。 |

Abstractions 项目不要暴露 WPF 控件类型。如果需要命令契约，可以在 Abstractions 中使用 `System.Windows.Input.ICommand`。

## 生命周期

| 方法 | 时机 | 用途 |
| --- | --- | --- |
| `RegisterAsync(ContainerBuilder builder)` | Autofac 容器构建之前。 | 注册必须在整个容器中可用的服务。 |
| `LoadAsync(ILifetimeScope lifetimeScope)` | 容器构建之后，并且模块处于启用状态时。 | 注册 Tab 工厂、主菜单项、状态栏项和运行时 Hook。 |
| `AfterLoadedAsync(ILifetimeScope lifetimeScope)` | 所有启用模块加载完成之后。 | 需要依赖其他模块已经完成注册的跨模块初始化。 |

大多数 UI 模块只需要实现 `LoadAsync`。

## 最小视图模块

对于单视图模块，可以在 `LoadAsync` 中注册 simple tab factory：

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

这是 `minimal` 项目模板使用的模式。当模块只需要把一个 WPF `UserControl` 暴露为 Tab 时，这种方式就足够了。

## 带路由的 Tab 模块

如果模块需要稳定 URI、路由匹配、参数、定制单例行为或多个 Tab，请使用 `TabItemFactoryBase`。

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

`TabItemRouteAttribute` 可以按 `Scheme`、`Host`、`Path` 和 `PathMatch` 匹配 URI。如果多个工厂匹配同一个 URI，`Priority` 更高的工厂会优先。`TabItemFactoryBase` 默认 `IsSingle = true`；如果模块允许打开多个实例，请覆盖它。

## TabItem 与 View

带路由的 Tab 通常用 `TabItemInstanceBase<TView>` 包装 WPF 视图：

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

Tab 生命周期行为放在 TabItem 中，视觉行为放在 View 中。这样可以把路由、Tab 标识和 UI 组合拆开。

## 主菜单入口

菜单项通常导航到模块 URI：

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

然后从模块注册：

```csharp
public override Task LoadAsync(ILifetimeScope lifetimeScope)
{
    lifetimeScope.RegisterTabItemFactory<ReportsTabItemFactory>();
    lifetimeScope.RegisterExtensionsMainMenuItem<ReportsMainMenuItem>();

    return Task.CompletedTask;
}
```

优先使用符合功能位置的现有菜单 Provider，例如 File、View、Tools、Extensions 或 About。只有当功能拥有多个子命令时，才增加模块自己的 Provider。

## 配置与状态

实现 `IConfig` 或 `IState` 的类型会在模块注册期间从 settings 目录读取，并自动注册到 Autofac。用户可编辑设置使用 config；运行时持久化内容，例如选中路径、待处理操作、窗口状态，使用 state。

不要把大型业务数据放进 config/state 类。它们应保持小型、可序列化，并且能承受版本演进。

## 模块加载与依赖

Host 会发现命名类似 `ZYC.Framework.Modules*.dll` 的标准模块程序集，然后追加 `ModuleConfig.AdditionalAssemblyNames` 中列出的程序集。`ModuleConfig.DisabledAssemblyNames` 中的程序集会被发现，但不会作为启用模块加载。

依赖关系通过对其他模块 `*.Abstractions.dll` 的引用推断。如果模块 A 引用了 `ZYC.Framework.Modules.B.Abstractions.dll`，运行时就可以报告 A 依赖 B，而不需要 A 直接引用 B 的 WPF 实现。

## 检查清单

- 公开常量和契约放在 `*.Abstractions`。
- WPF 视图和 TabItem 放在实现项目。
- 只有必须在容器构建前存在的服务才放进 `RegisterAsync`。
- Tab 工厂和菜单项在 `LoadAsync` 中注册。
- 稳定 URI 路由使用 `TabItemRouteAttribute`。
- 添加新菜单 Provider 前，优先使用现有主菜单 Provider。
- config/state 类保持小型且可序列化。
