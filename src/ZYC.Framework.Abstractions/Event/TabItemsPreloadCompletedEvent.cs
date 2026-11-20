namespace ZYC.Framework.Abstractions.Event;

/// <summary>
///     Event raised once during startup, after <see cref="TabManagerRestoreCompleted" />,
///     when every restored tab item has been selected once so its view is created and laid out.
///     Subscribe to it to run work that needs the restored tab views to exist.
/// </summary>
public sealed class TabItemsPreloadCompletedEvent
{
}