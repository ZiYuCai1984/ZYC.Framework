using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.NuGet.Abstractions;

/// <summary>
///     Defines NuGet configuration settings for module updates.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class NuGetConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the NuGet sources used for updates.
    /// </summary>
    public string Source { get; set; } = "https://api.nuget.org/v3/index.json";

    /// <summary>
    ///     Gets or sets the relative cache folder for NuGet packages.
    /// </summary>
    public string CacheFolder { get; set; } = ".cache";
}