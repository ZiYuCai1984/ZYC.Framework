using ZYC.CoreToolkit.CodeGenerator;

namespace ZYC.Framework.WebView2.Menu;

[GeneratePropertiesFrom(typeof(ExtendedMenuItem))]
public partial class PluginExtendedMenuItem
{
    // ReSharper disable once ConvertConstructorToMemberInitializers
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public PluginExtendedMenuItem()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        //TODO-zyc The GeneratePropertiesFrom method does not handle the initial values ​​of properties, this is a bug !!
        Localization = true;
    }
}