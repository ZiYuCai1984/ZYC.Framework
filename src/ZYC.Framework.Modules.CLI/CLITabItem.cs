using Autofac;
using ZYC.CoreToolkit.Common;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.CLI.Abstractions;
using ZYC.Framework.Modules.CLI.UI;

namespace ZYC.Framework.Modules.CLI;

[Register]
[ConstantsSource(typeof(CLIModuleConstants))]
internal class CLITabItem : TabItemInstanceBase
{
    public CLITabItem(
        ILifetimeScope lifetimeScope,
        TabReference tabReference,
        CLITabItemIndexPool indexPool) : base(lifetimeScope, tabReference)
    {
        IndexPool = indexPool;
        Lease = IndexPool.AcquireLease();
    }

    public override object View => _view ??= LifetimeScope.Resolve<CLIView>(
        new TypedParameter(typeof(CLIUriOptions), CLIUriOptions.Parse(TabReference.Uri)));

    private CLITabItemIndexPool IndexPool { get; }

    private IndexPool.Lease Lease { get; }

    public override string Title => $"{CLIModuleConstants.DefaultTitle} - {Lease.Index}";

    public override string Icon => CLIModuleConstants.Icon;

    public override bool Localization => false;

    public override void Dispose()
    {
        base.Dispose();
        Lease.Dispose();
    }
}