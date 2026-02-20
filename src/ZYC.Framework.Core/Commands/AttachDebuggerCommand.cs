using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class AttachDebuggerCommand : CommandBase
{
    protected override void InternalExecute(object? parameter)
    {
        DebuggerTools.Break();
    }
}