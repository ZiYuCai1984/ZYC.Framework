using System.IO;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;

namespace ZYC.Framework.Modules.FileExplorer;

[RegisterSingleInstance]
internal class FileExplorerTabItemFactory : ITabItemFactory
{
    public FileExplorerTabItemFactory(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    private ILifetimeScope LifetimeScope { get; }

    public int Priority => 20;

    public bool IsSingle => false;

    public async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<FileExplorerTabItem>(
            new TypedParameter(typeof(MutableTabReference), new MutableTabReference(context.Uri)));
    }

    public async Task<bool> CheckUriMatchedAsync(Uri uri)
    {
        await Task.CompletedTask;

        if (!uri.IsFile)
        {
            return false;
        }

        var localPath = uri.LocalPath;
        return Directory.Exists(localPath);
    }
}