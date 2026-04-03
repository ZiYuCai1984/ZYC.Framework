using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.TextEditor.Abstractions;

namespace ZYC.Framework.Modules.TextEditor.UI;

[Register]
internal partial class TextEditorView
{
    private readonly DispatcherTimer _reloadTimer;
    private FileSystemWatcher? _watcher;
    private bool _hasExternalChange;
    private bool _isLoading;
    private DateTime _lastKnownWriteUtc;
    private string _currentFilePath;
    private string _savedText = string.Empty;

    public TextEditorView(
        TextEditorTabItem instance,
        IToastManager toastManager)
    {
        Instance = instance;
        ToastManager = toastManager;

        _currentFilePath = instance.DocumentUri.LocalPath;
        FilePathText = _currentFilePath;
        StatusText = "Loading...";

        _reloadTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(250)
        };
        _reloadTimer.Tick += OnReloadTimerTick;
    }

    private TextEditorTabItem Instance { get; }

    private IToastManager ToastManager { get; }

    public string PageTitle { get; private set; } = TextEditorModuleConstants.EditorTitle;

    public string FilePathText { get; private set; }

    public bool IsReadOnlyDocument { get; private set; }

    public string StatusText { get; private set; }

    protected override void InternalOnLoaded()
    {
        ConfigureWatcher(_currentFilePath);
        _ = LoadDocumentAsync(force: true, triggeredByExternalChange: false);
    }

    public override void Dispose()
    {
        base.Dispose();

        _reloadTimer.Tick -= OnReloadTimerTick;
        _reloadTimer.Stop();
        DisposeWatcher();
    }

    private async Task HandleExternalFileChangeAsync()
    {
        if (!File.Exists(_currentFilePath))
        {
            _hasExternalChange = true;
            IsReadOnlyDocument = false;
            StatusText = "File was removed on disk. Save to recreate it or use Save As.";
            OnPropertyChanged(nameof(IsReadOnlyDocument));
            OnPropertyChanged(nameof(StatusText));
            return;
        }

        var currentLastWriteUtc = File.GetLastWriteTimeUtc(_currentFilePath);
        if (currentLastWriteUtc == _lastKnownWriteUtc && !_hasExternalChange)
        {
            return;
        }

        if (Instance.IsDirty)
        {
            if (!_hasExternalChange)
            {
                ToastManager.PromptMessage(
                    ToastMessage.Warn(
                        $"File changed on disk: {TextDocumentTools.GetDisplayName(_currentFilePath)}",
                        false));
            }

            _hasExternalChange = true;
            StatusText = "File changed on disk. Reload to sync or keep editing.";
            OnPropertyChanged(nameof(StatusText));
            return;
        }

        await LoadDocumentAsync(force: true, triggeredByExternalChange: true);
    }

    private async Task LoadDocumentAsync(bool force, bool triggeredByExternalChange)
    {
        try
        {
            if (!File.Exists(_currentFilePath))
            {
                _hasExternalChange = true;
                IsReadOnlyDocument = false;
                StatusText = "File is missing on disk. Save to recreate it or use Save As.";
                UpdatePageTitle();
                OnPropertyChanged(nameof(IsReadOnlyDocument));
                OnPropertyChanged(nameof(StatusText));
                return;
            }

            var currentLastWriteUtc = File.GetLastWriteTimeUtc(_currentFilePath);
            if (!force && currentLastWriteUtc == _lastKnownWriteUtc && !_hasExternalChange)
            {
                return;
            }

            _isLoading = true;

            var text = await TextDocumentTools.ReadAllTextAsync(_currentFilePath);

            _savedText = text;
            _lastKnownWriteUtc = File.GetLastWriteTimeUtc(_currentFilePath);
            _hasExternalChange = false;

            FilePathText = _currentFilePath;
            IsReadOnlyDocument = new FileInfo(_currentFilePath).IsReadOnly;

            TextDocumentTools.ApplySyntaxHighlighting(Editor, _currentFilePath);
            Editor.Text = text;

            Instance.SetDirty(false);
            UpdatePageTitle();

            StatusText = triggeredByExternalChange
                ? $"Reloaded from disk at {DateTime.Now:HH:mm:ss}"
                : $"Editing {TextDocumentTools.GetDisplayName(_currentFilePath)}";

            OnPropertyChanged(nameof(FilePathText));
            OnPropertyChanged(nameof(IsReadOnlyDocument));
            OnPropertyChanged(nameof(StatusText));
        }
        catch (Exception ex)
        {
            StatusText = "Failed to read file.";
            OnPropertyChanged(nameof(StatusText));
            ToastManager.PromptMessage(ToastMessage.Exception(ex));
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task ReloadFromDiskAsync()
    {
        if (Instance.IsDirty && !MessageBoxTools.Confirm(
                $"Discard unsaved changes to '{TextDocumentTools.GetDisplayName(_currentFilePath)}' and reload from disk?",
                "Reload",
                false))
        {
            return;
        }

        await LoadDocumentAsync(force: true, triggeredByExternalChange: true);
    }

    private async Task SaveDocumentAsync(bool saveAs)
    {
        try
        {
            var targetPath = _currentFilePath;
            if (saveAs || IsReadOnlyDocument)
            {
                var selectedPath = DialogTools.SaveFileDialog(
                    Path.GetFileName(targetPath),
                    Path.GetDirectoryName(targetPath) ?? string.Empty,
                    TextEditorModuleConstants.FileDialogFilter);

                if (string.IsNullOrWhiteSpace(selectedPath))
                {
                    return;
                }

                targetPath = selectedPath;
            }

            await File.WriteAllTextAsync(targetPath, Editor.Text);

            var isPathChanged = !string.Equals(_currentFilePath, targetPath, StringComparison.OrdinalIgnoreCase);

            _currentFilePath = targetPath;
            _savedText = Editor.Text;
            _lastKnownWriteUtc = File.GetLastWriteTimeUtc(targetPath);
            _hasExternalChange = false;

            FilePathText = targetPath;
            IsReadOnlyDocument = new FileInfo(targetPath).IsReadOnly;

            TextDocumentTools.ApplySyntaxHighlighting(Editor, targetPath);
            ConfigureWatcher(targetPath);
            UpdatePageTitle();

            StatusText = $"Saved at {DateTime.Now:HH:mm:ss}";

            Instance.SetDirty(false);

            OnPropertyChanged(nameof(FilePathText));
            OnPropertyChanged(nameof(IsReadOnlyDocument));
            OnPropertyChanged(nameof(StatusText));

            if (isPathChanged)
            {
                await Instance.UpdateDocumentUriAsync(new Uri(targetPath));
            }
        }
        catch (Exception ex)
        {
            StatusText = "Failed to save file.";
            OnPropertyChanged(nameof(StatusText));
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

    private void UpdatePageTitle()
    {
        PageTitle = $"{TextEditorModuleConstants.EditorTitle}: {TextDocumentTools.GetDisplayName(_currentFilePath)}";
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
        await HandleExternalFileChangeAsync();
    }

    private void OnEditorTextChanged(object sender, EventArgs e)
    {
        if (_isLoading)
        {
            return;
        }

        Instance.SetDirty(Editor.Text != _savedText);
    }

    private async void OnReloadButtonClick(object sender, RoutedEventArgs e)
    {
        await ReloadFromDiskAsync();
    }

    private async void OnReloadMenuItemClick(object sender, RoutedEventArgs e)
    {
        await ReloadFromDiskAsync();
    }

    private async void OnSaveButtonClick(object sender, RoutedEventArgs e)
    {
        await SaveDocumentAsync(false);
    }

    private async void OnSaveAsButtonClick(object sender, RoutedEventArgs e)
    {
        await SaveDocumentAsync(true);
    }

    private async void OnSaveMenuItemClick(object sender, RoutedEventArgs e)
    {
        await SaveDocumentAsync(false);
    }

    private async void OnSaveAsMenuItemClick(object sender, RoutedEventArgs e)
    {
        await SaveDocumentAsync(true);
    }

    private async void OnEditorPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.S || (Keyboard.Modifiers & ModifierKeys.Control) == 0)
        {
            return;
        }

        e.Handled = true;
        await SaveDocumentAsync((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
    }
}
