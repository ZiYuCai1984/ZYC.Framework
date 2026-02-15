using ModelContextProtocol.Server;
using System.Text.Json;
using ZYC.Automation.Abstractions.Config.Attributes;
using ZYC.Automation.Abstractions.Tab;
using ZYC.CoreToolkit.Abstractions.Attributes;

namespace ZYC.Automation.Modules.Gemini;

[McpServerToolType]
[TempCode]
public static class TabManagerTools
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

    private static Guid G(string value) => Guid.Parse(value);
    private static Uri U(string value) => new(value, UriKind.RelativeOrAbsolute);

    [McpServerTool, Description("Get navigation state for a workspace.")]
    public static Task<string> GetNavigationState(
        ITabManager tabs,
        IUIDispatcher ui,
        [Description("Workspace id (GUID).")] string workspaceId)
        => ui.InvokeAsync(() =>
        {
            var state = tabs.GetNavigationState(G(workspaceId));
            return JsonSerializer.Serialize(state, JsonOpts);
        });

    [McpServerTool, Description("Get the currently focused tab in the workspace.")]
    public static Task<string> GetFocusedTab(
        ITabManager tabs,
        IUIDispatcher ui,
        TabInstanceRegistry registry,
        [Description("Workspace id (GUID).")] string workspaceId)
        => ui.InvokeAsync(() =>
        {
            var inst = tabs.GetFocusedTabItemInstance(G(workspaceId));
            if (inst is null) return "{}";
            var id = registry.GetOrCreateId(inst);
            return JsonSerializer.Serialize(new { tabInstanceId = id }, JsonOpts);
        });

    [McpServerTool, Description("Set focused tab in the workspace. Pass null to clear focus.")]
    public static Task<string> SetFocusedTab(
        ITabManager tabs,
        IUIDispatcher ui,
        TabInstanceRegistry registry,
        [Description("Workspace id (GUID).")] string workspaceId,
        [Description("Tab instance id (GUID) or null.")] string? tabInstanceId)
        => ui.InvokeAsync(() =>
        {
            ITabItemInstance? inst = null;
            if (!string.IsNullOrWhiteSpace(tabInstanceId) && registry.TryResolve(G(tabInstanceId), out var resolved))
                inst = resolved;

            tabs.SetFocusedTabItemInstance(G(workspaceId), inst);
            return "ok";
        });

    [McpServerTool, Description("List all tab instances in the workspace.")]
    public static Task<string> ListTabs(
        ITabManager tabs,
        IUIDispatcher ui,
        TabInstanceRegistry registry,
        [Description("Workspace id (GUID).")] string workspaceId)
        => ui.InvokeAsync(() =>
        {
            var ws = G(workspaceId);
            var instances = tabs.GetTabItemInstances(ws);

            var result = instances.Select(i => new
            {
                tabInstanceId = registry.GetOrCreateId(i),
                workspaceId = ws
            });

            return JsonSerializer.Serialize(result, JsonOpts);
        });

    [McpServerTool, Description("Get which workspace the specified tab instance belongs to.")]
    public static Task<string> GetTabWorkspace(
        ITabManager tabs,
        IUIDispatcher ui,
        TabInstanceRegistry registry,
        [Description("Tab instance id (GUID).")] string tabInstanceId)
        => ui.InvokeAsync(() =>
        {
            if (!registry.TryResolve(G(tabInstanceId), out var inst))
                return JsonSerializer.Serialize(new { error = "tabInstanceId not found" }, JsonOpts);

            var node = tabs.GetTabItemInstanceWorkspace(inst!);
            return JsonSerializer.Serialize(node, JsonOpts);
        });

    [McpServerTool, Description("Navigate (global).")]
    public static async Task<string> Navigate(
        ITabManager tabs,
        IUIDispatcher ui,
        [Description("Target URI.")] string uri)
    {
        await ui.InvokeAsync(() => tabs.NavigateAsync(U(uri)));
        return "ok";
    }

    [McpServerTool, Description("Navigate inside a specific workspace.")]
    public static async Task<string> NavigateInWorkspace(
        ITabManager tabs,
        IUIDispatcher ui,
        [Description("Workspace id (GUID).")] string workspaceId,
        [Description("Target URI.")] string uri)
    {
        await ui.InvokeAsync(() => tabs.NavigateAsync(G(workspaceId), U(uri)));
        return "ok";
    }

    [McpServerTool, Description("Move a tab instance from one workspace to another.")]
    public static Task<string> MoveTab(
        ITabManager tabs,
        IUIDispatcher ui,
        TabInstanceRegistry registry,
        [Description("Tab instance id (GUID).")] string tabInstanceId,
        [Description("From workspace id (GUID).")] string fromWorkspaceId,
        [Description("To workspace id (GUID).")] string toWorkspaceId)
        => ui.InvokeAsync(() =>
        {
            if (!registry.TryResolve(G(tabInstanceId), out var inst))
                return JsonSerializer.Serialize(new { error = "tabInstanceId not found" }, JsonOpts);

            tabs.MoveTabItemInstance(inst!, G(fromWorkspaceId), G(toWorkspaceId));
            return "ok";
        });

    [McpServerTool, Description("Move all tab instances from one workspace to another.")]
    public static Task<string> MoveAllTabs(
        ITabManager tabs,
        IUIDispatcher ui,
        [Description("From workspace id (GUID).")] string fromWorkspaceId,
        [Description("To workspace id (GUID).")] string toWorkspaceId)
        => ui.InvokeAsync(() =>
        {
            tabs.MoveAllTabItemInstances(G(fromWorkspaceId), G(toWorkspaceId));
            return "ok";
        });

    [McpServerTool, Description("Change the URL inside the tab (internal navigating).")]
    public static async Task<string> TabInternalNavigate(
        ITabManager tabs,
        IUIDispatcher ui,
        [Description("Original URI.")] string oriUri,
        [Description("New URI.")] string newUri)
    {
        await ui.InvokeAsync(() => tabs.TabInternalNavigatingAsync(U(oriUri), U(newUri)));
        return "ok";
    }

    [McpServerTool, Description("Focus to an existing tab by URI.")]
    public static async Task<string> FocusByUri(
        ITabManager tabs,
        IUIDispatcher ui,
        [Description("Target URI.")] string uri)
    {
        await ui.InvokeAsync(() => tabs.FocusAsync(U(uri)));
        return "ok";
    }

    [McpServerTool, Description("Reload a tab by URI.")]
    public static async Task<string> ReloadByUri(
        ITabManager tabs,
        IUIDispatcher ui,
        [Description("Target URI.")] string uri)
    {
        await ui.InvokeAsync(() => tabs.ReloadAsync(U(uri)));
        return "ok";
    }

    [McpServerTool, Description("Close a tab instance.")]
    public static async Task<string> CloseTab(
        ITabManager tabs,
        IUIDispatcher ui,
        TabInstanceRegistry registry,
        [Description("Tab instance id (GUID).")] string tabInstanceId)
    {
        if (!registry.TryResolve(G(tabInstanceId), out var inst))
            return JsonSerializer.Serialize(new { error = "tabInstanceId not found" }, JsonOpts);

        await ui.InvokeAsync(() => tabs.CloseAsync(inst!));
        registry.Forget(G(tabInstanceId));
        return "ok";
    }

    [McpServerTool, Description("Close all tabs.")]
    public static async Task<string> CloseAll(
        ITabManager tabs,
        IUIDispatcher ui)
    {
        await ui.InvokeAsync(() => tabs.CloseAllAsync());
        return "ok";
    }
}