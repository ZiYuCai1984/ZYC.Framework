namespace ZYC.Framework.Abstractions.DragDrop;

/// <summary>
///     Represents the state of modifier keys during the operation.
/// </summary>
[Flags]
public enum ModifierKeys
{
    /// <summary>
    ///     None
    /// </summary>
    None = 0,

    /// <summary>
    ///     Alt
    /// </summary>
    Alt = 1 << 0,

    /// <summary>
    /// </summary>
    Ctrl = 1 << 1,

    /// <summary>
    ///     Shift
    /// </summary>
    Shift = 1 << 2,

    /// <summary>
    ///     Win
    /// </summary>
    Win = 1 << 3
}