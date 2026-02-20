using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Abstractions.Workspace;

public interface IMoveWorkSpaceTabItemHeaderContextMenuItemManager
{
    MoveWorkSpaceTabItemHeaderContextMenuSubItem[] GetSubItems(ITabItemInstance instance);
}