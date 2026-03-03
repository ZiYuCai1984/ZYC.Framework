using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.Notification.Toast.BuildIn;

[Register]
internal partial class PromptCopiedToastView
{
    public string CopiedText { get; }

    public PromptCopiedToastView(string copiedText)
    {
        CopiedText = copiedText;
        InitializeComponent();
    }
}