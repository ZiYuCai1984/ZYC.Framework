using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.MarkdownViewer.Abstractions;
using ZYC.Framework.Modules.MarkdownViewer.UI;

namespace ZYC.Framework.Modules.MarkdownViewer;

[RegisterAs(typeof(MarkdownViewerTabItem), typeof(IMarkdownViewerTabItem))]
[ConstantsSource(typeof(MarkdownViewerModuleConstants))]
internal class MarkdownViewerTabItem : TabItemInstanceBase, IMarkdownViewerTabItem, INotifyPropertyChanged
{
    public MarkdownViewerTabItem(
        ILifetimeScope lifetimeScope,
        MutableTabReference tabReference,
        MarkdownSource? markdownSource) : base(lifetimeScope, tabReference)
    {
        if (markdownSource != null)
        {
            MarkdownSource = markdownSource;
            Uri = MarkdownRoute.BuildWithDocument(markdownSource.SourceUri);
        }
    }

    private MutableTabReference MutableTabReference => (MutableTabReference)TabReference;

    public Uri Uri
    {
        get => MutableTabReference.Uri;
        set => MutableTabReference.Uri = value;
    }

    public override string Title
    {
        get
        {
            if (MarkdownSource == null)
            {
                return MarkdownViewerModuleConstants.Title;
            }

            return Uri.UnescapeDataString(MarkdownSource.SourceUri.Segments.Last());
        }
    }

    public override object View => LifetimeScope.Resolve<MarkdownViewerView>(
        new TypedParameter(typeof(IMarkdownViewerTabItem), this));

    private ITabManager TabManager => LifetimeScope.Resolve<ITabManager>();

    public MarkdownSource? MarkdownSource { get; private set; }

    public async Task UpdateMarkdownSourceAsync(MarkdownSource markdownSource)
    {
        var oldUri = Uri;
        Uri = MarkdownRoute.BuildWithDocument(markdownSource.SourceUri);

        if (UriTools.Equals(oldUri, Uri))
        {
            return;
        }

        MarkdownSource = markdownSource;
        await TabManager.TabInternalNavigatingAsync(oldUri, Uri);

        OnPropertyChanged(nameof(Title));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}