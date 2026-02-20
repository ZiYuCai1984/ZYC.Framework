using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.Mock.Abstractions;

/// <summary>
///     Stores the split ratio state for a mock grid layout.
/// </summary>
public class GridSplitState : IState
{
    /// <summary>
    ///     Gets or sets the split ratio for the grid.
    /// </summary>
    public double Ratio { get; set; } = 0.5;
}