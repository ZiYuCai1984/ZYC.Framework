using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZYC.Framework.Modules.Settings.Abstractions;

/// <summary>
///     Represents a single editable setting item.
/// </summary>
public sealed class SettingItem : INotifyPropertyChanged
{
    private object? _value;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SettingItem" /> class.
    /// </summary>
    /// <param name="name">The setting name.</param>
    /// <param name="value">The current value.</param>
    /// <param name="callbackAction">The callback invoked when the value changes.</param>
    /// <param name="description">The optional description.</param>
    public SettingItem(
        string name,
        object? value,
        Action<SettingItem, object?> callbackAction,
        string? description)
    {
        Name = name;
        Value = value;
        ValueChangedCallbackAction = callbackAction;
        Description = description;
    }

    /// <summary>
    ///     Gets the setting name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the setting description.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    ///     Gets or sets the setting value.
    /// </summary>
    public object? Value
    {
        get => _value;
        set
        {
            _value = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    ///     Gets the callback invoked when the value changes.
    /// </summary>
    public Action<SettingItem, object?> ValueChangedCallbackAction { get; }

    /// <summary>
    ///     Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    ///     Raises the <see cref="PropertyChanged" /> event.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}