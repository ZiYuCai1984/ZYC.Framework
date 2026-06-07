using System.Globalization;
using System.IO;
using System.Text.Json;
using ZYC.Framework.Core.Converters;

namespace ZYC.Framework.Modules.ChromeExtensions.UI;

public sealed class ChromeExtensionIconConverter : ValueConverterBase<object?, string>
{
    private const string FallbackIcon = "PuzzleOutline";

    private static readonly Dictionary<string, string> IconUriCache = new(StringComparer.OrdinalIgnoreCase);

    private static readonly object SyncRoot = new();

    protected override string InternalConvert(object? value)
    {
        var unpackedPath = value?.ToString();
        if (string.IsNullOrWhiteSpace(unpackedPath))
        {
            return FallbackIcon;
        }

        try
        {
            unpackedPath = Path.GetFullPath(unpackedPath);
        }
        catch
        {
            return FallbackIcon;
        }

        lock (SyncRoot)
        {
            if (IconUriCache.TryGetValue(unpackedPath, out var cachedIconUri))
            {
                return cachedIconUri;
            }
        }

        var iconUri = ResolveIconUri(unpackedPath);
        lock (SyncRoot)
        {
            IconUriCache[unpackedPath] = iconUri;
        }

        return iconUri;
    }

    protected override object InternalConvertBack(string value)
    {
        throw new NotSupportedException();
    }

    private static string ResolveIconUri(string unpackedPath)
    {
        var iconPath = ResolveIconPath(unpackedPath);
        if (string.IsNullOrWhiteSpace(iconPath))
        {
            return FallbackIcon;
        }

        return new Uri(iconPath, UriKind.Absolute).AbsoluteUri;
    }

    private static string? ResolveIconPath(string unpackedPath)
    {
        var manifestPath = Path.Combine(unpackedPath, "manifest.json");
        if (!File.Exists(manifestPath))
        {
            return null;
        }

        try
        {
            using var manifestStream = File.OpenRead(manifestPath);
            using var manifest = JsonDocument.Parse(manifestStream);
            var root = manifest.RootElement;

            var extensionIcons = new List<IconCandidate>();
            if (root.TryGetProperty("icons", out var icons))
            {
                AddIconCandidates(extensionIcons, unpackedPath, icons);
            }

            var extensionIconPath = SelectBestIconPath(extensionIcons);
            if (!string.IsNullOrWhiteSpace(extensionIconPath))
            {
                return extensionIconPath;
            }

            var actionIcons = new List<IconCandidate>();
            AddNestedIconCandidates(actionIcons, unpackedPath, root, "action", "default_icon");
            AddNestedIconCandidates(actionIcons, unpackedPath, root, "browser_action", "default_icon");
            AddNestedIconCandidates(actionIcons, unpackedPath, root, "page_action", "default_icon");

            return SelectBestIconPath(actionIcons);
        }
        catch (JsonException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static void AddNestedIconCandidates(
        ICollection<IconCandidate> candidates,
        string unpackedPath,
        JsonElement root,
        string parentPropertyName,
        string propertyName)
    {
        if (!root.TryGetProperty(parentPropertyName, out var parent)
            || parent.ValueKind != JsonValueKind.Object
            || !parent.TryGetProperty(propertyName, out var iconElement))
        {
            return;
        }

        AddIconCandidates(candidates, unpackedPath, iconElement);
    }

    private static void AddIconCandidates(
        ICollection<IconCandidate> candidates,
        string unpackedPath,
        JsonElement iconElement)
    {
        switch (iconElement.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in iconElement.EnumerateObject())
                {
                    AddIconCandidate(candidates, unpackedPath, property.Value, property.Name);
                }

                break;
            case JsonValueKind.String:
                AddIconCandidate(candidates, unpackedPath, iconElement, "");
                break;
        }
    }

    private static void AddIconCandidate(
        ICollection<IconCandidate> candidates,
        string unpackedPath,
        JsonElement iconElement,
        string sizeKey)
    {
        if (iconElement.ValueKind != JsonValueKind.String)
        {
            return;
        }

        var iconPath = ResolveResourcePath(unpackedPath, iconElement.GetString());
        if (iconPath == null)
        {
            return;
        }

        var size = int.TryParse(sizeKey, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedSize)
            ? parsedSize
            : 0;
        candidates.Add(new IconCandidate(size, iconPath));
    }

    private static string? SelectBestIconPath(IEnumerable<IconCandidate> candidates)
    {
        return candidates
            .OrderByDescending(t => t.Size)
            .Select(t => t.Path)
            .FirstOrDefault();
    }

    private static string? ResolveResourcePath(string unpackedPath, string? resourcePath)
    {
        if (string.IsNullOrWhiteSpace(resourcePath)
            || Uri.TryCreate(resourcePath, UriKind.Absolute, out _))
        {
            return null;
        }

        var root = Path.GetFullPath(unpackedPath);
        var rootPrefix = root.EndsWith(Path.DirectorySeparatorChar)
            ? root
            : $"{root}{Path.DirectorySeparatorChar}";

        var relativePath = resourcePath
            .Trim()
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar)
            .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var iconPath = Path.GetFullPath(Path.Combine(root, relativePath));

        return iconPath.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase) && File.Exists(iconPath)
            ? iconPath
            : null;
    }

    private sealed record IconCandidate(int Size, string Path);
}
