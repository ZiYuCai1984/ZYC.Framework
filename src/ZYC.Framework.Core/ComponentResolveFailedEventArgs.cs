using System.Windows;

namespace ZYC.Framework.Core;

public class ComponentResolveFailedEventArgs : RoutedEventArgs
{
    public ComponentResolveFailedEventArgs(Exception exception) : base(Component.ResolveFailedEvent)
    {
        Exception = exception;
    }

    public Exception Exception { get; }

    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
    {
        ((EventHandler<ComponentResolveFailedEventArgs>)genericHandler)(genericTarget, this);
    }
}
