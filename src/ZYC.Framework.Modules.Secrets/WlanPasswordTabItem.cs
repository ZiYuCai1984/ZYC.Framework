using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.Secrets.UI;

namespace ZYC.Framework.Modules.Secrets;

//TODO-zyc Pending move to extended modules

[Register]
internal class WlanPasswordTabItem : TabItemInstanceBase<WlanPasswordView>
{
    public WlanPasswordTabItem(
        ILifetimeScope lifetimeScope, 
        TabReference tabReference) : base(lifetimeScope,
        tabReference)
    {
    }

    public class Constants
    {
        public static string Icon => "WifiLockOpen";

        // ReSharper disable once StringLiteralTypo
        public static string Host => "wlanpassword";

        public static string Title => "WlanPassword";

        public static Uri Uri => UriTools.CreateAppUri(Host);
    }
}