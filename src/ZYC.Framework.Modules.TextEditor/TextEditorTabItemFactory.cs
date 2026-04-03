using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.TextEditor.Abstractions;

namespace ZYC.Framework.Modules.TextEditor;

[RegisterSingleInstance]
[TabItemRoute(Host = TextEditorModuleConstants.EditorHost, Path = TextEditorModuleConstants.EditorPath)]
internal class TextEditorTabItemFactory : TabItemFactoryBase
{
    public override int Priority => 40;

    public override bool IsSingle => true;

    public override async Task<bool> CheckUriMatchedAsync(Uri uri)
    {
        if (!await base.CheckUriMatchedAsync(uri))
        {
            return false;
        }

        return TextDocumentTools.TryGetEditorFileUri(uri, out var fileUri)
               && fileUri is not null
               && TextDocumentTools.IsTextFile(fileUri);
    }

    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<TextEditorTabItem>(
            new TypedParameter(typeof(MutableTabReference), new MutableTabReference(context.Uri)));
    }
}
