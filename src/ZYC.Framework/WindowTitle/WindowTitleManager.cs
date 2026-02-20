using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.WindowTitle;
using ZYC.Framework.WindowTitle.BuildIn;

namespace ZYC.Framework.WindowTitle;

[RegisterSingleInstanceAs(typeof(IWindowTitleManager))]
internal class WindowTitleManager : IWindowTitleManager
{
    public WindowTitleManager(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;

        RegisterItem<RestartTitleItem>();
        RegisterItem<OpenAppFolderInExplorerTitleItem>();
        RegisterItem<AttachDebuggerTitleItem>();
        RegisterItem<SetTopmostTitleItem>();
        RegisterItem<MinimizeTitleItem>();
        RegisterItem<FullScreenTitleItem>();
        RegisterItem<SetPreventExitTitleItem>();
        RegisterItem<ExitProcessTitleItem>();
    }

    private ILifetimeScope LifetimeScope { get; }

    private IList<IWindowTitleItem> Items { get; } = new List<IWindowTitleItem>();

    public void RegisterItem(IWindowTitleItem item)
    {
        Items.Add(item);
    }

    public void RegisterItem<T>() where T : IWindowTitleItem
    {
        var item = LifetimeScope.Resolve<T>();
        Items.Add(item);
    }

    public IWindowTitleItem[] GetItems()
    {
        return Items.ToArray();
    }
}