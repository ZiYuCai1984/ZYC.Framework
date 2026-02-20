using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Core.WindowTitle;

namespace ZYC.Framework.WindowTitle.BuildIn;

[RegisterSingleInstance]
internal class MinimizeTitleItem : WindowTitleItem
{
    public MinimizeTitleItem(MinimizeCommand minimizeCommand) : base("WindowMinimize", null!)
    {
        MinimizeCommand = minimizeCommand;
    }

    private MinimizeCommand MinimizeCommand { get; }

    public override ICommand Command => MinimizeCommand;
}