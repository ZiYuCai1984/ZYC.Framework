using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Abstractions.Workspace;

namespace ZYC.Framework.Workspace.CustomLayouts;

[Register]
internal sealed partial class SaveCustomLayoutDialog : INotifyPropertyChanged
{
    private string _layoutName = "";

    public SaveCustomLayoutDialog(
        ILogger<SaveCustomLayoutDialog> logger,
        CustomWorkspaceLayoutConfig customWorkspaceLayoutConfig,
        RootWorkspaceNodeState rootWorkspaceNodeState,
        IToastManager toastManager)
    {
        Logger = logger;
        CustomWorkspaceLayoutConfig = customWorkspaceLayoutConfig;
        RootWorkspaceNodeState = rootWorkspaceNodeState;
        ToastManager = toastManager;
        LayoutName = CreateDefaultLayoutName();
        DataContext = this;

        InitializeComponent();
    }

    public string LayoutName
    {
        get => _layoutName;
        set
        {
            if (_layoutName == value)
            {
                return;
            }

            _layoutName = value;
            OnPropertyChanged();
        }
    }

    private ILogger<SaveCustomLayoutDialog> Logger { get; }

    private CustomWorkspaceLayoutConfig CustomWorkspaceLayoutConfig { get; }

    private RootWorkspaceNodeState RootWorkspaceNodeState { get; }

    private IToastManager ToastManager { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnDialogLoaded(object sender, RoutedEventArgs e)
    {
        LayoutNameTextBox.Focus();
        LayoutNameTextBox.SelectAll();
    }

    private void OnSaveButtonClick(object sender, RoutedEventArgs e)
    {
        var layoutName = LayoutName.Trim();
        if (string.IsNullOrWhiteSpace(layoutName))
        {
            ToastManager.PromptMessage(ToastMessage.Warn("Layout name is required."));
            return;
        }

        try
        {
            var layouts = CustomWorkspaceLayoutConfig.Layouts.ToList();

            var node = JsonTools.DeepCopy(RootWorkspaceNodeState);
            node.NavigationState = new NavigationState();

            layouts.Add(new CustomWorkspaceLayout
            {
                Id = Guid.NewGuid(),
                Name = layoutName,
                WorkspaceNode = node,
                Thumbnail = CustomWorkspaceLayoutThumbnailBuilder.Build(node)
            });

            CustomWorkspaceLayoutConfig.Layouts = layouts.ToArray();

            ToastManager.PromptMessage(
                ToastMessage.Info($"Saved to {layoutName} success."));
            Close();
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private string CreateDefaultLayoutName()
    {
        var existingNames = CustomWorkspaceLayoutConfig.Layouts
            .Select(layout => layout.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var index = CustomWorkspaceLayoutConfig.Layouts.Length + 1;
        var name = $"Layout {index}";
        while (existingNames.Contains(name))
        {
            index++;
            name = $"Layout {index}";
        }

        return name;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}