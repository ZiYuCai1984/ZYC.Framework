using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Modules.Settings.Abstractions;
using ZYC.Framework.Modules.Settings.Commands;

namespace ZYC.Framework.Modules.Settings;

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