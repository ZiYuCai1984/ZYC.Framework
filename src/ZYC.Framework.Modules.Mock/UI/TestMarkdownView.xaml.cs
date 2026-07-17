using System.Windows;
using System.Windows.Controls;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.Modules.Mock.UI;

[Register]
public partial class TestMarkdownView
{
    private const string SampleMarkdown =
        """
        # MdXaml Rendering Test Page

        <!-- This HTML comment must not be displayed -->

        ## 1. Basic Syntax

        **bold** *italic* ~~strikethrough~~ `inline code` 😀 Flags: 🇨🇳 🇺🇸 🇯🇵 ZWJ sequence: 👨‍👩‍👧‍👦 ☀️

        - List item one
        - List item two

        [Markdown link (click should open directly)](https://example.com) / [mailto link (click should ask for confirmation)](mailto:test@example.com)

        ```csharp
        // Code block (AvalonEdit)
        Console.WriteLine("Hello");
        ```

        ## 2. Inline / Block HTML

        <p align="center">Centered paragraph, <b>HTML bold</b>, <a href="https://example.com" title="tooltip">HTML link (should be clickable)</a></p>

        Entity decoding: <span>&amp;lt;</span> (should display the four characters &lt; rather than <)

        <details>
        <summary>Click to expand details</summary>

        Inner paragraph; hovering here and scrolling the wheel should still scroll the outer document.

        </details>

        ## 3. Sanitizer

        The next line must not display any script text:

        <div><center><script>alert('script text must not be displayed')</script></center></div>

        <a href="javascript:alert(1)">javascript link (text should display but not be clickable)</a>

        ## 4. Images

        Markdown remote image (async, must not block the UI): ![badge](https://img.shields.io/badge/markdown-image-blue)

        HTML remote image: <img src="https://img.shields.io/badge/html-image-orange" alt="html image" />

        Repeated URL (should hit the cache): ![badge again](https://img.shields.io/badge/markdown-image-blue)

        Dead link (should fall back to alt text): ![dead image fallback](https://invalid.invalid/nope.png)

        data URI: <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==" width="32" height="32" alt="data uri" />

        Invalid size (must not crash): <img src="https://img.shields.io/badge/invalid-size-red" width="-5" height="Infinity" alt="invalid size" />

        ## 5. Tables

        A malicious colspan should be clamped and must not hang:

        <table>
        <tr><th>A</th><th>B</th></tr>
        <tr><td colspan="2000000000">colspan=2000000000</td></tr>
        <tr><td align="right">right-aligned</td><td>b</td></tr>
        </table>

        | Column 1 | Column 2 |
        |----------|----------|
        | a        | b        |

        ## 6. Malformed HTML (fault tolerance)

        <p>This paragraph has a stray </div> closing tag in the middle; the rest of this sentence must not be lost.</p>

        The last line is an unclosed tag; it should render as plain text (without breaking anything above):

        <div class="unclosed
        """;

    public TestMarkdownView()
    {
        InitializeComponent();

        SourceTextBox.Text = SampleMarkdown;
    }

    private void OnSourceTextChanged(object sender, TextChangedEventArgs e)
    {
        RenderPreview(false);
    }

    private void OnResetSampleBtnClick(object sender, RoutedEventArgs e)
    {
        if (SourceTextBox.Text == SampleMarkdown)
        {
            RenderPreview(true);
        }
        else
        {
            // TextChanged renders the preview.
            SourceTextBox.Text = SampleMarkdown;
        }
    }

    private void OnRenderBtnClick(object sender, RoutedEventArgs e)
    {
        RenderPreview(true);
    }

    private void OnDisabledLazyLoadChanged(object sender, RoutedEventArgs e)
    {
        RenderPreview(true);
    }

    private void RenderPreview(bool force)
    {
        if (MarkdownViewer is null || SourceTextBox is null)
        {
            return;
        }

        MarkdownViewer.DisabledLazyLoad = DisabledLazyLoadCheckBox?.IsChecked == true;

        var text = SourceTextBox.Text;
        if (force && MarkdownViewer.Markdown == text)
        {
            // Assigning the same string does not trigger a property change;
            // clear first so the document is rebuilt.
            MarkdownViewer.Markdown = string.Empty;
        }

        MarkdownViewer.Markdown = text;
    }
}
