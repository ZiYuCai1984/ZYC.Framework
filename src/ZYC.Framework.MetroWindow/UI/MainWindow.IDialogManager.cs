using MahApps.Metro.Controls.Dialogs;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.MetroWindow.UI;

internal partial class MainWindow : IDialogManager
{
    public async Task<string> ShowInputDialogAsync(
        string content,
        string caption,
        bool localization = true)
    {
        var result = await this.ShowInputAsync(caption, content);
        return result ?? "";
    }

    public async Task ShowMessageDialogAsync(
        string content,
        string caption,
        string buttonText,
        bool localization = true)
    {
        await this.ShowMessageAsync(caption,
            content,
            MessageDialogStyle.Affirmative,
            new MetroDialogSettings
            {
                AffirmativeButtonText = buttonText
            });
    }
}