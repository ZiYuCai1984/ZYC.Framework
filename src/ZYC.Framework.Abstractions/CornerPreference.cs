namespace ZYC.Framework.Abstractions;

/// <summary>
///     Represents window corner rounding preferences.
/// </summary>
public enum CornerPreference
{
    /// <summary>
    ///     Uses the system default rounding behavior.
    /// </summary>
    Default = 0,

    /// <summary>
    ///     Disables rounded corners.
    /// </summary>
    DoNotRound = 1,

    /// <summary>
    ///     Enables rounded corners.
    /// </summary>
    Round = 2,

    /// <summary>
    ///     Enables small rounded corners.
    /// </summary>
    RoundSmall = 3
}