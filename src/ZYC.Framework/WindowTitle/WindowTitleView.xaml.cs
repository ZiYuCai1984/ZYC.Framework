using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Windows;
using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.QuickBar;
using ZYC.Framework.Abstractions.WindowTitle;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.WindowTitle;

[RegisterSingleInstance]
internal partial class WindowTitleView : IDisposable
{
    private DateTime _lastClickTime;

    public WindowTitleView(
        WindowTitleConfig windowTitleConfig,
        IEventAggregator eventAggregator,
        IQuickBarManager quickBarManager,
        IWindowTitleExtendManager windowTitleExtendManager,
        IWindowTitleManager windowTitleManager,
        IMainWindow mainWindow,
        FullScreenCommand fullScreenCommand)
    {
        WindowTitleConfig = windowTitleConfig;
        EventAggregator = eventAggregator;
        QuickBarManager = quickBarManager;
        WindowTitleExtendManager = windowTitleExtendManager;
        WindowTitleManager = windowTitleManager;
        MainWindow = mainWindow;
        FullScreenCommand = fullScreenCommand;

        quickBarManager.RegisterQuickMenuItemsProvider<ISimpleQuickBarItemsProvider>();
        quickBarManager.RegisterQuickMenuItemsProvider<IStarQuickBarItemsProvider>();

        EventAggregator.Subscribe<QuickMenuItemsChangedEvent>(OnQuickMenuItemsChanged)
            .DisposeWith(CompositeDisposable);


        OnQuickMenuItemsChanged(null!);

        InitializeComponent();
    }

    private CompositeDisposable CompositeDisposable { get; } = new();

    public WindowTitleConfig WindowTitleConfig { get; }

    private IEventAggregator EventAggregator { get; }

    private IQuickBarManager QuickBarManager { get; }
    private IWindowTitleExtendManager WindowTitleExtendManager { get; }

    private IWindowTitleManager WindowTitleManager { get; }

    private IMainWindow MainWindow { get; }

    private FullScreenCommand FullScreenCommand { get; }

    public ObservableCollection<IQuickBarItem?> QuickMenuTitleItems { get; } = [];

    public ObservableCollection<IWindowTitleItem> WindowTitleItems { get; } = [];

    public ObservableCollection<IWindowTitleExtendItem> WindowTitleExtendItems { get; } = [];

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    private void OnQuickMenuItemsChanged(QuickMenuItemsChangedEvent e)
    {
        QuickMenuTitleItems.Clear();
        var quickMenuItems = QuickBarManager.GetQuickMenuTitleItems();
        foreach (var item in quickMenuItems)
        {
            QuickMenuTitleItems.Add(item);
        }
    }

    private void OnWindowTitleViewLoaded(object sender, RoutedEventArgs e)
    {
        {
            WindowTitleItems.Clear();
            var items = WindowTitleManager.GetItems();

            foreach (var item in items)
            {
                WindowTitleItems.Add(item);
            }
        }

        {
            WindowTitleExtendItems.Clear();
            var items = WindowTitleExtendManager.GetItems();

            foreach (var item in items)
            {
                WindowTitleExtendItems.Add(item);
            }
        }
    }

    private void OnWindowTitleViewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Handled || e.ChangedButton != MouseButton.Left)
        {
            return;
        }

        var now = DateTime.Now;
        if ((now - _lastClickTime).TotalMilliseconds < 200)
        {
            FullScreenCommand.Execute(null);
            return;
        }

        _lastClickTime = now;

        MainWindow.DragMove();
        e.Handled = true;
    }
}