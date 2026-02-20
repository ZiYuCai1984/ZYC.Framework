using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.Secrets.Abstractions;
using ZYC.Framework.Modules.Secrets.UI;

namespace ZYC.Framework.Modules.Secrets;

[Register]
[ConstantsSource(typeof(SecretsModuleConstants))]
internal class SecretsTabItem : TabItemInstanceBase<SecretsManagerView>
{
    public SecretsTabItem(ILifetimeScope lifetimeScope, TabReference tabReference) : base(lifetimeScope, tabReference)
    {
    }
}