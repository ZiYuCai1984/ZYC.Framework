using System.Text.Json.Serialization;

namespace ZYC.Framework.Modules.Accounts.Providers;

internal partial class GitHubAccountProvider
{
    private class GitHubUserResponse
    {
        [JsonPropertyName("login")] public string Login { get; set; } = "";

        [JsonPropertyName("id")] public long Id { get; set; }

        [JsonPropertyName("name")] public string? Name { get; set; }

        [JsonPropertyName("email")] public string? Email { get; set; }

        [JsonPropertyName("avatar_url")] public string? AvatarUrl { get; set; }
    }
}