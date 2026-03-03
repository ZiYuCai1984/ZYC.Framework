namespace ZYC.Framework.Abstractions.Notification.Toast;

/// <summary>
///     Represents a message model used for displaying toast notifications in the UI.
/// </summary>
public class ToastMessage
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ToastMessage" /> class.
    /// </summary>
    /// <param name="text">The message content to display.</param>
    /// <param name="icon">The identifier or path for the icon associated with the toast.</param>
    /// <param name="localization">Specifies whether the text should be passed through a localization provider.</param>
    public ToastMessage(string text, string icon, bool localization = true)
    {
        Text = text;
        Icon = icon;
        Localization = localization;
    }

    /// <summary>
    ///     Gets the icon identifier for the toast message.
    /// </summary>
    public string Icon { get; }

    /// <summary>
    ///     Gets the main text content of the toast message.
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///     Gets a value indicating whether the message text requires localization.
    /// </summary>
    public bool Localization { get; }

    /// <summary>
    ///     Creates a new informational toast message.
    /// </summary>
    /// <param name="text">The information message text.</param>
    /// <param name="localization">Whether to localize the text (defaults to true).</param>
    /// <returns>A configured <see cref="ToastMessage" /> with an information icon.</returns>
    public static ToastMessage Info(string text, bool localization = true)
    {
        return new ToastMessage(text, "InformationOutline", localization);
    }

    /// <summary>
    ///     Creates a new warning toast message.
    /// </summary>
    /// <param name="text">The warning message text.</param>
    /// <param name="localization">Whether to localize the text (defaults to true).</param>
    /// <returns>A configured <see cref="ToastMessage" /> with an alert icon.</returns>
    public static ToastMessage Warn(string text, bool localization = true)
    {
        return new ToastMessage(text, "AlertOutline", localization);
    }

    /// <summary>
    ///     Creates a new exception toast message from an <see cref="System.Exception" /> object.
    /// </summary>
    /// <param name="ex">The exception to extract the message from.</param>
    /// <param name="localization">Whether to localize the output (defaults to false for stack traces).</param>
    /// <returns>A configured <see cref="ToastMessage" /> containing exception details.</returns>
    public static ToastMessage Exception(Exception ex, bool localization = false)
    {
        return Exception(ex.ToString(), localization);
    }

    /// <summary>
    ///     Creates a new exception toast message from a string.
    /// </summary>
    /// <param name="text">The error description or stack trace.</param>
    /// <param name="localization">Whether to localize the text (defaults to false).</param>
    /// <returns>A configured <see cref="ToastMessage" /> with a bug/error icon.</returns>
    public static ToastMessage Exception(string text, bool localization = false)
    {
        return new ToastMessage(text, "BugOutline", localization);
    }
}