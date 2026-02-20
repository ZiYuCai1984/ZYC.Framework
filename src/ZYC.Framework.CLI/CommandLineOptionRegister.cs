using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using ZYC.CoreToolkit.Extensions.Autofac;

namespace ZYC.Framework.CLI;

public class CommandLineOptionRegister : ICommandLineOptionRegister
{
    public CommandLineOptionRegister(RootCommand rootCommand)
    {
        RootCommand = rootCommand;
    }

    public RootCommand RootCommand { get; }

    private List<Action<InvocationContext>> Handlers { get; } = new();

    public void AddOption<T>(Action<T> action, string name, string description)
    {
        var option = new Option<T>(name, description);
        RootCommand.AddOption(option);

        Handlers.Add(ctx =>
        {
            if (ctx.ParseResult.HasOption(option))
            {
                var value = ctx.ParseResult.GetValueForOption(option);
                action(value!);
            }
        });
    }

    public void FinalizeHandlers()
    {
        RootCommand.SetHandler(ctx =>
        {
            foreach (var h in Handlers)
            {
                h(ctx);
            }
        });
    }
}