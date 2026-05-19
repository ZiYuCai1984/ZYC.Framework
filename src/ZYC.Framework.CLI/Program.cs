using System.CommandLine;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Autofac;
using ZYC.CoreToolkit;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.State;
using ProductInfo = ZYC.Framework.Abstractions.ProductInfo;

namespace ZYC.Framework.CLI;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

        var rootCommand = new RootCommand($"Command line tool for {ProductInfo.ProductName}");
        var optionRegister = new CommandLineOptionRegister(rootCommand);

        var builder = new ContainerBuilder();

        var appContextDirectory = IOTools.GetExecutingFolder();
        var settingsDirectory = Path.Combine(
            new DirectoryInfo(appContextDirectory).Parent!.FullName, "settings");


        var moduleConfig = new ModuleConfig();

        ModuleTools.RegisterAllFromAssembly(settingsDirectory,
            builder,
            typeof(ProductInfo).Assembly, obj =>
            {
                if (obj is ModuleConfig initModuleConfig)
                {
                    moduleConfig = initModuleConfig;
                }
            });

        var modules = ModuleTools.RegisterModules(
            settingsDirectory,
            builder,
            moduleConfig,
            new PendingFileOperationsState());

        var container = builder.Build();

        foreach (var module in modules)
        {
            module.RegisterCommandLineOption(container, optionRegister);
        }



        //optionRegister.AddOption<bool>(_ =>
        //    {
        //        var exeFile = Path.Combine(appContextDirectory, ProductInfoExtended.MainExeName);
        //        Process.Start(new ProcessStartInfo(exeFile)
        //        {
        //            WorkingDirectory = appContextDirectory
        //        });
        //    }, "--gui",
        //    $"Start {ProductInfo.ProductName} with gui");


        optionRegister.AddOption<bool>(_ =>
            {
                Console.WriteLine("Hello World");
            }, "--test",
            "Hello World (no argument required)");


        RegisterNewProjectCommand(rootCommand);
        RegisterNewModuleCommand(rootCommand);

        optionRegister.FinalizeHandlers();

        var finalArgs = args.Length == 0 ? ["--help"] : args;
        return await rootCommand.InvokeAsync(finalArgs);
    }

    private static void RegisterNewProjectCommand(RootCommand rootCommand)
    {
        var newProjectCommand = new Command("new", "Create a new project from template.");

        var nameArgument = new Argument<string?>("name", () => null, "Project name.");

        var templateOption = new Option<string>("--template", () => NewProjectGenerator.DefaultTemplateName,
            $"Project template. Supported values: {string.Join(", ", NewProjectGenerator.TemplateNames)}.");
        templateOption.AddAlias("-t");

        var outputOption = new Option<string?>("--output", "Output directory. Defaults to ./<ProjectName>.");
        outputOption.AddAlias("-o");

        var packageVersionOption = new Option<string?>("--package-version",
            "ZYC.Framework.Alpha package version. Defaults to the CLI product version.");

        var overwriteOption = new Option<bool>("--overwrite", "Overwrite existing files.");
        overwriteOption.AddAlias("-f");

        newProjectCommand.AddArgument(nameArgument);
        newProjectCommand.AddOption(templateOption);
        newProjectCommand.AddOption(outputOption);
        newProjectCommand.AddOption(packageVersionOption);
        newProjectCommand.AddOption(overwriteOption);

        newProjectCommand.SetHandler(context =>
        {
            var name = context.ParseResult.GetValueForArgument(nameArgument);

            if (string.IsNullOrWhiteSpace(name))
            {
                context.Console.Error.Write(
                    $"Project name is required. Pass <ProjectName> as the positional argument.{Environment.NewLine}");
                context.ExitCode = 1;
                return;
            }

            var options = new NewProjectGenerationOptions
            {
                Name = name,
                Template = context.ParseResult.GetValueForOption(templateOption)!,
                OutputRoot = context.ParseResult.GetValueForOption(outputOption),
                PackageVersion = context.ParseResult.GetValueForOption(packageVersionOption),
                Overwrite = context.ParseResult.GetValueForOption(overwriteOption)
            };

            context.ExitCode = NewProjectCommandRunner.Run(
                options,
                line => context.Console.Out.Write($"{line}{Environment.NewLine}"),
                line => context.Console.Error.Write($"{line}{Environment.NewLine}"),
                "Use 'zyc new --help' to view command usage.");
        });

        rootCommand.AddCommand(newProjectCommand);
    }

    private static void RegisterNewModuleCommand(RootCommand rootCommand)
    {
        var newModuleCommand = new Command("new-module", "Create a new module from template.");

        var targetOption = new Option<string?>("--target", "Target module name.");
        targetOption.AddAlias("-t");

        var targetArgument = new Argument<string?>("target", () => null, "Target module name.");

        var sourceRootOption = new Option<string?>("--src-root", "Source root directory.");
        sourceRootOption.AddAlias("-s");

        var slnxOption = new Option<string?>("--slnx", "Optional slnx path. Omit to skip updating any slnx.");

        var overwriteOption = new Option<bool>("--overwrite", "Overwrite existing files.");
        overwriteOption.AddAlias("-f");

        newModuleCommand.AddOption(targetOption);
        newModuleCommand.AddArgument(targetArgument);
        newModuleCommand.AddOption(sourceRootOption);
        newModuleCommand.AddOption(slnxOption);
        newModuleCommand.AddOption(overwriteOption);

        newModuleCommand.SetHandler(context =>
        {
            var targetFromOption = context.ParseResult.GetValueForOption(targetOption);
            var targetFromArgument = context.ParseResult.GetValueForArgument(targetArgument);

            if (!string.IsNullOrWhiteSpace(targetFromOption)
                && !string.IsNullOrWhiteSpace(targetFromArgument)
                && !string.Equals(targetFromOption, targetFromArgument, StringComparison.Ordinal))
            {
                context.Console.Error.Write($"Target was provided multiple times with different values.{Environment.NewLine}");
                context.ExitCode = 1;
                return;
            }

            var target = !string.IsNullOrWhiteSpace(targetFromOption)
                ? targetFromOption
                : targetFromArgument;
            var sourceRoot = context.ParseResult.GetValueForOption(sourceRootOption);

            if (string.IsNullOrWhiteSpace(target))
            {
                context.Console.Error.Write($"Target is required. Pass --target <ModuleName> or provide <ModuleName> as positional argument.{Environment.NewLine}");
                context.ExitCode = 1;
                return;
            }

            if (string.IsNullOrWhiteSpace(sourceRoot))
            {
                context.Console.Error.Write($"Source root is required. Pass --src-root <SourceRoot>.{Environment.NewLine}");
                context.ExitCode = 1;
                return;
            }

            var options = new NewModuleGenerationOptions
            {
                Target = target,
                SourceRoot = sourceRoot,
                SlnxPath = context.ParseResult.GetValueForOption(slnxOption),
                Overwrite = context.ParseResult.GetValueForOption(overwriteOption)
            };

            context.ExitCode = NewModuleCommandRunner.Run(
                options,
                line => context.Console.Out.Write($"{line}{Environment.NewLine}"),
                line => context.Console.Error.Write($"{line}{Environment.NewLine}"),
                "Use 'zyc new-module --help' to view command usage.");
        });

        rootCommand.AddCommand(newModuleCommand);
    }
}
