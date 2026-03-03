namespace ZYC.Framework.Abstractions.Tab;

/// <summary>
///     Defines a manager responsible for registering and maintaining tab item factories.
/// </summary>
public interface ISimpleTabItemFactoryManager
{
    /// <summary>
    ///     Registers a new tab item factory using the provided information.
    /// </summary>
    /// <param name="info">The metadata and configuration for the tab item factory.</param>
    void Register(SimpleTabItemFactoryInfo info);
}