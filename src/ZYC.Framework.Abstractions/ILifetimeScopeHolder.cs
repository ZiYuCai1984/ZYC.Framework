namespace ZYC.Framework.Abstractions;

/// <summary>
///     Provides access to a dependency injection lifetime scope.
/// </summary>
public interface ILifetimeScopeHolder
{
    /// <summary>
    ///     Gets the lifetime scope instance.
    /// </summary>
    /// <returns>The lifetime scope instance.</returns>
    object GetLifetimeScope();
}