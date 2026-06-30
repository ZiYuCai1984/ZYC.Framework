using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.Accounts.Abstractions;
using ZYC.Framework.Modules.Accounts.Abstractions.Event;

namespace ZYC.Framework.Modules.Accounts.UI;

[RegisterSingleInstance]
internal partial class AccountsStatusBarItemView : IDisposable, INotifyPropertyChanged
{
    public AccountsStatusBarItemView(
        IAccountManager accountManager,
        IEventAggregator eventAggregator,
        IToastManager toastManager)
    {
        AccountManager = accountManager;
        ToastManager = toastManager;
        SignInCommand = new RelayCommand(_ => true, p => SignInAsync((string)p!));
        SignOutCommand = new RelayCommand(_ => IsSignedIn, _ => SignOutAsync());

        InitializeComponent();

        EventSubscription = eventAggregator.Subscribe<AccountSessionChangedEvent>(_ => Refresh(), true);
    }

    private IDisposable EventSubscription { get; }

    private IAccountManager AccountManager { get; }

    private IToastManager ToastManager { get; }

    public ICommand SignInCommand { get; }

    public ICommand SignOutCommand { get; }

    public AccountProviderDescriptor[] Providers => AccountManager.GetProviders();

    public AccountSession? CurrentSession => AccountManager.GetCurrentSession();

    public bool IsSignedIn => CurrentSession != null;

    public string Icon => IsSignedIn ? "AccountCheckOutline" : "AccountCircleOutline";

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

    public void Dispose()
    {
        EventSubscription.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        var contextMenu = new ContextMenu();
        var signInMenuItem = new MenuItem { Header = "Sign in" };
        foreach (var provider in Providers)
        {
            signInMenuItem.Items.Add(
                new MenuItem
                {
                    Header = provider.DisplayName,
                    Command = SignInCommand,
                    CommandParameter = provider.Id,
                    IsEnabled = provider.IsEnabled
                });
        }

        contextMenu.Items.Add(signInMenuItem);
        contextMenu.Items.Add(new Separator());
        contextMenu.Items.Add(
            new MenuItem
            {
                Header = "Sign out",
                Command = SignOutCommand,
                IsEnabled = IsSignedIn
            });

        button.ContextMenu = contextMenu;
        contextMenu.PlacementTarget = button;
        contextMenu.IsOpen = true;
    }

    private void SignInAsync(string providerId)
    {
        _ = ExecuteSignInAsync(providerId);
    }

    private async Task ExecuteSignInAsync(string providerId)
    {
        try
        {
            await AccountManager.SignInAsync(providerId, CancellationToken.None);
            Refresh();
        }
        catch (Exception ex)
        {
            ToastManager.PromptException(ex);
        }
    }

    private void SignOutAsync()
    {
        _ = ExecuteSignOutAsync();
    }

    private async Task ExecuteSignOutAsync()
    {
        var session = CurrentSession;
        if (session == null)
        {
            return;
        }

        try
        {
            await AccountManager.SignOutAsync(session.Profile.ProviderId, CancellationToken.None);
            Refresh();
        }
        catch (Exception ex)
        {
            ToastManager.PromptException(ex);
        }
    }

    private void Refresh()
    {
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
