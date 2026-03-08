using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Autofac;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.DragDrop;

namespace ZYC.Framework.Tab;

internal partial class TabManagerView
{
    private readonly Dictionary<FrameworkElement, TabHeaderDragState> _tabHeaderDragStates = new();
    private AdornerLayer? _rootAdornerLayer;

    private FrameworkElement? _rootGrid;

    private FrameworkElement RootGrid =>
        _rootGrid ??= (FrameworkElement)LifetimeScope.Resolve<IRootGrid>().GetRootGrid();

    private AdornerLayer RootAdornerLayer => _rootAdornerLayer ??=
        (AdornerLayer)LifetimeScope.Resolve<IRootAdornerLayer>().GetRootAdornerLayer();

    private static object DragDropLock { get; } = new();


    /// <summary>
    ///     !WARNING Need to use Win32 to get the coordinates because Mouse.GetPosition might not update during DragDrop.
    /// </summary>
    /// <param name="lpPoint"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    private void OnTabItemHeaderLoaded(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;

        DetachTabHeaderState(element);

        var state = new TabHeaderDragState();
        _tabHeaderDragStates[element] = state;

        var compositeDisposable = state.CompositeDisposable;

        var container = RootGrid;

        var mouseDown = Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                h => element.PreviewMouseLeftButtonDown += h,
                h => element.PreviewMouseLeftButtonDown -= h)
            .Select(ep => ep.EventArgs.GetPosition(container));

        var mouseMove = Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
            h => element.PreviewMouseMove += h,
            h => element.PreviewMouseMove -= h);

        var mouseUp = Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
            h => element.PreviewMouseLeftButtonUp += h,
            h => element.PreviewMouseLeftButtonUp -= h);

        var dragStart = mouseDown.SelectMany(startPt =>
            mouseMove
                .TakeUntil(mouseUp)
                .Where(ep => ep.EventArgs.LeftButton == MouseButtonState.Pressed)
                .Select(ep => ep.EventArgs.GetPosition(container))
                .Where(pt =>
                    Math.Abs(pt.X - startPt.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(pt.Y - startPt.Y) >= SystemParameters.MinimumVerticalDragDistance)
                .Take(1));

        dragStart
            .ObserveOnUI()
            .Subscribe(_ =>
            {
                if (element.DataContext is not ITabItemInstance tabItemInstance)
                {
                    return;
                }

                if (TabItemLockState.TabItems.Contains(tabItemInstance.TabReference))
                {
                    return;
                }

                StartDrag(container, element, tabItemInstance);
            }).DisposeWith(compositeDisposable);


        var dragEnter = Observable.FromEventPattern<DragEventHandler, DragEventArgs>(
            h => element.DragEnter += h,
            h => element.DragEnter -= h);

        var dragOver = Observable.FromEventPattern<DragEventHandler, DragEventArgs>(
            h => element.DragOver += h,
            h => element.DragOver -= h);

        var dragLeave = Observable.FromEventPattern<DragEventHandler, DragEventArgs>(
            h => element.DragLeave += h,
            h => element.DragLeave -= h);

        var drop = Observable.FromEventPattern<DragEventHandler, DragEventArgs>(
            h => element.Drop += h,
            h => element.Drop -= h);

        dragEnter
            .Merge(dragOver)
            .ObserveOnUI()
            .Subscribe(ep =>
            {
                UpdateTabHeaderDropAdorner(element, ep.EventArgs);
            })
            .DisposeWith(compositeDisposable);


        dragLeave
            .ObserveOnUI()
            .Subscribe(_ =>
            {
                ClearTabHeaderDropAdorner(element);
            })
            .DisposeWith(compositeDisposable);


        drop
            .ObserveOnUI()
            .Subscribe(ep =>
            {
                var args = ep.EventArgs;
                args.Handled = true;

                try
                {
                    ClearTabHeaderDropAdorner(element);

                    if (!args.Data.GetDataPresent(typeof(ITabItemInstance)))
                    {
                        return;
                    }

                    if (element.DataContext is not ITabItemInstance target)
                    {
                        return;
                    }

                    var source = (ITabItemInstance)args.Data.GetData(typeof(ITabItemInstance))!;
                    if (ReferenceEquals(source, target))
                    {
                        return;
                    }

                    var insertPosition = GetTabInsertPosition(
                        element,
                        args.GetPosition(element).X);

                    TabManager.MoveTabItemInstance(source, target, insertPosition);
                }
                finally
                {
                    ClearTabHeaderDropAdorner(element);
                }
            })
            .DisposeWith(compositeDisposable);


        element.Unloaded += (_, _) => compositeDisposable.Dispose();
    }

    private void UpdateTabHeaderDropAdorner(FrameworkElement element, DragEventArgs args)
    {
        if (!args.Data.GetDataPresent(typeof(ITabItemInstance)))
        {
            args.Effects = DragDropEffects.None;
            ClearTabHeaderDropAdorner(element);
            args.Handled = true;
            return;
        }

        if (element.DataContext is not ITabItemInstance target)
        {
            args.Effects = DragDropEffects.None;
            ClearTabHeaderDropAdorner(element);
            args.Handled = true;
            return;
        }

        var source = (ITabItemInstance)args.Data.GetData(typeof(ITabItemInstance))!;
        if (ReferenceEquals(source, target))
        {
            args.Effects = DragDropEffects.None;
            ClearTabHeaderDropAdorner(element);
            args.Handled = true;
            return;
        }

        var insertPosition = GetTabInsertPosition(
            element,
            args.GetPosition(element).X);

        ShowTabHeaderDropAdorner(element, insertPosition);

        args.Effects = DragDropEffects.Move;
        args.Handled = true;
    }

    private static TabInsertPosition GetTabInsertPosition(FrameworkElement element, double x)
    {
        return x < element.ActualWidth / 2.0
            ? TabInsertPosition.Before
            : TabInsertPosition.After;
    }

    private void ShowTabHeaderDropAdorner(FrameworkElement element, TabInsertPosition insertPosition)
    {
        if (!_tabHeaderDragStates.TryGetValue(element, out var state))
        {
            return;
        }

        state.AdornerLayer ??= AdornerLayer.GetAdornerLayer(element);
        if (state.AdornerLayer == null)
        {
            return;
        }

        if (state.Adorner == null)
        {
            state.Adorner = new TabInsertAdorner(element);
            state.AdornerLayer.Add(state.Adorner);
        }

        state.Adorner.Update(insertPosition);
    }

    private void ClearTabHeaderDropAdorner(FrameworkElement element)
    {
        if (!_tabHeaderDragStates.TryGetValue(element, out var state))
        {
            return;
        }

        if (state.AdornerLayer != null && state.Adorner != null)
        {
            state.AdornerLayer.Remove(state.Adorner);
            state.Adorner = null;
        }
    }

    private void DetachTabHeaderState(FrameworkElement element)
    {
        if (!_tabHeaderDragStates.TryGetValue(element, out var state))
        {
            return;
        }

        if (state.AdornerLayer != null && state.Adorner != null)
        {
            state.AdornerLayer.Remove(state.Adorner);
            state.Adorner = null;
        }

        state.CompositeDisposable.Dispose();
        _tabHeaderDragStates.Remove(element);
    }

    private void StartDrag(FrameworkElement container, FrameworkElement element, ITabItemInstance tabItemInstance)
    {
        DragAdorner? adorner = null;
        AdornerLayer? layer = null;
        var lockTaken = false;

        try
        {
            lockTaken = Monitor.TryEnter(DragDropLock);
            if (!lockTaken)
            {
                return;
            }

            layer = RootAdornerLayer;
            adorner = new DragAdorner(container, element);
            layer.Add(adorner);

            element.Opacity = 0.4;
            element.GiveFeedback += OnElementGiveFeedback;

            var data = new DataObject(typeof(ITabItemInstance), tabItemInstance);
            System.Windows.DragDrop.DoDragDrop(element, data, DragDropEffects.Move);
        }
        catch (COMException ex)
        {
            var hr = unchecked((uint)ex.HResult);
            Debug.WriteLine($"DoDragDrop HR=0x{hr:X8} {ex}");
            Logger.Error(ex);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
        finally
        {
            if (lockTaken)
            {
                Monitor.Exit(DragDropLock);
            }

            element.GiveFeedback -= OnElementGiveFeedback;
            element.Opacity = 1.0;

            if (adorner != null)
            {
                layer?.Remove(adorner);
            }
        }

        return;

        void OnElementGiveFeedback(object s, GiveFeedbackEventArgs args)
        {
            if (adorner == null)
            {
                return;
            }

            GetCursorPos(out var pt);
            var mousePosInContainer = container.PointFromScreen(new Point(pt.X, pt.Y));

            var left = mousePosInContainer.X - element.ActualWidth / 2;
            var top = mousePosInContainer.Y - element.ActualHeight / 2;

            adorner.UpdatePosition(left, top);
        }
    }

    private sealed class TabHeaderDragState
    {
        public CompositeDisposable CompositeDisposable { get; } = new();

        public AdornerLayer? AdornerLayer { get; set; }

        public TabInsertAdorner? Adorner { get; set; }
    }

    private struct POINT
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public int X;
        public int Y;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
    }
}