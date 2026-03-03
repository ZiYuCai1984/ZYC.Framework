namespace ZYC.Framework.Abstractions.BusyWindow;

/// <summary>
///     Represents a controller for an active busy window.
///     Inherits <see cref="IDisposable" /> to allow the 'using' pattern for automatic closing.
/// </summary>
public interface IBusyWindowHandler : IDisposable
{
    /// <summary>
    ///     Gets or sets the descriptive text indicating the current sub-task or status (e.g., "Downloading files...").
    /// </summary>
    string StatusText { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether a progress bar should be visible.
    /// </summary>
    bool ShowProgress { get; set; }

    /// <summary>
    ///     Gets or sets whether the progress bar is indeterminate (e.g., a continuous loading animation
    ///     instead of a specific percentage).
    /// </summary>
    bool IsProgressIndeterminate { get; set; }

    /// <summary>
    ///     Gets or sets the progress percentage value, typically ranging from 0 to 100.
    /// </summary>
    double ProgressValue { get; set; }

    /// <summary>
    ///     Gets or sets the main title of the busy window.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    ///     Explicitly closes the busy window and releases associated resources.
    /// </summary>
    void Close();
}