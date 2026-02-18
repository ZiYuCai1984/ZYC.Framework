using System.Windows.Input;
using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Mock.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Mock;

[Register]
internal class MockMainMenuItem : IMainMenuItem
{
    public MockMainMenuItem(MockTabItemInfo mockTabItemInfo, ILifetimeScope lifetimeScope)
    {
        MockTabItemInfo = mockTabItemInfo;

        Command = lifetimeScope.CreateNavigateCommand(
            MockTabItemInfo.Uri);
    }

    private MockTabItemInfo MockTabItemInfo { get; }

    public ICommand Command { get; }

    public IMainMenuItem[] SubItems { get; } = [];

    public string Title => MockTabItemInfo.Title;

    public string Icon => MockTabItemInfo.Icon;

    public string Anchor => "";

    public int Priority => 0;

    public bool Localization => false;

    public bool IsHidden => false;
}