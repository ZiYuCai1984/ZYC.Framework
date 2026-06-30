using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;

namespace ZYC.Framework.Modules.Accounts.GitHub;

[Register]
internal class GitHubAuthenticationBrowserTabItemFactory : TabItemFactoryBase
{
    private const string Host = "github-authentication";
    private const string SignInPath = "/sign-in";
    private const string AuthorizationUriParameter = "authorizationUri";

    public override bool IsSingle => false;

    public static Uri CreateUri(Uri authorizationUri)
    {
        return UriTools.CreateAppUri(
            Host,
            SignInPath,
            $"{AuthorizationUriParameter}={Uri.EscapeDataString(authorizationUri.ToString())}");
    }

    public override Task<bool> CheckUriMatchedAsync(Uri uri)
    {
        return Task.FromResult(
            string.Equals(uri.Scheme, ProductInfo.Scheme, StringComparison.OrdinalIgnoreCase)
            && string.Equals(uri.Host, Host, StringComparison.OrdinalIgnoreCase)
            && string.Equals(NormalizePath(uri.AbsolutePath), SignInPath, StringComparison.OrdinalIgnoreCase));
    }

    public override Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        var query = ParseQuery(context.Uri.Query);
        if (!query.TryGetValue(AuthorizationUriParameter, out var rawAuthorizationUri)
            || !Uri.TryCreate(rawAuthorizationUri, UriKind.Absolute, out var authorizationUri))
        {
            throw new InvalidOperationException("GitHub authorization URI is missing.");
        }

        return Task.FromResult<ITabItemInstance>(
            context.Resolve<GitHubAuthenticationBrowserTabItem>(
                new TypedParameter(typeof(TabReference), new TabReference(context.Uri)),
                new TypedParameter(typeof(Uri), authorizationUri)));
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

    private static string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return "/";
        }

        path = path.StartsWith("/", StringComparison.Ordinal) ? path : "/" + path;
        return path.Length > 1 ? path.TrimEnd('/') : path;
    }
}
