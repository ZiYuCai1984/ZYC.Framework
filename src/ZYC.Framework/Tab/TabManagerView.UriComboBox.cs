using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Autofac;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Workspace;

namespace ZYC.Framework.Tab;

internal partial class TabManagerView
{
    private void InitializeUriNavigation()
    {
        // WPF commits the highlighted history item before our bubbling key handler runs.
        // It marks Enter as handled when the drop-down is open, so include handled events.
        var enter = Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(
                h => UriComboBox.AddHandler(Keyboard.KeyDownEvent, h, true),
                h => UriComboBox.RemoveHandler(Keyboard.KeyDownEvent, h))
            .Where(e => e.EventArgs.Key == Key.Enter)
            .Do(e => e.EventArgs.Handled = true)
            .Where(e => !e.EventArgs.IsRepeat)
            .Select(_ => UriComboBox.Text);

        var go = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                h => UriGoButton.Click += h,
                h => UriGoButton.Click -= h)
            .Select(_ => UriComboBox.Text);

        // Text matching, arrow keys and binding updates can all change selection.
        // Only an actual click on a history item submits its address.
        var historyClick = Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                h => UriComboBox.AddHandler(Mouse.MouseUpEvent, h, true),
                h => UriComboBox.RemoveHandler(Mouse.MouseUpEvent, h))
            .Where(e => e.EventArgs.ChangedButton == MouseButton.Left)
            .Select(e => e.EventArgs.OriginalSource is DependencyObject source
                ? ItemsControl.ContainerFromElement(UriComboBox, source)
                : null)
            .OfType<ComboBoxItem>()
            .Where(item => item.IsSelected)
            .Select(item => item.Content)
            .OfType<string>();

        var context = new DispatcherSynchronizationContext(Dispatcher);
        Observable.Merge(enter, go, historyClick)
            // Capture each submitted string now, but start navigation only when it reaches
            // the front of the queue. Every queued operation must start on the UI thread.
            .Select(raw => Observable.FromAsync(() => CommitAndNavigateAsync(raw))
                .SubscribeOn(context))
            .Concat()
            .Subscribe(_ => { }, ex => Logger.Error(ex))
            .DisposeWith(CompositeDisposable);
    }

    private async Task CommitAndNavigateAsync(string raw)
    {
        try
        {
            if (Disposing)
            {
                return;
            }

            var uri = UriTools.NormalizeUri(raw);
            if (uri is null)
            {
                return;
            }

            // Preserve the binding that displays the focused tab's address.
            UriComboBox.SetCurrentValue(ComboBox.TextProperty, uri);

            if (uri == Uri || !System.Uri.TryCreate(uri, UriKind.Absolute, out var result))
            {
                return;
            }

            await TabManager.NavigateAsync(WorkspaceNode.Id, result);
        }
        catch (Exception ex)
        {
            // A failed request must not terminate the stream of future submissions.
            Logger.Error(ex);
        }
    }

    private void OnUriComboBoxGotFocus(object sender, RoutedEventArgs e)
    {
        LifetimeScope.Resolve<IParallelWorkspaceManager>().SetFocusedWorkspace(WorkspaceNode);
    }
}
