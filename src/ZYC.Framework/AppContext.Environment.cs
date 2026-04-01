using System.Diagnostics;
using System.IO;
using ZYC.CoreToolkit;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework;

internal partial class AppContext
{
    string IAppContext.GetLogsDirectory()
    {
        return GetLogsDirectory();
    }

    string IAppContext.GetAppRootDirectory()
    {
        return GetAppRootDirectory();
    }

    string IAppContext.GetCurrentDirectory()
    {
        return GetCurrentDirectory();
    }

    public string GetTempPath()
    {
        return Path.GetTempPath();
    }

    string IAppContext.GetProcessFileName()
    {
        return GetProcessFileName();
    }

    string IAppContext.GetArgumentString()
    {
        return GetArgumentString();
    }

    public static string GetLogsDirectory()
    {
        return Path.Combine(GetAppRootDirectory(), "logs");
    }

    public static string GetAppRootDirectory()
    {
        var current = GetCurrentDirectory();

        var directory = new DirectoryInfo(current);
        return directory.Parent!.FullName;
    }

    public static string GetCurrentDirectory()
    {
        return IOTools.GetExecutingFolder();
    }

    public static string GetSettingsDirectory()
    {
        return Path.Combine(GetAppRootDirectory(), "settings");
    }

    public static string GetProcessFileName()
    {
        var fullFileName = Process.GetCurrentProcess().MainModule!.FileName;
        return Path.GetFileName(fullFileName);
    }

    public static string GetArgumentString()
    {
        var arguments = Environment.GetCommandLineArgs();
        var argumentString = string.Join(" ", arguments.Skip(1));
        return argumentString;
    }
}