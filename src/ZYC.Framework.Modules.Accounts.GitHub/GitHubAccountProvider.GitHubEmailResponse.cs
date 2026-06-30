using System.Text.Json.Serialization;

namespace ZYC.Framework.Modules.Accounts.GitHub;

internal partial class GitHubAccountProvider
{
    private class GitHubEmailResponse
    {
        [JsonPropertyName("email")] public string Email { get; set; } = "";

        [JsonPropertyName("primary")] public bool Primary { get; set; }

        [JsonPropertyName("verified")] public bool Verified { get; set; }
    }
}