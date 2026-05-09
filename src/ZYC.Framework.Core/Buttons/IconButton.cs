using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.IconPacks;

namespace ZYC.Framework.Core.Buttons;

public class IconButton : Button
{
    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(
            nameof(Kind),
            typeof(PackIconMaterialKind),
            typeof(IconButton),
            new PropertyMetadata(PackIconMaterialKind.None));

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(IconButton),
            new PropertyMetadata(string.Empty));

    public IconButton()
    {
        Padding = new Thickness(8);
        FlowDirection = FlowDirection.LeftToRight;

        var dockPanel = new DockPanel();

        var icon = new PackIconMaterial
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Width = 16,
            Height = 16,
            Margin = new Thickness(0, 0, 8, 0)
        };

        DockPanel.SetDock(icon, Dock.Left);

        icon.SetBinding(
            PackIconMaterial.KindProperty,
            new Binding(nameof(Kind)) { Source = this });

        dockPanel.Children.Add(icon);

        var textBlock = new TextBlock
        {
            VerticalAlignment = VerticalAlignment.Center
        };

        textBlock.SetBinding(
            TextBlock.TextProperty,
            new Binding(nameof(Text)) { Source = this });

        dockPanel.Children.Add(textBlock);

        Content = dockPanel;
    }

    public PackIconMaterialKind Kind
    {
        get => (PackIconMaterialKind)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public string? Text
    {
        get => (string?)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}