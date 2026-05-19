using ZYC.Framework.Abstractions;

namespace ZYC.Framework;

internal static class StartupUriParser
{
    public static Uri? GetStartupUriArgument()
    {
        var arguments = Environment.GetCommandLineArgs().Skip(1);
        foreach (var argument in arguments)
        {
            if (TryParse(argument, out var uri))
            {
                return uri;
            }
        }

        return null;
    }

    public static bool TryParse(string? value, out Uri uri)
    {
        uri = null!;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var parsedUri))
        {
            return false;
        }

        if (!string.Equals(parsedUri.Scheme, ProductInfo.Scheme, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        uri = parsedUri;
        return true;
    }
}
