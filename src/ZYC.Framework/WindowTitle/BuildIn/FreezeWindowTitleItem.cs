using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Commands;
using ZYC.Framework.Core.WindowTitle;

namespace ZYC.Framework.WindowTitle.BuildIn;

[RegisterSingleInstance]
internal class FreezeWindowTitleItem : WindowTitleItem
{
    public FreezeWindowTitleItem(
        FreezeWindowCommand freezeWindowCommand) : base("Snowflake", null!)
    {
        FreezeWindowCommand = freezeWindowCommand;
    }

    private FreezeWindowCommand FreezeWindowCommand { get; }

    public override ICommand Command => FreezeWindowCommand;
}