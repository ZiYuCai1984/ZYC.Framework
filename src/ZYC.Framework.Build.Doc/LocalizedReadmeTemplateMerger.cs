using System.Text;
using System.Text.RegularExpressions;

namespace ZYC.Framework.Build.Doc;

internal static class LocalizedReadmeTemplateMerger
{
    public static string Merge(
        string defaultContent,
        IReadOnlyDictionary<string, string> localizedContents,
        IReadOnlyList<string>? localeOrder = null)
    {
        var defaultSections = SplitSections(defaultContent);
        var localizedSections = new Dictionary<string, IReadOnlyList<ReadmeSection>>(StringComparer.OrdinalIgnoreCase);

        foreach (var pair in localizedContents)
        {
            var sections = SplitSections(pair.Value);
            if (sections.Count != defaultSections.Count)
            {
                throw new InvalidOperationException(
                    $"README section count mismatch for locale '{pair.Key}'. Expected {defaultSections.Count}, actual {sections.Count}.");
            }

            localizedSections[pair.Key] = sections;
        }

        var orderedLocales = BuildOrderedLocales(localizedSections.Keys, localeOrder);
        var builder = new StringBuilder();

        for (var index = 0; index < defaultSections.Count; index++)
        {
            var section = defaultSections[index];
            builder.AppendLine($"<!--doc-l10n:begin {section.Name}-->");
            builder.Append(section.Content.TrimEnd());
            builder.AppendLine();

            foreach (var locale in orderedLocales)
            {
                builder.AppendLine($"<!--doc-l10n:locale {locale}-->");
                builder.Append(localizedSections[locale][index].Content.TrimEnd());
                builder.AppendLine();
            }

            builder.AppendLine("<!--doc-l10n:end-->");
            if (index < defaultSections.Count - 1)
            {
                builder.AppendLine();
            }
        }

        return builder.ToString().TrimEnd() + Environment.NewLine;
    }

    private static IReadOnlyList<string> BuildOrderedLocales(
        IEnumerable<string> locales,
        IReadOnlyList<string>? localeOrder)
    {
        var result = new List<string>();
        var remaining = new HashSet<string>(locales, StringComparer.OrdinalIgnoreCase);

        if (localeOrder != null)
        {
            foreach (var locale in localeOrder)
            {
                if (remaining.Remove(locale))
                {
                    result.Add(locale);
                }
            }
        }

        result.AddRange(remaining.OrderBy(locale => locale, StringComparer.OrdinalIgnoreCase));
        return result;
    }

    private static IReadOnlyList<ReadmeSection> SplitSections(string content)
    {
        var headingMatches = Regex.Matches(content, @"(?m)^##\s+");
        if (headingMatches.Count == 0)
        {
            return [new ReadmeSection("intro", content)];
        }

        var sections = new List<ReadmeSection>
        {
            new("intro", content[..headingMatches[0].Index])
        };

        for (var index = 0; index < headingMatches.Count; index++)
        {
            var sectionStart = headingMatches[index].Index;
            var sectionEnd = index + 1 < headingMatches.Count
                ? headingMatches[index + 1].Index
                : content.Length;

            var sectionContent = content[sectionStart..sectionEnd];
            sections.Add(new ReadmeSection(GetSectionName(sectionContent, index + 1), sectionContent));
        }

        return sections;
    }

    private static string GetSectionName(string sectionContent, int index)
    {
        if (index == 0)
        {
            return "intro";
        }

        var heading = sectionContent
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
            .FirstOrDefault(line => line.StartsWith("## ", StringComparison.Ordinal));

        if (string.IsNullOrWhiteSpace(heading))
        {
            return $"section-{index:00}";
        }

        var sectionName = heading[3..].Trim();
        sectionName = Regex.Replace(sectionName, @"[^A-Za-z0-9]+", "-")
            .Trim('-')
            .ToLowerInvariant();

        return string.IsNullOrWhiteSpace(sectionName)
            ? $"section-{index:00}"
            : sectionName;
    }

    private sealed record ReadmeSection(string Name, string Content);
}
