using System.IO;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.DragDrop;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Modules.WebBrowser.Abstractions;

namespace ZYC.Framework.Modules.WebBrowser;

#pragma warning disable CS1998

[RegisterSingleInstanceAs(typeof(WebBrowserDropProvider), typeof(IDropActionProvider), PreserveExistingDefaults = true)]
internal class WebBrowserDropProvider : IDropActionProvider
{
    public WebBrowserDropProvider(
        IWebBrowserUriPolicy webBrowserUriPolicy,
        ITabManager tabManager)
    {
        WebBrowserUriPolicy = webBrowserUriPolicy;
        TabManager = tabManager;
    }

    private IWebBrowserUriPolicy WebBrowserUriPolicy { get; }

    private ITabManager TabManager { get; }

    public Task<DropAction[]> GetActionsAsync(DropPayload payload, DropContext context)
    {
        var candidates = CollectCandidateUris(payload)
            .Where(u => u is not null && WebBrowserUriPolicy.IsAllowed(u))
            .ToArray();

        if (candidates.Length == 0)
        {
            return Task.FromResult(Array.Empty<DropAction>());
        }

        var action = new DropAction(
            "web.navigate",
            "Open in Web Browser",
            0,
            () => true,
            async progress =>
            {
                var total = candidates.Length;
                for (var i = 0; i < total; ++i)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();

                    await TabManager.NavigateAsync(context.WorkspaceId, candidates[i]);
                    progress?.Report((i + 1) / (double)total);
                }
            },
            "Icon.Browser"
        );

        return Task.FromResult(new[] { action });
    }

    private static Uri[] CollectCandidateUris(DropPayload payload)
    {
        var result = new List<Uri>();

        // 1) Paths: Could be files, folders, or URL strings.
        foreach (var raw in payload.Paths)
        {
            var s = raw.Trim();
            if (string.IsNullOrWhiteSpace(s))
            {
                continue;
            }

            // Prioritize Windows drive paths as file paths to prevent Uri.TryCreate 
            // from misinterpreting "C:\..." as a URI with scheme "c".
            if (UriTools.LooksLikeWindowsPath(s) || Path.IsPathRooted(s))
            {
                var full = Path.GetFullPath(s);

                // Append a trailing separator for directories to ensure a standard file:///.../ URI format.
                if (Directory.Exists(full) && !full.EndsWith(Path.DirectorySeparatorChar))
                {
                    full += Path.DirectorySeparatorChar;
                }

                result.Add(new Uri(full));
                continue;
            }

            // Otherwise, attempt to parse as an absolute URI.
            if (Uri.TryCreate(s, UriKind.Absolute, out var uri))
            {
                result.Add(uri);
                continue;
            }

            // Final fallback: treat as a local file path.
            try
            {
                var full = Path.GetFullPath(s);
                result.Add(new Uri(full));
            }
            catch
            {
                // ignore
            }
        }

        // 2) Extras["text"]: The parser stores UnicodeText in extras["text"].
        if (payload.Extras.TryGetValue("text", out var textObj) && textObj is string text)
        {
            var t = text.Trim();

            // Common case: Dragged links often contain trailing newlines; retrieve the first line.
            var firstLine = t.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault()?.Trim();

            if (!string.IsNullOrWhiteSpace(firstLine) &&
                Uri.TryCreate(firstLine, UriKind.Absolute, out var uri))
            {
                result.Add(uri);
            }
        }

        return result.ToArray();
    }
}