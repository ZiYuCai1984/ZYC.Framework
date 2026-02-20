using Autofac;
using MahApps.Metro.IconPacks;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Commands;

namespace ZYC.Framework.Workspace;

[RegisterSingleInstanceAs(typeof(IWorkspaceMenuManager))]
internal class WorkspaceMenuManager : IWorkspaceMenuManager
{
    public WorkspaceMenuManager(
        ILifetimeScope lifetimeScope,
        ToggleOrientationCommand toggleOrientationCommand,
        SwapCommand swapCommand,
        ResetCommand resetCommand,
        SplitVerticalCommand splitVerticalCommand,
        MergeCommand mergeCommand,
        SetFocusedWorkspaceCommand focusedWorkspaceCommand,
        SplitHorizontalCommand splitHorizontalCommand)
    {
        LifetimeScope = lifetimeScope;
        WorkspaceMenuItems.Add(
            new WorkspaceMenuItem(
                "Reset",
                resetCommand,
                nameof(PackIconMaterialKind.Reload)));

        WorkspaceMenuItems.Add(
            new WorkspaceMenuItem(
                "ToggleOrientation",
                toggleOrientationCommand,
                nameof(PackIconMaterialKind.OrbitVariant)));

        WorkspaceMenuItems.Add(
            new WorkspaceMenuItem(
                "Swap",
                swapCommand,
                nameof(PackIconMaterialKind.SwapHorizontal)));

        WorkspaceMenuItems.Add(
            new WorkspaceMenuItem(
                "Merge",
                mergeCommand,
                nameof(PackIconMaterialKind.FlipToFront)));

        WorkspaceMenuItems.Add(
            new WorkspaceMenuItem(
                "SplitVertical",
                splitVerticalCommand,
                nameof(PackIconMaterialKind.FlipVertical)));

        WorkspaceMenuItems.Add(
            new WorkspaceMenuItem(
                "SplitHorizontal",
                splitHorizontalCommand,
                nameof(PackIconMaterialKind.FlipHorizontal)));

        WorkspaceMenuItems.Add(
            new WorkspaceMenuItem(
                "Focus",
                focusedWorkspaceCommand,
                nameof(PackIconMaterialKind.ImageFilterCenterFocus)));
    }

    private ILifetimeScope LifetimeScope { get; }

    private List<IWorkspaceMenuItem> WorkspaceMenuItems { get; } = new();

    public void RegisterItem(IWorkspaceMenuItem item)
    {
        WorkspaceMenuItems.Add(item);
    }

    public void RegisterItem<T>() where T : IWorkspaceMenuItem
    {
        var item = LifetimeScope.Resolve<T>();
        WorkspaceMenuItems.Add(item);
    }

    public IWorkspaceMenuItem[] GetItems()
    {
        return WorkspaceMenuItems
            .ToArray();
    }
}