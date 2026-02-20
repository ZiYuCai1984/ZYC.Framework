using System.Windows.Input;
using Autofac;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Core.Menu;

public abstract class MainMenuItemsProvider : IMainMenuItemsProvider
{
    private readonly List<IMainMenuItem> _subItems = new();

    protected MainMenuItemsProvider(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    protected ILifetimeScope LifetimeScope { get; }

    public abstract MenuItemInfo Info { get; }

    public ICommand Command => new RelayCommand(
        _ => SubItems.Length != 0, _ =>
        {
            //ignore
        });

    public IMainMenuItem[] SubItems => _subItems.ToArray();

    public virtual string Title => Info.Title;

    public virtual string? Icon => Info.Icon;

    public string Anchor => Info.Anchor;

    public int Priority => Info.Priority;

    public bool Localization => Info.Localization;

    public virtual bool IsHidden { get; set; }

    public void RegisterSubItem<T>() where T : IMainMenuItem
    {
        RegisterSubItem(LifetimeScope.Resolve<T>());
    }

    public void RegisterSubItem(IMainMenuItem item)
    {
        _subItems.Add(item);
    }
}