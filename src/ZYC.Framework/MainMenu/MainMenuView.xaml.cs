using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.MainMenu;

namespace ZYC.Framework.MainMenu;

[RegisterSingleInstance]
internal partial class MainMenuView : INotifyPropertyChanged
{
    public MainMenuView(
        IHamburgerMenuManager hamburgerMenuManager,
        IMainMenuManager mainMenuManager,
        MainMenuConfig mainMenuConfig,
        HamburgerMenuConfig hamburgerMenuConfig)
    {
        HamburgerMenuManager = hamburgerMenuManager;
        MainMenuManager = mainMenuManager;

        MainMenuConfig = mainMenuConfig;
        HamburgerMenuConfig = hamburgerMenuConfig;

        InitializeComponent();

        OverflowMenuItem = new MenuItem
        {
            Style = (Style)FindResource("OverflowMenuItemStyle"),
            ItemContainerStyle = (Style)FindResource("MainMenuItemStyle"),
            ItemsSource = OverflowMainMenuItems,
            Visibility = Visibility.Collapsed
        };

        MainMenuDisplayItems.Add(new CollectionContainer { Collection = VisibleMainMenuItems });
        MainMenuDisplayItems.Add(OverflowMenuItem);
    }

    private IHamburgerMenuManager HamburgerMenuManager { get; }
    private IMainMenuManager MainMenuManager { get; }

    public MainMenuConfig MainMenuConfig { get; }

    public HamburgerMenuConfig HamburgerMenuConfig { get; }

    public IMainMenuItem?[] MainMenuItems { get; set; } = [];

    public IMainMenuItem?[] HamburgerMenuItems { get; set; } = [];

    public ObservableCollection<IMainMenuItem?> VisibleMainMenuItems { get; } = [];

    public ObservableCollection<IMainMenuItem?> OverflowMainMenuItems { get; } = [];

    public CompositeCollection MainMenuDisplayItems { get; } = new();

    private MenuItem OverflowMenuItem { get; }

    private bool UpdatingMenuItems { get; set; }

    private bool FirstRending { get; set; } = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnMainMenuViewLoaded(object sender, RoutedEventArgs e)
    {
        if (!FirstRending)
        {
            return;
        }

        FirstRending = false;

        MainMenuItems = MainMenuManager.GetSortedItems();
        OnPropertyChanged(nameof(MainMenuItems));

        HamburgerMenuItems = HamburgerMenuManager.GetSortedItems();
        OnPropertyChanged(nameof(HamburgerMenuItems));
    }

    private void OnMainMenuLayoutUpdated(object? sender, EventArgs e)
    {
        if (!IsLoaded || FirstRending || UpdatingMenuItems || !MainMenuHost.IsVisible
            || !MainMenuHost.IsArrangeValid || !MeasurementMenu.IsMeasureValid
            || !OverflowMeasurementMenu.IsMeasureValid
            || (MainMenuItems.Length > 0
                && MeasurementMenu.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated))
        {
            return;
        }

        var measuredItems = new List<(IMainMenuItem? Item, double Width)>();
        FrameworkElement? separator = null;

        for (var i = 0; i < MainMenuItems.Length; i++)
        {
            var item = MainMenuItems[i];
            // Collapsed containers can remain measure-invalid until they become visible.
            if (item?.IsHidden == true)
            {
                continue;
            }

            if (MeasurementMenu.ItemContainerGenerator.ContainerFromIndex(i) is not FrameworkElement container
                || !container.IsMeasureValid)
            {
                return;
            }

            if (item is null)
            {
                if (measuredItems.Count > 0)
                {
                    separator ??= container;
                }

                continue;
            }

            if (separator is not null)
            {
                measuredItems.Add((null, separator.DesiredSize.Width));
                separator = null;
            }

            measuredItems.Add((item, container.DesiredSize.Width));
        }

        var availableWidth = MainMenuHost.ActualWidth;
        var visibleCount = measuredItems.Count;

        if (measuredItems.Sum(item => item.Width) > availableWidth)
        {
            availableWidth = Math.Max(0, availableWidth - OverflowMeasurementMenu.DesiredSize.Width);
            var usedWidth = 0.0;
            visibleCount = 0;

            foreach (var item in measuredItems)
            {
                if (usedWidth + item.Width > availableWidth)
                {
                    break;
                }

                usedWidth += item.Width;
                visibleCount++;
            }
        }

        // A separator at the split belongs to neither of the two menus.
        while (visibleCount > 0 && measuredItems[visibleCount - 1].Item is null)
        {
            visibleCount--;
        }

        var overflowStart = visibleCount;
        while (overflowStart < measuredItems.Count && measuredItems[overflowStart].Item is null)
        {
            overflowStart++;
        }

        var visibleItems = measuredItems.Take(visibleCount).Select(item => item.Item).ToArray();
        var overflowItems = measuredItems.Skip(overflowStart).Select(item => item.Item).ToArray();
        var visiblePrefix = GetCommonPrefixLength(VisibleMainMenuItems, visibleItems);
        var overflowPrefix = GetCommonPrefixLength(OverflowMainMenuItems, overflowItems);

        if (visiblePrefix == VisibleMainMenuItems.Count && visiblePrefix == visibleItems.Length
            && overflowPrefix == OverflowMainMenuItems.Count && overflowPrefix == overflowItems.Length)
        {
            return;
        }

        UpdatingMenuItems = true;
        try
        {
            var restoreFocus = OverflowMenuItem.IsKeyboardFocusWithin;
            OverflowMenuItem.IsSubmenuOpen = false;

            for (var i = visiblePrefix; i < VisibleMainMenuItems.Count; i++)
            {
                if (PrimaryMenu.ItemContainerGenerator.ContainerFromIndex(i) is MenuItem menuItem)
                {
                    restoreFocus |= menuItem.IsKeyboardFocusWithin;
                    menuItem.IsSubmenuOpen = false;
                }
            }

            UpdateItems(VisibleMainMenuItems, visibleItems, visiblePrefix);
            UpdateItems(OverflowMainMenuItems, overflowItems, overflowPrefix);
            OverflowMenuItem.Visibility = overflowItems.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

            if (restoreFocus)
            {
                if (overflowItems.Length > 0)
                {
                    OverflowMenuItem.Focus();
                }
                else
                {
                    PrimaryMenu.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                }
            }
        }
        finally
        {
            UpdatingMenuItems = false;
        }
    }

    private static int GetCommonPrefixLength(IList<IMainMenuItem?> currentItems, IMainMenuItem?[] newItems)
    {
        var count = Math.Min(currentItems.Count, newItems.Length);
        var index = 0;
        while (index < count && ReferenceEquals(currentItems[index], newItems[index]))
        {
            index++;
        }

        return index;
    }

    private static void UpdateItems(ObservableCollection<IMainMenuItem?> target, IMainMenuItem?[] items, int prefixLength)
    {
        // Keep unchanged containers alive so resizing does not reset their open submenus.
        while (target.Count > prefixLength)
        {
            target.RemoveAt(target.Count - 1);
        }

        for (var i = prefixLength; i < items.Length; i++)
        {
            target.Add(items[i]);
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
