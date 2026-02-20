using System.Diagnostics;
using ZYC.CoreToolkit;

namespace ZYC.Framework.Core;

public partial class SplitView : IDisposable
{
    protected bool Disposing { get; set; }

    public void Dispose()
    {
        if (Disposing)
        {
            Debugger.Break();
            return;
        }

        Disposing = true;

        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;

        LeftContent?.TryDispose();
        RightContent?.TryDispose();
    }
}