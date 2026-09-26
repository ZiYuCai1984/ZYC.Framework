using System.IO;
using ZYC.Framework.Core;

namespace ZYC.Framework.Modules.HexEditor;

internal static class HexDocumentTools
{
    public static string GetDisplayName(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        return string.IsNullOrWhiteSpace(fileName) ? filePath : fileName;
    }

    public static Uri GetRequiredEditorFileUri(Uri routeUri)
    {
        if (TryGetEditorFileUri(routeUri, out var fileUri) && fileUri is not null)
        {
            return fileUri;
        }

        throw new InvalidOperationException($"Invalid hex editor route: '{routeUri}'.");
    }

    public static bool TryGetEditorFileUri(Uri routeUri, out Uri? fileUri)
    {
        fileUri = null;
        if (!UriBinder.TryBind<HexEditorRouteParameters>(routeUri, out var parameters)
            || parameters?.File is not { IsAbsoluteUri: true, IsFile: true } candidate
            || Directory.Exists(candidate.LocalPath))
        {
            return false;
        }

        fileUri = candidate;
        return true;
    }

    public static async Task<HexDocumentSnapshot> ReadDocumentAsync(string filePath, CancellationToken cancellationToken)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                var lastWriteUtc = File.GetLastWriteTimeUtc(filePath);
                var bytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
                var info = new FileInfo(filePath);
                if (!info.Exists || info.LastWriteTimeUtc != lastWriteUtc || info.Length != bytes.LongLength)
                {
                    throw new IOException("The file changed while it was being read. Please reload it.");
                }

                return new HexDocumentSnapshot(bytes, lastWriteUtc, info.IsReadOnly);
            }
            catch (IOException) when (attempt < 3)
            {
                await Task.Delay(100, cancellationToken);
            }
        }
    }

    public static async Task WriteDocumentAsync(string filePath, byte[] bytes, CancellationToken cancellationToken)
    {
        // Finish writing beside the destination before replacing it, so a failed write
        // does not truncate the user's original file.
        var targetPath = Path.GetFullPath(filePath);
        var temporaryPath = Path.Combine(Path.GetDirectoryName(targetPath)!, $".{Guid.NewGuid():N}.tmp");
        try
        {
            await File.WriteAllBytesAsync(temporaryPath, bytes, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (File.Exists(targetPath))
            {
                File.Replace(temporaryPath, targetPath, null);
            }
            else
            {
                File.Move(temporaryPath, targetPath);
            }
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }
}

internal record HexDocumentSnapshot(byte[] Bytes, DateTime LastWriteUtc, bool IsReadOnly);
