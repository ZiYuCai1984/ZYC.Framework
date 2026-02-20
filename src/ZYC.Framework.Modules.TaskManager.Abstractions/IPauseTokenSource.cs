namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Provides pause control for tasks.
/// </summary>
public interface IPauseTokenSource
{
    /// <summary>
    ///     Gets the pause token.
    /// </summary>
    IPauseToken Token { get; }

    /// <summary>
    ///     Requests a pause.
    /// </summary>
    void Pause();

    /// <summary>
    ///     Requests a resume.
    /// </summary>
    void Resume();
}