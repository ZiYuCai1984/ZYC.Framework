using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Modules.Accounts.Abstractions;
using ZYC.Framework.Modules.Accounts.GitHub.Abstractions;

namespace ZYC.Framework.Modules.Accounts.GitHub;

[RegisterSingleInstanceAs(typeof(IAccountProvider))]
internal partial class GitHubAccountProvider : IAccountProvider
{
    private const string ProviderId = "GitHub";
    private const string TokenCacheKey = "oauth";
    private const string AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
    private const string AccessTokenEndpoint = "https://github.com/login/oauth/access_token";
    private const string UserEndpoint = "https://api.github.com/user";
    private const string EmailsEndpoint = "https://api.github.com/user/emails";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GitHubAccountProvider(
        ITabManager tabManager,
        GitHubAccountConfig accountsConfig,
        IAccountTokenStore tokenStore,
        IToastManager toastManager,
        GitHubAuthenticationCallbackBroker callbackBroker)
    {
        TabManager = tabManager;
        AccountsConfig = accountsConfig;
        TokenStore = tokenStore;
        ToastManager = toastManager;
        CallbackBroker = callbackBroker;
    }

    private ITabManager TabManager { get; }

    private GitHubAccountConfig AccountsConfig { get; }

    private IAccountTokenStore TokenStore { get; }

    private IToastManager ToastManager { get; }

    private GitHubAuthenticationCallbackBroker CallbackBroker { get; }

    public AccountProviderDescriptor Descriptor => new()
    {
        Id = ProviderId,
        DisplayName = "GitHub",
        Icon = "Github"
    };

    public Task<AccountSession?> GetCachedSessionAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<AccountSession?>(null);
    }

    public async Task<AccountSession> SignInAsync(AccountLoginRequest request, CancellationToken cancellationToken)
    {
        var scopes = GetScopes(request.Scopes);
        var token = await RequestTokenByAuthorizationCodeAsync(scopes, cancellationToken);
        await SaveTokenAsync(token, cancellationToken);

        return await CreateSessionAsync(token, cancellationToken);
    }

    public async Task<string> AcquireTokenAsync(AccountTokenRequest request, CancellationToken cancellationToken)
    {
        var requestedScopes = GetScopes(request.Scopes);
        var token = request.ForceRefresh
            ? null
            : await ReadTokenAsync(cancellationToken);

        if (token == null || !ScopeContains(token.Scope, requestedScopes))
        {
            token = await RequestTokenByAuthorizationCodeAsync(requestedScopes, cancellationToken);
            await SaveTokenAsync(token, cancellationToken);
        }

        return token.AccessToken;
    }

    public async Task SignOutAsync(CancellationToken cancellationToken)
    {
        await TokenStore.ClearProviderAsync(ProviderId, cancellationToken);
    }

    private async Task<GitHubTokenRecord> RequestTokenByAuthorizationCodeAsync(
        string[] scopes,
        CancellationToken cancellationToken)
    {
        EnsureClientId();
        EnsureTokenExchangeConfiguration();

        var context = CreateAuthorizationContext();
        var authorizationUri = CreateAuthorizationUri(context, scopes);
        using var timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutTokenSource.CancelAfter(TimeSpan.FromSeconds(GetAuthorizationTimeoutSeconds()));

        await TabManager.NavigateAsync(GitHubAuthenticationBrowserTabItemFactory.CreateUri(authorizationUri));
        ToastManager.PromptMessage(ToastMessage.Info("Complete GitHub sign-in in browser.", false));

        GitHubAuthenticationCallback callback;
        try
        {
            callback = await CallbackBroker.WaitAsync(context.Nonce, timeoutTokenSource.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("GitHub authorization callback timed out.");
        }

        ValidateAuthorizationCallback(callback, context);

        var response = await ExchangeAuthorizationCodeAsync(
            callback.Code,
            context,
            scopes,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(response.Error))
        {
            throw new InvalidOperationException(CreateTokenExchangeFailureMessage(response));
        }

        if (string.IsNullOrWhiteSpace(response.AccessToken))
        {
            throw new InvalidOperationException("GitHub authorization-code exchange returned no access token.");
        }

        return new GitHubTokenRecord
        {
            AccessToken = response.AccessToken,
            TokenType = string.IsNullOrWhiteSpace(response.TokenType) ? "bearer" : response.TokenType,
            Scope = response.Scope ?? string.Join(" ", scopes),
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    private GitHubAuthorizationContext CreateAuthorizationContext()
    {
        var nonce = CreateOpaqueValue(32);
        var codeVerifier = CreateOpaqueValue(64);
        var deepLinkUri = CreateDeepLinkUri(nonce);

        return new GitHubAuthorizationContext
        {
            RedirectUri = GetConfiguredRedirectUri(),
            State = Uri.EscapeDataString(deepLinkUri.ToString()),
            Nonce = nonce,
            CodeVerifier = codeVerifier
        };
    }

    private Uri CreateAuthorizationUri(GitHubAuthorizationContext context, string[] scopes)
    {
        var codeChallenge = CreateCodeChallenge(context.CodeVerifier);
        return new Uri(AppendQuery(
            AuthorizationEndpoint,
            new Dictionary<string, string>
            {
                ["client_id"] = AccountsConfig.GitHubClientId.Trim(),
                ["redirect_uri"] = context.RedirectUri.ToString(),
                ["scope"] = string.Join(" ", scopes),
                ["state"] = context.State,
                ["code_challenge"] = codeChallenge,
                ["code_challenge_method"] = "S256"
            }));
    }

    private static void ValidateAuthorizationCallback(
        GitHubAuthenticationCallback callback,
        GitHubAuthorizationContext context)
    {
        if (!string.IsNullOrWhiteSpace(callback.Error))
        {
            throw new InvalidOperationException(
                $"GitHub authorization failed: {callback.ErrorDescription}");
        }

        if (!string.Equals(callback.Nonce, context.Nonce, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("GitHub authorization nonce validation failed.");
        }

        if (!string.IsNullOrWhiteSpace(callback.State)
            && !StateEquals(callback.State, context.State))
        {
            throw new InvalidOperationException("GitHub authorization state validation failed.");
        }

        if (string.IsNullOrWhiteSpace(callback.Code))
        {
            throw new InvalidOperationException("GitHub authorization callback returned no code.");
        }
    }

    private async Task<GitHubAccessTokenResponse> ExchangeAuthorizationCodeAsync(
        string code,
        GitHubAuthorizationContext context,
        string[] scopes,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(AccountsConfig.GitHubTokenExchangeEndpoint))
        {
            return await ExchangeAuthorizationCodeWithRelayAsync(code, context, scopes, cancellationToken);
        }

        return await ExchangeAuthorizationCodeWithGitHubAsync(code, context, cancellationToken);
    }

    private async Task<GitHubAccessTokenResponse> ExchangeAuthorizationCodeWithGitHubAsync(
        string code,
        GitHubAuthorizationContext context,
        CancellationToken cancellationToken)
    {
        using var httpClient = CreateGitHubHttpClient();
        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = AccountsConfig.GitHubClientId.Trim(),
            ["code"] = code,
            ["redirect_uri"] = context.RedirectUri.ToString(),
            ["code_verifier"] = context.CodeVerifier
        };

        if (!string.IsNullOrWhiteSpace(AccountsConfig.GitHubClientSecret))
        {
            parameters["client_secret"] = AccountsConfig.GitHubClientSecret.Trim();
        }

        using var content = new FormUrlEncodedContent(parameters);
        using var response = await httpClient.PostAsync(AccessTokenEndpoint, content, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"GitHub authorization-code exchange failed: {payload}");
        }

        return DeserializeTokenResponse(payload);
    }

    private async Task<GitHubAccessTokenResponse> ExchangeAuthorizationCodeWithRelayAsync(
        string code,
        GitHubAuthorizationContext context,
        string[] scopes,
        CancellationToken cancellationToken)
    {
        using var httpClient = CreateGitHubHttpClient();
        using var content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["client_id"] = AccountsConfig.GitHubClientId.Trim(),
                ["code"] = code,
                ["redirect_uri"] = context.RedirectUri.ToString(),
                ["code_verifier"] = context.CodeVerifier,
                ["scope"] = string.Join(" ", scopes),
                ["state"] = context.State
            });

        using var response = await httpClient.PostAsync(
            AccountsConfig.GitHubTokenExchangeEndpoint.Trim(),
            content,
            cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"GitHub token exchange relay failed: {payload}");
        }

        return DeserializeTokenResponse(payload);
    }

    private async Task<AccountSession> CreateSessionAsync(
        GitHubTokenRecord token,
        CancellationToken cancellationToken)
    {
        using var httpClient = CreateGitHubHttpClient(token.AccessToken, true);
        using var response = await httpClient.GetAsync(UserEndpoint, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"GitHub user profile request failed: {payload}");
        }

        var user = JsonSerializer.Deserialize<GitHubUserResponse>(payload, JsonSerializerOptions);
        if (user == null)
        {
            throw new InvalidOperationException("GitHub user profile response is empty.");
        }

        var email = string.IsNullOrWhiteSpace(user.Email)
            ? await TryGetPrimaryEmailAsync(httpClient, token.Scope, cancellationToken)
            : user.Email;

        return new AccountSession
        {
            Profile = new AccountProfile
            {
                ProviderId = ProviderId,
                UserId = user.Id.ToString(),
                DisplayName = string.IsNullOrWhiteSpace(user.Name) ? user.Login : user.Name,
                UserName = user.Login,
                Email = email,
                AvatarUri = Uri.TryCreate(user.AvatarUrl, UriKind.Absolute, out var avatarUri)
                    ? avatarUri
                    : null
            },
            Scopes = SplitScopes(token.Scope),
            ExpiresOn = null
        };
    }

    private async Task<string?> TryGetPrimaryEmailAsync(
        HttpClient httpClient,
        string scope,
        CancellationToken cancellationToken)
    {
        if (!ScopeContainsAny(scope, ["user", "user:email"]))
        {
            return null;
        }

        using var response = await httpClient.GetAsync(EmailsEndpoint, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        var emails = JsonSerializer.Deserialize<GitHubEmailResponse[]>(payload, JsonSerializerOptions) ?? [];
        return emails.FirstOrDefault(t => t.Primary && t.Verified)?.Email
               ?? emails.FirstOrDefault(t => t.Verified)?.Email;
    }

    private async Task<GitHubTokenRecord?> ReadTokenAsync(CancellationToken cancellationToken)
    {
        var payload = await TokenStore.GetAsync(ProviderId, TokenCacheKey, cancellationToken);
        if (payload == null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<GitHubTokenRecord>(payload, JsonSerializerOptions);
    }

    private async Task SaveTokenAsync(GitHubTokenRecord token, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.SerializeToUtf8Bytes(token, JsonSerializerOptions);
        await TokenStore.SetAsync(ProviderId, TokenCacheKey, payload, cancellationToken);
    }

    private HttpClient CreateGitHubHttpClient(string? accessToken = null, bool useRestApi = false)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(useRestApi ? "application/vnd.github+json" : "application/json"));
        httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("ZYC.Framework");

        if (useRestApi && !string.IsNullOrWhiteSpace(AccountsConfig.GitHubApiVersion))
        {
            httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", AccountsConfig.GitHubApiVersion.Trim());
        }

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return httpClient;
    }

    private string[] GetScopes(string[] requestedScopes)
    {
        var scopes = requestedScopes.Length == 0
            ? AccountsConfig.GitHubScopes
            : requestedScopes;

        scopes = scopes
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return scopes.Length == 0 ? ["read:user"] : scopes;
    }

    private Uri GetConfiguredRedirectUri()
    {
        if (string.IsNullOrWhiteSpace(AccountsConfig.GitHubRedirectUri))
        {
            throw new InvalidOperationException(
                "GitHub WebView2 authentication requires AccountsConfig.GitHubRedirectUri.");
        }

        if (!Uri.TryCreate(AccountsConfig.GitHubRedirectUri.Trim(), UriKind.Absolute, out var redirectUri))
        {
            throw new InvalidOperationException("AccountsConfig.GitHubRedirectUri must be an absolute URI.");
        }

        return redirectUri;
    }

    private Uri CreateDeepLinkUri(string nonce)
    {
        var rawUri = string.IsNullOrWhiteSpace(AccountsConfig.GitHubDeepLinkUri)
            ? "vscode://vscode.github-authentication/did-authenticate"
            : AccountsConfig.GitHubDeepLinkUri.Trim();

        if (!Uri.TryCreate(rawUri, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException("AccountsConfig.GitHubDeepLinkUri must be an absolute URI.");
        }

        return new Uri(AppendQuery(
            rawUri,
            new Dictionary<string, string>
            {
                ["nonce"] = nonce
            }));
    }

    private int GetAuthorizationTimeoutSeconds()
    {
        return AccountsConfig.GitHubAuthorizationTimeoutSeconds <= 0
            ? 300
            : AccountsConfig.GitHubAuthorizationTimeoutSeconds;
    }

    private void EnsureClientId()
    {
        if (string.IsNullOrWhiteSpace(AccountsConfig.GitHubClientId))
        {
            throw new InvalidOperationException(
                "GitHub account login requires AccountsConfig.GitHubClientId.");
        }
    }

    private void EnsureTokenExchangeConfiguration()
    {
        if (!string.IsNullOrWhiteSpace(AccountsConfig.GitHubTokenExchangeEndpoint))
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(AccountsConfig.GitHubClientSecret))
        {
            return;
        }

        throw new InvalidOperationException(
            "GitHub direct authorization-code exchange requires AccountsConfig.GitHubClientSecret. "
            + "Configure your GitHub OAuth app secret or set AccountsConfig.GitHubTokenExchangeEndpoint "
            + "to a server-side token exchange endpoint.");
    }

    private static GitHubAccessTokenResponse DeserializeTokenResponse(string payload)
    {
        var tokenResponse = JsonSerializer.Deserialize<GitHubAccessTokenResponse>(payload, JsonSerializerOptions);
        if (tokenResponse == null)
        {
            throw new InvalidOperationException("GitHub access token response is empty.");
        }

        return tokenResponse;
    }

    private static string CreateTokenExchangeFailureMessage(GitHubAccessTokenResponse response)
    {
        if (string.Equals(response.Error, "incorrect_client_credentials", StringComparison.OrdinalIgnoreCase))
        {
            return "GitHub authorization-code exchange failed: GitHub rejected the configured OAuth client "
                   + "credentials. Verify AccountsConfig.GitHubClientId and AccountsConfig.GitHubClientSecret, "
                   + "or use AccountsConfig.GitHubTokenExchangeEndpoint for server-side exchange.";
        }

        return $"GitHub authorization-code exchange failed: {response.ErrorDescription ?? response.Error}";
    }

    private static bool ScopeContains(string grantedScope, string[] requestedScopes)
    {
        var granted = SplitScopes(grantedScope);
        return requestedScopes.All(t => granted.Contains(t, StringComparer.OrdinalIgnoreCase));
    }

    private static bool ScopeContainsAny(string grantedScope, string[] requestedScopes)
    {
        var granted = SplitScopes(grantedScope);
        return requestedScopes.Any(t => granted.Contains(t, StringComparer.OrdinalIgnoreCase));
    }

    private static bool StateEquals(string actualState, string expectedState)
    {
        return string.Equals(actualState, expectedState, StringComparison.Ordinal)
               || string.Equals(DecodeRepeatedly(actualState), DecodeRepeatedly(expectedState),
                   StringComparison.Ordinal);
    }

    private static string[] SplitScopes(string scope)
    {
        return scope.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static string CreateOpaqueValue(int byteCount)
    {
        var bytes = RandomNumberGenerator.GetBytes(byteCount);
        return Base64UrlEncode(bytes);
    }

    private static string CreateCodeChallenge(string codeVerifier)
    {
        var hash = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
        return Base64UrlEncode(hash);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string DecodeRepeatedly(string value)
    {
        var current = value;
        for (var i = 0; i < 2; i++)
        {
            var decoded = Uri.UnescapeDataString(current);
            if (string.Equals(decoded, current, StringComparison.Ordinal))
            {
                break;
            }

            current = decoded;
        }

        return current;
    }

    private static string AppendQuery(string uri, Dictionary<string, string> parameters)
    {
        var builder = new StringBuilder(uri);
        var hasQuery = uri.Contains('?', StringComparison.Ordinal);

        foreach (var pair in parameters)
        {
            builder.Append(hasQuery ? '&' : '?');
            builder.Append(Uri.EscapeDataString(pair.Key));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(pair.Value));
            hasQuery = true;
        }

        return builder.ToString();
    }
}