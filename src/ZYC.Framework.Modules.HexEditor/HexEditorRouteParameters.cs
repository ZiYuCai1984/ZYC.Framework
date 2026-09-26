namespace ZYC.Framework.Modules.HexEditor;

internal class HexEditorRouteParameters
{
    public HexEditorRouteParameters(Uri file)
    {
        File = file;
    }

    public Uri File { get; }
}
