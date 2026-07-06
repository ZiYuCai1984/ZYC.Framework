using Microsoft.Extensions.Hosting;
using ZYC.CoreToolkit;

namespace ZYC.Framework.Modules.Aspire;

internal partial class AspireService
{
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
