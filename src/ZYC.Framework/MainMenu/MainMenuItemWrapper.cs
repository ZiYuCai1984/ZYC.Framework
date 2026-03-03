using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ZYC.CoreToolkit;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;

namespace ZYC.Framework.MainMenu;

internal class MainMenuItemWrapper : IMainMenuItem, INotifyPropertyChanged, IDisposable
{
    private readonly IMainMenuItem _original;

    public MainMenuItemWrapper(IMainMenuItem original, IMainMenuItem[] sortedSubItems)
    {
        _original = original;
        SubItems = sortedSubItems;

        if (original is INotifyPropertyChanged e)
        {
            e.ObserveProperty(
                    nameof(IMainMenuItem.Title))
                .Subscribe(_ => OnPropertyChanged(nameof(Title)))
                .DisposeWith(CompositeDisposable);

            e.ObserveProperty(
                    nameof(IMainMenuItem.Icon))
                .Subscribe(_ => OnPropertyChanged(nameof(Icon)))
                .DisposeWith(CompositeDisposable);

            e.ObserveProperty(
                    nameof(IMainMenuItem.IsHidden))
                .Subscribe(_ => OnPropertyChanged(nameof(IsHidden)))
                .DisposeWith(CompositeDisposable);

            e.ObserveProperty(
                    nameof(IMainMenuItem.SubItems))
                .Subscribe(_ =>
                {
                    var ori = SubItems;

                    //!WARNING When an external entity actively adjusts its own SubItem, the external entity should take precedence.
                    //bug All the child items below will lose their MainMenuItemWrapper.
                    SubItems = original.SubItems;
                    OnPropertyChanged(nameof(SubItems));

                    foreach (var item in ori)
                    {
                        item.TryDispose();
                    }

                })
                .DisposeWith(CompositeDisposable);
        }
    }

    public CompositeDisposable CompositeDisposable { get; } = new();

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    public ICommand Command => _original.Command;

    public string Title => _original.Title;

    public string? Icon => _original.Icon;

    public string Anchor => _original.Anchor;

    public int Priority => _original.Priority;

    public bool Localization => _original.Localization;

    public bool IsHidden => _original.IsHidden;

    public IMainMenuItem[] SubItems { get; private set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}