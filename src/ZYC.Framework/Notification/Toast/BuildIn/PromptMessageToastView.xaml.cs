using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Notification.Toast;

namespace ZYC.Framework.Notification.Toast.BuildIn;

[Register]
internal partial class PromptMessageToastView
{
    public PromptMessageToastView(ToastMessage toastMessage)
    {
        ToastMessage = toastMessage;

        InitializeComponent();
    }

    public ToastMessage ToastMessage { get; }
}