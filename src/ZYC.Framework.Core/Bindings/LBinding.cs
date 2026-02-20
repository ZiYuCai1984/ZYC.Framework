using System.Windows.Data;
using ZYC.Framework.Core.Converters;

namespace ZYC.Framework.Core.Bindings;

public class LBinding : Binding
{
    public LBinding()
    {
        Initialize();
    }

    public LBinding(string path) : base(path)
    {
        Initialize();
    }

    private void Initialize()
    {
        Converter = new TranslatorWrapperConverter(Converter);
    }
}