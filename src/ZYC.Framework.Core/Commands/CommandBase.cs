using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ZYC.Framework.Core.Commands;

public abstract class CommandBase<T> : CommandBase
{
    public override bool CanExecute(object? parameter)
    {
        return InternalCanExecute((T)parameter!);
    }

    public override void Execute(object? parameter)
    {
        try
        {
            IsExecuting = true;
            InternalExecute((T)parameter!);
        }
        finally
        {
            IsExecuting = false;
        }

        RaiseExecuted();
    }

    protected virtual void InternalExecute(T parameter)
    {
    }

    protected virtual bool InternalCanExecute(T parameter)
    {
        return true;
    }
}

public abstract class CommandBase : ICommand, INotifyPropertyChanged
{
    private bool _isExecuting;

    public bool IsExecuting
    {
        get => _isExecuting;
        protected set
        {
            _isExecuting = value;
            RaiseCanExecuteChanged();
            OnPropertyChanged();
        }
    }

    public virtual bool CanExecute(object? parameter)
    {
        return true;
    }

    public virtual void Execute(object? parameter)
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
    }


    public event EventHandler? CanExecuteChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void InternalExecute(object? parameter)
    {
    }

    public event EventHandler? Executed;

    public void RaiseExecuted()
    {
        Executed?.Invoke(this, EventArgs.Empty);
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}