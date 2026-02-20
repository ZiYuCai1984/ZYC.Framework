namespace ZYC.Framework.Modules.Settings.Abstractions;

/// <summary>
///     Wraps multi-line text values for settings display and comparison.
/// </summary>
public class MultilineText : IEquatable<MultilineText>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MultilineText" /> class.
    /// </summary>
    /// <param name="text">The text value.</param>
    public MultilineText(string text)
    {
        Text = text;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultilineText" /> class.
    /// </summary>
    /// <param name="value">The value to convert to text.</param>
    public MultilineText(object? value) : this(value?.ToString() ?? "")
    {
    }

    /// <summary>
    ///     Gets the text value.
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///     Determines whether the specified <see cref="MultilineText" /> is equal to the current instance.
    /// </summary>
    /// <param name="other">The other instance to compare.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(MultilineText? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Text == other.Text;
    }

    /// <summary>
    ///     Returns the string representation of the text.
    /// </summary>
    /// <returns>The text value.</returns>
    public override string ToString()
    {
        return Text;
    }

    /// <summary>
    ///     Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((MultilineText)obj);
    }

    /// <summary>
    ///     Returns a hash code for the text.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return Text.GetHashCode();
    }
}