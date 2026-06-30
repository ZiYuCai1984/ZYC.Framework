using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.Accounts.Abstractions;
using ZYC.Framework.Modules.Accounts.Abstractions.Event;

namespace ZYC.Framework.Modules.Accounts;

[RegisterSingleInstanceAs(typeof(IAccountManager))]
internal class AccountsManager : IAccountManager
{
    public AccountsManager(
        IAccountProvider[] providers,
        AccountsState accountsState,
        IEventAggregator eventAggregator)
    {
        Providers = providers;
        AccountsState = accountsState;
        EventAggregator = eventAggregator;
    }

    private IAccountProvider[] Providers { get; }

    private AccountsState AccountsState { get; }

    private IEventAggregator EventAggregator { get; }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var sessions = new List<AccountSession>();

        foreach (var provider in Providers)
        {
            var session = await provider.GetCachedSessionAsync(cancellationToken);
            if (session != null)
            {
                sessions.Add(session);
            }
        }

        if (sessions.Count == 0)
        {
            return;
        }

        AccountsState.Sessions = sessions.ToArray();
        if (string.IsNullOrWhiteSpace(AccountsState.ActiveProviderId)
            || AccountsState.Sessions.All(t => t.Profile.ProviderId != AccountsState.ActiveProviderId))
        {
            AccountsState.ActiveProviderId = AccountsState.Sessions[0].Profile.ProviderId;
        }

        EventAggregator.Publish(new AccountSessionChangedEvent(GetCurrentSession()));
    }

    public AccountProviderDescriptor[] GetProviders()
    {
        return Providers
            .Select(t => t.Descriptor)
            .OrderBy(t => t.DisplayName)
            .ToArray();
    }

    public AccountSession? GetCurrentSession()
    {
        var activeProviderId = AccountsState.ActiveProviderId;
        if (string.IsNullOrWhiteSpace(activeProviderId))
        {
            return null;
        }

        return GetSession(activeProviderId);
    }

    public AccountSession? GetSession(string providerId)
    {
        return AccountsState.Sessions.FirstOrDefault(t =>
            string.Equals(t.Profile.ProviderId, providerId, StringComparison.OrdinalIgnoreCase));
    }

    public Task<AccountSession?> GetSessionAsync(string providerId, CancellationToken cancellationToken)
    {
        return Task.FromResult(GetSession(providerId));
    }

    public async Task<AccountSession> SignInAsync(string providerId, CancellationToken cancellationToken)
    {
        var provider = GetProvider(providerId);
        var session = await provider.SignInAsync(
            new AccountLoginRequest
            {
                ProviderId = providerId,
                ForceLogin = true
            },
            cancellationToken);

        UpsertSession(session);
        AccountsState.ActiveProviderId = session.Profile.ProviderId;

        EventAggregator.Publish(new AccountSignedInEvent(session));
        EventAggregator.Publish(new AccountSessionChangedEvent(session));

        return session;
    }

    public async Task SignOutAsync(string providerId, CancellationToken cancellationToken)
    {
        var provider = GetProvider(providerId);
        await provider.SignOutAsync(cancellationToken);

        AccountsState.Sessions = AccountsState.Sessions
            .Where(t => !string.Equals(t.Profile.ProviderId, providerId, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (string.Equals(AccountsState.ActiveProviderId, providerId, StringComparison.OrdinalIgnoreCase))
        {
            AccountsState.ActiveProviderId = AccountsState.Sessions.FirstOrDefault()?.Profile.ProviderId;
        }

        EventAggregator.Publish(new AccountSignedOutEvent(providerId));
        EventAggregator.Publish(new AccountSessionChangedEvent(GetCurrentSession()));
    }

    public async Task<string> AcquireTokenAsync(
        string providerId,
        string[] scopes,
        CancellationToken cancellationToken)
    {
        var provider = GetProvider(providerId);
        return await provider.AcquireTokenAsync(
            new AccountTokenRequest
            {
                ProviderId = providerId,
                Scopes = scopes
            },
            cancellationToken);
    }

    private IAccountProvider GetProvider(string providerId)
    {
        var provider = Providers.FirstOrDefault(t =>
            string.Equals(t.Descriptor.Id, providerId, StringComparison.OrdinalIgnoreCase));

        if (provider == null)
        {
            throw new InvalidOperationException($"Account provider <{providerId}> is not registered.");
        }

        if (!provider.Descriptor.IsEnabled)
        {
            throw new InvalidOperationException($"Account provider <{provider.Descriptor.DisplayName}> is disabled.");
        }

        return provider;
    }

    private void UpsertSession(AccountSession session)
    {
        var list = AccountsState.Sessions
            .Where(t => !string.Equals(
                t.Profile.ProviderId,
                session.Profile.ProviderId,
                StringComparison.OrdinalIgnoreCase))
            .ToList();

        list.Add(session);
        AccountsState.Sessions = list.ToArray();
    }
}
