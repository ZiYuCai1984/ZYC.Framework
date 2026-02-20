using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZYC.Framework.Core.Converters;

public class ConverterBase : MarkupExtension, IValueConverter
{
    public virtual object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }

    public virtual object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}