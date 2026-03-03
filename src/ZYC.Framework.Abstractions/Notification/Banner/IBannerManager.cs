namespace ZYC.Framework.Abstractions.Notification.Banner;

/// <summary>
///     Defines the contract for managing and displaying banners across the application.
///     Provides high-level methods to trigger specific system notifications.
/// </summary>
public interface IBannerManager
{
    /// <summary>
    ///     Displays a standardized banner prompting the user to restart the application.
    ///     Typically used after configuration changes or plugin updates.
    /// </summary>
    void PromptRestart();

    /// <summary>
    ///     Displays a specific type of banner identified by the type parameter <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The specific type of <see cref="IBanner" /> to be displayed.</typeparam>
    void Prompt<T>() where T : IBanner;
}