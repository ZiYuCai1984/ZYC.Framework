using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZYC.Framework.Core.Converters;

public abstract class MultiValueConverterBase<TResult> : MarkupExtension, IMultiValueConverter
{
    protected abstract IReadOnlyList<Type> ValueTypes { get; }

    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != ValueTypes.Count)
        {
            if (DesignMode.IsInDesignMode)
            {
                return null;
            }

            throw new ArgumentException(
                $"invalid value count, {ValueTypes.Count} expected",
                nameof(values));
        }

        var values1 = new object?[ValueTypes.Count];
        for (var index = 0; index < ValueTypes.Count; ++index)
        {
            var o = values[index];
            var valueType = ValueTypes[index];
            if (!valueType.IsInstanceOfType(o))
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (o == null || o == DependencyProperty.UnsetValue)
                {
                    values1[index] = valueType.IsValueType ? Activator.CreateInstance(valueType) : null;
                }
                else
                {
                    if (DesignMode.IsInDesignMode)
                    {
                        return null;
                    }

                    throw new ArgumentException(
                        $"{nameof(values)}[{index}] is not an instance of '{valueType}'",
                        nameof(values));
                }
            }
            else
            {
                values1[index] = values[index];
            }
        }

        return Convert(values1!);
    }

    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    protected abstract TResult Convert(object[] values);

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}

public abstract class MultiValueConverterBase<T1, T2, TResult> : MultiValueConverterBase<TResult>
{
    protected sealed override IReadOnlyList<Type> ValueTypes
    {
        get
        {
            return new[]
            {
                typeof(T1),
                typeof(T2)
            };
        }
    }

    protected sealed override TResult Convert(object[] values)
    {
        return Convert((T1)values[0], (T2)values[1]);
    }

    protected abstract TResult Convert(T1 value1, T2 value2);
}

public abstract class MultiValueConverterBase<T1, T2, T3, TResult> : MultiValueConverterBase<TResult>
{
    protected sealed override IReadOnlyList<Type> ValueTypes
    {
        get
        {
            return new[]
            {
                typeof(T1),
                typeof(T2),
                typeof(T3)
            };
        }
    }

    protected sealed override TResult Convert(object[] values)
    {
        return Convert((T1)values[0], (T2)values[1], (T3)values[2]);
    }

    protected abstract TResult Convert(T1 value1, T2 value2, T3 value3);
}