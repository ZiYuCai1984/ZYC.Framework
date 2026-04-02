namespace ZYC.Framework.Abstractions.Workspace;

/// <summary>
///     Defines a specialized manager for workspace-related context menu items.
///     Inherits from <see cref="IMenuManager{T}" /> to provide standard lifecycle
///     and collection management for <see cref="IWorkspaceMenuItem" /> objects.
/// </summary>
public interface IWorkspaceContextMenuManager : IMenuManager<IWorkspaceMenuItem>
{

}