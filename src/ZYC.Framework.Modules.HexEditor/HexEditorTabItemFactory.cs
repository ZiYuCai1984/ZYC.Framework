using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.HexEditor.Abstractions;

namespace ZYC.Framework.Modules.HexEditor;

[RegisterSingleInstance]
[TabItemRoute(Host = HexEditorModuleConstants.Host, Path = HexEditorModuleConstants.EditorPath)]
internal class HexEditorTabItemFactory : TabItemFactoryBase
{
    public override int Priority => 40;

    public override async Task<bool> CheckUriMatchedAsync(Uri uri)
    {
        return uri.IsAbsoluteUri
               && await base.CheckUriMatchedAsync(uri)
               && HexDocumentTools.TryGetEditorFileUri(uri, out _);
    }

    public override Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        return Task.FromResult<ITabItemInstance>(context.Resolve<HexEditorTabItem>(
            new TypedParameter(typeof(MutableTabReference), new MutableTabReference(context.Uri))));
    }
}
