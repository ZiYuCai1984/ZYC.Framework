using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Abstractions.Notification.Banner;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Core.Menu;

namespace ZYC.Framework.MainMenu.BuildIn;

[RegisterSingleInstance]
internal class SwitchVersionMainMenuItem : MainMenuItemsProvider
{
    private static RelayCommand DisabledCommand { get; } = new(_ => false, _ => { });

    public SwitchVersionMainMenuItem(
        ILifetimeScope lifetimeScope,
        IAppContext appContext,
        IProduct product,
        AppState appState,
        IBannerManager bannerManager,
        IAppLogger<SwitchVersionMainMenuItem> logger) : base(lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = "Switch Version",
            Icon = null,
            Anchor = AboutMainMenuAnchors.Update,
            Priority = -10
        };

        try
        {
            var versions = EnumerateInstalledVersions(appContext).ToArray();
            if (versions.Length == 0)
            {
                RegisterSubItem(CreateDisabledItem("No installed versions found"));
                return;
            }

            for (var i = 0; i < versions.Length; i++)
            {
                var item = new SwitchVersionOptionMainMenuItem(
                    versions[i],
                    i,
                    appContext,
                    product,
                    appState,
                    bannerManager,
                    NotifyVersionStateChanged);

                VersionItems.Add(item);
                RegisterSubItem(item);
            }
        }
        catch (Exception e)
        {
            logger.Error(e);
            RegisterSubItem(CreateDisabledItem("Failed to enumerate installed versions"));
        }
    }

    private IList<SwitchVersionOptionMainMenuItem> VersionItems { get; } = new List<SwitchVersionOptionMainMenuItem>();

    public override MenuItemInfo Info { get; }

    private static IEnumerable<string> EnumerateInstalledVersions(IAppContext appContext)
    {
        var appRootDirectory = appContext.GetAppRootDirectory();
        var processFileName = appContext.GetProcessFileName();

        return Directory.GetDirectories(appRootDirectory)
            .Where(directory => File.Exists(Path.Combine(directory, processFileName)))
            .Select(Path.GetFileName)
            .Where(version => !string.IsNullOrWhiteSpace(version))
            .Cast<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(version => version, InstalledVersionComparer.Instance);
    }

    private static MainMenuItem CreateDisabledItem(string title)
    {
        return new MainMenuItem(
            title,
            null,
            DisabledCommand,
            localization: false);
    }

    private void NotifyVersionStateChanged()
    {
        foreach (var item in VersionItems)
        {
            item.NotifyVersionStateChanged();
        }
    }
}

internal class SwitchVersionOptionMainMenuItem : MainMenuItem, INotifyPropertyChanged
{
    public SwitchVersionOptionMainMenuItem(
        string targetVersion,
        int priority,
        IAppContext appContext,
        IProduct product,
        AppState appState,
        IBannerManager bannerManager,
        Action refreshVersionState)
    {
        TargetVersion = targetVersion;
        AppContext = appContext;
        Product = product;
        AppState = appState;
        BannerManager = bannerManager;
        RefreshVersionState = refreshVersionState;

        Info = new MenuItemInfo
        {
            Title = targetVersion,
            Icon = null,
            Localization = false,
            Priority = priority
        };

        Command = new RelayCommand(
            _ => !IsStartupVersion,
            _ =>
            {
                AppContext.UpdateStartupVersion(TargetVersion);
                BannerManager.PromptRestart();
                RefreshVersionState();
            });
    }

    private IAppContext AppContext { get; }

    private IProduct Product { get; }

    private AppState AppState { get; }

    private IBannerManager BannerManager { get; }

    private Action RefreshVersionState { get; }

    private string TargetVersion { get; }

    private bool IsRunningVersion =>
        string.Equals(Product.Version, TargetVersion, StringComparison.OrdinalIgnoreCase);

    private bool IsStartupVersion =>
        string.Equals(AppState.StartupVersion, TargetVersion, StringComparison.OrdinalIgnoreCase);

    public override string Title
    {
        get
        {
            if (IsRunningVersion)
            {
                return $"{TargetVersion} (Current)";
            }

            return TargetVersion;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void NotifyVersionStateChanged()
    {
        OnPropertyChanged(nameof(Title));
        CommandManager.InvalidateRequerySuggested();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

internal sealed class InstalledVersionComparer : IComparer<string>
{
    public static InstalledVersionComparer Instance { get; } = new();

    public int Compare(string? x, string? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (x == null)
        {
            return -1;
        }

        if (y == null)
        {
            return 1;
        }

        var xCore = GetCoreVersion(x);
        var yCore = GetCoreVersion(y);
        if (Version.TryParse(xCore, out var xVersion) &&
            Version.TryParse(yCore, out var yVersion))
        {
            var compareResult = xVersion.CompareTo(yVersion);
            if (compareResult != 0)
            {
                return compareResult;
            }

            var xIsPrerelease = !string.Equals(x, xCore, StringComparison.OrdinalIgnoreCase);
            var yIsPrerelease = !string.Equals(y, yCore, StringComparison.OrdinalIgnoreCase);
            if (xIsPrerelease != yIsPrerelease)
            {
                return xIsPrerelease ? -1 : 1;
            }
        }

        return StringComparer.OrdinalIgnoreCase.Compare(x, y);
    }

    private static string GetCoreVersion(string version)
    {
        var separatorIndex = version.IndexOf('-');
        if (separatorIndex < 0)
        {
            return version;
        }

        return version[..separatorIndex];
    }
}
