namespace ZYC.Framework.Abstractions.WindowTitle;

/// <summary>
///     Defines a manager responsible for handling a collection of items that extend the window's title bar.
/// </summary>
/// <remarks>
///     This interface inherits from <see cref="IMenuManager{T}" /> to provide standard
///     operations for adding, removing, and organizing title bar extension components.
/// </remarks>
public interface IWindowTitleExtendManager : IMenuManager<IWindowTitleExtendItem>
{
}