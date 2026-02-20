using System.Windows;
using System.Windows.Controls;

namespace ZYC.Framework.Core;

public static class ItemsControlTools
{
    public static FrameworkElement? BringIntoView(this ItemsControl itemsControl, object target)
    {
        return itemsControl.BringIntoView((_, item) => item == target);
    }

    public static FrameworkElement? BringIntoView(
        this ItemsControl itemsControl,
        Func<FrameworkElement, object, bool> func)
    {
        var items = itemsControl.Items;

        foreach (var item in items)
        {
            var element = (FrameworkElement)itemsControl.ItemContainerGenerator.ContainerFromItem(item);
            if (!func.Invoke(element, item))
            {
                continue;
            }

            element.BringIntoView();
            return element;
        }

        return null;
    }
}