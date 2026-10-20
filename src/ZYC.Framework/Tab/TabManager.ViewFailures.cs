using System.Windows;
using System.Windows.Threading;
using Autofac;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Tab.BuildIn;

namespace ZYC.Framework.Tab;

internal partial class TabManager
{
    private readonly Dictionary<ITabItemInstance,
        (UIElement View, EventHandler<ComponentResolveFailedEventArgs> Handler)> _viewFailureHandlers = new();

    private void ObserveViewFailures(ITabItemInstance instance, object view)
    {
        if (view is not UIElement element)
        {
            return;
        }

        var failurePending = false;
        EventHandler<ComponentResolveFailedEventArgs> handler = (_, args) =>
        {
            args.Handled = true;
            if (failurePending)
            {
                return;
            }

            failurePending = true;
            Logger.Error(args.Exception);

            // Let the current Loaded broadcast finish before replacing and disposing its visual tree.
            element.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                new Action(() => ReplaceFaultedTabItem(instance, args.Exception)));
        };

        element.AddHandler(Component.ResolveFailedEvent, handler);
        _viewFailureHandlers.Add(instance, (element, handler));
    }

    private void StopObservingViewFailures(ITabItemInstance instance)
    {
        if (_viewFailureHandlers.Remove(instance, out var registration))
        {
            registration.View.RemoveHandler(Component.ResolveFailedEvent, registration.Handler);
        }
    }

    private void ReplaceFaultedTabItem(ITabItemInstance instance, Exception exception)
    {
        // The tab may have been closed, reloaded or moved while the replacement was queued.
        if (!TabItemInstances.Contains(instance))
        {
            return;
        }

        var workspace = GetTabItemInstanceWorkspace(instance);
        var items = WorkspaceTabItemInstanceListDictionary[workspace];
        var index = items.IndexOf(instance);
        var wasFocused = GetFocusedTabItemInstance(workspace.Id) == instance;
        var errorInstance = LifetimeScope.Resolve<ErrorTabItem>(
            new TypedParameter(typeof(Exception), exception),
            new TypedParameter(typeof(TabReference), instance.TabReference));
        _ = errorInstance.View;

        StopObservingViewFailures(instance);
        items[index] = errorInstance;
        TabItemInstances.Remove(instance);
        TabItemInstances.Add(errorInstance);

        EventAggregator.Publish(new TabItemViewFaultedEvent(workspace.Id, instance, errorInstance, index));

        if (wasFocused)
        {
            SetFocusedTabItemInstance(workspace.Id, errorInstance);
            InvokeNavigateCompletedEvent(workspace.Id, errorInstance);
        }

        try
        {
            instance.Dispose();
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}