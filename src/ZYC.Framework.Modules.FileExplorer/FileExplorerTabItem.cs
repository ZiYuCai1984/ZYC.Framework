using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.FileExplorer.Abstractions;
using ZYC.Framework.Modules.FileExplorer.UI;

namespace ZYC.Framework.Modules.FileExplorer;

[Register]
[ConstantsSource(typeof(FileExplorerModuleConstants))]
public class FileExplorerTabItem : TabItemInstanceBase, IFileExplorerTabItemInstance, INavigableTabItemInstance, INotifyPropertyChanged
{
    public FileExplorerTabItem(
        ILifetimeScope lifetimeScope,
        ITabManager tabManager,
        MutableTabReference tabReference) : base(lifetimeScope, tabReference)
    {
        TabManager = tabManager;

        if (lifetimeScope.TryResolve<IRecentPathMainMenuItemsProvider>(out var provider))
        {
            provider.AddRecentPath(tabReference.Uri.LocalPath);
            RecentPathMainMenuItemsProvider = provider;
        }
    }

    private IRecentPathMainMenuItemsProvider? RecentPathMainMenuItemsProvider { get; }

    private MutableTabReference MutableTabReference => (MutableTabReference)TabReference;

    public Uri Uri
    {
        get => MutableTabReference.Uri;
        set => MutableTabReference.Uri = value;
    }


    private ITabManager TabManager { get; }

    public override bool Localization => false;

    public override string Title => Uri.UnescapeDataString(Uri.Segments.Last());

    public new string Icon { get; set; } = FileExplorerModuleConstants.Icon;

    public override object View => _view ??= LifetimeScope.Resolve<FileExplorerView>(
        new TypedParameter(typeof(Uri), Uri),
        new TypedParameter(typeof(IFileExplorerTabItemInstance), this));

    public async Task<bool> TryNavigateAsync(Uri uri)
    {
        if (!uri.IsAbsoluteUri || !uri.IsFile || File.Exists(uri.LocalPath))
        {
            return false;
        }

        if (!Directory.Exists(uri.LocalPath))
        {
            throw new DirectoryNotFoundException($"The directory is unavailable: {uri.LocalPath}");
        }

        await ((FileExplorerView)View).NavigateAsync(uri);
        return true;
    }

    public async Task NotifyUriChangedAsync(Uri newUri)
    {
        var oldUri = Uri;
        Uri = newUri;

        if (UriTools.Equals(oldUri, newUri))
        {
            return;
        }

        OnPropertyChanged(nameof(Title));
        RecentPathMainMenuItemsProvider?.AddRecentPath(newUri.LocalPath);
        await TabManager.NotifyUriChangedAsync(this);
    }

    public void UpdateIcon(string base64Icon)
    {
        Icon = base64Icon;
        OnPropertyChanged(nameof(Icon));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
