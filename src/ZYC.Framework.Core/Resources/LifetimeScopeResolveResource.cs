using System.Diagnostics;
using Autofac;

namespace ZYC.Framework.Core.Resources;

public class LifetimeScopeResolveResource : LifetimeScopeResource
{
    private object? _instance;

    public object Instance
    {
        get
        {
            Debug.Assert(Type != null);

            _instance ??= LifetimeScope.Resolve(Type);
            return _instance;
        }
    }

    public Type? Type { get; set; }
}