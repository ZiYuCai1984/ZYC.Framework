using Autofac;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core.Tab;
using ZYC.Automation.Modules.Gemini.Abstractions;
using ZYC.Automation.Modules.Gemini.UI;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;


namespace ZYC.Automation.Modules.Gemini;

[Register]
[ConstantsSource(typeof(GeminiModuleConstants))]
internal class GeminiTabItem : TabItemInstanceBase<GeminiView>
{
    public GeminiTabItem(
        ILifetimeScope lifetimeScope,
        TabReference tabReference) : base(lifetimeScope, tabReference)
    {
    }
}