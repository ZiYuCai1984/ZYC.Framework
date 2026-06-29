using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
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
internal class DeleteMutexOverrideIdCommand : CommandBase, IDisposable
{
    public DeleteMutexOverrideIdCommand(
        IEventAggregator eventAggregator,
        ILogger<DeleteMutexOverrideIdCommand> logger,
        IToastManager toastManager,
        IBannerManager bannerManager)
    {
        EventAggregator = eventAggregator;
        Logger = logger;
        ToastManager = toastManager;
        BannerManager = bannerManager;

        EventAggregator
            .Observe<MutexOverrideIdChangedEvent>()
            .Throttle(TimeSpan.FromMilliseconds(200))
            .Distinct()
            .ObserveOnUI()
            .Subscribe(_ =>
            {
                RaiseCanExecuteChanged();
            }).DisposeWith(CompositeDisposable);
    }

    private CompositeDisposable CompositeDisposable { get; } = new();

    private IEventAggregator EventAggregator { get; }

    private ILogger<DeleteMutexOverrideIdCommand> Logger { get; }

    private IToastManager ToastManager { get; }

    private IBannerManager BannerManager { get; }

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    protected override void InternalExecute(object? parameter)
    {
        base.InternalExecute(parameter);

        try
        {
            var path = MutexTools.GetMutexOverridePath();
            if (!File.Exists(path))
            {
                ToastManager.PromptMessage(ToastMessage.Info("Mutex override does not exist."));
                return;
            }

            File.Delete(path);

            ToastManager.PromptMessage(ToastMessage.Info("Mutex override removed."));
            BannerManager.PromptRestart();

            EventAggregator.Publish(new MutexOverrideIdChangedEvent());
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }


    public override bool CanExecute(object? parameter)
    {
        var path = MutexTools.GetMutexOverridePath();
        return File.Exists(path);
    }
}