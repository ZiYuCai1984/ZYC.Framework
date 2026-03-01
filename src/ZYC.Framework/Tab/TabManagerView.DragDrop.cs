using Autofac;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.DragDrop;

namespace ZYC.Framework.Tab;

internal partial class TabManagerView
{
    private AdornerLayer? _rootAdornerLayer;

    private UIElement? _rootGrid;

    private UIElement RootGrid => _rootGrid ??= (UIElement)LifetimeScope.Resolve<IRootGrid>().GetRootGrid();

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
        var tabItemInstance = (ITabItemInstance)element.DataContext;

        var move = Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                h => element.MouseMove += h, h => element.MouseMove -= h)
            .Where(ep => ep.EventArgs.LeftButton == MouseButtonState.Pressed)
            .Where(_ => !TabItemLockState.TabItems.Contains(tabItemInstance.TabReference))
            //.Throttle(TimeSpan.FromMilliseconds(50))
            .ObserveOnUI()
            .Subscribe(_ =>
            {
                DragAdorner? adorner = null;
                AdornerLayer? layer = null;

                var container = RootGrid;

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
                    if (adorner == null || container == null)
                    {
                        return;
                    }

                    GetCursorPos(out var pt);

                    var mousePosInContainer = container.PointFromScreen(new Point(pt.X, pt.Y));

                    var left = mousePosInContainer.X - element.ActualWidth / 2;
                    var top = mousePosInContainer.Y - element.ActualHeight / 2;

                    adorner.UpdatePosition(left, top);
                }
            });

        element.Unloaded += (_, _) =>
        {
            move.Dispose();
        };
    }

    private struct POINT
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public int X;
        public int Y;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
    }
}