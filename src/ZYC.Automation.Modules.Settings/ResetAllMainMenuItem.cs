using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Modules.Settings.Abstractions;
using ZYC.Automation.Modules.Settings.Commands;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Settings;

[RegisterSingleInstance]
internal class ResetAllMainMenuItem : MainMenuItem
{
    public ResetAllMainMenuItem(ResetAllCommand resetAllCommand)
    {
        Command = resetAllCommand;
        Info = new MenuItemInfo
        {
            Icon = "RestoreAlert",
            Title = "ResetAll",
            Anchor = SettingMainMenuAnchors.Other,
            Priority = 10
        };
    }
}