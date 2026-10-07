using System.Windows;

namespace ZYC.Framework.Core.Page;

public partial class NoItemView
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(NoItemView),
            new PropertyMetadata("No item"));

    public NoItemView()
    {
        InitializeComponent();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}