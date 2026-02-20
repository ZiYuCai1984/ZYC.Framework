using System.Windows.Input;

namespace ZYC.Framework.Modules.Update.Abstractions.Commands;

public interface ICheckUpdateCommand : ICommand
{
    void Execute()
    {
        Execute(null);
    }
}