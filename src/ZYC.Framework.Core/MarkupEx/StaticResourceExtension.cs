using System.Windows.Markup;

namespace ZYC.Framework.Core.MarkupEx;

[MarkupExtensionReturnType(typeof(object))]
public class StaticResourceExtension : System.Windows.StaticResourceExtension
{
    public StaticResourceExtension()
    {
    }

    public StaticResourceExtension(object resourceKey)
        : base(resourceKey)
    {
    }
}