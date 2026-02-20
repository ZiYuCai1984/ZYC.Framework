using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.WindowTitle;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.WindowTitle.BuildIn;

// ReSharper disable once HeuristicUnreachableCode
#pragma warning disable CS0162 // Unreachable code detected

[RegisterSingleInstance]
internal class RestartTitleItem : IWindowTitleItem
{
    public RestartTitleItem(
        RestartCommand restartCommand,
        AppConfig appConfig)
    {
        RestartCommand = restartCommand;
        AppConfig = appConfig;
    }

    private RestartCommand RestartCommand { get; }

    private AppConfig AppConfig { get; }

    public string Icon => "Restart";

    public ICommand Command => RestartCommand;

    public bool IsVisible
    {
        get
        {
#if DEBUG
            return true;
#endif

            return AppConfig.DebugMode;
        }
    }
}