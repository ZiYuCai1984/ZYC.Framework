using System.Text;

namespace ZYC.Framework.Core.Converters;

public class DigitsToEmojiConverter : ValueConverterBase<object, string>
{
    public bool ConvertKeycapSymbols { get; set; } = false;

    protected override string InternalConvert(object value)
    {
        var s = value.ToString();
        if (string.IsNullOrWhiteSpace(s))
        {
            return "";
        }

        var sb = new StringBuilder(s.Length * 2);

        foreach (var ch in s)
        {
            if (ch >= '0' && ch <= '9')
            {
                sb.Append(ToKeycap(ch));
            }
            else if (ConvertKeycapSymbols && (ch == '#' || ch == '*'))
            {
                sb.Append(ToKeycap(ch));
            }
            else
            {
                sb.Append(ch);
            }
        }

        return sb.ToString();
    }

    protected override string InternalConvertBack(string value)
    {
        throw new NotSupportedException();
    }

    private static string ToKeycap(char c)
    {
        return string.Concat(c, "\uFE0F\u20E3");
    }
}