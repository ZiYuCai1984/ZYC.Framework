using System.Diagnostics;
using System.Reactive.Disposables;
using Autofac;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Commands;

namespace ZYC.Framework.Tab;

internal partial class TabManagerView : IDisposable
{
    private CompositeDisposable CompositeDisposable { get; }=new();

    public StarCommand StarCommand => _starCommand ??= LifetimeScope.Resolve<StarCommand>(
        new TypedParameter(typeof(WorkspaceNode), WorkspaceNode),
        new TypedParameter(typeof(TabManagerView), this));


    private bool Disposing { get; set; }

    public void Dispose()
    {
        if (Disposing)
        {
            Debugger.Break();
            return;
        }

        Disposing = true;

        StarCommand.Dispose();
        CompositeDisposable.Dispose();

        DisposeTabManagerViewAsyncFunc.Invoke(WorkspaceNode);
    }
}