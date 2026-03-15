using System.Text.RegularExpressions;

namespace ZYC.Framework.Build.Doc;

internal sealed partial class TemplateRenderer
{
    private readonly IReadOnlyDictionary<string, string> _variables;

    public TemplateRenderer(IReadOnlyDictionary<string, string> variables)
    {
        _variables = variables;
    }

    public string Render(string input, ISet<string>? missingPlaceholders = null)
    {
        return PlaceholderRegex().Replace(input, match =>
        {
            var name = match.Groups["name"].Value;
            if (_variables.TryGetValue(name, out var value))
            {
                return value;
            }

            missingPlaceholders?.Add(name);
            return match.Value;
        });
    }

    [GeneratedRegex(@"\$\((?<name>[A-Za-z0-9_.:-]+)\)", RegexOptions.CultureInvariant)]
    private static partial Regex PlaceholderRegex();
}