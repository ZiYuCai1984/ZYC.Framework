using System.Windows.Input;
using ZYC.Framework.Abstractions.MainMenu;

namespace ZYC.Framework.Modules.Accounts;

internal class AccountsMenuItem : MainMenuItem
{
    public AccountsMenuItem(string title, string? icon, ICommand command, object? commandParameter)
    {
        Info = new MenuItemInfo
        {
            Title = title,
            Icon = icon,
            Localization = false
        };
        Command = command;
        CommandParameter = commandParameter;
    }

    public object? CommandParameter { get; set; }
}