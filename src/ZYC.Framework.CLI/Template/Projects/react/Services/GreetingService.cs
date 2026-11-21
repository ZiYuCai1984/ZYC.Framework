using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;

namespace __PROJECT_NAME__.Services;

[RegisterSingleInstanceAs(typeof(IGreetingService))]
internal class GreetingService : IGreetingService
{
    public Task<HostGreeting> GreetAsync(string name)
    {
        return Task.FromResult(new HostGreeting(
            $"Hello {name}, from {ProductInfo.ProductName} on .NET {Environment.Version}.",
            ProductInfo.Version,
            DateTimeOffset.Now));
    }
}
