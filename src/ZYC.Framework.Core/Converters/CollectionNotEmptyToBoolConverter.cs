using System.Collections;
using System.Globalization;

namespace ZYC.Framework.Core.Converters;

public class CollectionNotEmptyToBoolConverter : ConverterBase
{
    public bool Reverse { get; set; }

    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var result = false;

        if (value is IEnumerable enumerable)
        {
            // ReSharper disable once GenericEnumeratorNotDisposed
            var enumerator = enumerable.GetEnumerator();
            result = enumerator.MoveNext();
        }

        if (Reverse)
        {
            result = !result;
        }

        return result;
    }
}