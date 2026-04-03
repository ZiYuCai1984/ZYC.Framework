using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;

namespace ZYC.Framework.Modules.TextEditor;

[RegisterSingleInstance]
internal class TextPreviewTabItemFactory : ITabItemFactory
{
    public int Priority => 30;

    public bool IsSingle => true;

    public async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<TextPreviewTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }

    public async Task<bool> CheckUriMatchedAsync(Uri uri)
    {
        await Task.CompletedTask;
        return TextDocumentTools.IsTextFile(uri);
    }
}
