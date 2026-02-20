using System.ComponentModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Core;

// ReSharper disable SuspiciousTypeConversion.Global
public static class ReactiveExtensions
{
    public static SynchronizationContext? UISynchronizationContext { get; private set; }

    public static IObservable<T> ObserveAnyChange<T>(this T persistedData) where T : IPersistedData
    {
        if (persistedData is not INotifyPropertyChanged t)
        {
            throw new InvalidOperationException();
        }

        return Observable
            .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => t.PropertyChanged += h,
                h => t.PropertyChanged -= h)
            .Select(_ => persistedData);
    }

    public static IObservable<Unit> ObserveProperty<T>(this T persistedData, string propertyName)
    {
        if (persistedData is not INotifyPropertyChanged t)
        {
            throw new InvalidOperationException();
        }

        return Observable
            .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => t.PropertyChanged += h,
                h => t.PropertyChanged -= h)
            .Where(e => string.Equals(e.EventArgs.PropertyName, propertyName, StringComparison.Ordinal))
            .Select(_ => Unit.Default);
    }

    public static IObservable<TSource> ObserveOnUI<TSource>(this IObservable<TSource> source)
    {
        if (UISynchronizationContext == null)
        {
            DebuggerTools.Break();
        }

        Debug.Assert(UISynchronizationContext != null);
        return source.ObserveOn(UISynchronizationContext);
    }

    internal static void SetSynchronizationContext(SynchronizationContext context)
    {
        if (context == null)
        {
            DebuggerTools.Break();
        }

        UISynchronizationContext = context;
    }
}