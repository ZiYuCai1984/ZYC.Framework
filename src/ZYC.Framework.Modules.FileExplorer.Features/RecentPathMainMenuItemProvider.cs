using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Core.Menu;
using ZYC.Framework.Modules.FileExplorer.Abstractions;

namespace ZYC.Framework.Modules.FileExplorer.Features;

[RegisterSingleInstanceAs(typeof(RecentPathMainMenuItemsProvider), typeof(IRecentPathMainMenuItemsProvider))]
internal class RecentPathMainMenuItemsProvider : MainMenuItemsProvider, IRecentPathMainMenuItemsProvider,
    INotifyPropertyChanged
{
    private readonly IList<IMainMenuItem> _subItems = new List<IMainMenuItem>();

    public RecentPathMainMenuItemsProvider(
        ILifetimeScope lifetimeScope,
        RecentPathConfig recentPathConfig,
        RecentPathState recentPathState) :
        base(lifetimeScope)
    {
        RecentPathConfig = recentPathConfig;
        RecentPathState = recentPathState;

        Info = new MenuItemInfo
        {
            Title = "Recent Path"
        };


        recentPathState.ObserveProperty(nameof(RecentPathState.Paths))
            .Subscribe(_ =>
            {
                _subItems.Clear();

                for (var i = 0; i < RecentPathState.Paths.Length; ++i)
                {
                    _subItems.Add(
                        lifetimeScope.Resolve<RecentPathMainMenuItem>(
                            new TypedParameter(typeof(int), i),
                            new TypedParameter(typeof(string), RecentPathState.Paths[i])));
                }

                OnPropertyChanged(nameof(SubItems));
            });

        recentPathConfig.ObserveProperty(nameof(RecentPathConfig.MaxCount))
            .Subscribe(_ =>
            {
                var max = RecentPathConfig.MaxCount;
                if (max < 0)
                {
                    max = 0;
                }

                var current = RecentPathState.Paths;

                if (current.Length <= max)
                {
                    return;
                }


                if (max == 0)
                {
                    RecentPathState.Paths = [];
                    return;
                }

                var trimmed = new string[max];
                Array.Copy(current, trimmed, max);
                RecentPathState.Paths = trimmed;
            });
    }


    private RecentPathConfig RecentPathConfig { get; }

    private RecentPathState RecentPathState { get; }

    public override MenuItemInfo Info { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void AddRecentPath(string path)
    {
        var paths = new List<string>(RecentPathState.Paths);
        if (paths.Contains(path))
        {
            paths.Remove(path);
        }

        paths.Insert(0, path);
        if (paths.Count > RecentPathConfig.MaxCount && paths.Count > 0)
        {
            paths.RemoveAt(paths.Count - 1);
        }

        RecentPathState.Paths = paths.ToArray();
    }

    public string[] GetRecentPaths()
    {
        return RecentPathState.Paths.ToArray();
    }

    public override IMainMenuItem[] SubItems => _subItems.ToArray();


    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}