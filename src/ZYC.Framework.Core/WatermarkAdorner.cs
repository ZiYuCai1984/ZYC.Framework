using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ZYC.Framework.Core;

public class WatermarkAdorner : Adorner
{
    public WatermarkAdorner(TextBox adornedElement, string watermark)
        : base(adornedElement)
    {
        Watermark = watermark;
        IsHitTestVisible = false;
    }

    public string Watermark { get; set; }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        if (AdornedElement is not TextBox textBox)
        {
            return;
        }

        var dpi = VisualTreeHelper.GetDpi(this);

        var formattedText = new FormattedText(
            Watermark,
            CultureInfo.CurrentUICulture,
            textBox.FlowDirection,
            new Typeface(
                textBox.FontFamily,
                textBox.FontStyle,
                textBox.FontWeight,
                textBox.FontStretch),
            textBox.FontSize,
            Brushes.Gray,
            dpi.PixelsPerDip);

        var x = textBox.Padding.Left + 4;

        var y = textBox.Padding.Top +
                (textBox.ActualHeight - textBox.Padding.Top - textBox.Padding.Bottom - formattedText.Height) / 2;

        drawingContext.DrawText(formattedText, new Point(x, y));
    }
}