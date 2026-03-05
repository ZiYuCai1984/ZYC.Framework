using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Abstractions.MCP;

namespace ZYC.Framework.Modules.Settings.Abstractions;

/// <summary>
///     Manages application settings, states, and secrets.
/// </summary>
[ExposeToMCP]
public partial interface ISettingsManager
{
    /// <summary>
    ///     Brings the specified configuration type into view.
    /// </summary>
    /// <param name="configType">The configuration type.</param>
    [ExposeToMCP(RequiresUIThread = true)]
    void BringIntoView(Type configType);

    /// <summary>
    ///     Brings the specified configuration type into view.
    /// </summary>
    /// <typeparam name="T">The configuration type.</typeparam>
    [ExposeToMCP(RequiresUIThread = true)]
    void BringIntoView<T>();


    /// <summary>
    ///     Gets setting groups that are not hidden.
    /// </summary>
    /// <returns>The visible setting groups.</returns>
    SettingGroup[] GetSettingGroupsWithoutHidden();

    /// <summary>
    ///     Saves the specified configuration.
    /// </summary>
    /// <typeparam name="T">The configuration type.</typeparam>
    void SaveConfig<T>() where T : IConfig;

    /// <summary>
    ///     Gets the URI of the settings page.
    /// </summary>
    /// <returns>The settings page URI.</returns>
    Uri GetPageUri();

    /// <summary>
    ///     Resets all settings, states, and secrets.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    async Task ResetAllAsync()
    {
        await ResetStatesAsync();
        await ResetConfigsAsync();
        await ResetSecretsAsync();
    }

    /// <summary>
    ///     Resets all states to defaults.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ResetStatesAsync();

    /// <summary>
    ///     Resets all configurations to defaults.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ResetConfigsAsync();

    /// <summary>
    ///     Resets all secrets to defaults.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ResetSecretsAsync();
}

public partial interface ISettingsManager
{
    /// <summary>
    ///     Sets the settings view instance.
    /// </summary>
    /// <param name="settingsView">The settings view instance.</param>
    [MCPIgnore]
    internal void SetSettingsView(ISettingsView? settingsView);

    /// <summary>
    ///     Creates setting groups for the specified configurations.
    /// </summary>
    /// <typeparam name="T">The configuration type.</typeparam>
    /// <param name="configs">The configuration instances.</param>
    /// <returns>The created setting groups.</returns>
    [MCPIgnore]
    SettingGroup[] CreateSettingGroups<T>(T[] configs) where T : IConfig;
}