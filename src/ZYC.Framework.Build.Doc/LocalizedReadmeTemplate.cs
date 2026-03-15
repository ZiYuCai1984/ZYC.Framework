using System.Text;
using System.Text.RegularExpressions;

namespace ZYC.Framework.Build.Doc;

internal sealed class LocalizedReadmeTemplate
{
    private static readonly Regex BlockRegex = new(
        @"<!--doc-l10n:begin\s+(?<name>[^>]+?)-->(?<body>.*?)<!--doc-l10n:end-->",
        RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex LocaleRegex = new(
        @"<!--doc-l10n:locale\s+(?<locale>[^>]+?)-->",
        RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private readonly IReadOnlyList<ReadmeFragment> _fragments;

    private LocalizedReadmeTemplate(IReadOnlyList<ReadmeFragment> fragments)
    {
        _fragments = fragments;
    }

    public IReadOnlyCollection<string> Locales => _fragments
        .OfType<LocalizedBlockFragment>()
        .SelectMany(fragment => fragment.LocalizedContents.Keys)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(locale => locale, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    public static bool ContainsBlocks(string content)
    {
        return content.Contains("<!--doc-l10n:begin ", StringComparison.Ordinal);
    }

    public static LocalizedReadmeTemplate Parse(string content)
    {
        var fragments = new List<ReadmeFragment>();
        var currentIndex = 0;

        foreach (Match match in BlockRegex.Matches(content))
        {
            if (match.Index > currentIndex)
            {
                fragments.Add(new PlainTextFragment(content[currentIndex..match.Index]));
            }

            fragments.Add(ParseBlock(match));
            currentIndex = match.Index + match.Length;
        }

        if (currentIndex < content.Length)
        {
            fragments.Add(new PlainTextFragment(content[currentIndex..]));
        }

        return new LocalizedReadmeTemplate(fragments);
    }

    public string Render(string? locale)
    {
        var builder = new StringBuilder();

        foreach (var fragment in _fragments)
        {
            switch (fragment)
            {
                case PlainTextFragment plainTextFragment:
                    builder.Append(plainTextFragment.Content);
                    break;
                case LocalizedBlockFragment localizedBlockFragment:
                    builder.Append(localizedBlockFragment.GetContent(locale));
                    break;
            }
        }

        return builder.ToString().TrimEnd() + Environment.NewLine;
    }

    private static LocalizedBlockFragment ParseBlock(Match blockMatch)
    {
        var name = blockMatch.Groups["name"].Value.Trim();
        var body = blockMatch.Groups["body"].Value;
        var localeMatches = LocaleRegex.Matches(body);
        var localizedContents = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var defaultContent = localeMatches.Count == 0
            ? body
            : body[..localeMatches[0].Index];

        for (var index = 0; index < localeMatches.Count; index++)
        {
            var localeMatch = localeMatches[index];
            var locale = localeMatch.Groups["locale"].Value.Trim();
            if (string.IsNullOrWhiteSpace(locale))
            {
                continue;
            }

            var contentStart = localeMatch.Index + localeMatch.Length;
            var contentEnd = index + 1 < localeMatches.Count
                ? localeMatches[index + 1].Index
                : body.Length;

            localizedContents[locale] = body[contentStart..contentEnd];
        }

        return new LocalizedBlockFragment(name, defaultContent, localizedContents);
    }

    private abstract record ReadmeFragment;

    private sealed record PlainTextFragment(string Content) : ReadmeFragment;

    private sealed record LocalizedBlockFragment(
        string Name,
        string DefaultContent,
        IReadOnlyDictionary<string, string> LocalizedContents) : ReadmeFragment
    {
        public string GetContent(string? locale)
        {
            if (!string.IsNullOrWhiteSpace(locale)
                && LocalizedContents.TryGetValue(locale, out var localizedContent))
            {
                return localizedContent;
            }

            return DefaultContent;
        }
    }
}