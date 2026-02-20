namespace ZYC.Framework.Modules.Aspire.Abstractions;

/// <summary>
///     Defines command-line service options for the Aspire extension.
/// </summary>
public class CommandlineServiceOptions
{
    /// <summary>
    ///     !WARNING Validate that a model name is valid.
    ///     Validate that a model name is valid.
    ///     - Must start with an ASCII letter.
    ///     - Must contain only ASCII letters, digits, and hyphens.
    ///     - Must not end with a hyphen.
    ///     - Must not contain consecutive hyphens.
    ///     - Must be between 1 and 64 characters long.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    ///     Gets or sets the command line used to launch the service.
    /// </summary>
    public string Command { get; set; } = "";

    /// <summary>
    ///     Gets or sets the working directory for the command.
    /// </summary>
    public string WorkDirectory { get; set; } = "";
}