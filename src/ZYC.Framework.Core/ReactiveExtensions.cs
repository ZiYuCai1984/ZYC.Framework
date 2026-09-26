using System.ComponentModel;
using System.Diagnostics;
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
            //!WARNING // Retrieve directly from the Sender provided by EventPattern to avoid closure capturing of external variables.
            .Select(e => (T)e.Sender!);
    }

    public static IObservable<T> ObserveProperty<T>(this T persistedData, string propertyName)
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
            .Select(e => (T)e.Sender!);
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
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (context == null)
        {
            DebuggerTools.Break();
        }

        UISynchronizationContext = context;
    }
}