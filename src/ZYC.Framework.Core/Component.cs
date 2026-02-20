using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autofac;
using Autofac.Core;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core.MarkupEx;

namespace ZYC.Framework.Core;

public class Component : UserControl, IDisposable
{
    public static readonly DependencyProperty LifetimeScopeProperty =
        DependencyProperty.Register(
            nameof(LifetimeScope),
            typeof(ILifetimeScope),
            typeof(Component),
            new PropertyMetadata(null, OnAnyResolveInputChanged));

    public static readonly DependencyProperty TypeProperty =
        DependencyProperty.Register(
            nameof(Type),
            typeof(Type),
            typeof(Component),
            new PropertyMetadata(null, OnAnyResolveInputChanged));

    public static readonly DependencyProperty ParameterProperty =
        DependencyProperty.Register(
            nameof(Parameter),
            typeof(object),
            typeof(Component),
            new PropertyMetadata(null, OnAnyResolveInputChanged));

    public static readonly DependencyProperty ParameterTypeProperty =
        DependencyProperty.Register(
            nameof(ParameterType),
            typeof(Type),
            typeof(Component),
            new PropertyMetadata(null, OnAnyResolveInputChanged));

    public static readonly DependencyProperty ParameterNameProperty =
        DependencyProperty.Register(
            nameof(ParameterName),
            typeof(string),
            typeof(Component),
            new PropertyMetadata(null, OnAnyResolveInputChanged));

    private bool _hasResolved;

    public Component()
    {
        SetBinding(LifetimeScopeProperty, new Binding(nameof(LifetimeScope))
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(IMainWindow), 1)
        });

        Loaded += OnLoaded;
        Parameters.CollectionChanged += OnParametersChanged;
    }

    public Type? Type
    {
        get => (Type?)GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    public ILifetimeScope? LifetimeScope
    {
        get => (ILifetimeScope?)GetValue(LifetimeScopeProperty);
        set => SetValue(LifetimeScopeProperty, value);
    }

    public ObservableCollection<AutofacCtorParam> Parameters { get; } = new();

    public object? Parameter
    {
        get => GetValue(ParameterProperty);
        set => SetValue(ParameterProperty, value);
    }

    public Type? ParameterType
    {
        get => (Type?)GetValue(ParameterTypeProperty);
        set => SetValue(ParameterTypeProperty, value);
    }

    public string? ParameterName
    {
        get => (string?)GetValue(ParameterNameProperty);
        set => SetValue(ParameterNameProperty, value);
    }

    public bool ResolveOnlyOnce { get; set; } = true;

    public void Dispose()
    {
        Loaded -= OnLoaded;
        Parameters.CollectionChanged -= OnParametersChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TryResolveContent();
    }

    private void OnParametersChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        TryResolveContent();
    }

    private static void OnAnyResolveInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Component)d).TryResolveContent();
    }

    private void TryResolveContent()
    {
        if (ResolveOnlyOnce && _hasResolved)
        {
            return;
        }

        if (!IsLoaded)
        {
            return;
        }

        var scope = LifetimeScope;
        var type = Type;

        if (scope is null || type is null)
        {
            return;
        }

        var ps = BuildAutofacParameters();

        Content = ps.Length == 0
            ? scope.Resolve(type)
            : scope.Resolve(type, ps);

        _hasResolved = true;
    }

    private Parameter[] BuildAutofacParameters()
    {
        var list = new List<Parameter>();

        if (!string.IsNullOrWhiteSpace(ParameterName))
        {
            list.Add(new NamedParameter(ParameterName!, Parameter));
        }
        else if (Parameter is not null || ParameterType is not null)
        {
            var t = ParameterType ?? Parameter!.GetType();
            list.Add(new TypedParameter(t, Parameter));
        }

        if (Parameters.Count > 0)
        {
            list.AddRange(Parameters.Select(p => p.ToAutofacParameter()));
        }

        return list.ToArray();
    }
}