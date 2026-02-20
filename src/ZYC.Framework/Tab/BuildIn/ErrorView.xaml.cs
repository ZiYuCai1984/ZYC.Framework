using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Core.Page;

namespace ZYC.Framework.Tab.BuildIn;

[RegisterAs(typeof(ErrorView), typeof(IErrorView))]
internal partial class ErrorView : IErrorView
{
    public ErrorView(
        Exception exception, 
        ILifetimeScope lifetimeScope, 
        CopyAndNotifyCommand copyAndNotifyCommand, 
        ErrorViewWrapSwitchCommand errorViewWrapSwitchCommand)
    {
        Exception = exception;
        LifetimeScope = lifetimeScope;
        CopyAndNotifyCommand = copyAndNotifyCommand;
        ErrorViewWrapSwitchCommand = errorViewWrapSwitchCommand;

        InitializeComponent();
    }

    public Exception Exception { get; }

    private ILifetimeScope LifetimeScope { get; }

    public CopyAndNotifyCommand CopyAndNotifyCommand { get; }

    public ErrorViewWrapSwitchCommand ErrorViewWrapSwitchCommand { get; }

    protected override void InternalOnLoaded()
    {
        Grid.Children.Add(LifetimeScope.Resolve<InnerErrorView>(
            new TypedParameter(typeof(Exception), Exception)));
    }
}