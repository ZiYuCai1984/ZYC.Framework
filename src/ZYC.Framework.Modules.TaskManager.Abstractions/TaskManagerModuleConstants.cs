using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.TaskManager.Abstractions;

#pragma warning disable CS1591

public static class TaskManagerModuleConstants
{
    public const string Icon = "DnsOutline";

    public const string Host = "taskmanager";

    public const string Title = "TaskManager";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}