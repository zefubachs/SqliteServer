using System.Text.RegularExpressions;

namespace SqliteServer;

public partial class Variables
{
    private readonly Dictionary<string, string> variables;

    public Variables(Dictionary<string, string> variables)
    {
        this.variables = variables;
    }

    public string Translate(string input)
    {
        return placeholderRegex.Replace(input, match =>
        {
            var placeholderName = match.Value.Trim(' ', '{', '}');
            if (variables.TryGetValue(placeholderName, out var value))
                return value!;

            // Placeholder not defined
            return placeholderName;
        });
    }

    private static readonly Regex placeholderRegex = PlaceholderRegex();
    public static Variables Create(IConfigurationSection section)
    {
        var configurationValues = section.GetChildren().ToDictionary(x => x.Key, x => x.Value);
        var values = new Dictionary<string, string>();
        foreach (var entry in configurationValues)
        {
            if (string.IsNullOrWhiteSpace(entry.Value) || values.ContainsKey(entry.Key))
            {
                // Value is already procssed in recursive regex
                continue;
            }

            var value = placeholderRegex.Replace(entry.Value, Replace);
            values.Add(entry.Key, value);
        }

        return new Variables(values);

        string Replace(Match match)
        {
            // Remove leading { and trailing }
            var placeholderName = match.Value.Trim('{', '}', ' ');
            if (values.TryGetValue(placeholderName, out var value))
            {
                // Placeholder is already processed
                return value;
            }

            if (!configurationValues.TryGetValue(placeholderName, out var placeholderValue) || string.IsNullOrWhiteSpace(placeholderValue))
            {
                // Placeholder is not defined
                return placeholderName;
            }

            value = placeholderRegex.Replace(placeholderValue, Replace);
            value = Environment.ExpandEnvironmentVariables(value);
            values.TryAdd(placeholderName, value);
            return value;
        }
    }

    [GeneratedRegex("{[a-zA-Z0-9_]+}")]
    private static partial Regex PlaceholderRegex();
}
