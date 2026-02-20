using System.ComponentModel;
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
public class FileExplorerTabItem : TabItemInstanceBase, IFileExplorerTabItemInstance, INotifyPropertyChanged
{
    public FileExplorerTabItem(
        ILifetimeScope lifetimeScope,
        ITabManager tabManager,
        MutableTabReference tabReference) : base(lifetimeScope, tabReference)
    {
        TabManager = tabManager;
    }

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

    public async Task TabInternalNavigatingAsync(Uri newUri)
    {
        var oldUri = Uri;
        Uri = newUri;

        if (UriTools.Equals(oldUri, newUri))
        {
            return;
        }

        OnPropertyChanged(nameof(Title));
        await TabManager.TabInternalNavigatingAsync(oldUri, newUri);
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