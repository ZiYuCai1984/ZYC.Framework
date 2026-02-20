using System.Diagnostics;

namespace ZYC.Framework.Abstractions;

/// <summary>
///     Defines a helper that owns a view instance and controls its lifecycle.
/// </summary>
/// <typeparam name="T">The view type.</typeparam>
public interface IViewSetter<T> : IDisposable
{
    /// <summary>
    ///     Gets or sets the current view instance.
    /// </summary>
    T? View { get; protected set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the view setter is disposing.
    /// </summary>
    bool Disposing { get; protected set; }

    /// <summary>
    ///     Disposes the view setter and clears the view reference.
    /// </summary>
    void IDisposable.Dispose()
    {
        if (Disposing)
        {
            Debugger.Break();
            return;
        }

        Disposing = true;

        SetView(default);
    }

    /// <summary>
    ///     Sets the current view instance.
    /// </summary>
    /// <param name="view">The view instance to store.</param>
    void SetView(T? view)
    {
        View = view;
    }
}