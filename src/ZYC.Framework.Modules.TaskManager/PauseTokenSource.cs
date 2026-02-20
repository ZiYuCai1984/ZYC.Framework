using ZYC.Framework.Core;
using ZYC.Framework.Modules.TaskManager.Abstractions;

namespace ZYC.Framework.Modules.TaskManager;

internal sealed class PauseTokenSource : IPauseTokenSource
{
    private readonly AsyncManualResetEvent _gate = new();
    private volatile bool _paused;

    public IPauseToken Token => new PauseToken(this);

    public void Pause()
    {
        _paused = true;
        _gate.Reset();
    }

    public void Resume()
    {
        _paused = false;
        _gate.Set();
    }

    private sealed class PauseToken : IPauseToken
    {
        private readonly PauseTokenSource _src;

        public PauseToken(PauseTokenSource src)
        {
            _src = src;
        }

        public bool IsPaused => _src._paused;

        public Task WaitIfPausedAsync(CancellationToken ct)
        {
            if (!_src._paused)
            {
                return Task.CompletedTask;
            }

            return _src._gate.WaitAsync(ct);
        }
    }
}