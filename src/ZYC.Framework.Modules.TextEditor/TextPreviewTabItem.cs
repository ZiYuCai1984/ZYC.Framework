using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.TextEditor.Abstractions;
using ZYC.Framework.Modules.TextEditor.UI;

namespace ZYC.Framework.Modules.TextEditor;

[Register]
internal class TextPreviewTabItem : TabItemInstanceBase
{
    public TextPreviewTabItem(
        ILifetimeScope lifetimeScope,
        TabReference tabReference) : base(lifetimeScope, tabReference)
    {
    }

    private Uri DocumentUri => TabReference.Uri;

    public override string Host => TextEditorModuleConstants.PreviewHost;

    public override string Title => TextDocumentTools.GetDisplayName(DocumentUri.LocalPath);

    public override string Icon => TextEditorModuleConstants.PreviewIcon;

    public override bool Localization => false;

    public override object View => _view ??= LifetimeScope.Resolve<TextPreviewView>(
        new TypedParameter(typeof(Uri), DocumentUri));
}
