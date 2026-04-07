using System.Text.RegularExpressions;
using Serilog;

namespace MessagingTemplateLib.Templating;

/// <summary>
/// Renders templates by replacing placeholders with actual values
/// </summary>
public static class TemplateRenderer
{
    private static readonly ILogger _log = Log.ForContext(typeof(TemplateRenderer));
    private static readonly Regex CurlyPlaceholderRegex = new(@"{{\s*([A-Za-z0-9_\.-]+)\s*}}", RegexOptions.Compiled);
    private static readonly Regex BracketPlaceholderRegex = new(@"\[\[\s*([A-Za-z0-9_\.-]+)\s*\]\]", RegexOptions.Compiled);

    /// <summary>
    /// Renders a template by replacing all {{placeholder}} tokens with values from the dictionary
    /// </summary>
    /// <param name="template">The template string with {{placeholders}}</param>
    /// <param name="values">Dictionary of placeholder names and their values</param>
    /// <returns>The rendered template</returns>
    public static string Render(string template, IDictionary<string, object> values)
    {
        _log.Debug("Render called - Template length: {Length}, Values count: {Count}, Keys: [{Keys}]",
            template?.Length ?? 0, values.Count, string.Join(", ", values.Keys));

        var curlyMatches = CurlyPlaceholderRegex.Matches(template);
        _log.Debug("CurlyPlaceholderRegex found {MatchCount} match(es) in template: [{Matches}]",
            curlyMatches.Count,
            string.Join(", ", curlyMatches.Take(10).Select(m => m.Value)));

        var lookup = values is Dictionary<string, object> dictionary && dictionary.Comparer.Equals(StringComparer.OrdinalIgnoreCase)
            ? dictionary
            : new Dictionary<string, object>(values, StringComparer.OrdinalIgnoreCase);

        var normalizedLookup = lookup
            .ToDictionary(kvp => NormalizePlaceholderKey(kvp.Key), kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);

        var rendered = ReplacePlaceholders(template, CurlyPlaceholderRegex, lookup, normalizedLookup);
        rendered = ReplacePlaceholders(rendered, BracketPlaceholderRegex, lookup, normalizedLookup);

        var snippet = rendered.Length > 200 ? rendered[..200] : rendered;
        _log.Debug("Render result (Length={Length}): {Snippet}", rendered.Length, snippet);

        return rendered;
    }

    private static string ReplacePlaceholders(
        string template,
        Regex regex,
        IDictionary<string, object> lookup,
        IDictionary<string, object> normalizedLookup)
    {
        return regex.Replace(template, match =>
        {
            var key = match.Groups[1].Value.Trim();

            if (lookup.TryGetValue(key, out var directValue))
            {
                return directValue?.ToString() ?? string.Empty;
            }

            var dottedKeyValue = key.Contains('.')
                ? key.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).LastOrDefault()
                : null;

            if (!string.IsNullOrWhiteSpace(dottedKeyValue) && lookup.TryGetValue(dottedKeyValue, out var dottedValue))
            {
                return dottedValue?.ToString() ?? string.Empty;
            }

            var normalizedKey = NormalizePlaceholderKey(key);
            if (normalizedLookup.TryGetValue(normalizedKey, out var normalizedValue))
            {
                return normalizedValue?.ToString() ?? string.Empty;
            }

            return match.Value;
        });
    }

    private static string NormalizePlaceholderKey(string value)
    {
        return new string(value.Where(char.IsLetterOrDigit).ToArray());
    }

    /// <summary>
    /// Renders a content template and injects it into a master template
    /// </summary>
    /// <param name="contentTemplate">The content template to render</param>
    /// <param name="masterTemplate">The master/layout template containing {{Content}} placeholder</param>
    /// <param name="values">Dictionary of placeholder names and their values</param>
    /// <returns>The fully rendered template with content injected into master</returns>
    public static string RenderWithMaster(string contentTemplate, string masterTemplate, IDictionary<string, object> values)
    {
        // First render the content template
        string renderedContent = Render(contentTemplate, values);
        
        // Create a new dictionary with Content placeholder
        var masterValues = new Dictionary<string, object>(values)
        {
            ["Content"] = renderedContent
        };
        
        // Render the master template with the content injected
        return Render(masterTemplate, masterValues);
    }

    /// <summary>
    /// Converts an object's properties to a dictionary for use in template rendering
    /// </summary>
    /// <typeparam name="T">The type of the model object</typeparam>
    /// <param name="model">The model object to convert</param>
    /// <returns>Dictionary with property names as keys and property values as values</returns>
    public static IDictionary<string, object> ModelToDictionary<T>(T model) where T : class
    {
        var props = model.GetType().GetProperties();
        _log.Debug("ModelToDictionary - Type: {TypeName}, PropertyCount: {Count}, Properties: [{Props}]",
            model.GetType().Name, props.Length,
            string.Join(", ", props.Select(p => $"{p.Name}='{p.GetValue(model)}'").Take(10)));

        return props.ToDictionary(p => p.Name, p => p.GetValue(model) ?? (object)"");
    }
}
