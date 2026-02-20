namespace ZYC.Framework.Abstractions;

/// <summary>
///     Provides extension methods for <see cref="IViewSetter{T}" />.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IViewSetterEx
{
    /// <summary>
    ///     Sets the current view instance.
    /// </summary>
    /// <typeparam name="T">The view type.</typeparam>
    /// <param name="viewSetter">The view setter to update.</param>
    /// <param name="view">The view instance to store.</param>
    public static void SetView<T>(this IViewSetter<T> viewSetter, T? view)
    {
        viewSetter.SetView(view);
    }
}