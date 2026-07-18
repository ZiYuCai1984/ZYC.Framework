using System.IO;

namespace ZYC.Framework.WebView2;

public static class WebBrowserPluginArgumentTools
{
    private const string LoadExtensionArgumentName = "--load-extension";

    public static string[] ReplaceConfiguredExtensionPaths(
        string[] customBrowserArguments,
        IEnumerable<string> paths)
    {
        var configuredPaths = paths
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .GroupBy(NormalizeFileSystemPath, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToArray();

        var arguments = customBrowserArguments
            .Where(argument => !IsLoadExtensionArgument(argument))
            .ToList();

        if (configuredPaths.Length > 0)
        {
            arguments.Add(BuildLoadExtensionArgument(configuredPaths));
        }

        return arguments.ToArray();
    }



    public static IReadOnlyList<string> GetConfiguredExtensionPaths(string[] customBrowserArguments)
    {
        return customBrowserArguments
            .Where(IsLoadExtensionArgument)
            .SelectMany(ReadLoadExtensionArgumentPaths)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public static bool SamePath(string left, string right)
    {
        if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right))
        {
            return false;
        }

        return string.Equals(
            NormalizeFileSystemPath(left),
            NormalizeFileSystemPath(right),
            StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildLoadExtensionArgument(IEnumerable<string> paths)
    {
        var value = string.Join(",", paths.Select(path => path.Trim().Trim('"')));
        return $"{LoadExtensionArgumentName}=\"{value}\"";
    }

    private static IEnumerable<string> ReadLoadExtensionArgumentPaths(string argument)
    {
        var trimmed = argument.Trim();
        var equalsIndex = trimmed.IndexOf('=');
        if (equalsIndex < 0 || equalsIndex >= trimmed.Length - 1)
        {
            return Array.Empty<string>();
        }

        var value = trimmed[(equalsIndex + 1)..].Trim().Trim('"', '\'');
        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(path => path.Trim().Trim('"', '\''));
    }

    private static bool IsLoadExtensionArgument(string argument)
    {
        var trimmed = argument.Trim();
        if (!trimmed.StartsWith(LoadExtensionArgumentName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return trimmed.Length == LoadExtensionArgumentName.Length
               || trimmed[LoadExtensionArgumentName.Length] == '=';
    }

    private static string NormalizeFileSystemPath(string path)
    {
        var normalized = path.Trim().Trim('"', '\'');
        try
        {
            normalized = Path.GetFullPath(normalized);
        }
        catch
        {
            // Keep the raw value if it is not a normal file-system path.
        }

        return normalized.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
}
