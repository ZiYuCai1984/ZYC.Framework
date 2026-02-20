using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.ModuleManager.Abstractions;

// ReSharper disable MemberHidesStaticFromOuterClass
public static class ModuleManagerModuleConstants
{
    public const string Host = "modules";

    public static class NuGet
    {
        public const string Icon = Base64IconResources.NuGet;

        public const string Path = "/nuget";

        public const string Title = "NuGet Modules";

        public static Uri Uri => UriTools.CreateAppUri(Host, Path);
    }


    public class Local
    {
        public const string Icon = "PuzzleOutline";

        public const string Path = "/local";

        public const string Title = "Local Modules";

        public static Uri Uri => UriTools.CreateAppUri(Host, Path);
    }
}