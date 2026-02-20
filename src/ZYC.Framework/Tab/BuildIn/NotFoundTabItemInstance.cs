using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;

namespace ZYC.Framework.Tab.BuildIn;

[RegisterAs(typeof(NotFoundTabItemInstance), typeof(INotFoundTabItemInstance))]
internal class NotFoundTabItemInstance : TabItemInstanceBase<NotFoundView>, INotFoundTabItemInstance
{
    public NotFoundTabItemInstance(ILifetimeScope lifetimeScope, TabReference tabReference) : base(lifetimeScope, tabReference)
    {
        
    }

    public static class Constants
    {
        public static string Title => "Page not found";

        public static string Icon => "BugOutline";
    }
}