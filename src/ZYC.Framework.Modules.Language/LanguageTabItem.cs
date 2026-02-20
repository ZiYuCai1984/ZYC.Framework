using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.Language.Abstractions;
using ZYC.Framework.Modules.Language.UI;
using ZYC.Framework.Modules.Translator.Abstractions;

namespace ZYC.Framework.Modules.Language;

[Register]
[ConstantsSource(typeof(LanguageModuleConstants))]
internal class LanguageTabItem : TabItemInstanceBase<LanguageView>
{
    public LanguageTabItem(
        ILanguageManager languageManager,
        ILifetimeScope lifetimeScope, 
        TabReference tabReference) : base(lifetimeScope, tabReference)
    {
        LanguageManager = languageManager;
    }

    private ILanguageManager LanguageManager { get; }

    public override string Icon
    {
        get
        {
            if (LanguageManager.GetCurrentLanguageType() == LanguageType.ja)
            {
                return "SyllabaryHiragana";
            }

            return LanguageModuleConstants.DefaultIcon;
        }
    }
}