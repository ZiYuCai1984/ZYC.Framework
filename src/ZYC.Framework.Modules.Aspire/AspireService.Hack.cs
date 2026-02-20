using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ZYC.CoreToolkit;

namespace ZYC.Framework.Modules.Aspire;

internal partial class AspireService
{
    private static void HackDcpOptions(
        IDistributedApplicationBuilder builder,
        AspireServiceEnvironment aspireServiceEnvironment)
    {
        var dcpOptionsType = typeof(DistributedApplication).Assembly.GetType("Aspire.Hosting.Dcp.DcpOptions")
                             ?? throw new InvalidOperationException("DcpOptions not found");

        var dcpOptions = Activator.CreateInstance(dcpOptionsType)!;

        void TrySet(string name, object? value)
        {
            var prop = dcpOptionsType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(dcpOptions, value);
            }
        }

        TrySet("CliPath", aspireServiceEnvironment.OrchestrationCliPath);
        TrySet("BinPath", aspireServiceEnvironment.OrchestrationBinPath);
        TrySet("DashboardPath", aspireServiceEnvironment.DashboardPath);
        TrySet("EnableDashboard", true);

        var wrapperType = typeof(OptionsWrapper<>).MakeGenericType(dcpOptionsType);
        var wrapper = Activator.CreateInstance(wrapperType, dcpOptions)!;

        builder.Services.AddSingleton(typeof(IOptions<>).MakeGenericType(dcpOptionsType), wrapper);
    }


    private static IHostApplicationBuilder GetInnerHostApplicationBuilder(object distributedBuilder)
    {
        return ObjectGraphSearchTools.FindFirstOrThrow<IHostApplicationBuilder>(
            distributedBuilder,
            new ObjectGraphSearchTools.Options
            {
                MaxDepth = 64,
                MaxNodes = 200_000,
                TraverseEnumerables = true,
                MaxEnumerableItems = 64,
                TrackPath = true
            });
    }
}