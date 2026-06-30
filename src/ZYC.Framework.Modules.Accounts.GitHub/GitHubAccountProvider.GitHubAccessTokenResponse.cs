using System.Text.Json.Serialization;

namespace ZYC.Framework.Modules.Accounts.GitHub;

internal partial class GitHubAccountProvider
{
    private class GitHubAccessTokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; } = "";

        [JsonPropertyName("token_type")] public string? TokenType { get; set; }

        [JsonPropertyName("scope")] public string? Scope { get; set; }

        [JsonPropertyName("error")] public string? Error { get; set; }

        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; set; }
    }
}