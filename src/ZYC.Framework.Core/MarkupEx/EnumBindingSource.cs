using System.Windows.Markup;

namespace ZYC.Framework.Core.MarkupEx;

public class EnumBindingSource : MarkupExtension
{
    private Type? _enumType;

    public EnumBindingSource()
    {
    }

    public EnumBindingSource(Type type)
    {
        _enumType = type;
    }

    public Type EnumType
    {
        get => _enumType!;
        set
        {
            if (value == _enumType)
            {
                return;
            }

            var enumType = Nullable.GetUnderlyingType(value) ?? value;

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type must be for an Enum.");
            }

            _enumType = value;
        }
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (null == _enumType)
        {
            throw new InvalidOperationException("The EnumType must be specified.");
        }

        var actualEnumType = Nullable.GetUnderlyingType(_enumType) ?? _enumType;
        var enumValues = Enum.GetValues(actualEnumType);

        if (actualEnumType == _enumType)
        {
            return enumValues;
        }

        var tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);

        enumValues.CopyTo(tempArray, 1);
        return tempArray;
    }
}