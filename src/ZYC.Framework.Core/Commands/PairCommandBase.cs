using System.Windows.Input;
using Autofac;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Core.Commands;

public abstract class PairCommandBase<T1, T2> : CommandBase, IPairCommand
    where T1 : PairCommandBase<T1, T2>
    where T2 : PairCommandBase<T1, T2>
{
    private T1? _command1;

    private T2? _command2;

    protected PairCommandBase(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    protected ILifetimeScope LifetimeScope { get; }

    public T1 Command1 => _command1 ??= LifetimeScope.Resolve<T1>();

    public T2 Command2 => _command2 ??= LifetimeScope.Resolve<T2>();

    public ICommand Self => this;

    public void RaisePairCommandsChanged()
    {
        Command1.OnPropertyChanged(nameof(Self));
        Command1.RaiseCanExecuteChanged();

        Command2.OnPropertyChanged(nameof(Self));
        Command2.RaiseCanExecuteChanged();
    }

    public override void Execute(object? parameter)
    {
        try
        {
            IsExecuting = true;
            InternalExecute(parameter);
        }
        finally
        {
            IsExecuting = false;
        }

        RaiseExecuted();
        RaisePairCommandsChanged();
    }
}

public abstract class PairCommandBase<T1, T2, T3> : CommandBase, IPairCommand
    where T1 : PairCommandBase<T1, T2, T3>
    where T2 : PairCommandBase<T1, T2, T3>
    where T3 : PairCommandBase<T1, T2, T3>
{
    private T1? _command1;

    private T2? _command2;

    private T3? _command3;

    protected PairCommandBase(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    protected ILifetimeScope LifetimeScope { get; }

    public T1 Command1 => _command1 ??= LifetimeScope.Resolve<T1>();

    public T2 Command2 => _command2 ??= LifetimeScope.Resolve<T2>();

    public T3 Command3 => _command3 ??= LifetimeScope.Resolve<T3>();

    public ICommand Self => this;

    public void RaisePairCommandsChanged()
    {
        Command1.OnPropertyChanged(nameof(Self));
        Command1.RaiseCanExecuteChanged();

        Command2.OnPropertyChanged(nameof(Self));
        Command2.RaiseCanExecuteChanged();

        Command3.OnPropertyChanged(nameof(Self));
        Command3.RaiseCanExecuteChanged();
    }

    public override void Execute(object? parameter)
    {
        try
        {
            IsExecuting = true;
            InternalExecute(parameter);
        }
        finally
        {
            IsExecuting = false;
        }

        RaiseExecuted();
        RaisePairCommandsChanged();
    }
}