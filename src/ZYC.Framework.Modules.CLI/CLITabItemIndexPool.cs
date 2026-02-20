using ZYC.CoreToolkit.Common;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.Modules.CLI;

[RegisterSingleInstance]
internal class CLITabItemIndexPool : IndexPool
{
    protected override int Start => 1;
}