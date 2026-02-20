using System.Windows.Input;

namespace ZYC.Framework.Abstractions.Workspace;

public class MoveToNewWorkSpaceTabItemHeaderContextMenuSubItem : MoveWorkSpaceTabItemHeaderContextMenuSubItem
{
    public MoveToNewWorkSpaceTabItemHeaderContextMenuSubItem(string title, ICommand command) : base(null!, title,
        command)
    {
    }
}