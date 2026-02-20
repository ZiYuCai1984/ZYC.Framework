using ZYC.Framework.Core;
using ZYC.Framework.Modules.Secrets.Abstractions;
using ZYC.Framework.Modules.Settings.Abstractions;

namespace ZYC.Framework.Modules.Secrets.UI;

internal sealed partial class SecretsManagerView : ISecretsManager
{
    public ISecrets[] Secrets { get; } = [];

    private TaskCompletionSource LoadedTaskCompletionSource { get; } = new();

    public ISecrets[] GetSecretsConfigs()
    {
        return Secrets.ToArray();
    }

    public void BringIntoView<T>()
    {
        BringIntoView(typeof(T));
    }

    public void BringIntoView(Type configType)
    {
        Task.Run(async () =>
        {
            await LoadedTaskCompletionSource.Task;
            await Dispatcher.InvokeAsync(() =>
            {
                FilterText = "";
                ItemsControl.BringIntoView((_, item) =>
                {
                    var group = (SettingGroup)item;
                    return group.Type == configType;
                });
            });
        });
    }

    public Uri GetPageUri()
    {
        return SecretsModuleConstants.Uri;
    }
}