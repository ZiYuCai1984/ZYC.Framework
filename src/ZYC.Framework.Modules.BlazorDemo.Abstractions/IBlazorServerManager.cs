using ZYC.CoreToolkit.Abstractions.Attributes;

namespace ZYC.Framework.Modules.BlazorDemo.Abstractions;

#pragma warning disable CS1591
[TempCode]
public interface IBlazorServerManager
{
    Task<IBlazorServer> StartBlazorServerAsync<TRootComponent>(string wwwrootFolder);
}