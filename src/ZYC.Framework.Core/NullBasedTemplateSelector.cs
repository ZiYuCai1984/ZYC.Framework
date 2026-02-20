using System.Windows;
using System.Windows.Controls;

namespace ZYC.Framework.Core;

public class NullBasedTemplateSelector : DataTemplateSelector
{
    public DataTemplate? NonNullTemplate { get; set; }
    public DataTemplate? NullTemplate { get; set; }

    public override DataTemplate SelectTemplate(object? item, DependencyObject container)
    {
        return item == null ? NullTemplate! : NonNullTemplate!;
    }
}