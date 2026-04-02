using System.CommandLine;
using System.CommandLine.Invocation;
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
        //        Task.Run(async () =>
        //        {
        //            var currentFolder = IOTools.GetExecutingFolder();
        //            IOTools.SetCurrentDirectory(currentFolder);

        //            var packageId = ProductInfo.PackageId;

        //            var version = (await NuGetTools.GetNuGetVersionV3Async(packageId))!.ToString();
        //            var package = new NuGetPackage
        //            {
        //                Name = packageId,
        //                Version = version
        //            };

        //            var cachePath = "./packages";
        //            await DotnetNuGetTools.DownloadNuGetPackagesAsync(package, cachePath);

        //            Console.WriteLine("---------------------");

        //            var sourceDir = Path.Combine(cachePath, packageId, version);
        //            var currentDir = IOTools.GetExecutingFolder();

        //            FileReplaceTools.SafeCopyWithDelayedReplace(sourceDir, currentDir);
        //            Process.Start(new ProcessStartInfo
        //            {
        //                FileName = "apply_update.bat",
        //                UseShellExecute = true
        //            });

        //            Environment.Exit(0);
        //        }).Wait();
        //    }, "--init", $"Download the latest full of {ProductInfo.ProductName}");


        optionRegister.AddOption<bool>(_ =>
            {
                var exeFile = Path.Combine(appContextDirectory, ProductInfoExtended.MainExeName);
                Process.Start(new ProcessStartInfo(exeFile)
                {
                    WorkingDirectory = appContextDirectory
                });
            }, "--gui",
            $"Start {ProductInfo.ProductName} with gui");


        optionRegister.AddOption<bool>(_ =>
            {
                Console.WriteLine("Hello World");
            }, "--test",
            "Hello World (no argument required)");


        RegisterNewModuleCommand(rootCommand);

        optionRegister.FinalizeHandlers();

        var finalArgs = args.Length == 0 ? ["--help"] : args;
        return await rootCommand.InvokeAsync(finalArgs);
    }

    private static void RegisterNewModuleCommand(RootCommand rootCommand)
    {
        var newModuleCommand = new Command("new-module", "Create a new module from template.");
        newModuleCommand.AddAlias("new");

        var targetOption = new Option<string?>("--target", "Target module name.");
        targetOption.AddAlias("-t");

        var targetArgument = new Argument<string?>("target", () => null, "Target module name.");

        var sourceRootOption = new Option<string?>("--src-root", "Repository root or src directory.");
        sourceRootOption.AddAlias("-s");

        var overwriteOption = new Option<bool>("--overwrite", "Overwrite existing files.");
        overwriteOption.AddAlias("-f");

        newModuleCommand.AddOption(targetOption);
        newModuleCommand.AddArgument(targetArgument);
        newModuleCommand.AddOption(sourceRootOption);
        newModuleCommand.AddOption(overwriteOption);

        newModuleCommand.SetHandler((InvocationContext context) =>
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

            if (string.IsNullOrWhiteSpace(target))
            {
                context.Console.Error.Write($"Target is required. Pass --target <ModuleName> or provide <ModuleName> as positional argument.{Environment.NewLine}");
                context.ExitCode = 1;
                return;
            }

            var options = new NewModuleGenerationOptions
            {
                Target = target,
                SourceRoot = context.ParseResult.GetValueForOption(sourceRootOption),
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
