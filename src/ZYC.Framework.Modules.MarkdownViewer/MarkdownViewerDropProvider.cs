using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Abstractions.Attributes;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.DragDrop;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Modules.MarkdownViewer;

[RegisterSingleInstanceAs(typeof(MarkdownViewerDropProvider), typeof(IDropActionProvider),
    PreserveExistingDefaults = true)]
internal class MarkdownViewerDropProvider : IDropActionProvider
{
    public MarkdownViewerDropProvider(ITabManager tabManager, ILogger<MarkdownViewerDropProvider> logger)
    {
        TabManager = tabManager;
        Logger = logger;
    }

    private ITabManager TabManager { get; }
    private ILogger<MarkdownViewerDropProvider> Logger { get; }

    [TempCode]
    public async Task<DropAction[]> GetActionsAsync(DropPayload payload, DropContext context)
    {
        await Task.CompletedTask;

        if (payload.Paths.Length < 1)
        {
            return [];
        }

        if (payload.Paths[0].EndsWith(".md"))
        {
            return
            [
                new DropAction("Markdown", "Open in markdown viewer", 5, () => true,
                    async _ =>
                    {
                        try
                        {
                            var uri = MarkdownRoute.BuildWithDocument(new Uri(payload.Paths[0]));
                            await TabManager.NavigateAsync(uri);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);
                        }
                    }, IsDefault: true)
            ];
        }
        return [];
    }
}