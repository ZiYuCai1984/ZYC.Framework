using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Modules.Mock.Abstractions;

/// <summary>
///     Creates mock tab items for the mock module.
/// </summary>
public interface IMockTabItemFactory : ITabItemFactory
{
    /// <summary>
    ///     Registers a mock tab item so it can be created later.
    /// </summary>
    /// <param name="info">Metadata describing the mock tab item.</param>
    void RegisterMockTabItem(MockTabItemInfo info);
}