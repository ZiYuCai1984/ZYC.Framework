using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.DragDrop;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.DragDrop.BuildIn;

[RegisterSingleInstanceAs(typeof(TabManagerDropProvider), typeof(IDropActionProvider), PreserveExistingDefaults = true)]
internal class TabManagerDropProvider : IDropActionProvider
{
    public TabManagerDropProvider(ITabManager tabManager, ILogger<TabManagerDropProvider> logger)
    {
        TabManager = tabManager;
        Logger = logger;
    }

    private ITabManager TabManager { get; }

    private ILogger<TabManagerDropProvider> Logger { get; }

    public static string TabItemInstanceKey => $"{nameof(TabManagerDropProvider)}.{nameof(ITabItemInstance)}";

    public async Task<DropAction[]> GetActionsAsync(DropPayload payload, DropContext context)
    {
        await Task.CompletedTask;

        var dropActions = new List<DropAction>();

        if (payload.Extras.ContainsKey(TabItemInstanceKey))
        {
            var instance = (ITabItemInstance)payload.Extras[TabItemInstanceKey]!;
            var fromWorkspace = TabManager.GetTabItemInstanceWorkspace(instance);

            dropActions.Add(new DropAction("TabItem",
                "Move tab item", 0,
                () => true,
                async _ =>
                {
                    await Task.CompletedTask;
                    try
                    {
                        if (fromWorkspace.Id == context.WorkspaceId)
                        {
                            return;
                        }

                        Debug.WriteLine($"Drag move {fromWorkspace.Id} -> {context.WorkspaceId}");

                        TabManager.MoveTabItemInstance(instance, fromWorkspace.Id, context.WorkspaceId);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                }));
        }

        return dropActions.ToArray();
    }
}