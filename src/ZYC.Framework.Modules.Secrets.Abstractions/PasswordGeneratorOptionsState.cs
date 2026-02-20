using ZYC.CoreToolkit.Abstractions;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.Secrets.Abstractions;

/// <summary>
///     Stores options for generating passwords.
/// </summary>
public class PasswordGeneratorOptionsState : IState
{
    /// <summary>
    ///     Gets or sets the character options to use when generating passwords.
    /// </summary>
    public PasswordCharOptions PasswordCharOptions { get; set; }
        = PasswordCharOptions.Uppercase
          | PasswordCharOptions.Lowercase
          | PasswordCharOptions.Digits
          | PasswordCharOptions.Symbols;

    /// <summary>
    ///     Gets or sets the length of each generated password.
    /// </summary>
    public int Length { get; set; } = 12;

    /// <summary>
    ///     Gets or sets the number of passwords to generate.
    /// </summary>
    public int Count { get; set; } = 5;
}