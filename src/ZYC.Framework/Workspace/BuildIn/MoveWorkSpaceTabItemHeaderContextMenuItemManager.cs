using System.Diagnostics;
using Microsoft.Xaml.Behaviors.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core.Localizations;

namespace ZYC.Framework.Workspace.BuildIn;

[RegisterSingleInstanceAs(typeof(IMoveWorkSpaceTabItemHeaderContextMenuItemManager))]
internal class MoveWorkSpaceTabItemHeaderContextMenuItemManager : IMoveWorkSpaceTabItemHeaderContextMenuItemManager
{
    public MoveWorkSpaceTabItemHeaderContextMenuItemManager(
        ITabManager tabManager,
        IParallelWorkspaceManager parallelWorkspaceManager)
    {
        TabManager = tabManager;
        ParallelWorkspaceManager = parallelWorkspaceManager;
    }

    private ITabManager TabManager { get; }

    private IParallelWorkspaceManager ParallelWorkspaceManager { get; }


    public MoveWorkSpaceTabItemHeaderContextMenuSubItem[] GetSubItems(ITabItemInstance instance)
    {
        var currentWorkspace = TabManager.GetTabItemInstanceWorkspace(instance);

        var moveAvailableWorkspaces =
            ParallelWorkspaceManager.GetWorkspaceDictionary().Values.Where(t => t != currentWorkspace).ToArray();

        var moveWorkSpaceTabItemHeaderContextMenuSubItems = new List<MoveWorkSpaceTabItemHeaderContextMenuSubItem>();

        foreach (var toWorkspace in moveAvailableWorkspaces)
        {
            var title = $"{L.Translate("Workspace")} {toWorkspace.Index.ToString()}";
            var item = new MoveWorkSpaceTabItemHeaderContextMenuSubItem(
                toWorkspace,
                title,
                new ActionCommand(() =>
                {
                    TabManager.MoveTabItemInstance(
                        instance,
                        currentWorkspace.Id,
                        toWorkspace.Id);
                }));

            moveWorkSpaceTabItemHeaderContextMenuSubItems.Add(item);
        }


        moveWorkSpaceTabItemHeaderContextMenuSubItems.Add(new MoveToNewWorkSpaceTabItemHeaderContextMenuSubItem(
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

        moveWorkSpaceTabItemHeaderContextMenuSubItems.Add(new MoveToNewWorkSpaceTabItemHeaderContextMenuSubItem(
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

        return moveWorkSpaceTabItemHeaderContextMenuSubItems.ToArray();
    }
}