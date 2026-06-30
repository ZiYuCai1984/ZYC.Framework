namespace ZYC.Framework.Modules.Accounts.GitHub;

internal partial class GitHubAccountProvider
{
    private class GitHubTokenRecord
    {
        public string AccessToken { get; set; } = "";

        public string TokenType { get; set; } = "bearer";

        public string Scope { get; set; } = "";

        public DateTimeOffset CreatedAt { get; set; }
    }
}