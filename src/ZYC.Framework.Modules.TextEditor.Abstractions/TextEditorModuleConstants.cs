using ZYC.Framework.Abstractions;

#pragma warning disable CS1591


namespace ZYC.Framework.Modules.TextEditor.Abstractions;

public static class TextEditorModuleConstants
{
    public const string PreviewHost = "file";

    public const string PreviewTitle = "Text Preview";

    public const string EditorHost = "texteditor";

    public const string EditorPath = "edit";

    public const string EditorTitle = "Text Editor";

    public const string MenuTitle = "Text File";

    public const string Icon = "FileDocumentOutline";

    public const string FileDialogFilter =
        "Text Files (*.txt;*.md;*.json;*.xml;*.xaml;*.cs;*.csproj;*.sln;*.slnx;*.props;*.targets;*.config;*.yml;*.yaml;*.js;*.ts;*.tsx;*.jsx;*.html;*.htm;*.css;*.scss;*.less;*.csv;*.log;*.sql;*.ps1;*.psm1;*.cmd;*.bat;*.editorconfig)"
        + "|*.txt;*.md;*.json;*.xml;*.xaml;*.cs;*.csproj;*.sln;*.slnx;*.props;*.targets;*.config;*.yml;*.yaml;*.js;*.ts;*.tsx;*.jsx;*.html;*.htm;*.css;*.scss;*.less;*.csv;*.log;*.sql;*.ps1;*.psm1;*.cmd;*.bat;*.editorconfig"
        + "|All Files(*.*)|*.*";

    public static Uri CreateEditorUri(Uri fileUri)
    {
        ArgumentNullException.ThrowIfNull(fileUri);

        return UriTools.CreateAppUri(
            EditorHost,
            EditorPath,
            $"file={Uri.EscapeDataString(fileUri.ToString())}");
    }
}
