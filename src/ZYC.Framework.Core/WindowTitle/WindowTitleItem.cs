using System.Windows.Input;
using ZYC.Framework.Abstractions.WindowTitle;

namespace ZYC.Framework.Core.WindowTitle;

public class WindowTitleItem : IWindowTitleItem
{
    public WindowTitleItem(string icon, ICommand command, string description = "")
    {
        Icon = icon;
        Command = command;
        Description = description;
    }

    public virtual string Description { get; }

    public virtual string Icon { get; }

    public virtual ICommand Command { get; }

    public virtual bool IsVisible => true;
}