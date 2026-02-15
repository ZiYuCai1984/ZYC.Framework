using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using ZYC.Automation.Abstractions.Tab;

namespace ZYC.Automation.Modules.Gemini;

public sealed class TabInstanceRegistry
{
    private sealed class Box { public Guid Id { get; init; } }

    private readonly ConditionalWeakTable<ITabItemInstance, Box> _idByInstance = new();
    private readonly ConcurrentDictionary<Guid, WeakReference<ITabItemInstance>> _instanceById = new();

    public Guid GetOrCreateId(ITabItemInstance instance)
    {
        var box = _idByInstance.GetValue(instance, _ => new Box { Id = Guid.NewGuid() });
        _instanceById.TryAdd(box.Id, new WeakReference<ITabItemInstance>(instance));
        return box.Id;
    }

    public bool TryResolve(Guid id, out ITabItemInstance? instance)
    {
        instance = null;
        if (!_instanceById.TryGetValue(id, out var wr)) return false;
        if (!wr.TryGetTarget(out var target)) return false;
        instance = target;
        return true;
    }

    public void Forget(Guid id) => _instanceById.TryRemove(id, out _);
}