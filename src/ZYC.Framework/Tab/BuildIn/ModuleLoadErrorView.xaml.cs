using System.Windows;
using System.Windows.Controls;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Core.Page;

namespace ZYC.Framework.Tab.BuildIn;

[Register]
internal partial class ModuleLoadErrorView
{
    public ModuleLoadErrorView(
        ILifetimeScope lifetimeScope,
        IModuleLoadInfoManager moduleLoadInfoManager,
        ErrorViewWrapSwitchCommand errorViewWrapSwitchCommand)
    {
        LifetimeScope = lifetimeScope;
        ModuleLoadInfoManager = moduleLoadInfoManager;
        ErrorViewWrapSwitchCommand = errorViewWrapSwitchCommand;

        InitializeComponent();
    }

    private ILifetimeScope LifetimeScope { get; }

    public IModuleLoadInfoManager ModuleLoadInfoManager { get; }

    public ErrorViewWrapSwitchCommand ErrorViewWrapSwitchCommand { get; }

    protected override void InternalOnLoaded()
    {
        var loadErrorInfos = ModuleLoadInfoManager.GetLoadErrorInfos();
        for (var i = 0; i < loadErrorInfos.Length; ++i)
        {
            var info = loadErrorInfos[i];

            StackPanel.Children.Add(new TextBlock
            {
                Text = $"{i+1}.    {info.ModuleInfo.ModuleAssemblyName}    {info.Function}",
                Margin = new Thickness(0, 8, 0, 8)
            });
            StackPanel.Children.Add(LifetimeScope.Resolve<InnerErrorView>(
                new TypedParameter(typeof(Exception), info.Exception)));
        }
    }
}