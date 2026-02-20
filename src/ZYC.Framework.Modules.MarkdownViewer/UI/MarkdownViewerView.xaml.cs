using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core.DragDrop;
using ZYC.Framework.Modules.MarkdownViewer.Abstractions;
using ZYC.MdXaml;

namespace ZYC.Framework.Modules.MarkdownViewer.UI;

[Register]
internal partial class MarkdownViewerView : INotifyPropertyChanged
{
    public MarkdownViewerView(
        IMarkdownViewerTabItem markdownViewerTabItem,
        ILogger<MarkdownViewerView> logger)
    {
        MarkdownViewerTabItem = markdownViewerTabItem;
        Logger = logger;

        InitializeComponent();
    }


    public MarkdownSource? MarkdownSource => MarkdownViewerTabItem.MarkdownSource;

    private IMarkdownViewerTabItem MarkdownViewerTabItem { get; }

    private ILogger<MarkdownViewerView> Logger { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnMarkdownScrollViewerLoaded(object sender, RoutedEventArgs e)
    {
        var markdownScrollViewer = (MarkdownScrollViewer)sender;

        Debug.Assert(MarkdownSource != null);


        markdownScrollViewer.BaseUri = MarkdownSource.BaseUri;
        markdownScrollViewer.Source = MarkdownSource.SourceUri;
    }

    private async void OnDragDropPickerViewPathsPicked(object? sender, PathsPickedEventArgs e)
    {
        try
        {
            var path = e.Paths.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(path))
            {
                Uri? sourceUri = null;


                if (UriTools.LooksLikeWindowsPath(path))
                {
                    try
                    {
                        sourceUri = new Uri(Path.GetFullPath(path));
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }

                if (sourceUri is null)
                {
                    if (Uri.TryCreate(path, UriKind.Absolute, out var u))
                    {
                        sourceUri = u;
                    }
                    else if (Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out var ru) && ru.IsAbsoluteUri)
                    {
                        sourceUri = ru;
                    }
                }


                if (sourceUri is null)
                {
                    DebuggerTools.Break();
                    return;
                }

                var baseUri = new Uri(sourceUri, ".");
                await MarkdownViewerTabItem.UpdateMarkdownSourceAsync(new MarkdownSource(sourceUri, baseUri));

                OnPropertyChanged(nameof(MarkdownSource));
            }
        }
        catch
        {
            //ignore
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}