using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.Secrets.UI;

namespace ZYC.Framework.Modules.Secrets;


//TODO-zyc Pending move to extended modules

[Register]
internal class PasswordGeneratorTabItem : TabItemInstanceBase<PasswordGeneratorView>
{
    public PasswordGeneratorTabItem(ILifetimeScope lifetimeScope, TabReference tabReference) : base(lifetimeScope, tabReference)
    {
    }

    public class Constants
    {
        public static string Icon => "FormTextboxPassword";

        // ReSharper disable once StringLiteralTypo
        public static string Host => "passwordgenerator";

        public static string Title => "PasswordGenerator";

        public static Uri Uri => UriTools.CreateAppUri(Host);
    }
}