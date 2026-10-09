using System.Windows;

namespace ZYC.Framework.Core.Converters;

public class DoubleToGridLengthConverter : ValueConverterBase<double, GridLength>
{
    protected override GridLength InternalConvert(double value)
    {
        return new GridLength(value);
    }

    protected override double InternalConvertBack(GridLength value)
    {
        return value.Value;
    }
}
