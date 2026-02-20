using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class ThrowExceptionCommand : CommandBase
{
    protected override void InternalExecute(object? parameter)
    {
        if (parameter is Exception e)
        {
            throw e;
        }

        throw new Exception(parameter?.ToString() ?? "Unknown exception !!");
    }
}