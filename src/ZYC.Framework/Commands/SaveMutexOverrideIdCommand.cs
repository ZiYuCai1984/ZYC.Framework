using System.IO;
using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.Notification.Banner;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Commands;

[RegisterSingleInstance]
internal class SaveMutexOverrideIdCommand : CommandBase<string>
{
    public SaveMutexOverrideIdCommand(
        IEventAggregator eventAggregator,
        IBannerManager bannerManager,
        IToastManager toastManager,
        ILogger<SaveMutexOverrideIdCommand> logger)
    {
        EventAggregator = eventAggregator;
        BannerManager = bannerManager;
        ToastManager = toastManager;
        Logger = logger;
    }

    private IEventAggregator EventAggregator { get; }

    private IBannerManager BannerManager { get; }

    private IToastManager ToastManager { get; }

    private ILogger<SaveMutexOverrideIdCommand> Logger { get; }


    protected override void InternalExecute(string parameter)
    {
        base.InternalExecute(parameter);

        var mutexOverrideId = parameter.Trim();
        if (string.IsNullOrWhiteSpace(mutexOverrideId))
        {
            ToastManager.PromptMessage(ToastMessage.Warn("Mutex override is required."));
            return;
        }

        if (!IsValidMutexOverrideId(mutexOverrideId))
        {
            ToastManager.PromptMessage(
                ToastMessage.Warn("Mutex override cannot contain backslash or control characters."));
            return;
        }

        try
        {
            var path = MutexTools.GetMutexOverridePath();
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, mutexOverrideId);

            ToastManager.PromptMessage(ToastMessage.Info("Mutex override saved."));
            BannerManager.PromptRestart();

            EventAggregator.Publish(new MutexOverrideIdChangedEvent());
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }

    private static bool IsValidMutexOverrideId(string value)
    {
        return !value.Contains('\\') && value.All(c => !char.IsControl(c));
    }
}