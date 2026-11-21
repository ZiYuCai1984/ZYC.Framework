using System.CommandLine;
using System.Diagnostics;
using System.Globalization;
using Autofac;
using ZYC.CoreToolkit;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.State;
using ProductInfo = ZYC.Framework.Abstractions.ProductInfo;

namespace ZYC.Framework.CLI;

internal class Program
{
    private const string WebTypesCommandName = "web-types";

    public static async Task<int> Main(string[] args)
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

        //!WARNING web-types only reads assembly metadata. It skips module loading so it can run from a host
        // output folder, whose WPF modules cannot be loaded by this non-WPF process !!
        if (args.Length > 0 && string.Equals(args[0], WebTypesCommandName, StringComparison.Ordinal))
        {
            var webTypesRootCommand =
                new RootCommand($"Command line tool for {ProductInfo.ProductName} {ProductInfo.Version}");
            RegisterWebTypesCommand(webTypesRootCommand);
            return await webTypesRootCommand.InvokeAsync(args);
        }

        var rootCommand = new RootCommand($"Command line tool for {ProductInfo.ProductName} {ProductInfo.Version}");
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
        RegisterClearNuGetHttpCacheCommand(rootCommand);
        RegisterUpdateCommand(rootCommand);
        RegisterWebTypesCommand(rootCommand);

        optionRegister.FinalizeHandlers();

        var finalArgs = args.Length == 0 ? ["--help"] : args;
        return await rootCommand.InvokeAsync(finalArgs);
    }

    private static void RegisterClearNuGetHttpCacheCommand(RootCommand rootCommand)
    {
        var clearNuGetHttpCacheCommand = new Command("clear-nuget-http-cache", "Clear the NuGet HTTP cache using dotnet.");

        clearNuGetHttpCacheCommand.SetHandler(async context =>
        {
            try
            {
                using var process = Process.Start(new ProcessStartInfo("dotnet", "nuget locals http-cache --clear")
                {
                    UseShellExecute = false
                })!;

                await process.WaitForExitAsync();
                context.ExitCode = process.ExitCode;
            }
            catch (Exception exception)
            {
                context.Console.Error.Write($"{exception.Message}{Environment.NewLine}");
                context.ExitCode = 1;
            }
        });

        rootCommand.AddCommand(clearNuGetHttpCacheCommand);
    }

    private static void RegisterWebTypesCommand(RootCommand rootCommand)
    {
        var webTypesCommand = new Command(WebTypesCommandName,
            "Generate TypeScript declarations for the [ExposeToWeb] interfaces of the given assemblies.");

        var assemblyOption = new Option<string[]>("--assembly", "Assembly to scan. Repeat the option for more assemblies.")
        {
            IsRequired = true
        };
        assemblyOption.AddAlias("-a");

        var outputOption = new Option<string>("--output",
            "The .d.ts file to write. It is only rewritten when its content changes.")
        {
            IsRequired = true
        };
        outputOption.AddAlias("-o");

        webTypesCommand.AddOption(assemblyOption);
        webTypesCommand.AddOption(outputOption);

        webTypesCommand.SetHandler(context =>
        {
            try
            {
                var assemblyPaths = context.ParseResult.GetValueForOption(assemblyOption)!;
                var outputPath = Path.GetFullPath(context.ParseResult.GetValueForOption(outputOption)!);

                var result = WebTypesGenerator.Generate(assemblyPaths);
                foreach (var warning in result.Warnings)
                {
                    context.Console.Error.Write($"warning: {warning}{Environment.NewLine}");
                }

                var changed = WebTypesGenerator.WriteIfChanged(outputPath, result.Content);
                context.Console.Out.Write($"{(changed ? "Generated" : "Up to date")}: {outputPath}{Environment.NewLine}");
            }
            catch (Exception exception)
            {
                context.Console.Error.Write($"{exception.Message}{Environment.NewLine}");
                context.ExitCode = 1;
            }
        });

        rootCommand.AddCommand(webTypesCommand);
    }

    private static void RegisterUpdateCommand(RootCommand rootCommand)
    {
        var updateCommand = new Command("update",
            $"Update the {ProductInfoExtended.CLIPackageId} global dotnet tool to the latest version.");

        updateCommand.SetHandler(async context =>
        {
            try
            {
                using var process = Process.Start(
                    new ProcessStartInfo("dotnet", $"tool update --global {ProductInfoExtended.CLIPackageId}")
                    {
                        UseShellExecute = false
                    })!;

                // On Windows this running process locks its own files under the tool store,
                // so 'dotnet tool update' can only replace them after we exit. Let it finish on its own.
                if (OperatingSystem.IsWindows())
                {
                    context.Console.Out.Write(
                        $"Updating {ProductInfoExtended.CLIPackageId} in the background. Its output will follow below.{Environment.NewLine}");
                    return;
                }

                await process.WaitForExitAsync();
                context.ExitCode = process.ExitCode;
            }
            catch (Exception exception)
            {
                context.Console.Error.Write($"{exception.Message}{Environment.NewLine}");
                context.ExitCode = 1;
            }
        });

        rootCommand.AddCommand(updateCommand);
    }

    private static void RegisterNewProjectCommand(RootCommand rootCommand)
    {
        var newProjectCommand = new Command("new",
            $"Create a new project from template. [-t|--template <{string.Join("|", NewProjectGenerator.TemplateNames)}>] " +
            "[-o|--output <directory>] [--package-version <version>] [-f|--overwrite]");

        var nameArgument = new Argument<string?>("name", "Project name.");

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

        var sourceRootOption = new Option<string?>("--src-root",
            "Source root directory. Defaults to the current directory.");
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
                context.Console.Error.Write(
                    $"Target was provided multiple times with different values.{Environment.NewLine}");
                context.ExitCode = 1;
                return;
            }

            var target = !string.IsNullOrWhiteSpace(targetFromOption)
                ? targetFromOption
                : targetFromArgument;
            var sourceRoot = context.ParseResult.GetValueForOption(sourceRootOption);

            if (string.IsNullOrWhiteSpace(target))
            {
                context.Console.Error.Write(
                    $"Target is required. Pass --target <ModuleName> or provide <ModuleName> as positional argument.{Environment.NewLine}");
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
