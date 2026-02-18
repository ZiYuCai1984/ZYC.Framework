namespace ZYC.Automation.Abstractions.Tab;

public abstract class TabItemFactoryBase : ITabItemFactory
{
    private TabItemRouteAttribute[]? _routes;

    protected IReadOnlyList<TabItemRouteAttribute> Routes
        => _routes ??= GetType().GetCustomAttributes(typeof(TabItemRouteAttribute), false)
            .Cast<TabItemRouteAttribute>()
            .ToArray();

    public virtual bool IsSingle => true;

    public virtual int Priority => 0;

    public abstract Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context);

    public virtual Task<bool> CheckUriMatchedAsync(Uri uri)
    {
        return Task.FromResult(AttributeMightMatch(uri));
    }

    protected bool AttributeMightMatch(Uri uri)
    {
        return Routes.Any(r => r.MightMatch(uri));
    }
}