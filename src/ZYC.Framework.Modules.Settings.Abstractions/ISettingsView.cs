namespace ZYC.Framework.Modules.Settings.Abstractions;

/// <summary>
///     Represents an internal settings view capable of bringing configs into view.
/// </summary>
internal interface ISettingsView
{
    /// <summary>
    ///     Brings the specified configuration type into view.
    /// </summary>
    /// <param name="configType">The configuration type.</param>
    void BringIntoView(Type configType);
}