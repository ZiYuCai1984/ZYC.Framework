using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZYC.Framework.Core.MarkupEx;

public class InterfaceTemplateSelectorExtension : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new InterfaceTemplateSelector();
    }
}

public sealed class InterfaceTemplateSelector : DataTemplateSelector
{
    // ① Cache: cache templates by runtime Type to avoid frequent resource lookups.
    private static readonly ConcurrentDictionary<Type, DataTemplate?> cache
        = new();

    // ② Fallback template (lazy-loaded + bindable).
    private static DataTemplate? _notFound;
    private static DataTemplate NotFoundTemplate => _notFound ??= CreateNotFoundTemplate();

    private static DataTemplate CreateNotFoundTemplate()
    {
        // FrameworkElementFactory is obsolete but still usable; XamlReader.Parse is a cleaner option if desired.
#pragma warning disable 0618
        var fef = new FrameworkElementFactory(typeof(TextBlock));
#pragma warning restore 0618
        fef.SetBinding(TextBlock.TextProperty, new Binding()); // Bind to item's ToString().
        return new DataTemplate { VisualTree = fef };
    }

    public override DataTemplate SelectTemplate(object? item, DependencyObject container)
    {
        if (item is null)
        {
            return NotFoundTemplate;
        }

        // ③ Container may be FrameworkElement or FrameworkContentElement; avoid forced casting.
        var fe = container as FrameworkElement;
        var fce = container as FrameworkContentElement;

        if (fe is null && fce is null)
        {
            return NotFoundTemplate; // Extreme case (rare), return defensively.
        }

        var type = item.GetType();

        // ④ Cache hit: return immediately.
        if (cache.TryGetValue(type, out var cached))
        {
            return cached ?? NotFoundTemplate;
        }

        // ⑤ Lookup order: concrete type -> "more specific interface" -> base class chain.
        // "More specific interface" is determined by interface inheritance depth (desc);
        // e.g. IDefaultSideBarItem before ISideBarItem.
        DataTemplate? resolved = null;

        // a) Concrete type
        resolved ??= TryFindDataTemplate(fe, fce, new DataTemplateKey(type));

        // b) Interfaces (sorted by inheritance depth; more "specific" first)
        if (resolved is null)
        {
            var interfaces = type.GetInterfaces()
                .OrderByDescending(InterfaceDepth);

            foreach (var itf in interfaces)
            {
                resolved = TryFindDataTemplate(fe, fce, new DataTemplateKey(itf));
                if (resolved is not null)
                {
                    break;
                }
            }
        }

        // c) Base type fallback
        if (resolved is null)
        {
            var t = type.BaseType;
            while (t is not null && t != typeof(object))
            {
                resolved = TryFindDataTemplate(fe, fce, new DataTemplateKey(t));
                if (resolved is not null)
                {
                    break;
                }

                t = t.BaseType;
            }
        }

        // ⑥ Store in cache (cache null too to avoid repeated lookups).
        cache[type] = resolved;

        return resolved ?? NotFoundTemplate;
    }

    private static DataTemplate? TryFindDataTemplate(FrameworkElement? fe, FrameworkContentElement? fce, object key)
    {
        if (fe is not null && fe.TryFindResource(key) is DataTemplate dt1)
        {
            return dt1;
        }

        if (fce is not null && fce.TryFindResource(key) is DataTemplate dt2)
        {
            return dt2;
        }

        // Compatibility: if the container tree is odd, fall back to Application lookup.
        if (Application.Current?.TryFindResource(key) is DataTemplate dt3)
        {
            return dt3;
        }

        return null;
    }

    // Compute interface "depth": larger means more specific.
    private static int InterfaceDepth(Type itf)
    {
        // Self + all ancestor interfaces
        var depth = 1;
        var q = new Queue<Type>();
        q.Enqueue(itf);
        while (q.Count > 0)
        {
            var t = q.Dequeue();
            foreach (var p in t.GetInterfaces())
            {
                depth++;
                q.Enqueue(p);
            }
        }

        return depth;
    }
}