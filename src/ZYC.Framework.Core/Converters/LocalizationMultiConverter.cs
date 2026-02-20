using ZYC.Framework.Core.Localizations;

namespace ZYC.Framework.Core.Converters;

public class LocalizationMultiConverter : MultiValueConverterBase<object, bool, string>
{
    protected override string Convert(object value, bool localization)
    {
        var s = value.ToString() ?? "";

        if (localization)
        {
            return L.Translate(s);
        }

        return s;
    }
}