using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Abstractions.Workspace;

/// <summary>
///     Defines a manager responsible for generating sub-items for the "Move Tab" context menu.
///     This is typically used to provide a list of target workspace nodes where a tab can be relocated.
/// </summary>
public interface IMoveWorkSpaceTabItemHeaderContextMenuItemManager
{
    /// <summary>
    ///     Retrieves the available move destinations as menu sub-items for a specific tab instance.
    /// </summary>
    /// <param name="instance">The specific tab item instance being moved.</param>
    /// <returns>An array of menu sub-items representing potential move targets.</returns>
    MoveWorkSpaceTabItemHeaderContextMenuSubItem[] GetSubItems(ITabItemInstance instance);
}