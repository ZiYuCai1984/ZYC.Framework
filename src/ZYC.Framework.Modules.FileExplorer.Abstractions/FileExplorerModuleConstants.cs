namespace ZYC.Framework.Modules.FileExplorer.Abstractions;

public static class FileExplorerModuleConstants
{
    public const string Icon = "FolderOutline";

    public const string MenuTitle = "FileExplorer";

    public const string Host = "file";

    public static Uri InitialUri => new("file:///C:/");

    public static Uri Uri => InitialUri;
}