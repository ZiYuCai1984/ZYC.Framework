using System.IO;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Dotnet;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.Aspire.Abstractions;

namespace ZYC.Framework.Modules.Aspire;

[RegisterSingleInstance]
public class AspireServiceEnvironment
{
    private string? _aspirePackageVersion = "";

    public AspireServiceEnvironment(IAppContext appContext, AspireConfig aspireConfig)
    {
        AppContext = appContext;
        AspireConfig = aspireConfig;
    }

    private IAppContext AppContext { get; }

    private AspireConfig AspireConfig { get; }

    /// <summary>
    ///     NuGetCache:
    ///     "C:\\Users\\Administrator\\.nuget\\packages\\aspire.hosting.orchestration.win-x64\\9.4.2\\tools\\ext\\bin\\"
    /// </summary>
    public string OrchestrationBinPath
    {
        get
        {
            switch (AspireConfig.AspireBinarySource)
            {
                case AspireBinarySource.NuGetCache:
                    return Path.Combine(
                        DotnetNuGetTools.GetDefaultNuGetPackageCachePath(),
                        OrchestrationPackageName,
                        AspirePackageVersion,
                        "tools",
                        "ext",
                        "bin");
                case AspireBinarySource.ApplicationFolder:
                    return Path.Combine(
                        AspireToolsFolder,
                        OrchestrationPackageName,
                        AspirePackageVersion,
                        "tools",
                        "ext",
                        "bin");
                case AspireBinarySource.Custom:
                    return AspireConfig.CustomOrchestrationBinPath;
                default:
                    throw new InvalidOperationException("");
            }
        }
    }

    /// <summary>
    ///     NuGetCache:
    ///     "C:\\Users\\Administrator\\.nuget\\packages\\aspire.hosting.orchestration.win-x64\\9.4.2\\tools\\dcp.exe"
    /// </summary>
    public string OrchestrationCliPath
    {
        get
        {
            switch (AspireConfig.AspireBinarySource)
            {
                case AspireBinarySource.NuGetCache:
                    return Path.Combine(
                        DotnetNuGetTools.GetDefaultNuGetPackageCachePath(),
                        OrchestrationPackageName,
                        AspirePackageVersion,
                        "tools",
                        "dcp.exe");
                case AspireBinarySource.ApplicationFolder:
                    return Path.Combine(
                        AspireToolsFolder,
                        OrchestrationPackageName,
                        AspirePackageVersion,
                        "tools",
                        "dcp.exe");
                case AspireBinarySource.Custom:
                    return AspireConfig.CustomOrchestrationCliPath;
                default:
                    throw new InvalidOperationException("");
            }
        }
    }

    /// <summary>
    ///     NuGetCache:
    ///     "C:\\Users\\Administrator\\.nuget\\packages\\aspire.dashboard.sdk.win-x64\\9.4.2\\tools\\Aspire.Dashboard.exe"
    /// </summary>
    public string DashboardPath
    {
        get
        {
            switch (AspireConfig.AspireBinarySource)
            {
                case AspireBinarySource.NuGetCache:
                    return Path.Combine(
                        DotnetNuGetTools.GetDefaultNuGetPackageCachePath(),
                        DashboardPackageName,
                        AspirePackageVersion,
                        "tools",
                        "Aspire.Dashboard.exe");
                case AspireBinarySource.ApplicationFolder:
                    return Path.Combine(
                        AspireToolsFolder,
                        DashboardPackageName,
                        AspirePackageVersion,
                        "tools",
                        "Aspire.Dashboard.exe");
                case AspireBinarySource.Custom:
                    return AspireConfig.CustomDashboardPath;
                default:
                    throw new InvalidOperationException("");
            }
        }
    }

    public string AspireToolsFolder => Path.Combine(AppContext.GetMainAppDirectory(), "aspire-tools");

    public string OrchestrationPackageName => "aspire.hosting.orchestration.win-x64";

    public string DashboardPackageName => "aspire.dashboard.sdk.win-x64";

    public string AspirePackageVersion
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_aspirePackageVersion))
            {
                return _aspirePackageVersion;
            }

            var version = AssemblyTools.GetVersion<DistributedApplication>();
            _aspirePackageVersion = $"{version.Major}.{version.Minor}.{version.Build}";
            return _aspirePackageVersion;
        }
    }
}