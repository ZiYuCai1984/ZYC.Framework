using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Abstractions.Autofac;
using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.CoreToolkit.Dotnet;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.CoreToolkit.Extensions.Settings;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.State;
using ModuleNameTools = ZYC.Framework.Abstractions.ModuleNameTools;

namespace ZYC.Framework.CLI;

public static class ModuleTools
{
    public static string[] GetAllStandardModuleDlls()
    {
        var currentDirectory = IOTools.GetExecutingFolder();

        var regex = new Regex(@"^ZYC\.Framework\.Modules.*\.dll$",
            RegexOptions.IgnoreCase);
        var dllFiles = Directory.GetFiles(currentDirectory)
            .Where(file => regex.IsMatch(Path.GetFileName(file)))
            .OrderBy(s => s.Contains("Abstractions.dll"))
            .ToArray();

        return dllFiles;
    }

    public static string[] GetAllModuleDlls(ModuleConfig moduleConfig)
    {
        var dlls = new List<string>(GetAllStandardModuleDlls());

        var currentDirectory = IOTools.GetExecutingFolder();
        foreach (var assembly in moduleConfig.AdditionalAssemblyNames)
        {
            dlls.Add(Path.Combine(currentDirectory, assembly));
        }

        return dlls.ToArray();
    }

    public static ModuleBase[] RegisterModules(
        string appContextDirectory,
        ContainerBuilder builder,
        ModuleConfig moduleConfig,
        PendingFileOperationsState pendingFileOperationsState,
        IAppLogger? logger = null)
    {
        var dllFiles = GetAllModuleDlls(moduleConfig);
        var modules = new List<ModuleBase>();

        foreach (var dllFile in dllFiles)
        {
            var dllFileName = Path.GetFileName(dllFile);

            if (pendingFileOperationsState.FilesToDelete.Contains(dllFileName))
            {
                CleanupModule(dllFile);

                var assemblyNames = new List<string>(pendingFileOperationsState.FilesToDelete);
                assemblyNames.Remove(dllFileName);
                pendingFileOperationsState.FilesToDelete = assemblyNames.ToArray();

                continue;
            }

            if (!File.Exists(dllFile))
            {
                logger?.Warn($"<{dllFile}> not exist !!");
                continue;
            }

            var assembly = Assembly.LoadFrom(dllFile);

            RegisterAllFromAssembly(appContextDirectory, builder, assembly);

            var types = assembly.SafeGetTypes();
            foreach (var type in types)
            {
                if (!type.IsSubclassOf(typeof(ModuleBase)))
                {
                    continue;
                }


                var isModuleEnabled = false;

                var assemblyName = Path.GetFileName(dllFile);
                if (!moduleConfig.DisabledAssemblyNames.Contains(assemblyName))
                {
                    isModuleEnabled = true;
                }
                else
                {
                    logger?.Warn($"Ignore module <{dllFiles}>");
                }


                var instance = Activator.CreateInstance(type) as ModuleBase;


                Debug.Assert(instance != null);

                instance.ModulePath = dllFile;
                instance.IsEnabled = isModuleEnabled;
                instance.ReferenceAssemblyNames = ResolveModuleDependencyOn(assembly);


                builder.RegisterInstance(instance)
                    .As<ModuleBase>()
                    .As<IModuleInfo>();

                //!WARNING Wait for module RegisterAsync
                instance.RegisterAsync(builder).Wait();

                modules.Add(instance);
                break;
            }
        }


        //TODO Refactoring is required(register nuget moodule from nuget cache)
        var assetsJsonPath = Path.Combine(appContextDirectory, ProductInfoExtended.NuGetModuleAssetsJsonFile);
        if (File.Exists(assetsJsonPath))
        {
            var assemblyFromNuGetCache =
                AssetsRuntimeLoaderTools.LoadRuntimeAssemblies(
                    assetsJsonPath,
                    "net10.0-windows");

            foreach (var assembly in assemblyFromNuGetCache)
            {
                RegisterAllFromAssembly(appContextDirectory, builder, assembly);

                var types = assembly.SafeGetTypes();
                foreach (var type in types)
                {
                    if (!type.IsSubclassOf(typeof(ModuleBase)))
                    {
                        continue;
                    }

                    var instance = Activator.CreateInstance(type) as ModuleBase;

                    Debug.Assert(instance != null);

                    instance.ModulePath = assembly.Location;
                    instance.IsEnabled = true;
                    instance.ReferenceAssemblyNames = ResolveModuleDependencyOn(assembly);

                    instance.RegisterAsync(builder).Wait();

                    modules.Add(instance);
                    break;
                }
            }
        }

        var allModules = modules.ToArray();
        foreach (var module in allModules)
        {
            module.DependencyOn = ResolveModuleDependencyOn(module, allModules);
            module.DependencyBy = ResolveModuleDependencyBy(module, allModules);
        }

        return modules.ToArray();
    }

    public static void RegisterAllFromAssembly(
        string appContextDirectory,
        ContainerBuilder builder,
        Assembly assembly,
        //!WARNING Design defeat !!
        Action<object>? registerAction = null)
    {
        var folder = appContextDirectory;

        Trace.WriteLine($"Register from {assembly.FullName}");
        AutofacTools.RegisterFromAssembly(builder, assembly);

        var types = assembly.SafeGetTypes();
        foreach (var type in types)
        {
            if (typeof(IConfig).IsAssignableFrom(type)
                && type != typeof(IConfig)
                && !type.IsInterface
                && !type.IsAbstract)
            {
                var result = SettingsTools.GetFromFolderGeneric(folder, type);
                builder.RegisterConfigR(result);

                registerAction?.Invoke(result);

                continue;
            }

            if (typeof(IState).IsAssignableFrom(type)
                && type != typeof(IState)
                && !type.IsInterface
                && !type.IsAbstract)
            {
                var result = SettingsTools.GetFromFolderGeneric(folder, type);
                builder.RegisterStateR(result);

                registerAction?.Invoke(result);

                // ReSharper disable once RedundantJumpStatement
                continue;
            }
        }
    }


    private static void CleanupModule(string dllFile, IAppLogger? logger = null)
    {
        var timeout = TimeSpan.FromSeconds(8);

        if (File.Exists(dllFile))
        {
            DeleteFileWhenNotInUse(dllFile, timeout);

            logger?.Warn($"Clean up module <{dllFile}>");
        }
    }


    private static string[] ResolveModuleDependencyOn(Assembly assembly)
    {
        var list = new List<string>();

        var regex = new Regex(@"^ZYC\.Framework\.Modules.*\.Abstractions\.dll$",
            RegexOptions.IgnoreCase);

        var selfAbstractionsDllName = ModuleNameTools.GetModuleAbstractionsDllName(assembly);

        var referencedAssemblies = assembly.GetReferencedAssemblies();
        foreach (var referencedAssembly in referencedAssemblies)
        {
            var dllName = $"{referencedAssembly.Name}.dll";
            if (string.IsNullOrWhiteSpace(dllName))
            {
                continue;
            }

            if (!regex.IsMatch(dllName))
            {
                continue;
            }

            if (selfAbstractionsDllName == dllName)
            {
                continue;
            }

            //!WARNING Convert abstractions.dll -> .dll
            list.Add(ModuleNameTools.GetModuleDllName(dllName));
        }

        return list.ToArray();
    }

    private static void DeleteFileWhenNotInUse(string filePath, TimeSpan timeout)
    {
        var endTime = DateTime.Now.Add(timeout);

        while (DateTime.Now < endTime)
        {
            if (IsFileAvailable(filePath))
            {
                File.Delete(filePath);
                return;
            }

            Thread.Sleep(200);
        }

        throw new TimeoutException(
            $"Unable to delete the <{filePath}> within <{timeout.TotalSeconds}> seconds because the file is still occupied.");
    }

    private static bool IsFileAvailable(string filePath)
    {
        try
        {
            using var fs = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.ReadWrite,
                FileShare.None);
            return true;
        }
        catch (IOException)
        {
            DebuggerTools.Break();
            return false;
        }
    }


    private static IModuleInfo[] ResolveModuleDependencyOn(ModuleBase module, ModuleBase[] allModules)
    {
        var list = new List<IModuleInfo>();
        foreach (var assembly in module.ReferenceAssemblyNames)
        {
            var target = allModules.FirstOrDefault(t => t.ModuleAssemblyName == assembly);
            if (target == null)
            {
                continue;
            }

            list.Add(target);
        }

        return list.ToArray();
    }

    private static IModuleInfo[] ResolveModuleDependencyBy(ModuleBase module, ModuleBase[] allModules)
    {
        var list = new List<IModuleInfo>();

        foreach (var m in allModules)
        {
            if (m == module)
            {
                continue;
            }

            if (m.ReferenceAssemblyNames.Contains(module.ModuleAssemblyName))
            {
                list.Add(m);
            }
        }

        return list.ToArray();
    }

    /// <summary>
    ///     !WARNING Based on the metadata, there may be a problem.
    /// </summary>
    /// <returns></returns>
    private static bool IsAlreadyLoadedBySimpleName(string assemblyPath)
    {
        var simpleName = AssemblyName.GetAssemblyName(assemblyPath).Name;
        return AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => string.Equals(a.GetName().Name, simpleName, StringComparison.OrdinalIgnoreCase));
    }
}