using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.WindowTitle;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.WindowTitle.BuildIn;

[RegisterSingleInstance]
internal class AttachDebuggerTitleItem : IWindowTitleItem
{
    public AttachDebuggerTitleItem(AttachDebuggerCommand attachDebuggerCommand, AppConfig appConfig)
    {
        AttachDebuggerCommand = attachDebuggerCommand;
        AppConfig = appConfig;
    }

    private AttachDebuggerCommand AttachDebuggerCommand { get; }

    private AppConfig AppConfig { get; }

    public string Icon => "BugStopOutline";

    public ICommand Command => AttachDebuggerCommand;

    public bool IsVisible => AppConfig.GetIsDebugItemVisible();
}