using ZYC.CoreToolkit;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core;

namespace ZYC.Framework;

internal partial class Program
{
    private static Mutex? Mutex { get; set; }

    private static void EnsureSingleInstance()
    {
        //!WARNING Regardless of whether it is a singleton or not, the message should be sent first.
        NativeMethods.PostMessage(
            NativeMethods.HWND_BROADCAST,
            NativeMethods.WM_SHOWME,
            IntPtr.Zero,
            IntPtr.Zero);

        var mutexId = GetMutexId();
        if (string.IsNullOrEmpty(mutexId))
        {
            return;
        }

        Mutex = new Mutex(true, mutexId);
        try
        {
            if (Mutex.WaitOne(TimeSpan.FromSeconds(3), false))
            {
                return;
            }
        }
        catch (AbandonedMutexException)
        {
            //!WARNING Do not release Mutex manually, this exception will be triggered here.
            return;
        }
        catch
        {
            return;
        }


        AppContext.FocusExitProcess();
    }

    private static string GetMutexId()
    {
        var userName = "";

        try
        {
            userName = AccountTools.GetCurrentUserName();
        }
        catch
        {
            //ignore
        }

        //!WARNING Dealing with multi-user
        return $"{userName}-{ProductInfo.PackageId}";
    }
}