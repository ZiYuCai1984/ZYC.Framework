using Autofac;
using Autofac.Core;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Core;

public static class TabItemCreationContextEx
{
    extension(TabItemCreationContext context)
    {
        public T Resolve<T>() where T : notnull
        {
            return context.GetLifetimeScope().Resolve<T>();
        }

        public T Resolve<T>(params Parameter[] parameters) where T : notnull
        {
            return context.GetLifetimeScope().Resolve<T>(parameters);
        }

        public ILifetimeScope GetLifetimeScope()
        {
            return (ILifetimeScope)context.LifetimeScope;
        }
    }
}