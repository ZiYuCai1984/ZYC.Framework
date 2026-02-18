using System.Globalization;

namespace ZYC.Automation.Core.Converters;

/// <summary>
///     !WARNING Used to create objects in advance to avoid virtualization problems
/// </summary>
public class PackIconMaterialConverter : ConverterBase
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null!;
        }

        var s = value.ToString()!;
        return PackIconMaterialTools.CreateIcon(s);
    }
}