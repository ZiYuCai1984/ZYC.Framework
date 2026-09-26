using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.HexEditor.Abstractions;

/// <summary>
///     Defines the hex editor routes and presentation constants.
/// </summary>
public static class HexEditorModuleConstants
{
    /// <summary>The application route host.</summary>
    public const string Host = "hexeditor";

    /// <summary>The route path for editing a file.</summary>
    public const string EditorPath = "edit";

    /// <summary>The editor page title.</summary>
    public const string Title = "Hex Editor";

    /// <summary>The entry in the File/Open menu.</summary>
    public const string MenuTitle = "Binary File";

    /// <summary>The module and tab icon.</summary>
    public const string Icon = "AlphaH";

    /// <summary>The file dialog filter; any file can be opened as binary data.</summary>
    public const string FileDialogFilter = "All Files (*.*)|*.*";

    /// <summary>The base application URI for the module.</summary>
    public static Uri Uri => UriTools.CreateAppUri(Host);

    /// <summary>Creates an editor route for a local file.</summary>
    /// <param name="fileUri">The absolute file URI to edit.</param>
    /// <returns>The application URI identifying the file's editor tab.</returns>
    /// <exception cref="ArgumentNullException">The file URI is null.</exception>
    /// <exception cref="ArgumentException">The URI is not an absolute file URI.</exception>
    public static Uri CreateEditorUri(Uri fileUri)
    {
        ArgumentNullException.ThrowIfNull(fileUri);
        if (!fileUri.IsAbsoluteUri || !fileUri.IsFile)
        {
            throw new ArgumentException("An absolute file URI is required.", nameof(fileUri));
        }

        return UriTools.CreateAppUri(
            Host,
            EditorPath,
            $"file={Uri.EscapeDataString(fileUri.AbsoluteUri)}");
    }
}