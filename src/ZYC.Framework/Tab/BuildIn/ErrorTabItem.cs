using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Tab.BuildIn;

/// <summary>
///     !WARNING To keep the structure simple and less prone to errors, it does not inherit from TabItemInstanceBase
/// </summary>
[Register]
internal class ErrorTabItem : ITabItemInstance
{
    private object? _view;

    public ErrorTabItem(TabReference tabReference, Exception exception, ILifetimeScope lifetimeScope)
    {
        TabReference = tabReference;
        Exception = exception;
        LifetimeScope = lifetimeScope;
    }

    private Exception Exception { get; }

    private ILifetimeScope LifetimeScope { get; }


    public object View => _view ??= LifetimeScope.Resolve<ErrorView>(
        new TypedParameter(typeof(Exception), Exception));

    public void Dispose()
    {
        //ignore
    }

    public TabReference TabReference { get; }


    public string Title => "Error";

    public string Icon => "BugOutline";

    public bool Localization => true;

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }

    public bool OnClosing()
    {
        return true;
    }
}