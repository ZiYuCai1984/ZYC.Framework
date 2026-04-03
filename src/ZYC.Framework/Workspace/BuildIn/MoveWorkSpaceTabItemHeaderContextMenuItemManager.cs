using System.Diagnostics;
using Microsoft.Xaml.Behaviors.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core.Localizations;

namespace ZYC.Framework.Workspace.BuildIn;

[RegisterSingleInstanceAs(typeof(IMoveWorkspaceTabItemHeaderContextMenuItemManager))]
internal class MoveWorkspaceTabItemHeaderContextMenuItemManager : IMoveWorkspaceTabItemHeaderContextMenuItemManager
{
    public MoveWorkspaceTabItemHeaderContextMenuItemManager(
        ITabManager tabManager,
        IParallelWorkspaceManager parallelWorkspaceManager)
    {
        TabManager = tabManager;
        ParallelWorkspaceManager = parallelWorkspaceManager;
    }

    private ITabManager TabManager { get; }

    private IParallelWorkspaceManager ParallelWorkspaceManager { get; }


    public MoveWorkspaceTabItemHeaderContextMenuSubItem[] GetSubItems(ITabItemInstance instance)
    {
        var currentWorkspace = TabManager.GetTabItemInstanceWorkspace(instance);

        var moveAvailableWorkspaces =
            ParallelWorkspaceManager.GetWorkspaceDictionary().Values.Where(t => t != currentWorkspace).ToArray();

        var moveWorkspaceTabItemHeaderContextMenuSubItems = new List<MoveWorkspaceTabItemHeaderContextMenuSubItem>();

        foreach (var toWorkspace in moveAvailableWorkspaces)
        {
            var title = $"{L.Translate("Workspace")} {toWorkspace.Index.ToString()}";
            var item = new MoveWorkspaceTabItemHeaderContextMenuSubItem(
                toWorkspace,
                title,
                new ActionCommand(() =>
                {
                    TabManager.MoveTabItemInstance(
                        instance,
                        currentWorkspace.Id,
                        toWorkspace.Id);
                }));

            moveWorkspaceTabItemHeaderContextMenuSubItems.Add(item);
        }


        moveWorkspaceTabItemHeaderContextMenuSubItems.Add(new MoveToNewWorkspaceTabItemHeaderContextMenuSubItem(
            L.Translate("New Horizontal"), new ActionCommand(
                // ReSharper disable AsyncVoidLambda
                async () =>
                {
                    await ParallelWorkspaceManager.SplitHorizontalAsync(currentWorkspace);

                    var toWorkspace = currentWorkspace.Right;
                    if (toWorkspace == null)
                    {
                        Debugger.Break();
                        return;
                    }

                    var fromWorkspace = TabManager.GetTabItemInstanceWorkspace(instance);
                    TabManager.MoveTabItemInstance(
                        instance,
                        fromWorkspace.Id,
                        toWorkspace.Id);
                })));

        moveWorkspaceTabItemHeaderContextMenuSubItems.Add(new MoveToNewWorkspaceTabItemHeaderContextMenuSubItem(
            L.Translate("New Vertical"), new ActionCommand(
                // ReSharper disable AsyncVoidLambda
                async () =>
                {
                    await ParallelWorkspaceManager.SplitVerticalAsync(currentWorkspace);

                    var toWorkspace = currentWorkspace.Right;
                    if (toWorkspace == null)
                    {
                        Debugger.Break();
                        return;
                    }

                    var fromWorkspace = TabManager.GetTabItemInstanceWorkspace(instance);
                    TabManager.MoveTabItemInstance(
                        instance,
                        fromWorkspace.Id,
                        toWorkspace.Id);
                })));

        return moveWorkspaceTabItemHeaderContextMenuSubItems.ToArray();
    }
}