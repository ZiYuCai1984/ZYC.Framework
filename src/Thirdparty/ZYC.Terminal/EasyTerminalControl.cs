using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using EasyWindowsTerminalControl.Internals;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Terminal.Wpf;

namespace EasyWindowsTerminalControl;

public class EasyTerminalControl : UserControl, IDisposable
{
    [Flags]
    [TypeConverter(typeof(EnumConverter))]
    public enum INPUT_CAPTURE
    {
        None = 1 << 0,
        TabKey = 1 << 1,
        DirectionKeys = 1 << 2
    }

    private bool? _IsCursorVisible;
    private bool? _IsReadOnly;
    private TerminalTheme? _Theme;

    public EasyTerminalControl()
    {
        InitializeComponent();
        SetKBCaptureOptions();
    }

    /// <summary>
    ///     Helper property for setting KeyboardNavigation.Set*Navigation commands to prevent arrow keys or tabs from causing
    ///     us to leave the control (aka pass through to conpty)
    /// </summary>
    public INPUT_CAPTURE InputCapture
    {
        get => (INPUT_CAPTURE)GetValue(InputCaptureProperty);
        set => SetValue(InputCaptureProperty, value);
    }

    [Description("Write only, sets the terminal theme")]
    [Category("Common")]
    public TerminalTheme? Theme
    {
        set => SetTheme(_Theme = value);
        private get => _Theme;
    }


    [Description(
        "Write only, When true user cannot give input through the Terminal UI (can still write to the Term from code behind using Term.WriteToTerm)")]
    [Category("Common")]
    public bool? IsReadOnly
    {
        set => SetReadOnly(_IsReadOnly = value);
        private get => _IsReadOnly;
    }

    [Description("Write only, if the type cursor shows on the Terminal UI")]
    [Category("Common")]
    public bool? IsCursorVisible
    {
        set => SetCursor(_IsCursorVisible = value);
        private get => _IsCursorVisible;
    }

    [Description("Direct access to the UI terminal control itself that handles rendering")]
    public TerminalControl Terminal
    {
        get => (TerminalControl)GetValue(TerminalPropertyKey.DependencyProperty);
        set => SetValue(TerminalPropertyKey, value);
    }

    /// <summary>
    ///     Update the Term if you want to set to an existing
    /// </summary>
    [Description("The backend TermPTY connection allows changing the application the control is connected to")]
    public TermPTY ConPTYTerm
    {
        get => (TermPTY)GetValue(ConPTYTermProperty);
        set => SetValue(ConPTYTermProperty, value);
    }

    public string StartupCommandLine
    {
        get => (string)GetValue(StartupCommandLineProperty);
        set => SetValue(StartupCommandLineProperty, value);
    }

    public bool LogConPTYOutput
    {
        get => (bool)GetValue(LogConPTYOutputProperty);
        set => SetValue(LogConPTYOutputProperty, value);
    }

    private TerminalContainer TerminalContainer
    {
        get
        {
            var f = typeof(TerminalControl).GetField("termContainer",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var v = f.GetValue(Terminal);

            return (TerminalContainer)v!;
        }
    }


    /// <summary>
    ///     Sets if the GUI Terminal control communicates to ConPTY using extended key events (handles certain control
    ///     sequences better)
    ///     https://github.com/microsoft/terminal/blob/main/doc/specs/%234999%20-%20Improved%20keyboard%20handling%20in%20Conpty.md
    /// </summary>
    public bool Win32InputMode
    {
        get => (bool)GetValue(Win32InputModeProperty);
        set => SetValue(Win32InputModeProperty, value);
    }

    public FontFamily FontFamilyWhenSettingTheme
    {
        get => (FontFamily)GetValue(FontFamilyWhenSettingThemeProperty);
        set => SetValue(FontFamilyWhenSettingThemeProperty, value);
    }

    public int FontSizeWhenSettingTheme
    {
        get => (int)GetValue(FontSizeWhenSettingThemeProperty);
        set => SetValue(FontSizeWhenSettingThemeProperty, value);
    }

    private bool IsDisposed { get; set; }


    private bool FirstRending { get; set; } = true;

    public void Dispose()
    {
        Dispose(NullLogger.Instance);
    }

    public void Dispose(ILogger logger)
    {
        if (IsDisposed)
        {
            Debugger.Break();
            return;
        }

        IsDisposed = true;

        var term = DisconnectConPTYTerm();

        try
        {
            term.CloseStdinToApp();
        }
        catch (Exception e)
        {
            logger.LogError(e, "");
        }

        try
        {
            term.StopExternalTermOnly();
        }
        catch (Exception e)
        {
            logger.LogError(e, "");
        }

        try
        {
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            term.Process?.Kill();
        }
        catch (Exception e)
        {
            logger.LogError(e, "");
        }

        try
        {
            TerminalContainer.Dispose();
        }
        catch (Exception e)
        {
            logger.LogError(e, "");
        }
    }

    /// <summary>
    ///     Converts Color to COLOREF, note that COLOREF does not support alpha channels so it is ignored
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static uint ColorToVal(Color color)
    {
        return BitConverter.ToUInt32(new byte[] { color.R, color.G, color.B, 0 }, 0);
    }


    private static void InputCaptureChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
        var cntrl = target as EasyTerminalControl;
        cntrl.SetKBCaptureOptions();
    }

    private void SetKBCaptureOptions()
    {
        KeyboardNavigation.SetTabNavigation(this,
            InputCapture.HasFlag(INPUT_CAPTURE.TabKey)
                ? KeyboardNavigationMode.Contained
                : KeyboardNavigationMode.Continue);
        KeyboardNavigation.SetDirectionalNavigation(this,
            InputCapture.HasFlag(INPUT_CAPTURE.DirectionKeys)
                ? KeyboardNavigationMode.Contained
                : KeyboardNavigationMode.Continue);
    }

    private void SetTheme(TerminalTheme? v)
    {
        if (v != null)
        {
            Terminal?.SetTheme(v.Value, FontFamilyWhenSettingTheme.Source, (short)FontSizeWhenSettingTheme);
        }
    }

    private void SetReadOnly(bool? v)
    {
        if (v != null)
        {
            ConPTYTerm?.SetReadOnly(v.Value, false);
        }
    } //no cursor auto update if user wants that they can use the separate dependency property for the cursor visibility

    private void SetCursor(bool? v)
    {
        if (v != null)
        {
            ConPTYTerm?.SetCursorVisibility(v.Value);
        }
    }

    private static void OnTermChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
        var cntrl = target as EasyTerminalControl;
        var newTerm = e.NewValue as TermPTY;
        if (newTerm != null)
        {
            if (cntrl.Terminal.IsLoaded)
            {
                cntrl.Terminal_Loaded(cntrl.Terminal, null);
            }

            if (newTerm.TermProcIsStarted)
            {
                cntrl.Term_TermReady(newTerm, null);
            }
            else
            {
                newTerm.TermReady += cntrl.Term_TermReady;
            }
        }
    }


    public TermPTY DisconnectConPTYTerm()
    {
        if (Terminal != null)
        {
            Terminal.Connection = null;
        }

        if (ConPTYTerm != null)
        {
            ConPTYTerm.TermReady -= Term_TermReady;
        }

        var ret = ConPTYTerm;
        ConPTYTerm = null;
        return ret;
    }

    private void InitializeComponent()
    {
        Terminal = new TerminalControl();
        ConPTYTerm = new TermPTY();
        Terminal.AutoResize = true;
        Terminal.Loaded += Terminal_Loaded;
        var grid = new Grid();
        grid.Children.Add(Terminal);
        Content = grid;

        Focusable = true;
        Terminal.Focusable = true;
        Terminal.IsTabStop = true;
        GotFocus += (_, _) => Terminal.Focus();
        Terminal.PreviewMouseDown += (_, _) => EnsureTerminalKeyboardFocus();
        Terminal.PreviewTouchDown += (_, _) => EnsureTerminalKeyboardFocus();
        InputMethod.SetIsInputMethodEnabled(Terminal, true);
        InputMethod.SetPreferredImeState(Terminal, InputMethodState.On);
    }


    private void EnsureTerminalKeyboardFocus()
    {
        if (!Terminal.IsKeyboardFocusWithin)
        {
            Terminal.Focus();
            Keyboard.Focus(Terminal);
        }
    }


    private void MainThreadRun(Action action)
    {
        Dispatcher.Invoke(action);
    }


    private void Term_TermReady(object sender, EventArgs e)
    {
        MainThreadRun(() =>
        {
            var conPTYTerm = ConPTYTerm;
            if (conPTYTerm == null)
            {
                return;
            }

            Terminal.Connection = conPTYTerm;
            conPTYTerm.Win32DirectInputMode(Win32InputMode);
            conPTYTerm.Resize(Terminal.Columns, Terminal.Rows); //fix the size being partially off on first load
        });
    }

    /// <summary>
    ///     Restarts the command we are running in a brand new term and disposes of the old one
    /// </summary>
    /// <param name="useTerm">Optional term to use, note if useTerm.TermProcIsStarted this function will not do verry much</param>
    /// <param name="disposeOld">True if the old term should be killed off</param>
    public async Task RestartTerm(TermPTY useTerm = null, bool disposeOld = true)
    {
        var oldTerm = ConPTYTerm;
        DisconnectConPTYTerm();
        if (disposeOld)
        {
            try
            {
                oldTerm?.CloseStdinToApp();
            }
            catch
            {
            }

            try
            {
                oldTerm?.StopExternalTermOnly();
            }
            catch
            {
            }
        }

        ConPTYTerm = useTerm ?? new TermPTY(); //setting the term to a new value will automatically initalize everyhting
    }

    private void StartTerm(int column_width, int row_height)
    {
        if (ConPTYTerm?.TermProcIsStarted != false)
        {
            return;
        }

        MainThreadRun(() =>
        {
            var cmd = StartupCommandLine; //thread safety for dp
            var term = ConPTYTerm;
            var logOutput = LogConPTYOutput;
            Task.Run(() => term.Start(cmd, column_width, row_height, logOutput));
        });
    }

    private async void Terminal_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!FirstRending)
            {
                return;
            }

            FirstRending = false;

            await TermInit();
        }
        catch
        {
            //ignore
        }
    }

    private async Task TermInit()
    {
        StartTerm(Terminal.Columns, Terminal.Rows);
        SetTheme(Theme);
        SetCursor(IsCursorVisible);
        SetReadOnly(IsReadOnly);

        await Task.Delay(1000);
        SetCursor(IsCursorVisible);
    }

    #region Depdendency Properties

    public static readonly DependencyProperty InputCaptureProperty = DependencyProperty.Register(nameof(InputCapture),
        typeof(INPUT_CAPTURE), typeof(EasyTerminalControl), new
            PropertyMetadata(INPUT_CAPTURE.TabKey | INPUT_CAPTURE.DirectionKeys, InputCaptureChanged));

    public static readonly DependencyProperty ThemeProperty = PropHelper.GenerateWriteOnlyProperty(c => c.Theme);

    protected static readonly DependencyPropertyKey TerminalPropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(Terminal), typeof(TerminalControl), typeof(EasyTerminalControl),
            new PropertyMetadata());

    public static readonly DependencyProperty TerminalProperty = TerminalPropertyKey.DependencyProperty;

    public static readonly DependencyProperty ConPTYTermProperty = DependencyProperty.Register(nameof(ConPTYTerm),
        typeof(TermPTY), typeof(EasyTerminalControl), new PropertyMetadata(null, OnTermChanged));

    public static readonly DependencyProperty StartupCommandLineProperty =
        DependencyProperty.Register(nameof(StartupCommandLine), typeof(string), typeof(EasyTerminalControl),
            new PropertyMetadata("powershell.exe"));

    public static readonly DependencyProperty LogConPTYOutputProperty =
        DependencyProperty.Register(nameof(LogConPTYOutput), typeof(bool), typeof(EasyTerminalControl),
            new PropertyMetadata(false));

    public static readonly DependencyProperty Win32InputModeProperty =
        DependencyProperty.Register(nameof(Win32InputMode), typeof(bool), typeof(EasyTerminalControl),
            new PropertyMetadata(true));

    public static readonly DependencyProperty IsReadOnlyProperty =
        PropHelper.GenerateWriteOnlyProperty(c => c.IsReadOnly);

    public static readonly DependencyProperty IsCursorVisibleProperty =
        PropHelper.GenerateWriteOnlyProperty(c => c.IsCursorVisible);

    public static readonly DependencyProperty FontFamilyWhenSettingThemeProperty =
        DependencyProperty.Register(nameof(FontFamilyWhenSettingTheme), typeof(FontFamily), typeof(EasyTerminalControl),
            new PropertyMetadata(new FontFamily("Cascadia Code")));

    public static readonly DependencyProperty FontSizeWhenSettingThemeProperty =
        DependencyProperty.Register(nameof(FontSizeWhenSettingTheme), typeof(int), typeof(EasyTerminalControl),
            new PropertyMetadata(12));

    private class PropHelper : DepPropHelper<EasyTerminalControl>
    {
    }

    #endregion
}