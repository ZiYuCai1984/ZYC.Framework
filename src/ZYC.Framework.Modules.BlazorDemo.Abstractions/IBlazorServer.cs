using ZYC.CoreToolkit.Abstractions.Attributes;

namespace ZYC.Framework.Modules.BlazorDemo.Abstractions;

#pragma warning disable CS1591

[TempCode]
public interface IBlazorServer : IDisposable
{
    int Port => BaseUri.Port;

    Uri BaseUri { get; }
}