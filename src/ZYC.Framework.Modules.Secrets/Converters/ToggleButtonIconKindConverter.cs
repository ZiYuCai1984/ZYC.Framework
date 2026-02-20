using System.Globalization;
using ZYC.Framework.Core.Converters;

namespace ZYC.Framework.Modules.Secrets.Converters;

internal class ToggleButtonIconKindConverter : ConverterBase
{
    public string TrueValue { get; set; } = "LockOpenOutline";
    public string FalseValue { get; set; } = "LockOutline";

    public bool Reverse { get; set; }

    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var result = (bool)value! ^ Reverse;

        if (result)
        {
            return TrueValue;
        }

        return FalseValue;
    }
}