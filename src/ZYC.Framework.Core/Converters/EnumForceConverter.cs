using System.Windows.Data;

namespace ZYC.Framework.Core.Converters;

public class EnumForceConverter : MultiValueConverterBase<object, Type, object>
{
    /// <summary>
    ///     Whether matching by name is allowed (default true).
    /// </summary>
    public bool MatchByName { get; set; } = true;

    /// <summary>
    ///     Whether to throw when matching fails (default false).
    /// </summary>
    public bool ThrowOnFail { get; set; } = true;

    protected override object Convert(object value, Type targetEnumType)
    {
        var sourceEnum = value as Enum;
        if (sourceEnum == null)
        {
            return Binding.DoNothing;
        }

        try
        {
            if (MatchByName)
            {
                var name = Enum.GetName(sourceEnum.GetType(), sourceEnum);
                if (Enum.TryParse(targetEnumType, name, true, out var result))
                {
                    return result;
                }
            }

            // Force conversion by underlying value.
            var underlying = System.Convert.ChangeType(
                sourceEnum,
                Enum.GetUnderlyingType(sourceEnum.GetType()));
            return Enum.ToObject(targetEnumType, underlying);
        }
        catch
        {
            if (ThrowOnFail)
            {
                throw;
            }

            return Binding.DoNothing;
        }
    }
}