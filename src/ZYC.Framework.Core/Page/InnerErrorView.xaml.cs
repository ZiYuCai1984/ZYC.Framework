using System.Windows;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Core.Page;

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