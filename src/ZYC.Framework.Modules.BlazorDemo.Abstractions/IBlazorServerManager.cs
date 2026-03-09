namespace ZYC.Framework.Modules.BlazorDemo.Abstractions;

#pragma warning disable CS1591
public interface IBlazorServerManager
{
    Task<IBlazorServer> StartBlazorServerAsync<TRootComponent>(string wwwrootFolder);
}