using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Core.WindowTitle;

namespace ZYC.Framework.WindowTitle.BuildIn;

[RegisterSingleInstance]
internal class SetPreventExitTitleItem : WindowTitleItem, INotifyPropertyChanged, IDisposable
{
    public SetPreventExitTitleItem(
        IEventAggregator eventAggregator,
        SetPreventExitCommand setPreventExitCommand,
        DesktopWindowState desktopWindowState) : base(null!, null!)
    {
        SetPreventExitCommand = setPreventExitCommand;
        DesktopWindowState = desktopWindowState;

        eventAggregator.Subscribe<SetPreventExitCommandExecutedEvent>(OnSetPreventExitCommandExecuted)
            .DisposeWith(CompositeDisposable);
    }

    private CompositeDisposable CompositeDisposable { get; } = new();

    private SetPreventExitCommand SetPreventExitCommand { get; }

    private DesktopWindowState DesktopWindowState { get; }

    public override string Icon
    {
        get
        {
            if (DesktopWindowState.IsPreventExit)
            {
                return "LockOutline";
            }

            return "LockOpenOutline";
        }
    }

    public override ICommand Command => SetPreventExitCommand;

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnSetPreventExitCommandExecuted(SetPreventExitCommandExecutedEvent obj)
    {
        OnPropertyChanged(nameof(Icon));
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}