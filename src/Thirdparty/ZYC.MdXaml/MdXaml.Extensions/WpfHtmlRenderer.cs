using AngleSharp.Dom;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ZYC.MdXaml.MdXaml.Extensions;

internal sealed class WpfHtmlRenderer
{
    private readonly IMarkdownHtmlContext? _ctx;

    public WpfHtmlRenderer(IMarkdownHtmlContext? ctx)
    {
        _ctx = ctx;
    }

    public bool SupportTextAlignment { get; set; }

    public IEnumerable<Block> RenderBlocks(IElement wrapper)
    {
        foreach (var node in wrapper.ChildNodes)
        {
            foreach (var b in RenderBlockNode(node))
            {
                yield return b;
            }
        }
    }

    public IEnumerable<Inline> RenderInlines(IElement wrapper)
    {
        foreach (var node in wrapper.ChildNodes)
        {
            foreach (var i in RenderInlineNode(node))
            {
                yield return i;
            }
        }
    }

    private IEnumerable<Block> RenderBlockNode(INode node)
    {
        if (node is IText text)
        {
            var s = NormalizeText(text.Data);
            if (string.IsNullOrWhiteSpace(s))
            {
                yield break;
            }

            // Text at block level => paragraph
            var p = new Paragraph();
            p.Inlines.Add(new Run(s));
            yield return p;
            yield break;
        }

        if (node is not IElement el)
        {
            yield break;
        }

        var tag = el.TagName.ToLowerInvariant();

        // If an inline tag appears in block context, wrap it into a paragraph.
        if (HtmlAllowList.IsAllowedInlineTag(tag))
        {
            var p = new Paragraph();
            foreach (var inline in RenderInlineElement(el))
            {
                p.Inlines.Add(inline);
            }

            yield return p;
            yield break;
        }

        switch (tag)
        {
            case "p":
                yield return MakeParagraph(el);
                break;

            case "div":
                {
                    // If div contains any block tags, create Section; otherwise Paragraph.
                    var hasBlockChild = el.Children.Any(c => HtmlAllowList.IsAllowedBlockTag(c.TagName.ToLowerInvariant()));
                    if (hasBlockChild)
                    {
                        var sec = new Section();
                        foreach (var child in el.ChildNodes)
                            foreach (var b in RenderBlockNode(child))
                            {
                                sec.Blocks.Add(b);
                            }

                        ApplyAlignment(sec, el);
                        yield return sec;
                    }
                    else
                    {
                        yield return MakeParagraph(el);
                    }

                    break;
                }

            case "h1":
            case "h2":
            case "h3":
            case "h4":
            case "h5":
            case "h6":
                yield return MakeHeading(el, tag);
                break;

            case "blockquote":
                yield return MakeBlockQuote(el);
                break;

            case "pre":
                yield return MakePre(el);
                break;

            case "hr":
                yield return MakeHorizontalRule();
                break;

            case "ul":
                yield return MakeList(el, false);
                break;

            case "ol":
                yield return MakeList(el, true);
                break;

            case "li":
                // li should be consumed inside list; if encountered alone, render as paragraph
                yield return MakeParagraph(el);
                break;

            case "table":
                var table = MakeTable(el);
                if (table is not null)
                {
                    yield return table;
                }
                else
                {
                    yield return MakeParagraphFallback(el);
                }

                break;

            case "details":
                yield return MakeDetails(el);
                break;

            case "summary":
                // summary is consumed by details; if encountered alone, treat as paragraph
                yield return MakeParagraph(el);
                break;

            default:
                // unwrap fallback: render children blocks
                foreach (var child in el.ChildNodes)
                    foreach (var b in RenderBlockNode(child))
                    {
                        yield return b;
                    }

                break;
        }
    }

    private Paragraph MakeParagraph(IElement el)
    {
        var p = new Paragraph();
        foreach (var child in el.ChildNodes)
        {
            foreach (var inline in RenderInlineNode(child))
            {
                p.Inlines.Add(inline);
            }
        }

        ApplyAlignment(p, el);

        // Reasonable GitHub-like spacing defaults (tweak later)
        p.Margin = new Thickness(0, 6, 0, 6);
        return p;
    }

    private Paragraph MakeHeading(IElement el, string tag)
    {
        var p = new Paragraph();
        foreach (var child in el.ChildNodes)
        {
            foreach (var inline in RenderInlineNode(child))
            {
                p.Inlines.Add(inline);
            }
        }

        p.FontWeight = FontWeights.SemiBold;
        p.Margin = new Thickness(0, 14, 0, 8);

        // Rough GitHub-ish sizes
        p.FontSize = tag switch
        {
            "h1" => 26,
            "h2" => 22,
            "h3" => 18,
            "h4" => 16,
            "h5" => 14,
            _ => 13
        };

        ApplyAlignment(p, el);
        return p;
    }

    private Block MakeBlockQuote(IElement el)
    {
        var sec = new Section();
        foreach (var child in el.ChildNodes)
            foreach (var b in RenderBlockNode(child))
            {
                sec.Blocks.Add(b);
            }

        sec.Margin = new Thickness(0, 8, 0, 8);
        sec.Padding = new Thickness(12, 6, 0, 6);
        sec.BorderThickness = new Thickness(3, 0, 0, 0);

        // No hard-coded colors if you prefer theming; using a subtle default:
        sec.BorderBrush = Brushes.LightGray;

        return sec;
    }

    private Block MakePre(IElement el)
    {
        // GitHub: pre usually contains code; we take textual content.
        var codeText = el.TextContent ?? "";
        codeText = codeText.Replace("\r\n", "\n").Replace("\r", "\n");

        var p = new Paragraph
        {
            Margin = new Thickness(0, 10, 0, 10),
            Padding = new Thickness(10),
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.LightGray,
            Background = Brushes.Transparent,
            FontFamily = new FontFamily("Consolas"),
            FontSize = 12
        };

        // Preserve newlines by emitting LineBreaks
        var lines = codeText.Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            p.Inlines.Add(new Run(lines[i]));
            if (i < lines.Length - 1)
            {
                p.Inlines.Add(new LineBreak());
            }
        }

        return p;
    }



    private Block MakeHorizontalRule()
    {
        // A simple "empty paragraph with bottom border"
        return new Paragraph
        {
            Margin = new Thickness(0, 12, 0, 12),
            BorderThickness = new Thickness(0, 0, 0, 1),
            BorderBrush = Brushes.LightGray
        };
    }

    private List MakeList(IElement el, bool ordered)
    {
        var list = new List
        {
            Margin = new Thickness(0, 6, 0, 6),
            MarkerStyle = ordered ? TextMarkerStyle.Decimal : TextMarkerStyle.Disc
        };

        foreach (var li in el.Children.Where(c => c.TagName.Equals("li", StringComparison.OrdinalIgnoreCase)))
        {
            var item = new ListItem();
            // If li has block children, render blocks; else create a paragraph.
            var hasBlock = li.Children.Any(c =>
                HtmlAllowList.IsAllowedBlockTag(c.TagName.ToLowerInvariant()) && c.TagName != "li");
            if (hasBlock)
            {
                foreach (var child in li.ChildNodes)
                    foreach (var b in RenderBlockNode(child))
                    {
                        item.Blocks.Add(b);
                    }
            }
            else
            {
                var p = new Paragraph();
                foreach (var child in li.ChildNodes)
                    foreach (var inline in RenderInlineNode(child))
                    {
                        p.Inlines.Add(inline);
                    }

                item.Blocks.Add(p);
            }

            list.ListItems.Add(item);
        }

        return list;
    }

    // HTML spec cap for colspan; also bounds column allocation for hostile input.
    private const int MaxTableSpan = 1000;

    private Table? MakeTable(IElement tableEl)
    {
        // Collect rows from thead/tbody or direct tr
        var rowGroups = new List<(bool isHeader, List<IElement> rows)>();

        var thead = tableEl.Children.FirstOrDefault(e => e.TagName.Equals("thead", StringComparison.OrdinalIgnoreCase));
        if (thead is not null)
        {
            rowGroups.Add((true,
                thead.Children.Where(x => x.TagName.Equals("tr", StringComparison.OrdinalIgnoreCase)).ToList()));
        }

        var tbody = tableEl.Children.FirstOrDefault(e => e.TagName.Equals("tbody", StringComparison.OrdinalIgnoreCase));
        if (tbody is not null)
        {
            rowGroups.Add((false,
                tbody.Children.Where(x => x.TagName.Equals("tr", StringComparison.OrdinalIgnoreCase)).ToList()));
        }

        if (thead is null && tbody is null)
        {
            var trs = tableEl.Children.Where(x => x.TagName.Equals("tr", StringComparison.OrdinalIgnoreCase)).ToList();
            if (trs.Count > 0)
            {
                rowGroups.Add((false, trs));
            }
        }

        if (rowGroups.Count == 0)
        {
            return null;
        }

        // Determine max columns count
        var maxCols = 0;
        foreach (var (_, rows) in rowGroups)
        {
            foreach (var tr in rows)
            {
                var cells = tr.Children.Where(c =>
                    c.TagName.Equals("td", StringComparison.OrdinalIgnoreCase) ||
                    c.TagName.Equals("th", StringComparison.OrdinalIgnoreCase)).ToList();
                var count = 0;
                foreach (var cell in cells)
                {
                    count += Math.Clamp(ParseInt(cell.GetAttribute("colspan"), 1), 1, MaxTableSpan);
                }

                maxCols = Math.Max(maxCols, count);
            }
        }

        if (maxCols <= 0)
        {
            return null;
        }

        maxCols = Math.Min(maxCols, MaxTableSpan);

        var table = new Table
        {
            CellSpacing = 0,
            Margin = new Thickness(0, 10, 0, 10)
        };

        for (var i = 0; i < maxCols; i++)
        {
            table.Columns.Add(new TableColumn());
        }

        // Basic border like GitHub (tweak later)
        table.BorderBrush = Brushes.LightGray;
        table.BorderThickness = new Thickness(1);

        foreach (var (isHeader, rows) in rowGroups)
        {
            var rg = new TableRowGroup();
            foreach (var tr in rows)
            {
                var row = new TableRow();
                var cells = tr.Children.Where(c =>
                    c.TagName.Equals("td", StringComparison.OrdinalIgnoreCase) ||
                    c.TagName.Equals("th", StringComparison.OrdinalIgnoreCase)).ToList();
                foreach (var cellEl in cells)
                {
                    var cell = new TableCell();
                    cell.BorderBrush = Brushes.LightGray;
                    cell.BorderThickness = new Thickness(0.5);

                    var p = new Paragraph { Margin = new Thickness(6, 4, 6, 4) };
                    foreach (var child in cellEl.ChildNodes)
                        foreach (var inline in RenderInlineNode(child))
                        {
                            p.Inlines.Add(inline);
                        }

                    ApplyAlignment(p, cellEl);

                    if (cellEl.TagName.Equals("th", StringComparison.OrdinalIgnoreCase) || isHeader)
                    {
                        p.FontWeight = FontWeights.SemiBold;
                    }

                    cell.Blocks.Add(p);

                    cell.ColumnSpan = Math.Clamp(ParseInt(cellEl.GetAttribute("colspan"), 1), 1, MaxTableSpan);
                    cell.RowSpan = Math.Clamp(ParseInt(cellEl.GetAttribute("rowspan"), 1), 1, MaxTableSpan);

                    row.Cells.Add(cell);
                }

                rg.Rows.Add(row);
            }

            table.RowGroups.Add(rg);
        }

        return table;
    }

    private Block MakeDetails(IElement detailsEl)
    {
        // details/summary -> BlockUIContainer(Expander)
        var expander = new Expander
        {
            IsExpanded = detailsEl.HasAttribute("open"),
            Margin = new Thickness(0, 8, 0, 8)
        };

        // summary header
        var summary =
            detailsEl.Children.FirstOrDefault(c => c.TagName.Equals("summary", StringComparison.OrdinalIgnoreCase));
        if (summary is not null)
        {
            var header = new TextBlock { TextWrapping = TextWrapping.Wrap };
            foreach (var child in summary.ChildNodes)
            {
                foreach (var inline in RenderInlineNode(child))
                {
                    header.Inlines.Add(inline);
                }
            }

            expander.Header = header;
        }
        else
        {
            expander.Header = new TextBlock { Text = "Details" };
        }

        // body: render remaining children as FlowDocument inside FlowDocumentScrollViewer
        var bodyWrapper = new FlowDocument();
        foreach (var child in detailsEl.ChildNodes)
        {
            if (child is IElement ce && ce.TagName.Equals("summary", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            foreach (var b in RenderBlockNode(child))
            {
                bodyWrapper.Blocks.Add(b);
            }
        }

        var viewer = new FlowDocumentScrollViewer
        {
            Document = bodyWrapper,
            IsToolBarVisible = false,
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
            Margin = new Thickness(8, 6, 0, 0)
        };

        // The nested viewer swallows wheel events even with hidden scrollbars;
        // re-raise them from the expander so the outer document keeps scrolling.
        viewer.PreviewMouseWheel += (s, e) =>
        {
            if (e.Handled)
            {
                return;
            }

            e.Handled = true;
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = s,
            };
            expander.RaiseEvent(eventArg);
        };

        expander.Content = viewer;

        return new BlockUIContainer(expander);
    }

    private Paragraph MakeParagraphFallback(IElement el)
    {
        var p = new Paragraph();
        var text = NormalizeText(el.TextContent ?? "");
        if (!string.IsNullOrWhiteSpace(text))
        {
            p.Inlines.Add(new Run(text));
        }

        return p;
    }

    private IEnumerable<Inline> RenderInlineNode(INode node)
    {
        if (node is IText t)
        {
            var s = NormalizeHtmlTextCollapseWhitespace(t.Data);
            if (s.Length == 0)
            {
                yield break;
            }

            yield return new Run(s);
            yield break;
        }

        if (node is IElement el)
        {
            foreach (var i in RenderInlineElement(el))
            {
                yield return i;
            }
        }
    }

    private static string NormalizeHtmlTextCollapseWhitespace(string s)
    {
        // AngleSharp already decodes entities in text nodes; decoding again
        // would turn literal "&lt;" (written as "&amp;lt;") into "<".
        s = s.Replace("\0", "");

        // collapse any whitespace sequence into a single space (HTML behavior)
        var sb = new StringBuilder(s.Length);
        bool prevWs = false;

        foreach (var ch in s)
        {
            if (char.IsWhiteSpace(ch))
            {
                if (!prevWs)
                {
                    sb.Append(' ');
                    prevWs = true;
                }
            }
            else
            {
                sb.Append(ch);
                prevWs = false;
            }
        }

        return sb.ToString();
    }

    private IEnumerable<Inline> RenderInlineElement(IElement el)
    {
        var tag = el.TagName.ToLowerInvariant();

        // If a block tag appears inside inline context, just render its text content as a Run
        if (HtmlAllowList.IsAllowedBlockTag(tag))
        {
            var txt = NormalizeText(el.TextContent ?? "");
            if (txt.Length > 0)
            {
                yield return new Run(txt);
            }

            yield break;
        }

        switch (tag)
        {
            case "br":
                yield return new LineBreak();
                break;

            case "a":
                yield return MakeHyperlink(el);
                break;

            case "img":
                foreach (var i in MakeImageInline(el))
                {
                    yield return i;
                }

                break;

            case "strong":
            case "b":
                {
                    var b = new Bold();
                    foreach (var child in el.ChildNodes)
                        foreach (var i in RenderInlineNode(child))
                        {
                            b.Inlines.Add(i);
                        }

                    yield return b;
                    break;
                }

            case "em":
            case "i":
                {
                    var it = new Italic();
                    foreach (var child in el.ChildNodes)
                        foreach (var i in RenderInlineNode(child))
                        {
                            it.Inlines.Add(i);
                        }

                    yield return it;
                    break;
                }

            case "del":
            case "s":
            case "strike":
                {
                    var sp = new Span { TextDecorations = TextDecorations.Strikethrough };
                    foreach (var child in el.ChildNodes)
                        foreach (var i in RenderInlineNode(child))
                        {
                            sp.Inlines.Add(i);
                        }

                    yield return sp;
                    break;
                }

            case "code":
                {
                    var sp = new Span
                    {
                        FontFamily = new FontFamily("Consolas"),
                        FontSize = 12,
                        Background = Brushes.Transparent
                    };
                    foreach (var child in el.ChildNodes)
                        foreach (var i in RenderInlineNode(child))
                        {
                            sp.Inlines.Add(i);
                        }

                    yield return sp;
                    break;
                }

            case "kbd":
                {
                    var border = new Border
                    {
                        Padding = new Thickness(4, 1, 4, 1),
                        CornerRadius = new CornerRadius(4),
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brushes.LightGray,
                        Background = Brushes.Transparent,
                        Child = new TextBlock
                        {
                            FontFamily = new FontFamily("Consolas"),
                            FontSize = 12,
                            Text = NormalizeText(el.TextContent ?? "")
                        }
                    };
                    yield return new InlineUIContainer(border);
                    break;
                }

            case "sub":
                {
                    var sp = new Span { BaselineAlignment = BaselineAlignment.Subscript };
                    foreach (var child in el.ChildNodes)
                        foreach (var i in RenderInlineNode(child))
                        {
                            sp.Inlines.Add(i);
                        }

                    yield return sp;
                    break;
                }

            case "sup":
                {
                    var sp = new Span { BaselineAlignment = BaselineAlignment.Superscript };
                    foreach (var child in el.ChildNodes)
                        foreach (var i in RenderInlineNode(child))
                        {
                            sp.Inlines.Add(i);
                        }

                    yield return sp;
                    break;
                }

            case "span":
                {
                    var sp = new Span();
                    foreach (var child in el.ChildNodes)
                        foreach (var i in RenderInlineNode(child))
                        {
                            sp.Inlines.Add(i);
                        }

                    yield return sp;
                    break;
                }

            case "input":
                {
                    var type = (el.GetAttribute("type") ?? "").Trim();
                    if (type.Equals("checkbox", StringComparison.OrdinalIgnoreCase))
                    {
                        var isChecked = el.HasAttribute("checked");
                        yield return new Run(isChecked ? "☑ " : "☐ ");
                    }

                    break;
                }

            default:
                // Unknown inline tag: render children
                foreach (var child in el.ChildNodes)
                    foreach (var i in RenderInlineNode(child))
                    {
                        yield return i;
                    }

                break;
        }
    }

    private Hyperlink MakeHyperlink(IElement el)
    {
        var link = new Hyperlink();
        var title = el.GetAttribute("title");
        if (!string.IsNullOrWhiteSpace(title))
        {
            link.ToolTip = title;
        }

        var href = el.GetAttribute("href");
        if (HtmlUrlPolicy.TrySanitizeUrl(href, _ctx, false, out var uri) && uri is not null)
        {
            // Might be relative
            if (uri.IsAbsoluteUri)
            {
                link.NavigateUri = uri;
            }
            else if (_ctx?.BaseUri is not null && Uri.TryCreate(_ctx.BaseUri, uri, out var resolved))
            {
                link.NavigateUri = resolved;
            }

            // Same click pipeline as markdown-syntax links. Keep the raw href
            // as the parameter so "#anchor" jumps keep working.
            link.CommandParameter = href;
            link.Command = _ctx?.HyperlinkCommand;

            var callback = _ctx?.OnHyperLinkClicked;
            if (callback is not null)
            {
                link.Click += new CallbackHolder(callback).Clicked;
            }
        }

        foreach (var child in el.ChildNodes)
            foreach (var i in RenderInlineNode(child))
            {
                link.Inlines.Add(i);
            }

        // If no content, show href
        if (!link.Inlines.Any() && !string.IsNullOrWhiteSpace(href))
        {
            link.Inlines.Add(new Run(href));
        }

        return link;
    }



    private IEnumerable<Inline> MakeImageInline(IElement el)
    {
        var inlines = new List<Inline>();

        var alt = el.GetAttribute("alt") ?? "";
        var title = el.GetAttribute("title");
        var src = el.GetAttribute("src");
        var allowData = _ctx?.AllowDataImages == true;

        void AddFallback()
        {
            var fallback = string.IsNullOrWhiteSpace(alt) ? "[image]" : alt;
            inlines.Add(new Run(fallback));
        }

        if (!HtmlUrlPolicy.TrySanitizeUrl(src, _ctx, allowData, out var uri) || uri is null)
        {
            AddFallback();
            return inlines;
        }
        
        const double MaxInlineImageWidth = 480;
        const double MaxInlineImageHeight = 360;

        var img = new Image
        {
            Stretch = Stretch.Uniform,
            SnapsToDevicePixels = true,
            UseLayoutRounding = true,
        };

        var box = new Viewbox
        {
            Stretch = Stretch.Uniform,
            StretchDirection = StretchDirection.DownOnly,
            MaxWidth = MaxInlineImageWidth,
            MaxHeight = MaxInlineImageHeight,
            Child = img,
        };


        if (TryParseDouble(el.GetAttribute("width"), out var w) && IsValidImageSize(w))
        {
            img.Width = w;
        }

        if (TryParseDouble(el.GetAttribute("height"), out var h) && IsValidImageSize(h))
        {
            img.Height = h;
        }

        if (!string.IsNullOrWhiteSpace(title) || !string.IsNullOrWhiteSpace(alt))
        {
            img.ToolTip = string.IsNullOrWhiteSpace(title) ? alt : title;
        }

        try
        {
            if (!uri.IsAbsoluteUri)
            {
                AddFallback();
                return inlines;
            }

            if (uri.Scheme == Uri.UriSchemeHttp
                    || uri.Scheme == Uri.UriSchemeHttps)
            {
                // Never block the transform (UI) thread on the network:
                // show the element immediately and fill the source later.
                if (TryGetCachedImage(uri, out var cached))
                {
                    img.Source = cached;
                }
                else
                {
                    SetRemoteImageSourceAsync(img, uri);
                }

                inlines.Add(new InlineUIContainer(box)
                {
                    BaselineAlignment = BaselineAlignment.Center
                });
                return inlines;
            }

            if (allowData && uri.Scheme.Equals("data", StringComparison.OrdinalIgnoreCase))
            {
                var imageSource = LoadDataUriImage(src!);
                if (imageSource is null)
                {
                    AddFallback();
                    return inlines;
                }

                img.Source = imageSource;
                inlines.Add(new InlineUIContainer(box)
                {
                    BaselineAlignment = BaselineAlignment.Center
                });
                return inlines;
            }

            if (uri.Scheme == Uri.UriSchemeFile)
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = uri;
                bmp.CacheOption = BitmapCacheOption.OnDemand;
                bmp.CreateOptions = BitmapCreateOptions.DelayCreation;
                bmp.EndInit();
                img.Source = bmp;
                inlines.Add(new InlineUIContainer(box)
                {
                    BaselineAlignment = BaselineAlignment.Center
                });
                return inlines;
            }

            AddFallback();
            return inlines;
        }
        catch
        {
            AddFallback();
            return inlines;
        }
    }

    private static readonly HttpClient _httpClient = new(new HttpClientHandler
    {
        AllowAutoRedirect = true
    })
    {
        Timeout = TimeSpan.FromSeconds(15),
    };

    private const long MaxRemoteImageBytes = 20 * 1024 * 1024;

    private static readonly ConcurrentDictionary<Uri, WeakReference<ImageSource>> s_remoteImageCache = new();
    private static readonly ConcurrentDictionary<Uri, Task<ImageSource?>> s_pendingRemoteLoads = new();

    private static bool IsValidImageSize(double value)
    {
        return !double.IsNaN(value) && !double.IsInfinity(value) && value > 0 && value <= 4096;
    }

    private static bool TryGetCachedImage(Uri uri, out ImageSource? source)
    {
        source = null;
        if (s_remoteImageCache.TryGetValue(uri, out var reference))
        {
            if (reference.TryGetTarget(out var cached))
            {
                source = cached;
                return true;
            }

            s_remoteImageCache.TryRemove(uri, out _);
        }

        return false;
    }

    private static async void SetRemoteImageSourceAsync(Image img, Uri uri)
    {
        try
        {
            // Coalesce concurrent requests for the same url (e.g. repeated badges).
            var task = s_pendingRemoteLoads.GetOrAdd(uri, static u => Task.Run(() => LoadImage(u)));
            ImageSource? source;
            try
            {
                source = await task.ConfigureAwait(false);
            }
            finally
            {
                s_pendingRemoteLoads.TryRemove(uri, out _);
            }

            if (source is null)
            {
                return;
            }

            s_remoteImageCache[uri] = new WeakReference<ImageSource>(source);
            await img.Dispatcher.InvokeAsync(() => img.Source = source);
        }
        catch (Exception ex)
        {
            Trace.WriteLine("[IMG] async image load failed: " + ex);
        }
    }

    private static ImageSource? LoadImage(Uri uri)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, uri);
            req.Headers.UserAgent.ParseAdd("Mozilla/5.0");
            req.Headers.Accept.ParseAdd("image/*,*/*;q=0.8");

            using var resp = _httpClient.Send(req, HttpCompletionOption.ResponseHeadersRead);
            resp.EnsureSuccessStatusCode();

            if (resp.Content.Headers.ContentLength is long contentLength && contentLength > MaxRemoteImageBytes)
            {
                Trace.WriteLine("[IMG] image too large: " + uri);
                return null;
            }

            var ct = resp.Content.Headers.ContentType?.MediaType;

            using var s = resp.Content.ReadAsStream();
            using var ms = new MemoryStream();
            if (!TryCopyWithLimit(s, ms, MaxRemoteImageBytes))
            {
                Trace.WriteLine("[IMG] image too large: " + uri);
                return null;
            }

            ms.Position = 0;
            if (LooksLikeSvg(ct, uri, ms))
            {
                ms.Position = 0;
                return LoadSvgImageSource(ms);
            }

            ms.Position = 0;
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.StreamSource = ms;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }
        catch (Exception ex)
        {
            Trace.WriteLine("[IMG] LoadImage failed: " + ex);
            return null;
        }
    }

    private static bool TryCopyWithLimit(Stream source, MemoryStream destination, long limit)
    {
        var buffer = new byte[81920];
        long total = 0;
        int read;
        while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
        {
            total += read;
            if (total > limit)
            {
                return false;
            }

            destination.Write(buffer, 0, read);
        }

        return true;
    }

    private static ImageSource? LoadDataUriImage(string raw)
    {
        try
        {
            var comma = raw.IndexOf(',');
            if (comma < 0)
            {
                return null;
            }

            var meta = raw.Substring(0, comma);
            if (!meta.Contains("base64", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var bytes = Convert.FromBase64String(raw.Substring(comma + 1).Trim());
            if (bytes.Length == 0 || bytes.Length > MaxRemoteImageBytes)
            {
                return null;
            }

            using var ms = new MemoryStream(bytes, false);
            if (meta.Contains("svg", StringComparison.OrdinalIgnoreCase))
            {
                return LoadSvgImageSource(ms);
            }

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.StreamSource = ms;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }
        catch (Exception ex)
        {
            Trace.WriteLine("[IMG] LoadDataUriImage failed: " + ex);
            return null;
        }
    }

    private static bool LooksLikeSvg(string? contentType, Uri uri, MemoryStream ms)
    {
        if (!string.IsNullOrWhiteSpace(contentType) &&
            contentType.Contains("svg", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var path = uri.AbsolutePath;
        if (path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".svgz", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        var len = (int)Math.Min(ms.Length, 1024);
        if (len <= 0) return false;

        var head = Encoding.UTF8.GetString(ms.GetBuffer(), 0, len);
        return head.IndexOf("<svg", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static ImageSource? LoadSvgImageSource(Stream svgStream)
    {
        try
        {

            var settings = new WpfDrawingSettings
            {

            };

            var reader = new FileSvgReader(settings);

            var drawing = reader.Read(svgStream);
            if (drawing is null) return null;

            drawing.Freeze();
            var img = new DrawingImage(drawing);
            img.Freeze();
            return img;
        }
        catch (Exception ex)
        {
            Trace.WriteLine("[SVG] LoadSvgImageSource failed: " + ex);
            return null;
        }
    }

    private void ApplyAlignment(Block block, IElement el)
    {
        if (!SupportTextAlignment)
        {
            return;
        }

        var align = (el.GetAttribute("align") ?? "").Trim().ToLowerInvariant();
        if (align.Length == 0)
        {
            return;
        }

        var ta = align switch
        {
            "left" => TextAlignment.Left,
            "right" => TextAlignment.Right,
            "center" => TextAlignment.Center,
            "justify" => TextAlignment.Justify,
            _ => (TextAlignment?)null
        };

        if (ta is null)
        {
            return;
        }

        // Paragraph and Section both derive Block, but alignment property differs.
        if (block is Paragraph p)
        {
            p.TextAlignment = ta.Value;
        }
        // Section doesn't have TextAlignment; you can apply to contained paragraphs if needed.
    }

    private static string NormalizeText(string s)
    {
        // AngleSharp already decodes entities in text nodes; decoding again
        // would double-unescape. Keep whitespace mostly intact; remove nulls.
        return s.Replace("\0", "");
    }

    private static int ParseInt(string? s, int fallback)
    {
        if (int.TryParse((s ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var n))
        {
            return n;
        }

        return fallback;
    }

    private static bool TryParseDouble(string? s, out double value)
    {
        return double.TryParse((s ?? "").Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}