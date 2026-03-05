namespace ZYC.Framework.Modules.FileExplorer.Abstractions;

#pragma warning disable CS1591

public static class FileExplorerModuleConstants
{
    public const string Icon = "FolderOutline";

    public const string MenuTitle = "File Explorer";

    public const string Host = "file";

    public static Uri InitialUri => new("file:///C:/");

    public static Uri Uri => InitialUri;
}