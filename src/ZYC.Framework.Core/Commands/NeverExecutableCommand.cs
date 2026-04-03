using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class NeverExecutableCommand : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return false;
    }

    public void Execute(object? parameter)
    {
        //Ignore
    }

    public event EventHandler? CanExecuteChanged;
}