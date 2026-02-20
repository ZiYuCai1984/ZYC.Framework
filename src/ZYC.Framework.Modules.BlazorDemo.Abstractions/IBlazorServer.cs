namespace ZYC.Framework.Modules.BlazorDemo.Abstractions;

public interface IBlazorServer : IDisposable
{
    int Port => BaseUri.Port;

    Uri BaseUri { get; }
}