using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class ShowWindowCommand : CommandBase
{
    public ShowWindowCommand(IMainWindow mainWindow)
    {
        MainWindow = mainWindow;
    }

    private IMainWindow MainWindow { get; }

    protected override void InternalExecute(object? parameter)
    {
        MainWindow.Show();

        if (MainWindow.GetWindowState() == WindowState.Minimized)
        {
            MainWindow.SetWindowState(WindowState.Normal);
        }
    }
}