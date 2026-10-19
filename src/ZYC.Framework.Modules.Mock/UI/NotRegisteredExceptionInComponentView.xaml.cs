using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.Modules.Mock.UI;

[Register]
internal partial class NotRegisteredExceptionInComponentView
{
    public NotRegisteredExceptionInComponentView()
    {
        InitializeComponent();
    }
}