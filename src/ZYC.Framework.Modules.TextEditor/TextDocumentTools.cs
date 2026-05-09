using System.IO;
using System.Text;
using ICSharpCode.AvalonEdit.Highlighting;
using ZYC.Framework.Core;

namespace ZYC.Framework.Modules.TextEditor;

internal static class TextDocumentTools
{
    private const int ReadRetryCount = 3;
    private const int ReadRetryDelayMilliseconds = 100;
    public const string DefaultEncodingName = "UTF-8";

    private static readonly Encoding Utf8NoBomEncoding = new UTF8Encoding(false, true);
    private static readonly Encoding Utf8BomEncoding = new UTF8Encoding(true, true);
    private static readonly Encoding Utf16LeEncoding = new UnicodeEncoding(false, true, true);
    private static readonly Encoding Utf16BeEncoding = new UnicodeEncoding(true, true, true);
    private static readonly Encoding Utf16LeNoBomEncoding = new UnicodeEncoding(false, false, true);
    private static readonly Encoding Utf16BeNoBomEncoding = new UnicodeEncoding(true, false, true);
    private static readonly Encoding Utf32LeEncoding = new UTF32Encoding(false, true, true);
    private static readonly Encoding Utf32BeEncoding = new UTF32Encoding(true, true, true);
    private static Encoding Latin1Encoding =>
        Encoding.GetEncoding("iso-8859-1", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);

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

    static TextDocumentTools()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static Encoding DefaultEncoding => Utf8NoBomEncoding;

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

    public static async Task<TextDocumentSnapshot> ReadDocumentAsync(string filePath)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                var bytes = await File.ReadAllBytesAsync(filePath);
                return DecodeDocument(filePath, bytes);
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

    public static async Task WriteDocumentAsync(string filePath, string text, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(encoding);

        var preamble = encoding.GetPreamble();
        var textBytes = encoding.GetBytes(text);
        if (preamble.Length == 0)
        {
            await File.WriteAllBytesAsync(filePath, textBytes);
            return;
        }

        var outputBytes = new byte[preamble.Length + textBytes.Length];
        Buffer.BlockCopy(preamble, 0, outputBytes, 0, preamble.Length);
        Buffer.BlockCopy(textBytes, 0, outputBytes, preamble.Length, textBytes.Length);
        await File.WriteAllBytesAsync(filePath, outputBytes);
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

    private static TextDocumentSnapshot DecodeDocument(string filePath, byte[] bytes)
    {
        var detected = DetectEncoding(bytes);
        var text = detected.Encoding.GetString(bytes, detected.PreambleLength, bytes.Length - detected.PreambleLength);

        return new TextDocumentSnapshot(
            text,
            detected.Encoding,
            detected.Name,
            File.GetLastWriteTimeUtc(filePath));
    }

    private static DetectedEncoding DetectEncoding(byte[] bytes)
    {
        if (StartsWith(bytes, [0xEF, 0xBB, 0xBF]))
        {
            return new DetectedEncoding(Utf8BomEncoding, "UTF-8 BOM", 3);
        }

        if (StartsWith(bytes, [0xFF, 0xFE, 0x00, 0x00]))
        {
            return new DetectedEncoding(Utf32LeEncoding, "UTF-32 LE", 4);
        }

        if (StartsWith(bytes, [0x00, 0x00, 0xFE, 0xFF]))
        {
            return new DetectedEncoding(Utf32BeEncoding, "UTF-32 BE", 4);
        }

        if (StartsWith(bytes, [0xFF, 0xFE]))
        {
            return new DetectedEncoding(Utf16LeEncoding, "UTF-16 LE", 2);
        }

        if (StartsWith(bytes, [0xFE, 0xFF]))
        {
            return new DetectedEncoding(Utf16BeEncoding, "UTF-16 BE", 2);
        }

        var utf16WithoutBomEncoding = DetectUtf16WithoutBom(bytes);
        if (utf16WithoutBomEncoding is not null)
        {
            return utf16WithoutBomEncoding;
        }

        if (CanDecode(bytes, Utf8NoBomEncoding))
        {
            return new DetectedEncoding(Utf8NoBomEncoding, DefaultEncodingName, 0);
        }

        var ansiEncoding = GetSystemAnsiEncoding();
        if (CanDecode(bytes, ansiEncoding))
        {
            return new DetectedEncoding(ansiEncoding, ansiEncoding.WebName, 0);
        }

        return new DetectedEncoding(Latin1Encoding, Latin1Encoding.WebName, 0);
    }

    private static DetectedEncoding? DetectUtf16WithoutBom(byte[] bytes)
    {
        if (bytes.Length < 4)
        {
            return null;
        }

        var sampleLength = Math.Min(bytes.Length, 4096);
        var pairCount = sampleLength / 2;
        if (pairCount == 0)
        {
            return null;
        }

        var evenNulls = 0;
        var oddNulls = 0;
        for (var index = 0; index < pairCount * 2; index += 2)
        {
            if (bytes[index] == 0)
            {
                evenNulls++;
            }

            if (bytes[index + 1] == 0)
            {
                oddNulls++;
            }
        }

        if (oddNulls * 10 >= pairCount * 6 && evenNulls * 10 <= pairCount)
        {
            return new DetectedEncoding(Utf16LeNoBomEncoding, "UTF-16 LE no BOM", 0);
        }

        if (evenNulls * 10 >= pairCount * 6 && oddNulls * 10 <= pairCount)
        {
            return new DetectedEncoding(Utf16BeNoBomEncoding, "UTF-16 BE no BOM", 0);
        }

        return null;
    }

    private static Encoding GetSystemAnsiEncoding()
    {
        return Encoding.GetEncoding(
            0,
            EncoderFallback.ExceptionFallback,
            DecoderFallback.ExceptionFallback);
    }

    private static bool CanDecode(byte[] bytes, Encoding encoding)
    {
        try
        {
            encoding.GetString(bytes);
            return true;
        }
        catch (DecoderFallbackException)
        {
            return false;
        }
    }

    private static bool StartsWith(byte[] bytes, byte[] prefix)
    {
        if (bytes.Length < prefix.Length)
        {
            return false;
        }

        for (var index = 0; index < prefix.Length; index++)
        {
            if (bytes[index] != prefix[index])
            {
                return false;
            }
        }

        return true;
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

    private sealed record DetectedEncoding(Encoding Encoding, string Name, int PreambleLength);
}

internal sealed record TextDocumentSnapshot(
    string Text,
    Encoding Encoding,
    string EncodingName,
    DateTime LastWriteUtc);
