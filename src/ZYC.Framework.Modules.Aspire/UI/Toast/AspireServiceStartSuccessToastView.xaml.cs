using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Modules.Aspire.Abstractions;

namespace ZYC.Framework.Modules.Aspire.UI.Toast;

[Register]
internal partial class AspireServiceStartSuccessToastView
{
    public AspireServiceStartSuccessToastView()
    {
        InitializeComponent();
    }


    public Uri TargetUri => AspireModuleContansts.Uri;
}