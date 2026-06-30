namespace ZYC.Framework.Modules.Accounts.Providers;

internal partial class GitHubAccountProvider
{
    private class GitHubAuthorizationContext
    {
        public Uri RedirectUri { get; init; } = null!;

        public string State { get; init; } = "";

        public string Nonce { get; init; } = "";

        public string CodeVerifier { get; init; } = "";
    }
}