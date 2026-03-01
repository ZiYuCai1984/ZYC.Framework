namespace ZYC.Framework.Abstractions.WindowTitle;

/// <summary>
///     Manages a collection of <see cref="IWindowTitleItem" /> instances.
///     Inherits from <see cref="IMenuManager{T}" /> to provide standard menu-like management
///     (e.g., adding, removing, or enumerating items).
/// </summary>
public interface IWindowTitleManager : IMenuManager<IWindowTitleItem>
{
}