using System.Runtime.InteropServices;
using System.Windows;

namespace ZYC.Framework.Core;

public static class ClipboardTools
{
    public static bool SetText(string text)
    {
        try
        {
            Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }
        catch (COMException ex1) when (ex1.ErrorCode == -2147221040)
        {
            try
            {
                Clipboard.SetDataObject(text, true);
            }
            catch (COMException ex2) when (ex2.ErrorCode == -2147221040)
            {
                return false;
            }
        }

        return true;
    }
}