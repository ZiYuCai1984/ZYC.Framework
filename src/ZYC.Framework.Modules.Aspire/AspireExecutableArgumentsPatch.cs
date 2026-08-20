using System.Reflection;
using System.Runtime.CompilerServices;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.CoreToolkit.Hook;

namespace ZYC.Framework.Modules.Aspire;

[RegisterSingleInstance]
internal class AspireExecutableArgumentsPatch : IDisposable
{
    private const string MetadataTypeName =
        "Aspire.Hosting.Dashboard.ResourcePropertySnapshotMetadata";

    private delegate (string? DisplayName, bool IsHighlighted, int? SortOrder)
        MetadataGetter(string resourceType, string name);

    // ReSharper disable once InconsistentNaming
    private static MetadataGetter? MetadataGet;

    private MethodHook? MethodHook { get; set; }

    public void Enable()
    {
        if (MethodHook is not null)
        {
            return;
        }

        const BindingFlags flags =
            BindingFlags.Static | BindingFlags.NonPublic;

        var metadataType = typeof(ResourcePropertySnapshot).Assembly
            .GetType(MetadataTypeName, throwOnError: true)!;

        var sourceMethod = metadataType.GetMethod(
            "Create",
            flags,
            null,
            [
                typeof(string),
                typeof(string),
                typeof(object),
                typeof(bool)
            ],
            null) ?? throw new MissingMethodException(
            MetadataTypeName,
            "Create");

        var getMethod = metadataType.GetMethod(
            "Get",
            flags,
            null,
            [typeof(string), typeof(string)],
            null) ?? throw new MissingMethodException(
            MetadataTypeName,
            "Get");

        MetadataGet = getMethod.CreateDelegate<MetadataGetter>();

        var replacementMethod = typeof(AspireExecutableArgumentsPatch)
            .GetMethod(
                nameof(CreateSnapshot),
                BindingFlags.Static | BindingFlags.NonPublic)!;

        MethodHook = HookTools.HookMethod(
            sourceMethod,
            replacementMethod);

        MethodHook.Enable();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ResourcePropertySnapshot CreateSnapshot(
        string resourceType,
        string name,
        object? value,
        // ReSharper disable once UnusedParameter.Local
        bool isSensitive)
    {
        var (displayName, isHighlighted, sortOrder) =
            MetadataGet!(resourceType, name);

        return new ResourcePropertySnapshot(name, value)
        {
            IsSensitive = false,
            DisplayName = displayName,
            IsHighlighted = isHighlighted,
            SortOrder = sortOrder
        };
    }

    public void Dispose()
    {
        MethodHook?.Dispose();
        MethodHook = null;
    }
}