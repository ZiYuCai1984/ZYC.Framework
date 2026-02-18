using System.Diagnostics;
using EasyWindowsTerminalControl;
using Microsoft.Extensions.Logging;
using ZYC.Automation.Abstractions;
using ZYC.Automation.Modules.CLI.Abstractions;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.CLI.UI;

[Register]
internal partial class CLIView : IDisposable
{
    public CLIView(
        CLIConfig cliConfig,
        CLIUriOptions cliUriOptions, 
        ILogger<CLIView> logger)
    {
        CLIConfig = cliConfig;
        CLIUriOptions = cliUriOptions;
        Logger = logger;


        InitializeComponent();

        if (string.IsNullOrWhiteSpace(cliUriOptions.StartupCommandLineOverride))
        {
            EasyTerminalControl.StartupCommandLine = CLIConfig.StartupCommandLine;
        }


        Debug.Assert(ConPTYTerm != null);
        ConPTYTerm.TermReady += OnConPTYTermTermReady;
    }

    // ReSharper disable once ReturnTypeCanBeNotNullable
    private TermPTY? ConPTYTerm => EasyTerminalControl.ConPTYTerm;

    private CLIConfig CLIConfig { get; }

    private CLIUriOptions CLIUriOptions { get; }

    private ILogger<CLIView> Logger { get; }

    private bool IsDisposed { get; set; }

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
    }

    private async void OnConPTYTermTermReady(object? sender, EventArgs e)
    {
        try
        {
            //!WARNING To prevent strange reentrancy issues on the first boot
            await Task.Delay(500);

            await Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    var conPTYTerm = ConPTYTerm;
                    if (conPTYTerm == null)
                    {
                        return;
                    }


                    if (!string.IsNullOrWhiteSpace(CLIUriOptions.TypeText))
                    {
                        conPTYTerm.WriteToTerm(CLIUriOptions.TypeText);

                        if (CLIUriOptions.TypeOnly)
                        {
                            return;
                        }

                        await conPTYTerm.ExecuteAndWaitAsync(
                            CLIUriOptions.TypeText);
                    }

                    if (CLIUriOptions.ExecCommands is { Count: > 0 })
                    {
                        foreach (var command in CLIUriOptions.ExecCommands)
                        {
                            if (string.IsNullOrWhiteSpace(command))
                            {
                                continue;
                            }

                            await conPTYTerm.ExecuteAndWaitAsync(
                                command);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });
        }
        catch
        {
            //ignore
        }
    }
}