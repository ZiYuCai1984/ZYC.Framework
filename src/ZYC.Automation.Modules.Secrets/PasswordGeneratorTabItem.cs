using Autofac;
using ZYC.Automation.Abstractions;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core.Tab;
using ZYC.Automation.Modules.Secrets.UI;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Secrets;


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