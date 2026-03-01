namespace ZYC.Framework.Abstractions.DragDrop;

/// <summary>
///     Specifies how the system should handle and resolve a drop event.
/// </summary>
public enum DropResolutionMode
{
    /// <summary>
    ///     Nothing can handle this drop.
    /// </summary>
    None,

    /// <summary>
    ///     Execute DefaultAction directly.
    /// </summary>
    ExecuteDefault,

    /// <summary>
    ///     Show UI to let user pick one action.
    /// </summary>
    PickAction
}