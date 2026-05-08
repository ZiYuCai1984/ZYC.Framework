using System.Windows;
using Autofac;
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

    private void ShowCore<T>() where T : IDialog
    {
        var dialog = LifetimeScope.Resolve<T>();
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
