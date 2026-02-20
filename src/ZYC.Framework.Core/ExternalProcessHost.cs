//Resharper disable all

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ZYC.Framework.Core;

/// <summary>
///     ExternalProcessHost embeds an existing Win32 top-level window (usually the main window
///     of an EXE you start) into your WPF app. It creates a child HWND as a docking surface
///     and re-parents the external window to it using SetParent.
///     ✅ Works for most classic Win32/WinForms apps. Some apps with special window managers
///     (e.g., UWP/Store apps, elevated processes, apps with UIAccess, GPU-exclusive or layered
///     popups) may refuse to embed due to UIPI/permission or rendering constraints.
///     Usage (XAML):
///     <code>
///   <local:ExternalProcessHost x:Name="Host"
///             FileName="notepad.exe"
///             Arguments=""
///             AutoStart="True"
///             KillProcessOnDispose="False" />
/// </code>
///     Usage (code-behind):
///     <code>
///   await Host.StartAndEmbedAsync();
///   // or attach to an already running process
///   await Host.AttachToExistingProcessAsync(processId);
/// </code>
/// </summary>
public sealed class ExternalProcessHost : HwndHost, IDisposable
{
    #region Helpers

    private static async Task<IntPtr> FindFirstTopLevelWindowAsync(int processId, TimeSpan timeout,
        CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var found = IntPtr.Zero;

        while (found == IntPtr.Zero && sw.Elapsed < timeout)
        {
            ct.ThrowIfCancellationRequested();

            EnumWindows((h, l) =>
            {
                if (!IsWindowVisible(h))
                {
                    return true;
                }

                if (GetWindow(h, GW_OWNER) != IntPtr.Zero)
                {
                    return true; // skip owned tool windows
                }

                _ = GetWindowThreadProcessId(h, out var pid);
                if (pid == (uint)processId)
                {
                    found = h;
                    return false; // stop
                }

                return true; // continue
            }, IntPtr.Zero);

            if (found != IntPtr.Zero)
            {
                break;
            }

            await Task.Delay(100, ct);
        }

        if (found == IntPtr.Zero)
        {
            throw new TimeoutException(
                $"No top-level window found for PID {processId} within {timeout.TotalSeconds:F1}s.");
        }

        return found;
    }

    #endregion

    #region Public API

    /// <summary>Path to the EXE to start and embed. Optional if you only call AttachToExistingProcessAsync.</summary>
    public string? FileName { get; set; }

    /// <summary>Arguments for the EXE.</summary>
    public string? Arguments { get; set; }

    /// <summary>Automatically start and embed on Loaded if FileName is set.</summary>
    public bool AutoStart { get; set; } = false;

    /// <summary>Remove caption/border & convert external window to WS_CHILD.</summary>
    public bool RemoveWindowBorder { get; set; } = true;

    /// <summary>When disposing, attempt to close (or kill) the process we started.</summary>
    public bool KillProcessOnDispose { get; set; } = false;

    /// <summary>Timeout for locating the first top-level window for a process.</summary>
    public TimeSpan WindowSearchTimeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>Returns the embedded window handle if attached, else IntPtr.Zero.</summary>
    public IntPtr EmbeddedWindowHandle { get; private set; }

    /// <summary>Returns the host child window handle created by this control.</summary>
    public IntPtr HostHandle { get; private set; }

    /// <summary>Process started by this control, if any.</summary>
    public Process? StartedProcess { get; private set; }

    #endregion

    #region HwndHost plumbing

    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
        // Create a trivial child HWND to act as docking surface for the embedded window.
        HostHandle = CreateWindowEx(
            0,
            "Static",
            string.Empty,
            (int)(WS_CHILD | WS_VISIBLE | WS_CLIPSIBLINGS | WS_CLIPCHILDREN),
            0, 0, 100, 100,
            hwndParent.Handle,
            IntPtr.Zero,
            IntPtr.Zero,
            IntPtr.Zero);

        if (HostHandle == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateWindowEx failed for ExternalProcessHost");
        }

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        return new HandleRef(this, HostHandle);
    }

    protected override void DestroyWindowCore(HandleRef hwnd)
    {
        try
        {
            // Detach child (optional). Most apps tolerate SetParent back to desktop.
            // If we started the process, try to close it politely.
            if (KillProcessOnDispose && StartedProcess is not null && !StartedProcess.HasExited)
            {
                try
                {
                    if (EmbeddedWindowHandle != IntPtr.Zero)
                    {
                        // Ask nicely first
                        SendMessage(EmbeddedWindowHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    }

                    if (!StartedProcess.WaitForExit(1000))
                    {
                        StartedProcess.Kill(true);
                        StartedProcess.WaitForExit(2000);
                    }
                }
                catch
                {
                    /* best-effort */
                }
            }
        }
        finally
        {
            if (HostHandle != IntPtr.Zero)
            {
                DestroyWindow(HostHandle);
                HostHandle = IntPtr.Zero;
            }
        }
    }

    protected override void OnWindowPositionChanged(Rect rcBoundingBox)
    {
        base.OnWindowPositionChanged(rcBoundingBox);
        ResizeEmbeddedToHost();
    }

    protected override Size MeasureOverride(Size constraint)
    {
        // Ensure we always allocate some size to keep child alive
        return base.MeasureOverride(constraint.IsEmpty ? new Size(1, 1) : constraint);
    }

    #endregion

    #region Lifecycle hooks

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (AutoStart && !string.IsNullOrWhiteSpace(FileName))
        {
            try
            {
                await StartAndEmbedAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ExternalProcessHost AutoStart failed: {ex}");
            }
        }
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        // Intentionally keep the process running unless KillProcessOnDispose is true and
        // the control is being destroyed (handled in DestroyWindowCore). Here we only resize-detach.
    }

    #endregion

    #region Public ops

    /// <summary>
    ///     Starts <see cref="FileName" /> (if set) and embeds its first top-level window.
    /// </summary>
    public async Task StartAndEmbedAsync(CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(FileName))
        {
            throw new InvalidOperationException("FileName is not set.");
        }

        if (HostHandle == IntPtr.Zero)
        {
            throw new InvalidOperationException("Host window has not been created yet.");
        }

        if (StartedProcess is not null && !StartedProcess.HasExited)
        {
            throw new InvalidOperationException("Process already started.");
        }

        var psi = new ProcessStartInfo
        {
            FileName = FileName!,
            Arguments = Arguments ?? string.Empty,
            UseShellExecute = false,
            WorkingDirectory = Path.GetDirectoryName(FileName!) ?? Environment.CurrentDirectory,
            //!WARNING It can be displayed and flicker can be prevented !!
            WindowStyle = ProcessWindowStyle.Minimized
        };

        StartedProcess = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start process.");

        try
        {
            // Wait a bit for the UI to come up
            try
            {
                StartedProcess.WaitForInputIdle(4000);
            }
            catch
            {
                /* console apps etc. */
            }

            var hwnd = await FindFirstTopLevelWindowAsync(StartedProcess.Id, WindowSearchTimeout, ct);
            await AttachToWindowAsync(hwnd, ct);
        }
        catch
        {
            // If attach failed and we created the process, best-effort cleanup
            if (KillProcessOnDispose && StartedProcess is { HasExited: false })
            {
                try
                {
                    StartedProcess.Kill(true);
                }
                catch
                {
                }
            }

            throw;
        }
    }

    /// <summary>
    ///     Attaches to an already-running process by ID and embeds its first visible top-level window.
    /// </summary>
    public async Task AttachToExistingProcessAsync(int processId, CancellationToken ct = default)
    {
        if (HostHandle == IntPtr.Zero)
        {
            throw new InvalidOperationException("Host window has not been created yet.");
        }

        var hwnd = await FindFirstTopLevelWindowAsync(processId, WindowSearchTimeout, ct);
        await AttachToWindowAsync(hwnd, ct);
    }

    #endregion

    #region Core attach/resize

    private async Task AttachToWindowAsync(IntPtr targetHwnd, CancellationToken ct)
    {
        if (targetHwnd == IntPtr.Zero)
        {
            throw new ArgumentException("Target window handle is null.", nameof(targetHwnd));
        }

        // Optional: strip frame and force WS_CHILD
        if (RemoveWindowBorder)
        {
            var style = GetWindowLongPtrSafe(targetHwnd, GWL_STYLE);
            var newStyle = (IntPtr)((style.ToInt64()
                                     & ~((long)WS_CAPTION | WS_THICKFRAME | WS_MINIMIZE | WS_MAXIMIZE | WS_SYSMENU))
                                    | WS_CHILD);
            SetWindowLongPtrSafe(targetHwnd, GWL_STYLE, newStyle);
            SetWindowPos(targetHwnd, IntPtr.Zero, 0, 0, 0, 0,
                SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }

        // Re-parent into our child host
        if (SetParent(targetHwnd, HostHandle) == IntPtr.Zero)
        {
            var err = Marshal.GetLastWin32Error();
            throw new Win32Exception(err, "SetParent failed. Is the target process elevated or UIAccess?");
        }

        EmbeddedWindowHandle = targetHwnd;

        // Show and size to fit
        ShowWindow(targetHwnd, SW_SHOW);
        await Task.Delay(1, ct); // let layout settle
        ResizeEmbeddedToHost();
    }

    private void ResizeEmbeddedToHost()
    {
        if (EmbeddedWindowHandle == IntPtr.Zero || HostHandle == IntPtr.Zero)
        {
            return;
        }

        // Convert WPF DIPs to device pixels for MoveWindow
        double scaleX = 1.0, scaleY = 1.0;
        var src = PresentationSource.FromVisual(this);
        if (src?.CompositionTarget is not null)
        {
            var m = src.CompositionTarget.TransformToDevice;
            scaleX = m.M11;
            scaleY = m.M22;
        }

        var widthPx = Math.Max(1, (int)Math.Round(RenderSize.Width * scaleX));
        var heightPx = Math.Max(1, (int)Math.Round(RenderSize.Height * scaleY));

        SetWindowPos(EmbeddedWindowHandle, IntPtr.Zero, 0, 0, widthPx, heightPx,
            SWP_NOZORDER | SWP_NOACTIVATE | SWP_ASYNCWINDOWPOS);
    }

    #endregion

    #region Win32

    private const int GWL_STYLE = -16;
    private const int SW_SHOW = 5;
    private const uint GW_OWNER = 4;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_FRAMECHANGED = 0x0020;
    private const uint SWP_ASYNCWINDOWPOS = 0x4000;
    private const uint WS_CHILD = 0x40000000;
    private const uint WS_VISIBLE = 0x10000000;
    private const uint WS_CLIPSIBLINGS = 0x04000000;
    private const uint WS_CLIPCHILDREN = 0x02000000;
    private const uint WS_CAPTION = 0x00C00000;
    private const uint WS_THICKFRAME = 0x00040000;
    private const uint WS_MINIMIZE = 0x20000000;
    private const uint WS_MAXIMIZE = 0x01000000;
    private const uint WS_SYSMENU = 0x00080000;
    private const uint WM_CLOSE = 0x0010;

    private static IntPtr GetWindowLongPtrSafe(IntPtr hWnd, int nIndex)
    {
        if (IntPtr.Size == 8)
        {
            return GetWindowLongPtr64(hWnd, nIndex);
        }

        var val = GetWindowLong32(hWnd, nIndex);
        return new IntPtr(val);
    }

    private static IntPtr SetWindowLongPtrSafe(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        if (IntPtr.Size == 8)
        {
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        var prev = SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32());
        return new IntPtr(prev);
    }

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr CreateWindowEx(int exStyle, string className, string windowName, int style,
        int x, int y, int width, int height, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr param);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyWindow(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
        int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
    private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    #endregion
}