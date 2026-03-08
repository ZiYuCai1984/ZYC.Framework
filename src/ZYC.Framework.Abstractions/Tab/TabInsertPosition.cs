namespace ZYC.Framework.Abstractions.Tab;

/// <summary>
///     Specifies the relative insertion point for a tab item instance.
/// </summary>
public enum TabInsertPosition
{
    /// <summary>
    ///     Indicates the tab should be placed immediately before the target item.
    /// </summary>
    Before,

    /// <summary>
    ///     Indicates the tab should be placed immediately after the target item.
    /// </summary>
    After
}