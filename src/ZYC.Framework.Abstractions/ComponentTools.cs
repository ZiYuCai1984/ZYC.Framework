using System.Collections.Concurrent;
using System.Reflection;

namespace ZYC.Framework.Abstractions;

/// <summary>
///     Provides helper methods for component initialization.
/// </summary>
public static class ComponentTools
{
    private static ConcurrentDictionary<Type, MethodInfo?> InitializeComponentMethodCache { get; } = new();


    /// <summary>
    ///     Attempts to invoke the InitializeComponent method on an instance.
    /// </summary>
    /// <typeparam name="T">The instance type.</typeparam>
    /// <param name="instance">The instance to initialize.</param>
    public static void TryCallInitializeComponent<T>(T instance)
    {
        var type = instance!.GetType();

        if (!InitializeComponentMethodCache.TryGetValue(type, out var method))
        {
            method = type.GetMethod("InitializeComponent",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            InitializeComponentMethodCache.TryAdd(type, method);
        }

        method?.Invoke(instance, null);
    }
}