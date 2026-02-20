using System.Windows;
using System.Windows.Media;

namespace ZYC.Framework.Core;

public static class VisualTreeTools
{
    public static T? FindParent<T>(DependencyObject child) where T : class
    {
        var parent = VisualTreeHelper.GetParent(child);

        while (parent != null)
        {
            if (parent is T found)
            {
                return found;
            }

            parent = VisualTreeHelper.GetParent(parent);
        }

        return null;
    }
}