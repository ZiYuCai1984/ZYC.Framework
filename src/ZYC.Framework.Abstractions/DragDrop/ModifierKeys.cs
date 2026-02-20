namespace ZYC.Framework.Abstractions.DragDrop;


[Flags]
public enum ModifierKeys
{
    None = 0,
    Alt = 1 << 0,
    Ctrl = 1 << 1,
    Shift = 1 << 2,
    Win = 1 << 3
}