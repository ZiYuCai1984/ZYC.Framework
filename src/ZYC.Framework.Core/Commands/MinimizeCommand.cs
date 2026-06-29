using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class MinimizeCommand : CommandBase
{
    public MinimizeCommand(IMainWindow mainWindow)
    {
        MainWindow = mainWindow;
    }

    private IMainWindow MainWindow { get; }


    protected override void InternalExecute(object? parameter)
    {
        if (MainWindow.GetShowInTaskbar())
        {
            MainWindow.SetWindowState(WindowState.Minimized);
            return;
        }

        MainWindow.Hide();
    }
}
