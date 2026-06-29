using System.IO;
using ZYC.CoreToolkit;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Core;

public static class MutexTools
{
    public const string MutexOverridePathName = "mutex-id.override";

    public static string GetMutexOverridePath()
    {
        var currentDirectory = IOTools.GetExecutingFolder();
        var directory = new DirectoryInfo(currentDirectory);
        var rootPath = directory.Parent?.FullName ?? currentDirectory;
        var settingsPath = Path.Combine(rootPath, "settings");

        return Path.Combine(settingsPath, MutexOverridePathName);
    }

    public static string GetMutexId()
    {
        var mutexOverridePath = GetMutexOverridePath();

        var userName = "";

        try
        {
            userName = AccountTools.GetCurrentUserName();
        }
        catch
        {
            //ignore
        }

        //!WARNING Dealing with multi-user
        var baseMutexId = $"{userName}-{ProductInfo.PackageId}";

        if (!File.Exists(mutexOverridePath))
        {
            return baseMutexId;
        }

        var overrideId = File.ReadAllText(mutexOverridePath).Trim();

        if (!string.IsNullOrWhiteSpace(overrideId))
        {
            return $"{baseMutexId}-{overrideId}";
        }

        return baseMutexId;
    }
}
