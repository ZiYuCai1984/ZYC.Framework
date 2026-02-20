using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Core.WindowTitle;

namespace ZYC.Framework.WindowTitle.BuildIn;

[RegisterSingleInstance]
public class ExitProcessTitleItem : WindowTitleItem
{
    public ExitProcessTitleItem(
        ExitProcessCommand exitProcessCommand) : base("Close", null!)
    {
        ExitProcessCommand = exitProcessCommand;
    }

    private ExitProcessCommand ExitProcessCommand { get; }

    public override ICommand Command => ExitProcessCommand;
}