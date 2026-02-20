using System.Windows;

namespace ZYC.Framework.Core.Bindings;

public sealed class BindingProxy : Freezable
{
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
        nameof(Data), typeof(object), typeof(BindingProxy), new PropertyMetadata(default(object)));

    public object Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    protected override Freezable CreateInstanceCore()
    {
        return new BindingProxy();
    }

    public override string ToString()
    {
        return Data is FrameworkElement fe
            ? $"Binding Proxy: {fe.Name}"
            : $"Binding Proxy: {Data?.GetType().FullName}";
    }
}