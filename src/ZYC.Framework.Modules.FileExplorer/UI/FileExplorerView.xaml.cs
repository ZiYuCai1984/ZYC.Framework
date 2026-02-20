using System.Diagnostics;
using System.Windows;
using Microsoft.WindowsAPICodePack.Controls;
using Microsoft.WindowsAPICodePack.Shell;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.FileExplorer.Abstractions;

namespace ZYC.Framework.Modules.FileExplorer.UI;

[Register]
internal partial class FileExplorerView : IDisposable
{
    public FileExplorerView(
        IAppLogger<FileExplorerView> logger,
        Uri uri,
        IFileExplorerTabItemInstance fileExplorerTabItemInstance,
        IAppContext appContext)
    {
        Logger = logger;
        FileExplorerTabItemInstance = fileExplorerTabItemInstance;
        AppContext = appContext;

        InitializeComponent();

        ExplorerBrowser.NavigationOptions.PaneVisibility.Navigation = PaneVisibilityState.Hide;
        ExplorerBrowser.NavigationOptions.PaneVisibility.Commands = PaneVisibilityState.Hide;
        ExplorerBrowser.NavigationComplete += OnExplorerBrowserNavigationComplete;

        //!WARNING If an exception is thrown here, explorerBrowser will be released automatically, so there's no need to worry about memory leaks.
        ExplorerBrowser.Navigate(ShellObject.FromParsingName(
            uri.ToString()));
    }


    private IAppLogger<FileExplorerView> Logger { get; }
    private IFileExplorerTabItemInstance FileExplorerTabItemInstance { get; }
    private IAppContext AppContext { get; }

    private bool FirstRending { get; set; } = true;

    private bool Disposing { get; set; }

    public void Dispose()
    {
        if (Disposing)
        {
            Debugger.Break();
            return;
        }

        Disposing = true;

        Loaded -= OnFileExplorerViewLoaded;
        ExplorerBrowser.NavigationComplete -= OnExplorerBrowserNavigationComplete;

        ExplorerBrowser.Dispose();

        var content = Content;
        Content = null;
        content?.TryDispose();
    }

    private void OnFileExplorerViewLoaded(object sender, RoutedEventArgs e)
    {
        if (!FirstRending)
        {
            return;
        }

        FirstRending = false;
    }

    private async void OnExplorerBrowserNavigationComplete(object? sender, NavigationCompleteEventArgs e)
    {
        try
        {
            var path = e.NewLocation.ParsingName;

            await FileExplorerTabItemInstance.TabInternalNavigatingAsync(
                new Uri($"file:///{path}"));

            try
            {
                AppContext.InvokeOnUIThread(() =>
                {
                    var base64Icon = ShellIconBase64.TryGetFolderIconPngBase64(path);
                    if (string.IsNullOrWhiteSpace(base64Icon))
                    {
                        return;
                    }

                    FileExplorerTabItemInstance.UpdateIcon(base64Icon);
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }
}