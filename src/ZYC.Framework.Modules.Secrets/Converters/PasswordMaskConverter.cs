using System.Globalization;
using ZYC.Framework.Core.Converters;

namespace ZYC.Framework.Modules.Secrets.Converters;

internal class PasswordMaskConverter : ConverterBase
{
    public string PasswordMask { get; set; } = string.Empty;

    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var s = value?.ToString() ?? "";

        var mask = PasswordMask;
        if (string.IsNullOrWhiteSpace(mask))
        {
            mask = new string('*', s.Length);
        }

        return mask;
    }
}