using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Runtime.CompilerServices;
using System.Windows;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Modules.Accounts.Abstractions;
using ZYC.Framework.Modules.Accounts.Abstractions.Event;
using ZYC.Framework.Modules.Accounts.Commands;

namespace ZYC.Framework.Modules.Accounts.UI;

[RegisterSingleInstance]
internal partial class AccountsWindowTitleItemView : IDisposable, INotifyPropertyChanged
{
    public AccountsWindowTitleItemView(
        SignInCommand signInCommand,
        SignOutCommand signOutCommand,
        IAccountManager accountManager,
        IEventAggregator eventAggregator,
        IToastManager toastManager)
    {
        SignInCommand = signInCommand;
        SignOutCommand = signOutCommand;
        AccountManager = accountManager;
        ToastManager = toastManager;

        InitializeComponent();

        eventAggregator.Subscribe<AccountSessionChangedEvent>(_ => Refresh(), true)
            .DisposeWith(CompositeDisposable);

        Refresh();
    }

    private CompositeDisposable CompositeDisposable { get; } = new();

    private SignInCommand SignInCommand { get; }

    private SignOutCommand SignOutCommand { get; }

    private IAccountManager AccountManager { get; }

    private IToastManager ToastManager { get; }

    public AccountProviderDescriptor[] Providers => AccountManager.GetProviders();

    public AccountSession? CurrentSession => AccountManager.GetCurrentSession();

    public bool IsSignedIn => CurrentSession != null;

    public string Icon
    {
        get
        {
            if (!IsSignedIn)
            {
                return "AccountCircleOutline";
            }

            if (CurrentSession?.Profile.AvatarUri != null)
            {
                return CurrentSession?.Profile.AvatarUri.ToString()!;
            }

            return "AccountCheckOutline";
        }
    }

    public string DisplayText
    {
        get
        {
            var profile = CurrentSession?.Profile;
            if (profile == null)
            {
                return "Accounts";
            }

            if (!string.IsNullOrWhiteSpace(profile.DisplayName))
            {
                return profile.DisplayName;
            }

            return profile.UserName ?? "Accounts";
        }
    }


    public IMainMenuItem[] AccountMenuItems { get; set; } = [];

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        ////TODO-zyc OnButtonClick
        //if (sender is not Button button)
        //{
        //    return;
        //}

        //var contextMenu = new ContextMenu();
        //var signInMenuItem = new MenuItem { Header = "Sign in" };
        //foreach (var provider in Providers)
        //{
        //    signInMenuItem.Items.Add(
        //        new MenuItem
        //        {
        //            Header = provider.DisplayName,
        //            Command = SignInCommand,
        //            CommandParameter = provider.Id,
        //            IsEnabled = provider.IsEnabled
        //        });
        //}

        //contextMenu.Items.Add(signInMenuItem);
        //contextMenu.Items.Add(new Separator());
        //contextMenu.Items.Add(
        //    new MenuItem
        //    {
        //        Header = "Sign out",
        //        Command = SignOutCommand,
        //        IsEnabled = IsSignedIn
        //    });

        //button.ContextMenu = contextMenu;
        //contextMenu.PlacementTarget = button;
        //contextMenu.IsOpen = true;
    }

    //private void SignInAsync(string providerId)
    //{
    //    _ = ExecuteSignInAsync(providerId);
    //}

    //private async Task ExecuteSignInAsync(string providerId)
    //{
    //    try
    //    {
    //        await AccountManager.SignInAsync(providerId, CancellationToken.None);
    //        Refresh();
    //    }
    //    catch (Exception ex)
    //    {
    //        ToastManager.PromptException(ex);
    //    }
    //}

    //private void SignOutAsync()
    //{
    //    _ = ExecuteSignOutAsync();
    //}

    //private async Task ExecuteSignOutAsync()
    //{
    //    var session = CurrentSession;
    //    if (session == null)
    //    {
    //        return;
    //    }

    //    try
    //    {
    //        await AccountManager.SignOutAsync(session.Profile.ProviderId, CancellationToken.None);
    //        Refresh();
    //    }
    //    catch (Exception ex)
    //    {
    //        ToastManager.PromptException(ex);
    //    }
    //}

    private void Refresh()
    {
        var mainMenuItems = new List<IMainMenuItem>();
        foreach (var provider in Providers)
        {
            mainMenuItems.Add(new AccountsMenuItem(
                $"Sign in with {provider.DisplayName}",
                provider.Icon,
                SignInCommand,
                provider.Id));
        }


        var currentSession = AccountManager.GetCurrentSession();
        var currentProviderId = currentSession?.Profile.ProviderId;


        mainMenuItems.Add(new AccountsMenuItem(
            "Sign out",
            null,
            SignOutCommand,
            currentProviderId));


        AccountMenuItems = mainMenuItems.ToArray();


        OnPropertyChanged(nameof(AccountMenuItems));
        OnPropertyChanged(nameof(CurrentSession));
        OnPropertyChanged(nameof(IsSignedIn));
        OnPropertyChanged(nameof(Icon));
        OnPropertyChanged(nameof(DisplayText));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}