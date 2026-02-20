using System.Diagnostics;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class OpenGitHubCommand : CommandBase
{
    protected override void InternalExecute(object? parameter)
    {
        Process.Start(new ProcessStartInfo(ProductInfoExtended.Repository) { UseShellExecute = true });
    }
}