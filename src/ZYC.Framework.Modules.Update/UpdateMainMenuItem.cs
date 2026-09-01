using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Runtime.CompilerServices;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Update.Abstractions;

namespace ZYC.Framework.Modules.Update;

[RegisterSingleInstance]
internal class UpdateMainMenuItem : MainMenuItem, INotifyPropertyChanged, IDisposable
{
    public UpdateMainMenuItem(ILifetimeScope lifetimeScope, UpdateConfig updateConfig)
    {
        UpdateConfig = updateConfig;
        Info = new MenuItemInfo
        {
            Title = UpdateModuleConstants.Title,
            Icon = UpdateModuleConstants.Icon,
            Anchor = AboutMainMenuAnchors.Update
        };

        Command = lifetimeScope.CreateNavigateCommand(UpdateModuleConstants.Uri);

        updateConfig.ObserveProperty(nameof(UpdateConfig.ShowUpdateMenu)).Subscribe(_ =>
        {
            OnPropertyChanged(nameof(IsHidden));
        }).DisposeWith(CompositeDisposable);
    }

    private CompositeDisposable CompositeDisposable { get; } = new();

    private UpdateConfig UpdateConfig { get; }

    public override bool IsHidden => !UpdateConfig.ShowUpdateMenu;

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}