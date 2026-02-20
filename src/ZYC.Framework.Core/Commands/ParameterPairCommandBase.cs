using System.Windows.Input;
using Autofac;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Core.Commands;

public abstract class ParameterPairCommandBase<T1, T2, TParameter> : CommandBase<TParameter>, IPairCommand
    where T1 : ParameterPairCommandBase<T1, T2, TParameter>
    where T2 : ParameterPairCommandBase<T1, T2, TParameter>
{
    private T1? _command1;

    private T2? _command2;

    protected ParameterPairCommandBase(ILifetimeScope lifetimeScope)
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
            InternalExecute((TParameter)parameter!);
        }
        finally
        {
            IsExecuting = false;
        }

        RaiseExecuted();
        RaisePairCommandsChanged();
    }
}

public abstract class ParameterPairCommandBase<T1, T2, T3, TParameter> : CommandBase, IPairCommand
    where T1 : ParameterPairCommandBase<T1, T2, T3, TParameter>
    where T2 : ParameterPairCommandBase<T1, T2, T3, TParameter>
    where T3 : ParameterPairCommandBase<T1, T2, T3, TParameter>
{
    private T1? _command1;

    private T2? _command2;

    private T3? _command3;

    protected ParameterPairCommandBase(ILifetimeScope lifetimeScope)
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
            InternalExecute((TParameter)parameter!);
        }
        finally
        {
            IsExecuting = false;
        }

        RaiseExecuted();
        RaisePairCommandsChanged();
    }
}