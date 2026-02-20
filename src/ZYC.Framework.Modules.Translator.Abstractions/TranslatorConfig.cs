using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.Translator.Abstractions;

public class TranslatorConfig : IConfig
{
    public string Url { get; set; } = "http://127.0.0.1:5000";

    public bool IsEnabled { get; set; }
}