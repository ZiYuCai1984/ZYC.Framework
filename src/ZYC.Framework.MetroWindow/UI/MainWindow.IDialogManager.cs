using System.Windows;
using Autofac;
using Autofac.Core;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.MetroWindow.UI;

internal partial class MainWindow : IDialogManager
{
    public void Show<T>() where T : IDialog
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(() => ShowCore<T>());
            return;
        }

        ShowCore<T>();
    }

    public void Show<T>(params object[] parameters) where T : IDialog
    {
        var p = parameters.Cast<Parameter>().ToArray();
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(() => ShowCore<T>(p));
            return;
        }

        ShowCore<T>(p);
    }

    private void ShowCore<T>(params Parameter[] parameters) where T : IDialog
    {
        var dialog = LifetimeScope.Resolve<T>(parameters);
        if (dialog is not Window dialogWindow)
        {
            throw new InvalidOperationException(
                $"Dialog type '{typeof(T).FullName}' must inherit from {typeof(Window).FullName}.");
        }

        dialogWindow.Owner = this;
        dialogWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        dialogWindow.Show();
    }
}