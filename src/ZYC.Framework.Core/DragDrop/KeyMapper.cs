using ZYC.Framework.Abstractions.DragDrop;

namespace ZYC.Framework.Core.DragDrop;

public static class KeyMapper
{
    public static ModifierKeys Map(System.Windows.Input.ModifierKeys keys)
    {
        var r = ModifierKeys.None;
        if (keys.HasFlag(System.Windows.Input.ModifierKeys.Alt))
        {
            r |= ModifierKeys.Alt;
        }

        if (keys.HasFlag(System.Windows.Input.ModifierKeys.Control))
        {
            r |= ModifierKeys.Ctrl;
        }

        if (keys.HasFlag(System.Windows.Input.ModifierKeys.Shift))
        {
            r |= ModifierKeys.Shift;
        }

        if (keys.HasFlag(System.Windows.Input.ModifierKeys.Windows))
        {
            r |= ModifierKeys.Win;
        }

        return r;
    }
}