using System.Globalization;
using System.Text;

namespace ISPAdmin.Utilities;

/// <summary>
/// Helper class for normalizing strings for case-insensitive and culture-invariant comparisons
/// </summary>
public static class NormalizationHelper
{
    /// <summary>
    /// Normalizes a string for exact searches by converting to uppercase invariant culture
    /// and normalizing Unicode characters
    /// </summary>
    /// <param name="value">The string to normalize</param>
    /// <returns>The normalized string, or null if input is null</returns>
    public static string? Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        // Trim whitespace
        var normalized = value.Trim();

        // Normalize Unicode (convert to Form C - canonical composition)
        normalized = normalized.Normalize(NormalizationForm.FormC);

        // Convert to uppercase invariant culture for case-insensitive comparison
        normalized = normalized.ToUpperInvariant();

        return normalized;
    }

    /// <summary>
    /// Normalizes a string array
    /// </summary>
    /// <param name="values">The strings to normalize</param>
    /// <returns>Array of normalized strings</returns>
    public static string?[] Normalize(params string?[] values)
    {
        return values.Select(Normalize).ToArray();
    }
}
