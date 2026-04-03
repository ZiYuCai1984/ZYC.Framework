using System.IO;
using ICSharpCode.AvalonEdit.Highlighting;
using ZYC.Framework.Core;

namespace ZYC.Framework.Modules.TextEditor;

internal static class TextDocumentTools
{
    private const int ReadRetryCount = 3;
    private const int ReadRetryDelayMilliseconds = 100;

    private static readonly HashSet<string> KnownTextExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".bat",
        ".cmd",
        ".config",
        ".cs",
        ".cshtml",
        ".csproj",
        ".css",
        ".csv",
        ".editorconfig",
        ".gitattributes",
        ".gitignore",
        ".htm",
        ".html",
        ".js",
        ".json",
        ".jsx",
        ".less",
        ".log",
        ".markdown",
        ".md",
        ".props",
        ".ps1",
        ".psm1",
        ".razor",
        ".resx",
        ".scss",
        ".sln",
        ".slnx",
        ".sql",
        ".targets",
        ".ts",
        ".tsx",
        ".txt",
        ".xaml",
        ".xml",
        ".yaml",
        ".yml"
    };

    public static void ApplySyntaxHighlighting(ICSharpCode.AvalonEdit.TextEditor editor, string filePath)
    {
        ArgumentNullException.ThrowIfNull(editor);

        var extension = Path.GetExtension(filePath);
        var highlighting = HighlightingManager.Instance.GetDefinitionByExtension(extension);
        if (highlighting is null)
        {
            var fallbackName = GetFallbackHighlightingName(extension);
            if (!string.IsNullOrWhiteSpace(fallbackName))
            {
                highlighting = HighlightingManager.Instance.GetDefinition(fallbackName);
            }
        }

        editor.SyntaxHighlighting = highlighting;
    }

    public static string GetDisplayName(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        return string.IsNullOrWhiteSpace(fileName)
            ? filePath
            : fileName;
    }

    public static Uri GetRequiredEditorFileUri(Uri routeUri)
    {
        if (TryGetEditorFileUri(routeUri, out var fileUri) && fileUri is not null)
        {
            return fileUri;
        }

        throw new InvalidOperationException($"Invalid text editor route: '{routeUri}'.");
    }

    public static bool IsTextFile(Uri uri)
    {
        return uri.IsAbsoluteUri && uri.IsFile && IsTextFile(uri.LocalPath);
    }

    public static bool IsTextFile(string localPath)
    {
        if (!File.Exists(localPath) || Directory.Exists(localPath))
        {
            return false;
        }

        return KnownTextExtensions.Contains(Path.GetExtension(localPath))
               || IsProbablyTextFile(localPath);
    }

    public static async Task<string> ReadAllTextAsync(string filePath)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                return await File.ReadAllTextAsync(filePath);
            }
            catch (IOException) when (attempt < ReadRetryCount)
            {
                await Task.Delay(ReadRetryDelayMilliseconds);
            }
            catch (UnauthorizedAccessException) when (attempt < ReadRetryCount)
            {
                await Task.Delay(ReadRetryDelayMilliseconds);
            }
        }
    }

    public static bool TryGetEditorFileUri(Uri routeUri, out Uri? fileUri)
    {
        fileUri = null;

        if (!UriBinder.TryBind<TextEditorRouteParameters>(routeUri, out var parameters)
            || parameters is null)
        {
            return false;
        }

        if (!parameters.File.IsAbsoluteUri || !parameters.File.IsFile)
        {
            return false;
        }

        fileUri = parameters.File;
        return true;
    }

    private static string? GetFallbackHighlightingName(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return null;
        }

        return extension.ToLowerInvariant() switch
        {
            ".config" or ".csproj" or ".props" or ".resx" or ".targets" or ".xaml" or ".xml" => "XML",
            ".markdown" or ".md" => "Markdown",
            _ => null
        };
    }

    private static bool IsProbablyTextFile(string localPath)
    {
        const int SampleSize = 4096;

        using var stream = File.OpenRead(localPath);
        if (stream.Length == 0)
        {
            return true;
        }

        var buffer = new byte[SampleSize];
        var read = stream.Read(buffer, 0, buffer.Length);
        if (read == 0)
        {
            return true;
        }

        var suspiciousByteCount = 0;
        for (var index = 0; index < read; index++)
        {
            var value = buffer[index];
            if (value == 0)
            {
                return false;
            }

            if (value < 0x08 || (value > 0x0D && value < 0x20))
            {
                suspiciousByteCount++;
            }
        }

        return suspiciousByteCount * 10 < read;
    }
}
