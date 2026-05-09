using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.TextEditor.Abstractions;
using ZYC.Framework.Modules.TextEditor.UI;

namespace ZYC.Framework.Modules.TextEditor;

[Register]
internal class TextEditorTabItem : TabItemInstanceBase, INotifyPropertyChanged
{
    private Uri _documentUri;
    private bool _isDirty;

    public TextEditorTabItem(
        ILifetimeScope lifetimeScope,
        ITabManager tabManager,
        MutableTabReference tabReference) : base(lifetimeScope, tabReference)
    {
        TabManager = tabManager;
        _documentUri = TextDocumentTools.GetRequiredEditorFileUri(tabReference.Uri);
    }

    private ITabManager TabManager { get; }

    private MutableTabReference MutableTabReference => (MutableTabReference)TabReference;

    public Uri DocumentUri => _documentUri;

    public bool IsDirty => _isDirty;

    public override string Host => TextEditorModuleConstants.EditorHost;

    public override string Title => BuildTitle();

    public override string Icon => TextEditorModuleConstants.EditorIcon;

    public override bool Localization => false;

    public override object View => _view ??= LifetimeScope.Resolve<TextEditorView>(
        new TypedParameter(typeof(TextEditorTabItem), this));

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
        var newRouteUri = TextEditorModuleConstants.CreateEditorUri(newDocumentUri);
        var oldRouteUri = MutableTabReference.Uri;

        _documentUri = newDocumentUri;
        MutableTabReference.Uri = newRouteUri;
        OnPropertyChanged(nameof(Title));

        if (UriTools.Equals(oldRouteUri, newRouteUri))
        {
            return;
        }

        await TabManager.TabInternalNavigatingAsync(this, oldRouteUri, newRouteUri);
    }

    public override bool OnClosing()
    {
        return !_isDirty || MessageBoxTools.Confirm(
            $"Discard unsaved changes to '{TextDocumentTools.GetDisplayName(DocumentUri.LocalPath)}'?",
            "Unsaved Changes",
            false);
    }

    private string BuildTitle()
    {
        var displayName = TextDocumentTools.GetDisplayName(DocumentUri.LocalPath);
        return _isDirty ? $"{displayName} *" : displayName;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
