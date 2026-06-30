using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Accounts.Abstractions;

#pragma warning disable CS1591

public static class AccountsModuleConstants
{

    public const string Icon = "AccountCircleOutline";

    public const string Host = "accounts";


    public const string Title = "Accounts";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
