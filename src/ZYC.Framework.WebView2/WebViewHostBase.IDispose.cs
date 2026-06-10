using System.Diagnostics;

namespace ZYC.Framework.WebView2;

public partial class WebViewHostBase
{
    protected bool Disposing { get; private set; }

    public virtual void Dispose()
    {
        if (Disposing)
        {
            Debugger.Break();
            return;
        }

        Disposing = true;

        InternalDispose();

        CompositeDisposable.Dispose();

        //!WARNING WebView2.Dispose() releases the WebView2 control/runtime resources only.
        // Browser-extension native messaging hosts, for example Codex extension-host.exe,
        // are external processes started by the extension/native-messaging layer and may
        // remain alive after the WebView2 browser process exits if the host does not stop
        // itself when its native messaging pipe is closed.
        WebView2.Dispose();
        InnerHttpClient.Dispose();
    }

    protected virtual void InternalDispose()
    {
    }
}