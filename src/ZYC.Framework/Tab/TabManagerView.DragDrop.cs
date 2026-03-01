using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;

namespace ZYC.Framework.Tab;

internal partial class TabManagerView
{
    private static object DragDropLock { get; } = new();

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
                var lockTaken = false;

                try
                {
                    lockTaken = Monitor.TryEnter(DragDropLock);
                    if (!lockTaken)
                    {
                        return;
                    }

                    var data = new DataObject(typeof(ITabItemInstance), tabItemInstance);

                    System.Windows.DragDrop.DoDragDrop(
                        element,
                        data,
                        DragDropEffects.Move);
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
                }
            });

        element.Unloaded += (_, _) =>
        {
            move.Dispose();
        };
    }
}