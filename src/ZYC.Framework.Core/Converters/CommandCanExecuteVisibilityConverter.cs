using System.Windows;
using System.Windows.Input;

namespace ZYC.Framework.Core.Converters;

public class CommandCanExecuteVisibilityConverter : MultiValueConverterBase<ICommand, object?, Visibility>
{
    public Visibility TrueValue { get; set; } = Visibility.Visible;

    public Visibility FalseValue { get; set; } = Visibility.Collapsed;

    protected override Visibility Convert(ICommand command, object? value2)
    {
        var result = command.CanExecute(value2);
        if (result)
        {
            return TrueValue;
        }

        return FalseValue;
    }
}