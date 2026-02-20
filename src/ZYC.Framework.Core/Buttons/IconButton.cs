using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.IconPacks;

namespace ZYC.Framework.Core.Buttons;

public class IconButton : UserControl
{
    public static readonly DependencyProperty KindProperty
        = DependencyProperty.Register(nameof(Kind),
            typeof(PackIconMaterialKind), typeof(IconButton),
            new PropertyMetadata(PackIconMaterialKind.None));

    public static readonly DependencyProperty TextProperty
        = DependencyProperty.Register(nameof(Text), typeof(string), typeof(IconButton),
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty CommandProperty
        = DependencyProperty.Register(nameof(Command),
            typeof(ICommand), typeof(IconButton),
            new PropertyMetadata(null));

    public static readonly DependencyProperty CommandParameterProperty
        = DependencyProperty.Register(nameof(CommandParameter),
            typeof(object), typeof(IconButton),
            new PropertyMetadata(null));

    public IconButton()
    {
        Button = new Button
        {
            Padding = new Thickness(8),
            FlowDirection = FlowDirection.LeftToRight
        };

        var dockPanel = new DockPanel();

        PackIconMaterial = new PackIconMaterial
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Width = 16,
            Height = 16,
            Margin = new Thickness(0, 0, 8, 0)
        };
        DockPanel.SetDock(PackIconMaterial, Dock.Left);

        PackIconMaterial.SetBinding(PackIconMaterial.KindProperty, new Binding(nameof(Kind)) { Source = this });

        dockPanel.Children.Add(PackIconMaterial);

        TextBlock = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
        TextBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Text)) { Source = this });
        dockPanel.Children.Add(TextBlock);

        Button.Content = dockPanel;
        Content = Button;

        Button.SetBinding(ButtonBase.CommandProperty, new Binding(nameof(Command)) { Source = this });
        Button.SetBinding(ButtonBase.CommandParameterProperty, new Binding(nameof(CommandParameter)) { Source = this });
    }


    private Button Button { get; }


    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public ICommand? Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }


    private PackIconMaterial PackIconMaterial { get; }

    private TextBlock TextBlock { get; }

    public PackIconMaterialKind Kind
    {
        get => (PackIconMaterialKind)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public string? Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}