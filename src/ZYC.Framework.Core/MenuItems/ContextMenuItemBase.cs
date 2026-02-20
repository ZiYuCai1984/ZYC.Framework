using System.Windows;
using System.Windows.Controls;

namespace ZYC.Framework.Core.MenuItems;

public abstract class ContextMenuItemBase : MenuItem
{
    protected ContextMenuItemBase()
    {
        SetResourceReference(StyleProperty, "MahApps.Styles.MenuItem");

        Loaded += OnMenuItemBaseLoaded;
    }

    protected virtual void OnMenuItemBaseLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnMenuItemBaseLoaded;
        InternalOnMenuItemBaseLoaded();
    }

    /// <summary>
    ///     !WARNING For suppress Binding error,need move InitializeComponent() from ctor to here !!
    /// </summary>
    protected abstract void InternalOnMenuItemBaseLoaded();
}