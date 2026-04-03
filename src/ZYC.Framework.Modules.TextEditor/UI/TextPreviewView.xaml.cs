using System.IO;
using System.Windows;
using System.Windows.Threading;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Modules.TextEditor.Abstractions;

namespace ZYC.Framework.Modules.TextEditor.UI;

[Register]
internal partial class TextPreviewView
{
    private readonly DispatcherTimer _reloadTimer;
    private FileSystemWatcher? _watcher;
    private DateTime _lastKnownWriteUtc;
    private readonly string _currentFilePath;

    public TextPreviewView(
        Uri documentUri,
        ITabManager tabManager,
        IToastManager toastManager)
    {
        DocumentUri = documentUri;
        TabManager = tabManager;
        ToastManager = toastManager;

        _currentFilePath = documentUri.LocalPath;
        FilePathText = _currentFilePath;
        SyncStatusText = "Loading...";

        _reloadTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(250)
        };
        _reloadTimer.Tick += OnReloadTimerTick;
    }

    private Uri DocumentUri { get; }

    private ITabManager TabManager { get; }

    private IToastManager ToastManager { get; }

    public string PageTitle { get; private set; } = TextEditorModuleConstants.PreviewTitle;

    public string FilePathText { get; private set; }

    public bool IsFileAvailable { get; private set; }

    public string SyncStatusText { get; private set; }

    protected override void InternalOnLoaded()
    {
        ConfigureWatcher(_currentFilePath);
        _ = LoadDocumentAsync(force: true);
    }

    public override void Dispose()
    {
        base.Dispose();

        _reloadTimer.Tick -= OnReloadTimerTick;
        _reloadTimer.Stop();
        DisposeWatcher();
    }

    private async Task LoadDocumentAsync(bool force)
    {
        try
        {
            if (!File.Exists(_currentFilePath))
            {
                SetMissingState();
                return;
            }

            var currentLastWriteUtc = File.GetLastWriteTimeUtc(_currentFilePath);
            if (!force && currentLastWriteUtc == _lastKnownWriteUtc && IsFileAvailable)
            {
                return;
            }

            var text = await TextDocumentTools.ReadAllTextAsync(_currentFilePath);

            _lastKnownWriteUtc = File.GetLastWriteTimeUtc(_currentFilePath);
            IsFileAvailable = true;

            TextDocumentTools.ApplySyntaxHighlighting(Editor, _currentFilePath);
            Editor.Text = text;

            UpdatePageTitle();
            SyncStatusText = $"Watching for changes. Last sync {DateTime.Now:HH:mm:ss}";

            OnPropertyChanged(nameof(IsFileAvailable));
            OnPropertyChanged(nameof(SyncStatusText));
        }
        catch (Exception ex)
        {
            SyncStatusText = "Failed to synchronize from disk.";
            OnPropertyChanged(nameof(SyncStatusText));
            ToastManager.PromptMessage(ToastMessage.Exception(ex));
        }
    }

    private void ConfigureWatcher(string filePath)
    {
        DisposeWatcher();

        var directory = Path.GetDirectoryName(filePath);
        var fileName = Path.GetFileName(filePath);
        if (string.IsNullOrWhiteSpace(directory)
            || string.IsNullOrWhiteSpace(fileName)
            || !Directory.Exists(directory))
        {
            return;
        }

        _watcher = new FileSystemWatcher(directory, fileName)
        {
            NotifyFilter =
                NotifyFilters.LastWrite |
                NotifyFilters.Size |
                NotifyFilters.CreationTime |
                NotifyFilters.FileName
        };

        _watcher.Changed += OnWatcherChanged;
        _watcher.Created += OnWatcherChanged;
        _watcher.Deleted += OnWatcherChanged;
        _watcher.Renamed += OnWatcherRenamed;
        _watcher.EnableRaisingEvents = true;
    }

    private void DisposeWatcher()
    {
        if (_watcher is null)
        {
            return;
        }

        _watcher.EnableRaisingEvents = false;
        _watcher.Changed -= OnWatcherChanged;
        _watcher.Created -= OnWatcherChanged;
        _watcher.Deleted -= OnWatcherChanged;
        _watcher.Renamed -= OnWatcherRenamed;
        _watcher.Dispose();
        _watcher = null;
    }

    private void SetMissingState()
    {
        IsFileAvailable = false;
        SyncStatusText = "File missing on disk.";
        UpdatePageTitle();

        OnPropertyChanged(nameof(IsFileAvailable));
        OnPropertyChanged(nameof(SyncStatusText));
    }

    private void UpdatePageTitle()
    {
        var displayName = TextDocumentTools.GetDisplayName(_currentFilePath);
        PageTitle = $"{TextEditorModuleConstants.PreviewTitle}: {displayName}";
        OnPropertyChanged(nameof(PageTitle));
    }

    private void RestartReloadTimer()
    {
        Dispatcher.InvokeAsync(() =>
        {
            _reloadTimer.Stop();
            _reloadTimer.Start();
        });
    }

    private void OnWatcherChanged(object sender, FileSystemEventArgs e)
    {
        RestartReloadTimer();
    }

    private void OnWatcherRenamed(object sender, RenamedEventArgs e)
    {
        RestartReloadTimer();
    }

    private async void OnReloadTimerTick(object? sender, EventArgs e)
    {
        _reloadTimer.Stop();
        await LoadDocumentAsync(force: false);
    }

    private async void OnReloadButtonClick(object sender, RoutedEventArgs e)
    {
        await LoadDocumentAsync(force: true);
    }

    private async void OnEditButtonClick(object sender, RoutedEventArgs e)
    {
        if (!IsFileAvailable)
        {
            return;
        }

        await TabManager.NavigateAsync(TextEditorModuleConstants.CreateEditorUri(DocumentUri));
    }
}
