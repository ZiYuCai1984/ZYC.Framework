namespace ZYC.Automation.Abstractions.Tab;

/// <summary>
///     Represents a unique reference to a tab, identified by a persistent ID and an associated URI.
/// </summary>
public class TabReference : IEquatable<TabReference>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TabReference" /> class with a specific URI.
    /// </summary>
    /// <param name="uri">The initial address of the tab.</param>
    public TabReference(Uri uri)
    {
        Uri = uri;
    }

    /// <summary>
    ///     Gets the unique identifier for this tab reference.
    ///     This ID remains constant even if the URI changes.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the current URI of the tab.
    /// </summary>
    public Uri Uri { get; protected set; }

    /// <summary>
    ///     Gets the timestamp when this tab reference was first created.
    /// </summary>
    public DateTimeOffset CreateTime { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    ///     Determines whether the specified <see cref="TabReference" /> is equal to the current instance based on the
    ///     <see cref="Id" />.
    /// </summary>
    /// <param name="other">The other tab reference to compare.</param>
    /// <returns>True if the IDs match; otherwise, false.</returns>
    public bool Equals(TabReference? other)
    {
        return other is not null && (ReferenceEquals(this, other) || Id == other.Id);
    }

    /// <summary>
    ///     Determines whether the specified object is a <see cref="TabReference" /> and has the same <see cref="Id" />.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return Equals(obj as TabReference);
    }

    /// <summary>
    ///     Serves as the default hash function, using the <see cref="Id" /> to generate the hash code.
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    ///     Compares two <see cref="TabReference" /> instances for equality.
    /// </summary>
    public static bool operator ==(TabReference? left, TabReference? right)
    {
        return EqualityComparer<TabReference>.Default.Equals(left, right);
    }

    /// <summary>
    ///     Compares two <see cref="TabReference" /> instances for inequality.
    /// </summary>
    public static bool operator !=(TabReference? left, TabReference? right)
    {
        return !(left == right);
    }

    /// <summary>
    ///     Returns a string representation of the tab reference containing its ID and URI.
    /// </summary>
    public override string ToString()
    {
        return $"{Id} {Uri}";
    }
}

/// <summary>
///     Provides extension methods for managing collections of <see cref="TabReference" /> objects.
/// </summary>
public static class TabReferenceEx
{
    /// <param name="references">The source array.</param>
    extension(TabReference[] references)
    {
        /// <summary>
        ///     Returns a new array with the specified <see cref="TabReference" /> removed.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>A new array containing the remaining references.</returns>
        public TabReference[] RemoveReference(TabReference item)
        {
            var list = new List<TabReference>(references);
            list.Remove(item);
            return list.ToArray();
        }

        /// <summary>
        ///     Returns a new array with a new <see cref="TabReference" /> appended to the end.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>A new array including the added reference.</returns>
        public TabReference[] AddReference(TabReference item)
        {
            var list = new List<TabReference>(references) { item };
            return list.ToArray();
        }
    }
}