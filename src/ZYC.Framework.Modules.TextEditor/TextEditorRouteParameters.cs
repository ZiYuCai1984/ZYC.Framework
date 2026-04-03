namespace ZYC.Framework.Modules.TextEditor;

internal sealed class TextEditorRouteParameters
{
    public TextEditorRouteParameters(Uri file)
    {
        File = file;
    }

    public Uri File { get; }
}
