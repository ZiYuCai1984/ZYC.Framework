using ZYC.Framework.Modules.Secrets.Abstractions;
using ZYC.Framework.Modules.Settings.Abstractions;

namespace ZYC.Framework.Modules.Mock.Abstractions;

/// <summary>
///     Represents a mock secret configuration used for demos or testing.
/// </summary>
public class MockSecret : ISecrets
{
    /// <summary>
    ///     Gets or sets the mock username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the mock password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets remarks for the mock secret.
    /// </summary>
    public string Remarks { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets a long-form mock secret value.
    /// </summary>
    [MultilineText]
    public string LongSecret { get; set; } = string.Empty;
}