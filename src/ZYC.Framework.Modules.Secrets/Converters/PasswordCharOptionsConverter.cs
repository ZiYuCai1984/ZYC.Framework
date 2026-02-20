using ZYC.CoreToolkit.Abstractions;
using ZYC.Framework.Core.Converters;

namespace ZYC.Framework.Modules.Secrets.Converters;

internal class PasswordCharOptionsConverter : ValueConverterBase<PasswordCharOptions, bool>
{
    // Cache the last source value seen in Convert, used by ConvertBack to add/remove flags.
    private PasswordCharOptions _lastSourceValue;
    public PasswordCharOptions BaseValue { get; set; }

    protected override bool InternalConvert(PasswordCharOptions value)
    {
        _lastSourceValue = value;

        if (BaseValue == PasswordCharOptions.None)
        {
            return false;
        }

        // When BaseValue is a composite flag, require all bits to be present to return true.
        return (value & BaseValue) == BaseValue;
    }

    protected override PasswordCharOptions InternalConvertBack(bool isChecked)
    {
        // Update based on the most recent source value.
        var current = _lastSourceValue;

        if (BaseValue == PasswordCharOptions.None)
        {
            return current;
        }

        if (isChecked)
        {
            // Checked -> add the flag.
            current |= BaseValue;
        }
        else
        {
            // Unchecked -> clear this flag (or composite flags).
            current &= ~BaseValue;
        }

        // Update the cache to avoid "jitter" on rapid toggles.
        _lastSourceValue = current;
        return current;
    }
}