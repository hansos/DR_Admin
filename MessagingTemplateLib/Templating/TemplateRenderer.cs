using System.Text.RegularExpressions;

namespace MessagingTemplateLib.Templating;

/// <summary>
/// Renders templates by replacing placeholders with actual values
/// </summary>
public static class TemplateRenderer
{
    private static readonly Regex PlaceholderRegex = new(@"{{(\w+)}}", RegexOptions.Compiled);

    /// <summary>
    /// Renders a template by replacing all {{placeholder}} tokens with values from the dictionary
    /// </summary>
    /// <param name="template">The template string with {{placeholders}}</param>
    /// <param name="values">Dictionary of placeholder names and their values</param>
    /// <returns>The rendered template</returns>
    public static string Render(string template, IDictionary<string, object> values)
    {
        return PlaceholderRegex.Replace(template, match =>
        {
            var key = match.Groups[1].Value;
            return values.TryGetValue(key, out var val) ? val?.ToString() ?? "" : match.Value;
        });
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
        return model.GetType()
            .GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(model) ?? (object)"");
    }
}
