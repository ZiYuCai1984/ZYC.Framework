using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Commands;
using ZYC.Framework.Core.Menu;

namespace ZYC.Framework.MainMenu.BuildIn;

[RegisterSingleInstanceAs(typeof(IViewMainMenuItemsProvider))]
internal class ViewMainMenuItemsProvider : MainMenuItemsProvider, IViewMainMenuItemsProvider
{
    public ViewMainMenuItemsProvider(
        ILifetimeScope lifetimeScope,
        ApplyCustomLayoutMainMenuItemsProvider applyCustomLayoutMainMenuItemsProvider,
        MatrixLayoutCommand matrixLayoutCommand,
        ManageCustomLayoutsCommand manageCustomLayoutsCommand,
        ResetLayoutCommand resetLayoutCommand,
        SaveCustomLayoutCommand saveCustomLayoutCommand,
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
                "BorderVertical", leftRightLayoutCommand, priority: 10));
        RegisterSubItem(new MainMenuItem(
            "Top-Bottom Layout",
            "BorderHorizontal", topBottomLayoutCommand, priority: 20));
        RegisterSubItem(new MainMenuItem(
            "Matrix Layout",
            "BorderInside", matrixLayoutCommand, priority: 30));

        RegisterSubItem(new MainMenuItem(
            "Save To Custom Layout",
            null, saveCustomLayoutCommand, priority: 40));

        RegisterSubItem(new MainMenuItem(
            "Manage Custom Layouts",
            null, manageCustomLayoutsCommand, priority: 50));

        RegisterSubItem(applyCustomLayoutMainMenuItemsProvider);

        RegisterSubItem(new MainMenuItem(
            "Reset Layout",
            "BorderNoneVariant", resetLayoutCommand, priority: 70));
    }

    public override MenuItemInfo Info { get; }
}