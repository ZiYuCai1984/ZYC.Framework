using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.Workspace;

namespace ZYC.Framework.Workspace.CustomLayouts;

[Register]
internal sealed partial class ManageCustomLayoutsDialog : INotifyPropertyChanged
{
    private string _editName = "";
    private bool _hasLayouts;
    private CustomWorkspaceLayoutEditItem? _selectedLayout;

    public ManageCustomLayoutsDialog(
        ILogger<ManageCustomLayoutsDialog> logger,
        CustomWorkspaceLayoutConfig customWorkspaceLayoutConfig,
        IToastManager toastManager)
    {
        Logger = logger;
        CustomWorkspaceLayoutConfig = customWorkspaceLayoutConfig;
        ToastManager = toastManager;
        DataContext = this;

        InitializeComponent();
        LoadLayouts();
        SelectedLayout = LayoutItems.FirstOrDefault();
        RefreshButtonState();
    }

    public ObservableCollection<CustomWorkspaceLayoutEditItem> LayoutItems { get; } = new();

    public CustomWorkspaceLayoutEditItem? SelectedLayout
    {
        get => _selectedLayout;
        set
        {
            if (_selectedLayout == value)
            {
                return;
            }

            _selectedLayout = value;
            EditName = value?.Name ?? "";
            OnPropertyChanged();
            RefreshButtonState();
        }
    }

    public string EditName
    {
        get => _editName;
        set
        {
            if (_editName == value)
            {
                return;
            }

            _editName = value;
            OnPropertyChanged();
        }
    }

    public bool HasLayouts
    {
        get => _hasLayouts;
        private set
        {
            if (_hasLayouts == value)
            {
                return;
            }

            _hasLayouts = value;
            OnPropertyChanged();
        }
    }

    private ILogger<ManageCustomLayoutsDialog> Logger { get; }

    private CustomWorkspaceLayoutConfig CustomWorkspaceLayoutConfig { get; }

    private IToastManager ToastManager { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnRenameButtonClick(object sender, RoutedEventArgs e)
    {
        if (SelectedLayout == null)
        {
            return;
        }

        var newName = EditName.Trim();
        if (string.IsNullOrWhiteSpace(newName))
        {
            ToastManager.PromptMessage(ToastMessage.Warn("Layout name is required."));
            return;
        }

        try
        {
            var oriName = SelectedLayout.Name;

            SelectedLayout.Name = newName;
            EditName = newName;
            CommitLayouts();
            ToastManager.PromptMessage(ToastMessage.Info($"Custom layout renamed. [{oriName}] -> [{newName}]"));
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }

    private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
    {
        if (SelectedLayout == null)
        {
            return;
        }

        try
        {
            var index = LayoutItems.IndexOf(SelectedLayout);
            LayoutItems.Remove(SelectedLayout);
            CommitLayouts();

            if (LayoutItems.Count > 0)
            {
                SelectedLayout = LayoutItems[Math.Min(index, LayoutItems.Count - 1)];
            }
            else
            {
                SelectedLayout = null;
            }

            ToastManager.PromptMessage(ToastMessage.Info("Custom layout removed."));
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }

    private void OnMoveUpButtonClick(object sender, RoutedEventArgs e)
    {
        MoveSelectedLayout(-1);
    }

    private void OnMoveDownButtonClick(object sender, RoutedEventArgs e)
    {
        MoveSelectedLayout(1);
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void LoadLayouts()
    {
        LayoutItems.Clear();

        foreach (var layout in CustomWorkspaceLayoutConfig.Layouts)
        {
            LayoutItems.Add(new CustomWorkspaceLayoutEditItem(layout));
        }

        HasLayouts = LayoutItems.Count > 0;
    }

    private void MoveSelectedLayout(int offset)
    {
        if (SelectedLayout == null)
        {
            return;
        }

        var oldIndex = LayoutItems.IndexOf(SelectedLayout);
        var newIndex = oldIndex + offset;
        if (newIndex < 0 || newIndex >= LayoutItems.Count)
        {
            return;
        }

        try
        {
            LayoutItems.Move(oldIndex, newIndex);
            CommitLayouts();
            LayoutsListBox.SelectedItem = SelectedLayout;
            RefreshButtonState();
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }

    private void CommitLayouts()
    {
        CustomWorkspaceLayoutConfig.Layouts = LayoutItems
            .Select(item => item.Layout)
            .ToArray();
        HasLayouts = LayoutItems.Count > 0;
        RefreshButtonState();
    }

    private void RefreshButtonState()
    {
        if (!IsInitialized)
        {
            return;
        }

        var hasSelection = SelectedLayout != null;
        var selectedIndex = hasSelection ? LayoutItems.IndexOf(SelectedLayout!) : -1;

        EditNameTextBox.IsEnabled = hasSelection;
        RenameButton.IsEnabled = hasSelection;
        DeleteButton.IsEnabled = hasSelection;
        MoveUpButton.IsEnabled = selectedIndex > 0;
        MoveDownButton.IsEnabled = selectedIndex >= 0 && selectedIndex < LayoutItems.Count - 1;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

internal sealed class CustomWorkspaceLayoutEditItem : INotifyPropertyChanged
{
    public CustomWorkspaceLayoutEditItem(CustomWorkspaceLayout layout)
    {
        Layout = layout;
    }

    public CustomWorkspaceLayout Layout { get; }

    public string Name
    {
        get => Layout.Name;
        set
        {
            if (Layout.Name == value)
            {
                return;
            }

            Layout.Name = value;
            OnPropertyChanged();
        }
    }

    public string? Thumbnail => Layout.Thumbnail;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
