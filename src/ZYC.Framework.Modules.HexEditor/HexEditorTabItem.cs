using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.HexEditor.Abstractions;
using ZYC.Framework.Modules.HexEditor.UI;

namespace ZYC.Framework.Modules.HexEditor;

[Register]
internal class HexEditorTabItem : TabItemInstanceBase, INotifyPropertyChanged
{
    private Uri _documentUri;
    private bool _isDirty;

    public HexEditorTabItem(
        ILifetimeScope lifetimeScope,
        ITabManager tabManager,
        MutableTabReference tabReference) : base(lifetimeScope, tabReference)
    {
        TabManager = tabManager;
        _documentUri = HexDocumentTools.GetRequiredEditorFileUri(tabReference.Uri);
    }

    private ITabManager TabManager { get; }

    private MutableTabReference MutableTabReference => (MutableTabReference)TabReference;

    public Uri DocumentUri => _documentUri;

    public bool IsDirty => _isDirty;

    public override string Host => HexEditorModuleConstants.Host;

    public override string Title => HexDocumentTools.GetDisplayName(DocumentUri.LocalPath) + (_isDirty ? " *" : "");

    public override string Icon => HexEditorModuleConstants.Icon;

    public override bool Localization => false;

    public override object View => _view ??= LifetimeScope.Resolve<HexEditorView>(
        new TypedParameter(typeof(HexEditorTabItem), this));

    public event PropertyChangedEventHandler? PropertyChanged;

    public void SetDirty(bool isDirty)
    {
        if (_isDirty == isDirty)
        {
            return;
        }

        _isDirty = isDirty;
        OnPropertyChanged(nameof(IsDirty));
        OnPropertyChanged(nameof(Title));
    }

    public async Task UpdateDocumentUriAsync(Uri newDocumentUri)
    {
        var newRouteUri = HexEditorModuleConstants.CreateEditorUri(newDocumentUri);
        var oldRouteUri = MutableTabReference.Uri;

        _documentUri = newDocumentUri;
        MutableTabReference.Uri = newRouteUri;
        OnPropertyChanged(nameof(DocumentUri));
        OnPropertyChanged(nameof(Title));

        if (!UriTools.Equals(oldRouteUri, newRouteUri))
        {
            await TabManager.TabInternalNavigatingAsync(this, oldRouteUri, newRouteUri);
        }
    }

    public override bool OnClosing()
    {
        if (_view is HexEditorView { IsBusy: true })
        {
            return false;
        }

        return !_isDirty || MessageBoxTools.Confirm(
            $"Discard unsaved changes to '{HexDocumentTools.GetDisplayName(DocumentUri.LocalPath)}'?",
            "Unsaved Changes",
            false);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
