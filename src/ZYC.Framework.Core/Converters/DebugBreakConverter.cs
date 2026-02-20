using System.Globalization;
using ZYC.CoreToolkit;

namespace ZYC.Framework.Core.Converters;

internal class DebugBreakConverter : ConverterBase
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        DebuggerTools.Break();

        return base.Convert(value, targetType, parameter, culture);
    }

    public override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        DebuggerTools.Break();

        return base.ConvertBack(value, targetType, parameter, culture);
    }
}