using System.Windows.Input;

namespace ZYC.Framework.Core.Converters;

public class CommandCanExecuteConverter : MultiValueConverterBase<ICommand, object?, bool>
{
    public bool Reverse { get; set; }

    protected override bool Convert(ICommand command, object? value2)
    {
        var result = command.CanExecute(value2);
        return Reverse ^ result;
    }
}