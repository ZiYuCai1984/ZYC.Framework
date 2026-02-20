using System.Globalization;
using System.Windows.Data;
using ZYC.CoreToolkit;
using ZYC.Framework.Core.Localizations;

namespace ZYC.Framework.Core.Converters;

public class TranslatorWrapperConverter : IValueConverter
{
    private readonly IValueConverter? _originalConverter;

    public TranslatorWrapperConverter(IValueConverter originalConverter)
    {
        _originalConverter = originalConverter;
    }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (_originalConverter != null)
        {
            value = _originalConverter.Convert(value, targetType, parameter, culture);
        }


        return value is string text ? Translate(text) : Translate(value?.ToString() ?? "");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        //!WARNING TranslatorWrapperConverter.TranslateBack
        DebuggerTools.Break();

        if (value is string text)
        {
            value = TranslateBack(text);
        }

        return _originalConverter != null
            ? _originalConverter.ConvertBack(value, targetType, parameter, culture)
            : value;
    }

    private static string TranslateBack(string text)
    {
        return text;
    }

    private static string Translate(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        return L.Translate(text);
    }
}