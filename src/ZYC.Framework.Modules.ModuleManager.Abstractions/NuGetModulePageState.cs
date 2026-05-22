using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.ModuleManager.Abstractions;

/// <summary>
///     Stores nuget module manager page UI state.
/// </summary>
public class NuGetModulePageState : IState
{
    /// <summary>
    ///     Gets or sets the split ratio between the module list and detail panes.
    /// </summary>
    public double Ratio { get; set; } = 0.5;
}