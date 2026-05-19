using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ZYC.Framework.Core;

public static class UriSchemeAssociationTools
{
    private const string ClassesRoot = @"Software\Classes";

    public static void Associate(string scheme, string appName, string executablePath)
    {
        ValidateScheme(scheme);

        if (string.IsNullOrWhiteSpace(executablePath))
        {
            throw new InvalidOperationException("Executable path is empty.");
        }

        var schemeKeyPath = $@"{ClassesRoot}\{scheme}";

        using var schemeKey = Registry.CurrentUser.CreateSubKey(schemeKeyPath, true)
                              ?? throw new InvalidOperationException(
                                  $"Failed to create registry key: HKCU\\{schemeKeyPath}");

        schemeKey.SetValue("", $"URL:{appName} Protocol", RegistryValueKind.String);
        schemeKey.SetValue("URL Protocol", "", RegistryValueKind.String);

        using var iconKey = schemeKey.CreateSubKey("DefaultIcon", true)
                            ?? throw new InvalidOperationException("Failed to create DefaultIcon key.");

        iconKey.SetValue("", $"\"{executablePath}\",0", RegistryValueKind.String);

        using var commandKey = schemeKey.CreateSubKey(@"shell\open\command", true)
                               ?? throw new InvalidOperationException("Failed to create command key.");

        commandKey.SetValue("", $"\"{executablePath}\" \"%1\"", RegistryValueKind.String);

        NotifyShellAssociationChanged();
    }

    public static void Remove(string scheme)
    {
        ValidateScheme(scheme);

        var schemeKeyPath = $@"{ClassesRoot}\{scheme}";

        Registry.CurrentUser.DeleteSubKeyTree(schemeKeyPath, false);

        NotifyShellAssociationChanged();
    }

    private static void ValidateScheme(string scheme)
    {
        if (string.IsNullOrWhiteSpace(scheme))
        {
            throw new ArgumentException("URI scheme is empty.", nameof(scheme));
        }

        if (scheme.Contains("://"))
        {
            throw new ArgumentException("URI scheme should not contain ://.", nameof(scheme));
        }

        if (!Uri.CheckSchemeName(scheme))
        {
            throw new ArgumentException($"Invalid URI scheme: {scheme}", nameof(scheme));
        }
    }

    private static void NotifyShellAssociationChanged()
    {
        if (OperatingSystem.IsWindows())
        {
            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
        }
    }

    // ReSharper disable InconsistentNaming
    private const int SHCNE_ASSOCCHANGED = 0x08000000;
    private const uint SHCNF_IDLIST = 0x0000;

    [DllImport("shell32.dll")]
    private static extern void SHChangeNotify(
        int wEventId,
        uint uFlags,
        IntPtr dwItem1,
        IntPtr dwItem2);
}