using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;

namespace ZYC.Framework.Tab.BuildIn;

[Register]
internal class ModuleLoadErrorTabItem : TabItemInstanceBase<ModuleLoadErrorView>
{
    public ModuleLoadErrorTabItem(
        ILifetimeScope lifetimeScope, 
        TabReference tabReference) : base(lifetimeScope,
        tabReference)
    {
    }

    public static class Constants
    {
        public static string Title => "Module load error";

        public static string Icon => "BugOutline";

        public const string Host = "module-error";

        public static Uri Uri => UriTools.CreateAppUri(Host);
    }
}

