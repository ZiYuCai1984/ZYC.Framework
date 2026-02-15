using EasyWindowsTerminalControl;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using ZYC.Automation.Abstractions;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Gemini.UI;

[Register]
internal partial class GeminiView : IDisposable
{
    private ILogger<GeminiView> Logger { get; }
    private McpHost McpHost { get; }

    public GeminiView(ILogger<GeminiView> logger, McpHost mcpHost)
    {
        Logger = logger;
        McpHost = mcpHost;

        InitializeComponent();

        EasyTerminalControl.StartupCommandLine = "cmd.exe";

        Debug.Assert(ConPTYTerm != null);

        _ = McpHost.StartAsync(Dispatcher);

        ConPTYTerm.TermReady += OnConPTYTermTermReady;
    }

    public void Dispose()
    {
        if (IsDisposed)
        {
            DebuggerTools.Break();
            return;
        }

        IsDisposed = true;

        var conPTYTerm = ConPTYTerm;
        if (conPTYTerm != null)
        {
            conPTYTerm.TermReady -= OnConPTYTermTermReady;
        }


        EasyTerminalControl.Dispose(Logger);

        _ = McpHost.DisposeAsync();
    }

    private async void OnConPTYTermTermReady(object? sender, EventArgs e)
    {
        try
        {
            //!WARNING To prevent strange reentrancy issues on the first boot
            await Task.Delay(500);

            await Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    var conPTYTerm = ConPTYTerm;
                    if (conPTYTerm == null)
                    {
                        return;
                    }

                    conPTYTerm.WriteToTerm("gemini\r\n");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    private TermPTY? ConPTYTerm => EasyTerminalControl.ConPTYTerm;

    private bool IsDisposed { get; set; }
}