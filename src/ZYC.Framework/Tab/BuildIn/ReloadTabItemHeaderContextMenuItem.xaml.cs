using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Tab.BuildIn;

[RegisterAs(typeof(ITabItemHeaderContextMenuItemView))]
internal partial class ReloadTabItemHeaderContextMenuItem : ITabItemHeaderContextMenuItemView
{
    public int Order => 10;

    protected override void InternalOnMenuItemBaseLoaded()
    {
        InitializeComponent();
    }
}