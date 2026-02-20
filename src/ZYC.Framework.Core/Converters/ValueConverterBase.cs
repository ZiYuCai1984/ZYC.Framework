using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZYC.Framework.Core.Converters;

public abstract class ValueConverterBase<T, TResult> : MarkupExtension, IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return InternalConvert((T)value!)!;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return InternalConvertBack((TResult)value!)!;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }

    protected abstract TResult InternalConvert(T value);

    protected abstract T InternalConvertBack(TResult value);
}