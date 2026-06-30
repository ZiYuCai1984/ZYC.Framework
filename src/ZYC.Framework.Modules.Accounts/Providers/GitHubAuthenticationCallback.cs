namespace ZYC.Framework.Modules.Accounts.Providers;

internal class GitHubAuthenticationCallback
{
    public string Code { get; set; } = "";

    public string State { get; set; } = "";

    public string Nonce { get; set; } = "";

    public string Error { get; set; } = "";

    public string ErrorDescription { get; set; } = "";
}
