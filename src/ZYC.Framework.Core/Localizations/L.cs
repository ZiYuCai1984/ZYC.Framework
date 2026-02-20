using System.Windows.Markup;
using Autofac;

namespace ZYC.Framework.Core.Localizations;

/// <summary>
///     Localization placeholder for Xaml text entries.
/// </summary>
public class L : MarkupExtension
{
    private static ILocalizationer? _languageService;

    public L(string text)
    {
        Text = text;
    }

    public string Text { get; }

    private static ILifetimeScope? LifetimeScope { get; set; }

    private static ILocalizationer Localizationer =>
        _languageService ??= LifetimeScope!.Resolve<ILocalizationer>();

    public static void SetLifetimeScope(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Translate(Text);
    }

    public static string Translate(string text)
    {
        return Localizationer.Localization(text);
    }

    public static string T(string text)
    {
        return Translate(text);
    }
}