using Emoji.Wpf;

namespace ZYC.Framework.Core;

public static class EmojiWpfTools
{
    public static void FocusAndSelectAll(this TextBox textBox)
    {
        if (textBox.Template.FindName(
                "RichTextBox_INTERNAL", textBox)
            is RichTextBox editor)
        {
            editor.Focus();
            editor.SelectAll();
        }
    }

    public static void FocusZ(this TextBox textBox)
    {
        if (textBox.Template.FindName(
                "RichTextBox_INTERNAL", textBox)
            is RichTextBox editor)
        {
            editor.Focus();
        }
    }
}