using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Settings.Abstractions;

/// <summary>
///     Groups settings for a specific configuration type.
/// </summary>
public class SettingGroup
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SettingGroup" /> class.
    /// </summary>
    /// <param name="type">The configuration type.</param>
    /// <param name="settingItems">The settings in the group.</param>
    public SettingGroup(Type type, SettingItem[] settingItems)
    {
        Type = type;
        Name = ModuleNameTools.GetTypeModuleName(type);

        SettingItems = settingItems;
    }

    /// <summary>
    ///     Gets the group name derived from the module.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the configuration type represented by this group.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    ///     Gets the settings in this group.
    /// </summary>
    public SettingItem[] SettingItems { get; }
}