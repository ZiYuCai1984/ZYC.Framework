using System.Windows.Data;

namespace ZYC.Framework.Core.Bindings;

/// <summary>
///     !WARNING If you use internal here, the static analysis of Visual Studio will report an error (although the
///     compilation is normal)
/// </summary>
public class SelfBinding : Binding
{
    public SelfBinding()
    {
        RelativeSource = new RelativeSource(RelativeSourceMode.Self);
    }
}