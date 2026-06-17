using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ZYC.Framework.Core;

public static class WatermarkBehavior
{
    public static readonly DependencyProperty WatermarkProperty =
        DependencyProperty.RegisterAttached(
            "Watermark",
            typeof(string),
            typeof(WatermarkBehavior),
            new PropertyMetadata(null, OnWatermarkChanged));

    private static readonly DependencyProperty WatermarkAdornerProperty =
        DependencyProperty.RegisterAttached(
            nameof(WatermarkAdorner),
            typeof(WatermarkAdorner),
            typeof(WatermarkBehavior),
            new PropertyMetadata(null));

    public static string GetWatermark(DependencyObject obj)
    {
        return (string)obj.GetValue(WatermarkProperty);
    }

    public static void SetWatermark(DependencyObject obj, string value)
    {
        obj.SetValue(WatermarkProperty, value);
    }

    private static WatermarkAdorner? GetWatermarkAdorner(DependencyObject obj)
    {
        return (WatermarkAdorner?)obj.GetValue(WatermarkAdornerProperty);
    }

    private static void SetWatermarkAdorner(DependencyObject obj, WatermarkAdorner? value)
    {
        obj.SetValue(WatermarkAdornerProperty, value);
    }

    private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox)
        {
            return;
        }

        textBox.Loaded -= TextBox_Loaded;
        textBox.TextChanged -= TextBox_TextChanged;
        textBox.GotKeyboardFocus -= TextBox_FocusChanged;
        textBox.LostKeyboardFocus -= TextBox_FocusChanged;

        textBox.Loaded += TextBox_Loaded;
        textBox.TextChanged += TextBox_TextChanged;
        textBox.GotKeyboardFocus += TextBox_FocusChanged;
        textBox.LostKeyboardFocus += TextBox_FocusChanged;

        UpdateWatermark(textBox);
    }

    private static void TextBox_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateWatermark((TextBox)sender);
    }

    private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateWatermark((TextBox)sender);
    }

    private static void TextBox_FocusChanged(object sender, RoutedEventArgs e)
    {
        UpdateWatermark((TextBox)sender);
    }

    private static void UpdateWatermark(TextBox textBox)
    {
        var layer = AdornerLayer.GetAdornerLayer(textBox);
        if (layer == null)
        {
            return;
        }

        var adorner = GetWatermarkAdorner(textBox);
        var watermark = GetWatermark(textBox);

        var shouldShow =
            string.IsNullOrEmpty(textBox.Text)
            && !string.IsNullOrEmpty(watermark);

        if (shouldShow)
        {
            if (adorner == null)
            {
                adorner = new WatermarkAdorner(textBox, watermark);
                SetWatermarkAdorner(textBox, adorner);
                layer.Add(adorner);
            }
            else
            {
                adorner.Watermark = watermark;
                adorner.InvalidateVisual();
            }
        }
        else
        {
            if (adorner != null)
            {
                layer.Remove(adorner);
                SetWatermarkAdorner(textBox, null);
            }
        }
    }
}