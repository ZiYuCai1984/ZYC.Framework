using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Update.Abstractions;

namespace ZYC.Framework.Modules.Update;

[RegisterSingleInstance]
internal class UpdateMainMenuItem : MainMenuItem, INotifyPropertyChanged
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
        });
    }

    private UpdateConfig UpdateConfig { get; }

    public override bool IsHidden => !UpdateConfig.ShowUpdateMenu;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}