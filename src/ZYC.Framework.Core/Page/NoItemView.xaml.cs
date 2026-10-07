using System.Windows;
using ZYC.Framework.Core.DragDrop;

namespace ZYC.Framework.Core.Page;

public partial class NoItemView
{
    public NoItemView()
    {
        InitializeComponent();
    }
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(DragDropPickerView),
            new PropertyMetadata("No item"));


}