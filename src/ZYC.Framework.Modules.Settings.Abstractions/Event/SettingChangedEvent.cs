namespace ZYC.Framework.Modules.Settings.Abstractions.Event;

/// <summary>
///     Represents a setting change event scoped to a module.
/// </summary>
public class SettingChangedEvent<T>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SettingChangedEvent{T}" /> class.
    /// </summary>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    public SettingChangedEvent(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    /// <summary>
    ///     Gets the type of the setting value.
    /// </summary>
    public Type Type => typeof(T);

    /// <summary>
    ///     Gets the previous value.
    /// </summary>
    public T OldValue { get; }

    /// <summary>
    ///     Gets the new value.
    /// </summary>
    public T NewValue { get; }
}

/// <summary>
///     Provides helpers for creating setting change events at runtime.
/// </summary>
public static class SettingChangedEvent
{
    /// <summary>
    ///     Creates a <see cref="SettingChangedEvent{T}" /> instance for the specified type.
    /// </summary>
    /// <param name="type">The setting value type.</param>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    /// <returns>The created event instance.</returns>
    public static object CreateSettingChangedEvent(
        Type type,
        object oldValue,
        object newValue)
    {
        var genericType = typeof(SettingChangedEvent<>).MakeGenericType(type);
        return Activator.CreateInstance(genericType, oldValue, newValue)!;
    }
}