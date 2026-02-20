using System.Globalization;

namespace ZYC.Framework.Core.Converters;

public class HybridIconConverter : ConverterBase
{
    public int Width { get; set; } = 16;

    public int Height { get; set; } = 16;

    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var s = value?.ToString() ?? "";
        return new HybridIcon
        {
            Width = Width,
            Height = Height,
            Icon = s
        };
    }
}