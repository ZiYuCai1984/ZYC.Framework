using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Commands;
using ZYC.Automation.Core.Menu;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.MainMenu.BuildIn;

[RegisterSingleInstanceAs(typeof(IViewMainMenuItemsProvider))]
internal class ViewMainMenuItemsProvider : MainMenuItemsProvider, IViewMainMenuItemsProvider
{
    public ViewMainMenuItemsProvider(
        ILifetimeScope lifetimeScope,
        MatrixLayoutCommand matrixLayoutCommand,
        ResetLayoutCommand resetLayoutCommand,
        TopBottomLayoutCommand topBottomLayoutCommand,
        LeftRightLayoutCommand leftRightLayoutCommand) : base(lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = "View",
            Icon = null,
            Priority = MainMenuPriority.View
        };

        RegisterSubItem(
            new MainMenuItem(
                "Left-Right Layout",
                "BorderVertical", leftRightLayoutCommand));
        RegisterSubItem(new MainMenuItem(
            "Top-Bottom Layout",
            "BorderHorizontal", topBottomLayoutCommand));
        RegisterSubItem(new MainMenuItem(
            "Matrix Layout",
            "BorderInside", matrixLayoutCommand));
        RegisterSubItem(new MainMenuItem(
            "Reset Layout",
            "BorderNoneVariant", resetLayoutCommand));
    }

    public override MenuItemInfo Info { get; }
}