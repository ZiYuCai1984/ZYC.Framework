using System.Windows.Data;

namespace ZYC.Framework.Core.Bindings;

public class SelfBinding : Binding
{
    public SelfBinding()
    {
        RelativeSource = new RelativeSource(RelativeSourceMode.Self);
    }
}