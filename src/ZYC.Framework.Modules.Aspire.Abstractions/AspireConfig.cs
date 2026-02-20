using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Abstractions.Config.Attributes;

namespace ZYC.Framework.Modules.Aspire.Abstractions;

/// <summary>
///     Configuration options for the Aspire module.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class AspireConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the source used to resolve Aspire binaries.
    /// </summary>
    public AspireBinarySource AspireBinarySource { get; set; } = AspireBinarySource.ApplicationFolder;

    /// <summary>
    ///     Gets or sets the custom path to the orchestration binary.
    /// </summary>
    public string CustomOrchestrationBinPath { get; set; } = "";

    /// <summary>
    ///     Gets or sets the custom path to the orchestration CLI binary.
    /// </summary>
    public string CustomOrchestrationCliPath { get; set; } = "";

    /// <summary>
    ///     Gets or sets the custom path to the dashboard binary.
    /// </summary>
    public string CustomDashboardPath { get; set; } = "";

    /// <summary>
    ///     Gets or sets a value indicating whether the Aspire service should start automatically.
    /// </summary>
    public bool AutoStart { get; set; }

    //!WARNING Dictionary not support now in UI !!
    /// <summary>
    ///     Gets or sets environment variables used when starting Aspire services.
    /// </summary>
    [Hidden]
    public Dictionary<string, string> Environment { get; set; } = new()
    {
        ["DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"] = "http://localhost:18886",
        ["DOTNET_RESOURCE_SERVICE_ENDPOINT_URL"] = "http://localhost:18887",
        ["ASPNETCORE_URLS"] = "http://localhost:18888",
        ["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true",
        ["ASPIRE_DASHBOARD_AI_DISABLED"] = "true",
        ["ASPIRE_VERSION_CHECK_DISABLED"] = "false"
    };
}