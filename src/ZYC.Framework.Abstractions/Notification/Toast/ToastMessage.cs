namespace ZYC.Framework.Abstractions.Notification.Toast;

public class ToastMessage
{
    public ToastMessage(string text, string icon, bool localization = true)
    {
        Text = text;
        Icon = icon;
        Localization = localization;
    }

    public string Icon { get; }

    public string Text { get; }

    public bool Localization { get; }

    public static ToastMessage Info(string text, bool localization = true)
    {
        return new ToastMessage(text, "InformationOutline", localization);
    }

    public static ToastMessage Warn(string text, bool localization = true)
    {
        return new ToastMessage(text, "AlertOutline", localization);
    }

    public static ToastMessage Exception(Exception ex, bool localization = false)
    {
        return Exception(ex.ToString(), localization);
    }

    public static ToastMessage Exception(string text, bool localization = false)
    {
        return new ToastMessage(text, "BugOutline", localization);
    }
}