using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Abstractions.Workspace;

namespace ZYC.Framework.Workspace.BuildIn;

[RegisterAs(typeof(ITabItemHeaderContextMenuItemView))]
internal partial class MoveWorkspaceTabItemHeaderContextMenuItem : ITabItemHeaderContextMenuItemView,
    INotifyPropertyChanged
{
    public MoveWorkspaceTabItemHeaderContextMenuItem(
        IAppContext appContext,
        IParallelWorkspaceManager parallelWorkspaceManager,
        IMoveWorkspaceTabItemHeaderContextMenuItemManager moveWorkspaceTabItemHeaderContextMenuItemManager)
    {
        DataContext = this;

        AppContext = appContext;
        ParallelWorkspaceManager = parallelWorkspaceManager;
        MoveWorkspaceTabItemHeaderContextMenuItemManager = moveWorkspaceTabItemHeaderContextMenuItemManager;
    }

    private IAppContext AppContext { get; }

    private IParallelWorkspaceManager ParallelWorkspaceManager { get; }


    private IMoveWorkspaceTabItemHeaderContextMenuItemManager MoveWorkspaceTabItemHeaderContextMenuItemManager { get; }

    private bool FirstRendering { get; set; } = true;

    public MoveWorkspaceTabItemHeaderContextMenuSubItem[] SubItems { get; private set; } = [];

    private ITabItemInstance? TabItemInstance
    {
        get
        {
            if (Tag == null)
            {
                return null;
            }

            return (ITabItemInstance)Tag;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public int Order => 5;

    protected override void InternalOnMenuItemBaseLoaded()
    {
        InitializeComponent();
    }

    protected override void OnMenuItemBaseLoaded(object sender, RoutedEventArgs e)
    {
        if (FirstRendering)
        {
            InternalOnMenuItemBaseLoaded();
            FirstRendering = false;
        }

        AppContext.InvokeOnUIThreadAsync(async () =>
        {
            //!WARNING Design defeat
            await Task.Delay(100);
            while (TabItemInstance == null)
            {
                await Task.Delay(100);
            }

            SubItems = MoveWorkspaceTabItemHeaderContextMenuItemManager.GetSubItems(TabItemInstance);
            OnPropertyChanged(nameof(SubItems));
        });
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnMenuItemMouseEnter(object sender, MouseEventArgs e)
    {
        var menuItem = (MenuItem)sender;

        var node = (MoveWorkspaceTabItemHeaderContextMenuSubItem)menuItem.DataContext;
        ParallelWorkspaceManager.SetHighlight(node.Workspace, true);
    }

    private void OnMenuItemMouseLeave(object sender, MouseEventArgs e)
    {
        var menuItem = (MenuItem)sender;

        var node = (MoveWorkspaceTabItemHeaderContextMenuSubItem)menuItem.DataContext;
        ParallelWorkspaceManager.SetHighlight(node.Workspace, false);
    }
}