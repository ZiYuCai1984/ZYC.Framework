using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.HexEditor.Abstractions;
using HexEditorControl = WpfHexEditor.HexEditor.HexEditor;

namespace ZYC.Framework.Modules.HexEditor.UI;

// ReSharper disable AsyncVoidEventHandlerMethod

[Register]
internal sealed partial class HexEditorView
{
    private readonly CancellationTokenSource _lifetimeCancellation = new();
    private readonly DispatcherTimer _reloadTimer;
    private readonly DependencyPropertyDescriptor _modifiedDescriptor;
    private FileSystemWatcher? _watcher;
    private string _currentFilePath;
    private DateTime _lastKnownWriteUtc;
    private long _lastKnownLength;
    private bool _hasExternalChange;
    private bool _documentLoaded;
    private bool _isDisposed;

    public HexEditorView(
        ILogger<HexEditorView> logger,
        HexEditorTabItem instance,
        IToastManager toastManager)
    {
        Logger = logger;
        Instance = instance;
        ToastManager = toastManager;
        _currentFilePath = instance.DocumentUri.LocalPath;

        _reloadTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        _reloadTimer.Tick += OnReloadTimerTick;
        _modifiedDescriptor = DependencyPropertyDescriptor.FromProperty(
            HexEditorControl.IsModifiedProperty, typeof(HexEditorControl));
        _modifiedDescriptor.AddValueChanged(Editor, OnEditorModifiedChanged);

        OnPropertyChanged(nameof(FilePathText));
    }

    private ILogger<HexEditorView> Logger { get; }

    private HexEditorTabItem Instance { get; }

    private IToastManager ToastManager { get; }

    public string PageTitle => HexEditorModuleConstants.Title;

    public string FilePathText => _currentFilePath;

    public string StatusText { get; private set; } = "Loading...";

    public bool IsBusy { get; private set; }

    public bool IsIdle => !IsBusy && !_isDisposed;

    public bool CanSave => IsIdle && _documentLoaded;

    protected override void InternalOnLoaded()
    {
        _ = LoadDocumentAsync(false);
    }

    public override void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _lifetimeCancellation.Cancel();
        _reloadTimer.Stop();
        _reloadTimer.Tick -= OnReloadTimerTick;
        DisposeWatcher();
        _modifiedDescriptor.RemoveValueChanged(Editor, OnEditorModifiedChanged);
        Editor.Close();
        _lifetimeCancellation.Dispose();
        base.Dispose();
    }

    private async Task LoadDocumentAsync(bool reloading)
    {
        if (!IsIdle || Editor.IsOperationActive)
        {
            return;
        }

        SetBusy(true);
        SetStatus("Loading...");
        try
        {
            if (_watcher is null)
            {
                ConfigureWatcher();
            }

            var document = await HexDocumentTools.ReadDocumentAsync(_currentFilePath, _lifetimeCancellation.Token);
            if (_isDisposed)
            {
                return;
            }

            await SetDocumentBytesAsync(document.Bytes, document.IsReadOnly, reloading);
            _lastKnownWriteUtc = document.LastWriteUtc;
            _lastKnownLength = document.Bytes.LongLength;
            _hasExternalChange = false;
            Instance.SetDirty(false);
            SetStatus(document.IsReadOnly
                ? "Read-only file. Use Save As to create an editable copy."
                : $"{document.Bytes.LongLength:N0} bytes • {(reloading ? "Reloaded" : "Loaded")} at {DateTime.Now:HH:mm:ss}");
        }
        catch (OperationCanceledException) when (_isDisposed)
        {
        }
        catch (Exception ex)
        {
            if (!_isDisposed)
            {
                ReportFailure("Failed to read file.", ex);
            }
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task SetDocumentBytesAsync(byte[] bytes, bool readOnly, bool preservePosition)
    {
        var position = preservePosition ? Editor.Position : 0;
        _documentLoaded = false;
        await Editor.OpenByteArrayAsync(bytes, readOnly);
        Editor.ReadOnlyMode = readOnly;
        _documentLoaded = true;
        if (bytes.Length > 0)
        {
            Editor.SetPosition(Math.Clamp(position, 0, bytes.LongLength - 1));
        }
    }

    private async Task ReloadFromDiskAsync()
    {
        if (!IsIdle || Editor.IsOperationActive)
        {
            return;
        }

        if (Instance.IsDirty && !MessageBoxTools.Confirm(
                $"Discard unsaved changes to '{HexDocumentTools.GetDisplayName(_currentFilePath)}' and reload from disk?",
                "Reload",
                false))
        {
            return;
        }

        await LoadDocumentAsync(true);
    }

    private async Task SaveDocumentAsync(bool saveAs)
    {
        if (!CanSave || Editor.IsOperationActive)
        {
            return;
        }

        SetBusy(true);
        try
        {
            var targetPath = _currentFilePath;
            if (saveAs || Editor.ReadOnlyMode || (File.Exists(targetPath) && new FileInfo(targetPath).IsReadOnly))
            {
                var selectedPath = DialogTools.SaveFileDialog(
                    Path.GetFileName(targetPath),
                    Path.GetDirectoryName(targetPath) ?? string.Empty,
                    HexEditorModuleConstants.FileDialogFilter);
                if (string.IsNullOrWhiteSpace(selectedPath))
                {
                    return;
                }

                targetPath = selectedPath;
            }

            var pathChanged = !string.Equals(_currentFilePath, targetPath, StringComparison.OrdinalIgnoreCase);
            if (!pathChanged && HasFileChangedOnDisk() && !MessageBoxTools.Confirm(
                    $"'{HexDocumentTools.GetDisplayName(targetPath)}' changed on disk. Overwrite it with the editor contents?",
                    "File Changed",
                    false))
            {
                return;
            }

            var bytes = Editor.GetCurrentBytes();
            SetStatus("Saving...");
            await HexDocumentTools.WriteDocumentAsync(targetPath, bytes, _lifetimeCancellation.Token);
            if (_isDisposed)
            {
                return;
            }

            _currentFilePath = targetPath;
            _lastKnownWriteUtc = File.GetLastWriteTimeUtc(targetPath);
            _lastKnownLength = bytes.LongLength;
            _hasExternalChange = false;
            OnPropertyChanged(nameof(FilePathText));

            // WPFHexaEditor 3.4.5 SaveByteArray only clears a flag; it does not
            // commit the edit buffer. Rebase on the saved bytes (resetting undo
            // history) so undo and subsequent edits report the correct dirty state.
            await SetDocumentBytesAsync(bytes, new FileInfo(targetPath).IsReadOnly, true);
            Instance.SetDirty(false);
            ConfigureWatcher();

            if (pathChanged)
            {
                await Instance.UpdateDocumentUriAsync(new Uri(targetPath));
            }

            SetStatus($"{bytes.LongLength:N0} bytes • Saved at {DateTime.Now:HH:mm:ss}");
        }
        catch (OperationCanceledException) when (_isDisposed)
        {
        }
        catch (Exception ex)
        {
            if (!_isDisposed)
            {
                ReportFailure("Failed to save file.", ex);
            }
        }
        finally
        {
            SetBusy(false);
        }
    }

    private bool HasFileChangedOnDisk()
    {
        var info = new FileInfo(_currentFilePath);
        return _hasExternalChange || !info.Exists
                                  || info.LastWriteTimeUtc != _lastKnownWriteUtc
                                  || info.Length != _lastKnownLength;
    }

    private async Task HandleExternalFileChangeAsync()
    {
        if (!File.Exists(_currentFilePath))
        {
            _hasExternalChange = true;
            Editor.ReadOnlyMode = false;
            SetStatus("File was removed on disk. Save to recreate it or use Save As.");
            return;
        }

        if (!HasFileChangedOnDisk())
        {
            Editor.ReadOnlyMode = new FileInfo(_currentFilePath).IsReadOnly;
            return;
        }

        if (Instance.IsDirty)
        {
            if (!_hasExternalChange)
            {
                ToastManager.PromptMessage(ToastMessage.Warn(
                    $"File changed on disk: {HexDocumentTools.GetDisplayName(_currentFilePath)}", false));
            }

            _hasExternalChange = true;
            SetStatus("File changed on disk. Reload to sync or keep editing.");
            return;
        }

        await LoadDocumentAsync(true);
    }

    private void ConfigureWatcher()
    {
        DisposeWatcher();
        var directory = Path.GetDirectoryName(_currentFilePath);
        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
        {
            return;
        }

        _watcher = new FileSystemWatcher(directory, Path.GetFileName(_currentFilePath))
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime
                           | NotifyFilters.FileName | NotifyFilters.Attributes
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

    private void RestartReloadTimer()
    {
        if (_isDisposed || Dispatcher.HasShutdownStarted)
        {
            return;
        }

        Dispatcher.InvokeAsync(() =>
        {
            if (_isDisposed)
            {
                return;
            }

            _reloadTimer.Stop();
            _reloadTimer.Start();
        });
    }

    private void OnWatcherChanged(object sender, FileSystemEventArgs e) => RestartReloadTimer();

    private void OnWatcherRenamed(object sender, RenamedEventArgs e) => RestartReloadTimer();

    private async void OnReloadTimerTick(object? sender, EventArgs e)
    {
        _reloadTimer.Stop();
        if (_isDisposed)
        {
            return;
        }

        if (IsBusy || Editor.IsOperationActive)
        {
            _reloadTimer.Start();
            return;
        }

        try
        {
            await HandleExternalFileChangeAsync();
        }
        catch (Exception ex)
        {
            ReportFailure("Failed to check file changes.", ex);
        }
    }

    private void OnEditorModifiedChanged(object? sender, EventArgs e)
    {
        if (!IsBusy && !_isDisposed && _documentLoaded)
        {
            Instance.SetDirty(Editor.IsModified);
        }
    }

    private void SetBusy(bool isBusy)
    {
        IsBusy = isBusy;
        OnPropertyChanged(nameof(IsBusy));
        OnPropertyChanged(nameof(IsIdle));
        OnPropertyChanged(nameof(CanSave));
    }

    private void SetStatus(string status)
    {
        StatusText = status;
        OnPropertyChanged(nameof(StatusText));
    }

    private void ReportFailure(string status, Exception exception)
    {
        SetStatus(status);
        Logger.Error(exception);
        ToastManager.PromptException(exception);
    }

    private async void OnReloadButtonClick(object sender, RoutedEventArgs e) => await ReloadFromDiskAsync();

    private async void OnSaveButtonClick(object sender, RoutedEventArgs e) => await SaveDocumentAsync(false);

    private async void OnSaveAsButtonClick(object sender, RoutedEventArgs e) => await SaveDocumentAsync(true);

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
