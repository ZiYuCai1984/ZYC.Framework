using System.Globalization;

namespace ZYC.Framework.Core.Converters;

public class EnumToItemSourceConverter : ConverterBase
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var enumType = value?.GetType();
        if (enumType == null || !enumType.IsEnum)
        {
            throw new ArgumentException("Value must be an Enum type");
        }

        return Enum.GetValues(enumType).Cast<object>().ToList();
    }
}