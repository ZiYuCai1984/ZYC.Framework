using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ZYC.Framework.Core;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class NativeMethods
{
    public const int HWND_BROADCAST = 0xffff;

    public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");

    [DllImport("user32")]
    public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

    [DllImport("user32")]
    public static extern int RegisterWindowMessage(string message);


    public const int WM_CLOSE = 0x0010;

    public const int SC_CLOSE = 0xF060;

    public const int WM_SYSCOMMAND = 0x0112;
}