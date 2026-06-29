using ZYC.Framework.Core;

namespace ZYC.Framework;

internal partial class Program
{
    private static Mutex? Mutex { get; set; }

    private static void EnsureSingleInstance(Uri? startupUri)
    {
        //!WARNING Regardless of whether it is a singleton or not, the message should be sent first.
        NativeMethods.PostMessage(
            NativeMethods.HWND_BROADCAST,
            NativeMethods.WM_SHOWME,
            IntPtr.Zero,
            IntPtr.Zero);

        var mutexId = MutexTools.GetMutexId();
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


        SendStartupUriToCurrentInstance(startupUri);
        AppContext.FocusExitProcess();
    }

    private static void SendStartupUriToCurrentInstance(Uri? startupUri)
    {
        if (startupUri == null)
        {
            return;
        }

        var pipeName = GetStartupUriPipeName();
        if (string.IsNullOrWhiteSpace(pipeName))
        {
            return;
        }

        StartupUriPipeClient.TrySend(pipeName, startupUri);
    }

    private static string GetStartupUriPipeName()
    {
        return MutexTools.GetMutexId();
    }
}