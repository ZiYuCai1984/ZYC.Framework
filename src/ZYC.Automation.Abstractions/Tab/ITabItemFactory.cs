namespace ZYC.Automation.Abstractions.Tab;

public interface ITabItemFactory
{
    bool IsSingle { get; }

    int Priority { get; }

    Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context);

    Task<bool> CheckUriMatchedAsync(Uri uri);
}