using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Modules.Accounts.Abstractions;

namespace ZYC.Framework.Modules.Accounts.Providers;

[RegisterSingleInstanceAs(typeof(IAccountProvider))]
internal class GitHubAccountProvider : IAccountProvider
{
    private const string ProviderId = AccountProviderIds.GitHub;
    private const string TokenCacheKey = "oauth";
    private const string AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
    private const string AccessTokenEndpoint = "https://github.com/login/oauth/access_token";
    private const string UserEndpoint = "https://api.github.com/user";
    private const string EmailsEndpoint = "https://api.github.com/user/emails";
    private const string LoopbackPath = "/github/callback/";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GitHubAccountProvider(
        ITabManager tabManager,
        AccountsConfig accountsConfig,
        IAccountTokenStore tokenStore,
        IToastManager toastManager)
    {
        TabManager = tabManager;
        AccountsConfig = accountsConfig;
        TokenStore = tokenStore;
        ToastManager = toastManager;
    }

    private ITabManager TabManager { get; }
    private AccountsConfig AccountsConfig { get; }

    private IAccountTokenStore TokenStore { get; }

    private IToastManager ToastManager { get; }

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

        var configuredRedirectUri = GetConfiguredRedirectUri();
        using var loopbackListener = CreateLoopbackListener(configuredRedirectUri);
        var context = CreateAuthorizationContext(configuredRedirectUri, loopbackListener.RedirectUri);
        var authorizationUri = CreateAuthorizationUri(context, scopes);

        await TabManager.NavigateAsync(authorizationUri);

        ToastManager.PromptMessage(ToastMessage.Info("Complete GitHub sign-in in browser.", false));

        var callback = await WaitForAuthorizationCallbackAsync(
            loopbackListener,
            context,
            cancellationToken);

        var response = await ExchangeAuthorizationCodeAsync(
            callback.Code,
            context,
            scopes,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(response.Error))
        {
            throw new InvalidOperationException(
                $"GitHub authorization-code exchange failed: {response.ErrorDescription ?? response.Error}");
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

    private GitHubLoopbackCallbackListener CreateLoopbackListener(Uri? configuredRedirectUri)
    {
        if (configuredRedirectUri != null && IsLoopbackRedirectUri(configuredRedirectUri))
        {
            return GitHubLoopbackCallbackListener.Start(
                configuredRedirectUri.Host,
                configuredRedirectUri.Port,
                configuredRedirectUri.AbsolutePath);
        }

        return GitHubLoopbackCallbackListener.Start(
            "127.0.0.1",
            AccountsConfig.GitHubLoopbackPort,
            LoopbackPath);
    }

    private GitHubAuthorizationContext CreateAuthorizationContext(
        Uri? configuredRedirectUri,
        Uri localCallbackUri)
    {
        var nonce = CreateOpaqueValue(32);
        var codeVerifier = CreateOpaqueValue(64);
        var isRedirectRelay = configuredRedirectUri != null && !IsLoopbackRedirectUri(configuredRedirectUri);
        var localCallbackWithNonce = AppendQuery(
            localCallbackUri.ToString(),
            new Dictionary<string, string>
            {
                ["nonce"] = nonce
            });

        return new GitHubAuthorizationContext
        {
            RedirectUri = configuredRedirectUri ?? localCallbackUri,
            State = isRedirectRelay ? localCallbackWithNonce : nonce,
            Nonce = nonce,
            CodeVerifier = codeVerifier,
            IsRedirectRelay = isRedirectRelay
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

    private async Task<GitHubAuthorizationCallback> WaitForAuthorizationCallbackAsync(
        GitHubLoopbackCallbackListener listener,
        GitHubAuthorizationContext context,
        CancellationToken cancellationToken)
    {
        using var timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutTokenSource.CancelAfter(TimeSpan.FromSeconds(GetAuthorizationTimeoutSeconds()));

        try
        {
            var callback = await listener.WaitForCallbackAsync(timeoutTokenSource.Token);
            ValidateAuthorizationCallback(callback, context);

            return callback;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("GitHub authorization callback timed out.");
        }
    }

    private static void ValidateAuthorizationCallback(
        GitHubAuthorizationCallback callback,
        GitHubAuthorizationContext context)
    {
        if (!string.IsNullOrWhiteSpace(callback.Error))
        {
            throw new InvalidOperationException(
                $"GitHub authorization failed: {callback.ErrorDescription ?? callback.Error}");
        }

        if (context.IsRedirectRelay)
        {
            if (!string.Equals(callback.Nonce, context.Nonce, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("GitHub authorization nonce validation failed.");
            }

            if (!string.IsNullOrWhiteSpace(callback.State)
                && !string.Equals(callback.State, context.State, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("GitHub authorization state validation failed.");
            }
        }
        else if (!string.Equals(callback.State, context.State, StringComparison.Ordinal))
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

    private Uri? GetConfiguredRedirectUri()
    {
        if (string.IsNullOrWhiteSpace(AccountsConfig.GitHubRedirectUri))
        {
            return null;
        }

        if (!Uri.TryCreate(AccountsConfig.GitHubRedirectUri.Trim(), UriKind.Absolute, out var redirectUri))
        {
            throw new InvalidOperationException("AccountsConfig.GitHubRedirectUri must be an absolute URI.");
        }

        return redirectUri;
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

    private static GitHubAccessTokenResponse DeserializeTokenResponse(string payload)
    {
        var tokenResponse = JsonSerializer.Deserialize<GitHubAccessTokenResponse>(payload, JsonSerializerOptions);
        if (tokenResponse == null)
        {
            throw new InvalidOperationException("GitHub access token response is empty.");
        }

        return tokenResponse;
    }


    private static bool IsLoopbackRedirectUri(Uri redirectUri)
    {
        return string.Equals(redirectUri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
               && (string.Equals(redirectUri.Host, "127.0.0.1", StringComparison.OrdinalIgnoreCase)
                   || string.Equals(redirectUri.Host, "localhost", StringComparison.OrdinalIgnoreCase));
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

    private static Dictionary<string, string> ParseQuery(string query)
    {
        if (query.StartsWith("?", StringComparison.Ordinal))
        {
            query = query[1..];
        }

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var equalsIndex = pair.IndexOf('=', StringComparison.Ordinal);
            var key = equalsIndex < 0 ? pair : pair[..equalsIndex];
            var value = equalsIndex < 0 ? "" : pair[(equalsIndex + 1)..];
            values[Uri.UnescapeDataString(key.Replace("+", " ", StringComparison.Ordinal))] =
                Uri.UnescapeDataString(value.Replace("+", " ", StringComparison.Ordinal));
        }

        return values;
    }

    private class GitHubAuthorizationContext
    {
        public Uri RedirectUri { get; init; } = null!;

        public string State { get; init; } = "";

        public string Nonce { get; init; } = "";

        public string CodeVerifier { get; init; } = "";

        public bool IsRedirectRelay { get; init; }
    }

    private class GitHubAuthorizationCallback
    {
        public string Code { get; set; } = "";

        public string State { get; set; } = "";

        public string Nonce { get; set; } = "";

        public string Error { get; set; } = "";

        public string ErrorDescription { get; set; } = "";
    }

    private class GitHubLoopbackCallbackListener : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly string _path;

        private GitHubLoopbackCallbackListener(TcpListener listener, string host, string path)
        {
            _listener = listener;
            _path = NormalizePath(path);

            var port = ((IPEndPoint)_listener.LocalEndpoint).Port;
            RedirectUri = new Uri($"http://{host}:{port}{_path}");
        }

        public Uri RedirectUri { get; }

        public static GitHubLoopbackCallbackListener Start(string host, int port, string path)
        {
            var listener = new TcpListener(IPAddress.Loopback, Math.Max(port, 0));
            listener.Start();

            return new GitHubLoopbackCallbackListener(listener, host, path);
        }

        public async Task<GitHubAuthorizationCallback> WaitForCallbackAsync(CancellationToken cancellationToken)
        {
            using var tcpClient = await _listener.AcceptTcpClientAsync(cancellationToken);
            await using var stream = tcpClient.GetStream();
            using var reader = new StreamReader(stream, Encoding.ASCII, leaveOpen: true);

            var requestLine = await reader.ReadLineAsync(cancellationToken);
            while (!string.IsNullOrEmpty(await reader.ReadLineAsync(cancellationToken)))
            {
                // Drain headers before responding.
            }

            var callback = ParseCallback(requestLine);
            await WriteResponseAsync(stream, callback.Error, cancellationToken);

            return callback;
        }

        public void Dispose()
        {
            _listener.Stop();
        }

        private GitHubAuthorizationCallback ParseCallback(string? requestLine)
        {
            if (string.IsNullOrWhiteSpace(requestLine))
            {
                return new GitHubAuthorizationCallback
                {
                    Error = "invalid_request",
                    ErrorDescription = "The local callback request was empty."
                };
            }

            var parts = requestLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2 || !string.Equals(parts[0], "GET", StringComparison.OrdinalIgnoreCase))
            {
                return new GitHubAuthorizationCallback
                {
                    Error = "invalid_request",
                    ErrorDescription = "The local callback request was not a GET request."
                };
            }

            var uri = Uri.TryCreate(parts[1], UriKind.Absolute, out var absoluteUri)
                ? absoluteUri
                : new Uri($"http://127.0.0.1{parts[1]}");
            if (!string.Equals(NormalizePath(uri.AbsolutePath), _path, StringComparison.OrdinalIgnoreCase))
            {
                return new GitHubAuthorizationCallback
                {
                    Error = "invalid_request",
                    ErrorDescription = "The local callback path did not match the expected path."
                };
            }

            var query = ParseQuery(uri.Query);
            return new GitHubAuthorizationCallback
            {
                Code = query.GetValueOrDefault("code") ?? "",
                State = query.GetValueOrDefault("state") ?? "",
                Nonce = query.GetValueOrDefault("nonce") ?? "",
                Error = query.GetValueOrDefault("error") ?? "",
                ErrorDescription = query.GetValueOrDefault("error_description") ?? ""
            };
        }

        private static async Task WriteResponseAsync(
            NetworkStream stream,
            string error,
            CancellationToken cancellationToken)
        {
            var title = string.IsNullOrWhiteSpace(error) ? "GitHub sign-in complete" : "GitHub sign-in failed";
            var body = "<!doctype html><html><head><meta charset=\"utf-8\"><title>"
                       + WebUtility.HtmlEncode(title)
                       + "</title></head><body><h1>"
                       + WebUtility.HtmlEncode(title)
                       + "</h1><p>You can close this window.</p></body></html>";
            var bodyBytes = Encoding.UTF8.GetBytes(body);
            var header = Encoding.ASCII.GetBytes(
                "HTTP/1.1 200 OK\r\n"
                + "Content-Type: text/html; charset=utf-8\r\n"
                + $"Content-Length: {bodyBytes.Length}\r\n"
                + "Connection: close\r\n\r\n");

            await stream.WriteAsync(header, cancellationToken);
            await stream.WriteAsync(bodyBytes, cancellationToken);
        }

        private static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path == "/")
            {
                return LoopbackPath;
            }

            return path.StartsWith("/", StringComparison.Ordinal) ? path : "/" + path;
        }
    }

    private class GitHubTokenRecord
    {
        public string AccessToken { get; set; } = "";

        public string TokenType { get; set; } = "bearer";

        public string Scope { get; set; } = "";

        public DateTimeOffset CreatedAt { get; set; }
    }

    private class GitHubAccessTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; set; }
    }

    private class GitHubUserResponse
    {
        [JsonPropertyName("login")]
        public string Login { get; set; } = "";

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("avatar_url")]
        public string? AvatarUrl { get; set; }
    }

    private class GitHubEmailResponse
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = "";

        [JsonPropertyName("primary")]
        public bool Primary { get; set; }

        [JsonPropertyName("verified")]
        public bool Verified { get; set; }
    }
}
