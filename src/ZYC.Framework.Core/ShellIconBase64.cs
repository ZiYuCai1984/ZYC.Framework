using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ZYC.Framework.Core;

public static class ShellIconBase64
{
    // ReSharper disable InconsistentNaming
    private const uint SHGFI_PIDL = 0x000000008;
    private const uint SHGFI_ICON = 0x000000100;
    private const uint SHGFI_LARGEICON = 0x000000000; // large (typically 32x32)

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SHParseDisplayName(
        string pszName, IntPtr pbc, out IntPtr ppidl, uint sfgaoIn, out uint psfgaoOut);

    [DllImport("shell32.dll")]
    private static extern IntPtr SHGetFileInfo(
        IntPtr pidl, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

    [DllImport("ole32.dll")]
    private static extern void CoTaskMemFree(IntPtr pv);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);

    public static string? TryGetFolderIconPngBase64(string parsingName)
    {
        if (string.IsNullOrWhiteSpace(parsingName))
        {
            return null;
        }

        var pidl = IntPtr.Zero;
        var hIcon = IntPtr.Zero;

        try
        {
            var hr = SHParseDisplayName(parsingName, IntPtr.Zero, out pidl, 0, out _);
            if (hr != 0 || pidl == IntPtr.Zero)
            {
                return null;
            }

            _ = SHGetFileInfo(
                pidl, 0, out var sfi, (uint)Marshal.SizeOf<SHFILEINFO>(),
                SHGFI_PIDL | SHGFI_ICON | SHGFI_LARGEICON);

            hIcon = sfi.hIcon;
            if (hIcon == IntPtr.Zero)
            {
                return null;
            }

            // HICON -> PNG bytes -> base64
            var src = Imaging.CreateBitmapSourceFromHIcon(
                hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            src.Freeze();

            var enc = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(src));

            using var ms = new MemoryStream();
            enc.Save(ms);
            return Convert.ToBase64String(ms.ToArray());
        }
        finally
        {
            if (hIcon != IntPtr.Zero)
            {
                DestroyIcon(hIcon);
            }

            if (pidl != IntPtr.Zero)
            {
                CoTaskMemFree(pidl);
            }
        }
    }
    // private const uint SHGFI_SMALLICON = 0x000000001; // small (16x16)

    [StructLayout(LayoutKind.Sequential)]
    private struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }
}