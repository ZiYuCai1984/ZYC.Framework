namespace ZYC.Framework.Modules.BlazorDemo.Abstractions;

#pragma warning disable CS1591

public interface IBlazorServer : IDisposable
{
    int Port => BaseUri.Port;

    Uri BaseUri { get; }
}