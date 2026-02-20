using System.Collections.Concurrent;
using System.Reflection;
using Autofac;
using ZYC.CoreToolkit;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Core.Tab;

public abstract class TabItemInstanceBase<T> : TabItemInstanceBase where T : notnull
{
    protected TabItemInstanceBase(ILifetimeScope lifetimeScope, TabReference tabReference)
        : base(lifetimeScope, tabReference)
    {
    }

    public override object View => _view ??= LifetimeScope.Resolve<T>();
}

public abstract class TabItemInstanceBase : ITabItemInstance
{
    private static readonly ConcurrentDictionary<(Type type, string memberName), object?> ConstantsCache = new();

    // ReSharper disable once InconsistentNaming
    protected object? _view;

    protected TabItemInstanceBase(ILifetimeScope lifetimeScope, TabReference tabReference)
    {
        LifetimeScope = lifetimeScope;
        TabReference = tabReference;
    }

    protected ILifetimeScope LifetimeScope { get; }

    public virtual string Host => GetConstant<string>(nameof(Host));

    public TabReference TabReference { get; }

    public virtual string Title => GetConstant<string>(nameof(Title));

    public virtual string Icon => GetConstant<string>(nameof(Icon));

    public abstract object View { get; }

    public virtual bool Localization => true;


    public virtual Task LoadAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     OnClosing
    ///     true : Allow close
    ///     false : Prevent close
    /// </summary>
    public virtual bool OnClosing()
    {
        return true;
    }

    public virtual void Dispose()
    {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        View?.TryDispose();
    }

    protected T GetConstant<T>(string memberName)
    {
        var runtimeType = GetType();
        var key = (runtimeType, memberName);

        var value = ConstantsCache.GetOrAdd(key, static k =>
        {
            var (type, memberName) = k;

            var src = type.GetCustomAttribute<ConstantsSourceAttribute>(false);
            if (src is not null)
            {
                return GetRequiredStaticConstantValue(src.Type, memberName);
            }

#pragma warning disable CS0618
            return GetConstantLegacy(type, memberName);
#pragma warning restore CS0618
        });

        if (value is null)
        {
            throw new InvalidOperationException(
                $"Constant <{memberName}> resolved to null in <{runtimeType.FullName}>.");
        }

        if (value is T t)
        {
            return t;
        }

        throw new InvalidOperationException(
            $"Constant <{memberName}> type mismatch. Expected <{typeof(T).FullName}>, but got <{value.GetType().FullName}>.");
    }


    [Obsolete(
        "Use GetConstant(...) with [ConstantsSource] support. This legacy method resolves only <ThisType>.Constants.*",
        false)]
    protected static object? GetConstantLegacy(Type type, string propertyName)
    {
        var nestedTypeName = "Constants";

        var constantsType = type.GetNestedType(nestedTypeName, BindingFlags.Public | BindingFlags.NonPublic);
        if (constantsType is null)
        {
            throw new InvalidOperationException($"<{nestedTypeName}> must be defined in <{type.Name}>");
        }

        var property = constantsType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
        if (property is null)
        {
            throw new InvalidOperationException($"<{propertyName}> static property missing in <{nestedTypeName}>");
        }

        if (property.GetIndexParameters().Length != 0)
        {
            throw new InvalidOperationException($"<{constantsType.FullName}.{propertyName}> must not be an indexer.");
        }

        return property.GetValue(null);
    }


    private static object? GetRequiredStaticConstantValue(Type constantsType, string memberName)
    {
        // ReSharper disable once InconsistentNaming
        const BindingFlags Flags =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static | BindingFlags.FlattenHierarchy;

        // 1) property
        var prop = constantsType.GetProperty(memberName, Flags);
        if (prop is not null)
        {
            if (prop.GetIndexParameters().Length != 0)
            {
                throw new InvalidOperationException($"<{constantsType.FullName}.{memberName}> must not be an indexer.");
            }

            return prop.GetValue(null);
        }

        // 2) field（const / static readonly / static）
        var field = constantsType.GetField(memberName, Flags);
        if (field is not null)
        {
            if (!field.IsStatic)
            {
                throw new InvalidOperationException($"<{constantsType.FullName}.{memberName}> must be static.");
            }

            if (field.IsLiteral && !field.IsInitOnly)
            {
                return field.GetRawConstantValue();
            }

            return field.GetValue(null);
        }

        throw new InvalidOperationException(
            $"<{memberName}> static field/property missing in <{constantsType.FullName}>");
    }
}