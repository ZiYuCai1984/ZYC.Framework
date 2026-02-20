using NuGet.Configuration;
using NuGet.Protocol.Core.Types;

namespace ZYC.Framework.Modules.NuGet;

internal class NuGetSource
{
    public NuGetSource(string source)
    {
        Source = source;

        PackageSource = new PackageSource(source);
        SourceRepository = new SourceRepository(PackageSource, Repository.Provider.GetCoreV3());
    }

    public string Source { get; }

    public PackageSource PackageSource { get; }

    public SourceRepository SourceRepository { get; }
}