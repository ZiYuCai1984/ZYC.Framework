using AngleSharp.Dom;

namespace ZYC.MdXaml.MdXaml.Extensions;

internal static class HtmlDomSanitizer
{
    public static void Sanitize(IElement wrapper, IMarkdownHtmlContext? ctx)
    {
        // Sanitize all descendants. Unknown tags are unwrapped; reject tags removed.
        foreach (var child in wrapper.Children.ToArray())
        {
            SanitizeElement(child, ctx);
        }
    }

    private static void SanitizeElement(IElement el, IMarkdownHtmlContext? ctx)
    {
        var name = el.TagName.ToLowerInvariant();

        if (HtmlAllowList.RejectTags.Contains(name))
        {
            el.Remove();
            return;
        }

        // Special-case: input only if checkbox
        if (name.Equals("input", StringComparison.OrdinalIgnoreCase))
        {
            var type = (el.GetAttribute("type") ?? "").Trim();
            if (!type.Equals("checkbox", StringComparison.OrdinalIgnoreCase))
            {
                el.Remove();
                return;
            }
        }

        if (!HtmlAllowList.IsAllowedTag(name))
        {
            // Sanitize children first: Unwrap moves them into the parent,
            // where the caller's snapshot iteration never visits them again.
            foreach (var child in el.Children.ToArray())
            {
                SanitizeElement(child, ctx);
            }

            Unwrap(el);
            return;
        }

        // Remove all attributes except allowlisted
        if (HtmlAllowList.AllowedAttributes.TryGetValue(name, out var allowAttrs))
        {
            foreach (var attr in el.Attributes.ToArray())
            {
                if (!allowAttrs.Contains(attr.Name))
                {
                    el.RemoveAttribute(attr.Name);
                }
            }
        }
        else
        {
            // No attributes allowed for this tag
            foreach (var attr in el.Attributes.ToArray())
            {
                el.RemoveAttribute(attr.Name);
            }
        }

        // URL attribute sanitization
        if (name.Equals("a", StringComparison.OrdinalIgnoreCase))
        {
            var href = el.GetAttribute("href");
            if (!HtmlUrlPolicy.TrySanitizeUrl(href, ctx, false, out _))
            {
                el.RemoveAttribute("href");
            }
        }
        else if (name.Equals("img", StringComparison.OrdinalIgnoreCase))
        {
            var src = el.GetAttribute("src");
            var allowData = ctx?.AllowDataImages == true;
            if (!HtmlUrlPolicy.TrySanitizeUrl(src, ctx, allowData, out _))
            {
                // If image src is unsafe/invalid, drop the element entirely.
                // (Alternative: unwrap and keep alt text; renderer also falls back to alt.)
                el.Remove();
                return;
            }
        }

        // Recurse children
        foreach (var child in el.Children.ToArray())
        {
            SanitizeElement(child, ctx);
        }
    }

    private static void Unwrap(IElement el)
    {
        var parent = el.ParentElement;
        if (parent is null)
        {
            el.Remove();
            return;
        }

        // Move children before element, then remove element
        var insertBefore = el;
        foreach (var node in el.ChildNodes.ToArray())
        {
            parent.InsertBefore(node, insertBefore);
        }

        el.Remove();
    }
}