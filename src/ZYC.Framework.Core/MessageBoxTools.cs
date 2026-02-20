using System.Windows;
using ZYC.Framework.Core.Localizations;

namespace ZYC.Framework.Core;

public static class MessageBoxTools
{
    public static bool Confirm(string text, string caption = "Confirm", bool localization = true)
    {
        if (localization)
        {
            text = L.Translate(text);
            caption = L.Translate(caption);
        }

        var result = MessageBox.Show(
            text,
            caption, MessageBoxButton.OKCancel);
        if (result == MessageBoxResult.OK)
        {
            return true;
        }

        return false;
    }

    public static void Warning(string text, string caption = "Warning", bool localization = true)
    {
        if (localization)
        {
            text = L.Translate(text);
            caption = L.Translate(caption);
        }

        MessageBox.Show(
            text,
            caption, MessageBoxButton.OK);
    }

    public static void Error(string text, string caption = "Error", bool localization = true)
    {
        if (localization)
        {
            text = L.Translate(text);
            caption = L.Translate(caption);
        }

        MessageBox.Show(
            text,
            caption, MessageBoxButton.OK);
    }

    public static void Error(Exception exception, string caption = "Error")
    {
        MessageBox.Show(
            exception.ToString(),
            caption, MessageBoxButton.OK);
    }

    public static void NotImplemented()
    {
        Error(new NotImplementedException());
    }
}