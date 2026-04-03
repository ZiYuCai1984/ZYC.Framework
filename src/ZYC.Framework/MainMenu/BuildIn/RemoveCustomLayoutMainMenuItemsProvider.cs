using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Runtime.CompilerServices;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core;
using ZYC.Framework.Core.Menu;

namespace ZYC.Framework.MainMenu.BuildIn;

[RegisterSingleInstance]
internal class RemoveCustomLayoutMainMenuItemsProvider : MainMenuItemsProvider, INotifyPropertyChanged
{
    private RemoveCustomLayoutMainMenuItem[] _subItems = [];

    public RemoveCustomLayoutMainMenuItemsProvider(
        ILifetimeScope lifetimeScope,
        CustomWorkspaceLayoutConfig customWorkspaceLayoutConfig) : base(lifetimeScope)
    {
        CustomWorkspaceLayoutConfig = customWorkspaceLayoutConfig;

        Info = new MenuItemInfo
        {
            Title = "Remove Layout"
        };

        RefreshSubItems();

        customWorkspaceLayoutConfig.ObserveProperty(nameof(CustomWorkspaceLayoutConfig.Layouts))
            .Subscribe(_ => RefreshSubItems())
            .DisposeWith(CompositeDisposable);
    }

    public override RemoveCustomLayoutMainMenuItem[] SubItems => _subItems;

    private CustomWorkspaceLayoutConfig CustomWorkspaceLayoutConfig { get; }

    private CompositeDisposable CompositeDisposable { get; } = new();

    public override MenuItemInfo Info { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void RefreshSubItems()
    {
        var layouts = CustomWorkspaceLayoutConfig.Layouts.ToArray();

        _subItems = layouts
            .Select(layout =>
                LifetimeScope.Resolve<RemoveCustomLayoutMainMenuItem>(
                    new TypedParameter(typeof(CustomWorkspaceLayout), layout)))
            .ToArray();

        OnPropertyChanged(nameof(SubItems));
    }
}
