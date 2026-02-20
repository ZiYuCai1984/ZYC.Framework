using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Secrets.Abstractions;

// ReSharper disable once InconsistentNaming
/// <summary>
///     Provides dialog extensions for secrets navigation prompts.
/// </summary>
public static class IDialogManagerEx
{
    /// <summary>
    ///     Shows a dialog prompting the user to navigate to the secrets page.
    /// </summary>
    /// <param name="dialogManager">The dialog manager.</param>
    /// <param name="content">The dialog content.</param>
    /// <param name="caption">The dialog caption.</param>
    /// <param name="buttonText">The action button text.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task ShowNavigateToSecretsPageDialogAsync(
        this IDialogManager dialogManager,
        string content,
        string caption = "Prompt",
        string buttonText = "Go to secrets page")
    {
        return dialogManager.ShowMessageDialogAsync(content, caption, buttonText);
    }
}