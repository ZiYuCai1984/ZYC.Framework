using System.CommandLine;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Abstractions;
using ZYC.CoreToolkit.Dotnet;
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
        var modules = ModuleTools.RegisterModules(
            appContextDirectory,
            builder,
            new ModuleConfig(),
            new PendingFileOperationsState());

        var container = builder.Build();

        foreach (var module in modules)
        {
            module.RegisterCommandLineOption(container, optionRegister);
        }

        optionRegister.AddOption<bool>(_ =>
            {
                Task.Run(async () =>
                {
                    var currentFolder = IOTools.GetExecutingFolder();
                    IOTools.SetCurrentDirectory(currentFolder);

                    var packageId = ProductInfo.PackageId;

                    var version = (await NuGetTools.GetNuGetVersionV3Async(packageId))!.ToString();
                    var package = new NuGetPackage
                    {
                        Name = packageId,
                        Version = version
                    };

                    var cachePath = "./packages";
                    await DotnetNuGetTools.DownloadNuGetPackagesAsync(package, cachePath);

                    Console.WriteLine("---------------------");

                    var sourceDir = Path.Combine(cachePath, packageId, version);
                    var currentDir = IOTools.GetExecutingFolder();

                    FileReplaceTools.SafeCopyWithDelayedReplace(sourceDir, currentDir);
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "apply_update.bat",
                        UseShellExecute = true
                    });

                    Environment.Exit(0);
                }).Wait();
            }, "--init", $"Download the latest full of {ProductInfo.ProductName}");


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


        optionRegister.FinalizeHandlers();

        var finalArgs = args.Length == 0 ? ["--help"] : args;
        return await rootCommand.InvokeAsync(finalArgs);
    }
}