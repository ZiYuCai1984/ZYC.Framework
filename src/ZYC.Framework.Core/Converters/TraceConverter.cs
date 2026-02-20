using System.Diagnostics;
using System.Globalization;

namespace ZYC.Framework.Core.Converters;

internal class TraceConverter : ConverterBase
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        Trace.WriteLine($"{nameof(TraceConverter)} : {value}");
        return value!;
    }
}