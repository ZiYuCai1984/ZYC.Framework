using System.Windows;
using ZYC.Automation.Abstractions;
using ZYC.Automation.Core.Commands;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Core.Page;

[RegisterAs(typeof(InnerErrorView), typeof(IInnerErrorView))]
public partial class InnerErrorView : IInnerErrorView
{
    public InnerErrorView(Exception exception, ErrorViewWrapSwitchCommand errorViewWrapSwitchCommand)
    {
        Exception = exception;
        ErrorViewWrapSwitchCommand = errorViewWrapSwitchCommand;

        InitializeComponent();
    }

    public Exception Exception { get; }

    public ErrorViewWrapSwitchCommand ErrorViewWrapSwitchCommand { get; }

    private bool FirstRending { get; set; } = true;

    private void OnInnerErrorViewLoaded(object sender, RoutedEventArgs e)
    {
        if (!FirstRending)
        {
            return;
        }

        FirstRending = false;
        TextEditor.Text = Exception.ToString();
    }
}