using Autofac;
using ZYC.Framework.Abstractions.QuickBar;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Tab.BuildIn;

namespace ZYC.Framework.Tab;

internal partial class TabItemFactoryManager : ISimpleTabItemFactoryManager
{
    public void Register(SimpleTabItemFactoryInfo info)
    {
        var factory = LifetimeScope.Resolve<SimpleTabItemFactory>(
            new TypedParameter(typeof(SimpleTabItemFactoryInfo), info));
        TabItemFactories.Add(factory);


        if (info.AddQuickBarItem)
        {
            var navigateCommand = LifetimeScope.CreateNavigateCommand(info.TabItemInfo.Uri);

            var simpleQuickBarItemsProvider = LifetimeScope.Resolve<ISimpleQuickBarItemsProvider>();
            simpleQuickBarItemsProvider.AttachItem(
                new SimpleQuickBarItem(
                    info.TabItemInfo.Uri, info.TabItemInfo.Icon,
                    navigateCommand, info.TabItemInfo.Title));
        }
    }
}