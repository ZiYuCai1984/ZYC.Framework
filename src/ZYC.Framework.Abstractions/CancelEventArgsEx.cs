namespace ZYC.Framework.Abstractions;

/// <summary>
///     Provides cancel event data with a handled indicator.
/// </summary>
public class CancelEventArgsEx : EventArgs
{
    private bool _cancel;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CancelEventArgsEx" /> class.
    /// </summary>
    /// <param name="cancel">Whether the operation is canceled.</param>
    /// <param name="handled">Whether the cancel request was handled.</param>
    public CancelEventArgsEx(bool cancel = false, bool handled = false)
    {
        _cancel = cancel;
        Handled = handled;
    }

    /// <summary>
    ///     Gets or sets whether the operation is canceled.
    /// </summary>
    public bool Cancel
    {
        get => _cancel;
        set
        {
            _cancel = value;
            Handled = true;
        }
    }


    /// <summary>
    ///     Gets whether the cancel request was handled.
    /// </summary>
    public bool Handled { get; private set; }
}