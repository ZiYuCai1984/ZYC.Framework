using System.Reactive.Disposables;

namespace ZYC.Framework.Core;

public static class EventDisposables
{
    public static IDisposable Hook(Action add, Action remove)
    {
        add();
        return Disposable.Create(remove);
    }

    public static IDisposable Hook<TDelegate>(
        Action<TDelegate> add,
        Action<TDelegate> remove,
        TDelegate handler)
        where TDelegate : Delegate
        => Hook(() => add(handler), () => remove(handler));
}