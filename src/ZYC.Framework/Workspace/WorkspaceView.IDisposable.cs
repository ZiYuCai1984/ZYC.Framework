using System.Diagnostics;
using System.Reactive.Disposables;
using ZYC.CoreToolkit;

namespace ZYC.Framework.Workspace;

internal partial class WorkspaceView : IDisposable
{
    private bool Disposing { get; set; }

    private CompositeDisposable CompositeDisposable { get; } = new();

    public void Dispose()
    {
        if (Disposing)
        {
            Debugger.Break();
            return;
        }

        Disposing = true;

        CompositeDisposable.Dispose();
        Border.Child?.TryDispose();
    }
}