using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Accounts.Abstractions;

/// <summary>
///     Defines constants used by the accounts module.
/// </summary>
public static class AccountsModuleConstants
{
    /// <summary>
    ///     The accounts module icon key.
    /// </summary>
    public const string Icon = "AccountCircleOutline";

    /// <summary>
    ///     The accounts module host segment.
    /// </summary>
    public const string Host = "accounts";

    /// <summary>
    ///     The accounts module display title.
    /// </summary>
    public const string Title = "Accounts";

    /// <summary>
    ///     Gets the accounts module application URI.
    /// </summary>
    public static Uri Uri => UriTools.CreateAppUri(Host);
}
