using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.ModuleManager.Abstractions;
using ZYC.Framework.Modules.NuGet.Abstractions;

namespace ZYC.Framework.Modules.ModuleManager;

[RegisterSingleInstanceAs(typeof(INuGetModuleManager))]
internal class NuGetModuleManager : INuGetModuleManager
{
    private const int MinSearchTake = 1;

    private const int MaxSearchTake = 1000;

    public NuGetModuleManager(
        NuGetConfig nugetConfig,
        INuGetManager nuGetManager,
        NuGetModuleConfig config,
        NuGetModuleState state,
        IAppContext appContext,
        NuGetModuleOperationCoordinator operationCoordinator)
    {
        NuGetConfig = nugetConfig;
        NuGetManager = nuGetManager;
        NuGetModuleConfig = config;
        State = state;
        AppContext = appContext;
        OperationCoordinator = operationCoordinator;
    }

    private NuGetConfig NuGetConfig { get; }

    private INuGetManager NuGetManager { get; }

    private NuGetModuleConfig NuGetModuleConfig { get; }

    private NuGetModuleState State { get; }

    private IAppContext AppContext { get; }

    private NuGetModuleOperationCoordinator OperationCoordinator { get; }

    public async Task<INuGetModule[]> GetModulesAsync()
    {
        return await OperationCoordinator.RunAsync(async () =>
        {
            var source = new PackageSource(NuGetConfig.Source);
            var repository = Repository.Factory.GetCoreV3(source);
            var search = await repository.GetResourceAsync<PackageSearchResource>(CancellationToken.None);
            var filter = new SearchFilter(true)
            {
                OrderBy = SearchOrderBy.Id
            };
            var searchTake = Math.Clamp(
                NuGetModuleConfig.SearchTake,
                MinSearchTake,
                MaxSearchTake);
            var results = await search.SearchAsync(
                NuGetModuleConfig.SearchTerm,
                filter,
                NuGetModuleConfig.SearchSkip,
                searchTake,
                NullLogger.Instance,
                CancellationToken.None);

            var regex = new Regex(
                NuGetModuleConfig.IncludeRegex,
                RegexOptions.IgnoreCase);

            var modules = new List<INuGetModule>();
            foreach (var result in results)
            {
                if (!regex.IsMatch(result.Identity.Id))
                {
                    continue;
                }

                var installedModule = State.InstalledModules.LastOrDefault(m =>
                    string.Equals(m.PackageId, result.Identity.Id, StringComparison.OrdinalIgnoreCase));

                modules.Add(new NuGetModule(
                    result.Identity.Id,
                    result.Identity.Version.ToNormalizedString(),
                    result.Description ?? string.Empty,
                    installedModule?.Version));
            }

            return modules.ToArray();
        });
    }

    public async Task InstallAsync(INuGetModule module)
    {
        await OperationCoordinator.RunAsync(async () =>
        {
            var newModule = new InstalledNuGetModule
            {
                PackageId = module.PackageId,
                Version = module.Version
            };

            var newModules = State.InstalledModules
                .Where(m => !string.Equals(m.PackageId, module.PackageId, StringComparison.OrdinalIgnoreCase))
                .ToList();
            newModules.Add(newModule);


            await UpdateProjectAssetsJsonAsync(
                newModules.ToArray(),
                NuGetModuleConfig.TargetFramework,
                NuGetConfig.Source,
                GetNuGetModuleAssetsJsonPath());

            State.InstalledModules = newModules.ToArray();

            if (module is NuGetModule nugetModule)
            {
                nugetModule.InstalledVersion = module.Version;
            }
        });
    }

    public async Task UninstallAsync(INuGetModule module)
    {
        await OperationCoordinator.RunAsync(async () =>
        {
            var record = State.InstalledModules.FirstOrDefault(m =>
                string.Equals(m.PackageId, module.PackageId, StringComparison.OrdinalIgnoreCase));
            if (record == null)
            {
                return;
            }

            var newModules = State.InstalledModules
                .Where(m => !string.Equals(m.PackageId, module.PackageId, StringComparison.OrdinalIgnoreCase))
                .ToList();

            await UpdateProjectAssetsJsonAsync(
                newModules.ToArray(),
                NuGetModuleConfig.TargetFramework,
                NuGetConfig.Source,
                GetNuGetModuleAssetsJsonPath());

            State.InstalledModules = newModules.ToArray();

            if (module is NuGetModule nugetModule)
            {
                nugetModule.InstalledVersion = null;
            }
        });
    }

    public string GetNuGetModuleAssetsJsonPath()
    {
        return Path.Combine(
            AppContext.GetSettingsDirectory(),
            ProductInfoExtended.NuGetModuleAssetsJsonFile);
    }

    private static async Task UpdateProjectAssetsJsonAsync(
        InstalledNuGetModule[] modules,
        string targetFramework,
        string nugetSource,
        string projectAssetsJsonPath)
    {
        var tempProjectDirectory = Directory.CreateDirectory("temp").FullName;
        var tempCsprojPath = Path.Combine(tempProjectDirectory, "temp.csproj");

        try
        {
            var csprojContent = new StringBuilder();
            csprojContent.Append($"""
                                  <Project Sdk="Microsoft.NET.Sdk">
                                    <PropertyGroup>
                                      <TargetFramework>{targetFramework}</TargetFramework>
                                    </PropertyGroup>
                                    <ItemGroup>
                                    
                                  """);

            foreach (var module in modules)
            {
                csprojContent.AppendLine(
                    $"<PackageReference Include=\"{module.PackageId}\" Version=\"{module.Version}\" />");
            }

            csprojContent.Append("""
                                   </ItemGroup>
                                 </Project>
                                 """);

            await File.WriteAllTextAsync(tempCsprojPath, csprojContent.ToString());

            var restoreCommand = $"dotnet restore \"{tempCsprojPath}\" --source \"{nugetSource}\"";
            if (await CommandTools.ExecuteCommandAsync(restoreCommand) != 0)
            {
                throw new InvalidOperationException("Restore failed");
            }


            File.Copy(Path.Combine("temp", "obj", "project.assets.json"), projectAssetsJsonPath, true);
        }
        finally
        {
            IOTools.DeleteDirectoryIfExists(tempProjectDirectory);
        }
    }
}