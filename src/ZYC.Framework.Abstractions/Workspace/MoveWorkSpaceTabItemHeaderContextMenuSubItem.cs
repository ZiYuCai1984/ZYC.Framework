using System.Windows.Input;

namespace ZYC.Framework.Abstractions.Workspace;

/// <summary>
///     Represents a specific sub-item in the context menu used for moving workspace tabs.
///     It links a target <see cref="WorkspaceNode" /> with the command required to perform the move.
/// </summary>
public class MoveWorkSpaceTabItemHeaderContextMenuSubItem
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MoveWorkSpaceTabItemHeaderContextMenuSubItem" /> class.
    /// </summary>
    /// <param name="workspace">The target workspace node associated with this menu item.</param>
    /// <param name="title">The display text for the menu item.</param>
    /// <param name="command">The execution logic for moving the tab to the specified workspace.</param>
    public MoveWorkSpaceTabItemHeaderContextMenuSubItem(WorkspaceNode workspace, string title, ICommand command)
    {
        Workspace = workspace;
        Title = title;
        Command = command;
    }

    /// <summary>
    ///     Gets the <see cref="WorkspaceNode" /> that this menu item targets.
    /// </summary>
    public WorkspaceNode Workspace { get; }

    /// <summary>
    ///     Gets the display title of the menu item.
    /// </summary>
    public string Title { get; }

    /// <summary>
    ///     Gets the command that handles the relocation logic.
    /// </summary>
    public ICommand Command { get; }

    /// <summary>
    ///     Gets a value indicating whether the <see cref="Title" /> requires localization.
    ///     Hardcoded to <c>false</c> as workspace names are typically user-defined or dynamic.
    /// </summary>
    public bool Localization => false;

    /// <summary>
    ///     Gets a collection of nested sub-items if this menu item acts as a category.
    ///     Defaults to an empty array.
    /// </summary>
    public MoveWorkSpaceTabItemHeaderContextMenuSubItem[] SubItems => [];

    /// <summary>
    ///     Gets the visual icon for the menu item. Can be overridden in derived classes.
    /// </summary>
    public virtual string Icon => "";
}