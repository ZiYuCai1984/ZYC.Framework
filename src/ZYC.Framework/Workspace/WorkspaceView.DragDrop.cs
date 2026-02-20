using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Autofac;
using ZYC.Framework.Abstractions.DragDrop;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core.DragDrop;
using ZYC.Framework.Core.Localizations;

namespace ZYC.Framework.Workspace;

internal partial class WorkspaceView
{
    private IDropOrchestrator? _dropOrchestrator;

    private IDropPayloadParser<IDataObject>? _dropPayloadParser;

    private IDropPayloadParser<IDataObject> DropPayloadParser =>
        _dropPayloadParser ??= LifetimeScope.Resolve<IDropPayloadParser<IDataObject>>();

    private IDropOrchestrator DropOrchestrator => _dropOrchestrator ??= LifetimeScope.Resolve<IDropOrchestrator>();

    private async void OnWorkspaceViewDrop(object sender, DragEventArgs e)
    {
        try
        {
            e.Handled = true;

            var payload = DropPayloadParser.Parse(e.Data);
            var pos = PointToScreen(e.GetPosition(this));
            var context = new DropContext(
                this,
                Node.Id,
                KeyMapper.Map(Keyboard.Modifiers),
                (pos.X, pos.Y),
                CancellationToken.None);

            var resolution = await DropOrchestrator.ResolveAsync(payload, context);

            if (resolution.Mode == DropResolutionMode.None)
            {
                ToastManager.PromptMessage(ToastMessage.Warn("No module can handle this drop"));
                return;
            }

            if (resolution.Mode == DropResolutionMode.ExecuteDefault
                && resolution.DefaultAction is not null)
            {
                await RunWithBasicUIAsync(resolution.DefaultAction);
                return;
            }

            // PickAction
            var picked = await ShowContextMenuPickAsync(resolution.Actions, e.GetPosition(this));
            if (picked is not null)
            {
                await RunWithBasicUIAsync(picked);
            }
        }
        catch (Exception ex)
        {
            ToastManager.PromptMessage(ToastMessage.Exception(ex));
        }
    }

    private static async Task RunWithBasicUIAsync(DropAction action)
    {
        var progress = new Progress<double>(p =>
        {
        });

        await action.RunAsync(progress);
    }

    private Task<DropAction?> ShowContextMenuPickAsync(
        IReadOnlyList<DropAction> actions,
        Point localPoint)
    {
        var tcs = new TaskCompletionSource<DropAction?>();

        var menu = new ContextMenu();
        foreach (var action in actions)
        {
            var title = action.Title;
            if (action.Localization)
            {
                title = L.Translate(title);
            }

            var item = new MenuItem
            {
                Header = title,
                IsEnabled = action.CanRun()
            };

            item.Click += (_, __) =>
            {
                tcs.TrySetResult(action);
                menu.IsOpen = false;
            };

            menu.Items.Add(item);
        }

        menu.Closed += (_, __) => tcs.TrySetResult(null);

        menu.PlacementTarget = this;
        menu.Placement = PlacementMode.Relative;
        menu.HorizontalOffset = localPoint.X;
        menu.VerticalOffset = localPoint.Y;
        menu.IsOpen = true;

        return tcs.Task;
    }
}