using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Security;
using Windows.Win32.System.Threading;

namespace EasyWindowsTerminalControl.Internals;

public interface IProcess : IDisposable
{
    bool HasExited { get; }
    void WaitForExit();
    void Kill(bool EntireProcessTree = false);
}

public interface IProcessFactory
{
    IProcess Start(string command, nuint attributes, PseudoConsole console);
}

/// <summary>
///     Support for starting and configuring processes.
/// </summary>
public static class ProcessFactory
{
    /// <summary>
    ///     Start and configure a process. The return value represents the process and should be disposed.
    /// </summary>
    public static WrappedProcess Start(string command, nuint attributes, PseudoConsole console)
    {
        var startupInfo = ConfigureProcessThread(console.Handle, attributes);
        var processInfo = RunProcess(ref startupInfo, command);
        return new WrappedProcess(new Process(startupInfo, processInfo));
    }

    private static unsafe STARTUPINFOEXW ConfigureProcessThread(PseudoConsole.ConPtyClosePseudoConsoleSafeHandle hPC,
        nuint attributes)
    {
        // this method implements the behavior described in https://docs.microsoft.com/en-us/windows/console/creating-a-pseudoconsole-session#preparing-for-creation-of-the-child-process

        nuint lpSize = 0;
        var success = PInvoke.InitializeProcThreadAttributeList(
            default,
            1,
            0,
            &lpSize
        );
        if (success || lpSize == 0) // we're not expecting `success` here, we just want to get the calculated lpSize
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(),
                "Could not calculate the number of bytes for the attribute list.");
        }

        var startupInfo = new STARTUPINFOEXW();
        startupInfo.StartupInfo.cb = (uint)Marshal.SizeOf<STARTUPINFOEXW>();
        startupInfo.lpAttributeList = new LPPROC_THREAD_ATTRIBUTE_LIST((void*)Marshal.AllocHGlobal((int)lpSize));

        success = PInvoke.InitializeProcThreadAttributeList(
            startupInfo.lpAttributeList,
            1,
            0,
            &lpSize
        );
        if (!success)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not set up attribute list.");
        }

        success = PInvoke.UpdateProcThreadAttribute(
            startupInfo.lpAttributeList,
            0,
            attributes,
            (void*)hPC.DangerousGetHandle(),
            (nuint)IntPtr.Size,
            null,
            (nuint*)null
        );
        if (!success)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not set pseudoconsole thread attribute.");
        }

        return startupInfo;
    }

    private static unsafe PROCESS_INFORMATION RunProcess(ref STARTUPINFOEXW sInfoEx, string commandLine)
    {
        var securityAttributeSize = (uint)Marshal.SizeOf<SECURITY_ATTRIBUTES>();
        var pSec = new SECURITY_ATTRIBUTES { nLength = securityAttributeSize };
        var tSec = new SECURITY_ATTRIBUTES { nLength = securityAttributeSize };
        var info = sInfoEx;
        Span<char> spanChar = (commandLine + '\0').ToCharArray();

        var success = PInvoke.CreateProcess(null, ref spanChar, pSec, tSec, false,
            PROCESS_CREATION_FLAGS.EXTENDED_STARTUPINFO_PRESENT, null, null, info.StartupInfo, out var pInfo);

        if (!success)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not create process.");
        }

        return pInfo;
    }

    public class WrappedProcess : IDisposable, IProcess
    {
        internal Process _process;

        private System.Diagnostics.Process _Process;
        private bool IsDisposed;

        internal WrappedProcess(Process process)
        {
            _process = process;
        }

        public int Pid => (int)_process.ProcessInfo.dwProcessId;
        public System.Diagnostics.Process Process => _Process ??= System.Diagnostics.Process.GetProcessById(Pid);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool HasExited => Process.HasExited;

        public void WaitForExit()
        {
            Process.WaitForExit();
        }

        public void Kill(bool EntireProcessTree = false)
        {
            Process.Kill(EntireProcessTree);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    _process.Dispose();
                }

                IsDisposed = true;
            }
        }
    }
}