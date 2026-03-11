using System.Net.Http.Headers;
using System.Text.Json;
using PaymentGatewayLib.Infrastructure.Settings;

namespace ISPAdmin.Services;

/// <summary>
/// VAT validation provider using Stripe tax ID verification flow.
/// </summary>
public class StripeVatValidationProvider : IVatValidationProvider
{
    private static readonly HashSet<string> EuCountries = new(StringComparer.OrdinalIgnoreCase)
    {
        "AT", "BE", "BG", "CY", "CZ", "DE", "DK", "EE", "EL", "ES", "FI", "FR", "HR", "HU", "IE",
        "IT", "LT", "LU", "LV", "MT", "NL", "PL", "PT", "RO", "SE", "SI", "SK", "GR"
    };

    private readonly HttpClient _httpClient;
    private readonly StripeSettings _stripeSettings;

    /// <summary>
    /// Gets provider name.
    /// </summary>
    public string Name => "StripeVatProvider";

    public StripeVatValidationProvider(HttpClient httpClient, StripeSettings stripeSettings)
    {
        _httpClient = httpClient;
        _stripeSettings = stripeSettings;
    }

    /// <summary>
    /// Validates VAT number using Stripe tax ID create/verification lifecycle.
    /// </summary>
    /// <param name="countryCode">Country code.</param>
    /// <param name="taxId">Tax identifier.</param>
    /// <returns>Validation result.</returns>
    public async Task<VatValidationResult> ValidateAsync(string countryCode, string taxId)
    {
        if (string.IsNullOrWhiteSpace(_stripeSettings.SecretKey))
        {
            return new VatValidationResult
            {
                IsValid = false,
                ProviderName = Name,
                RawResponse = "Stripe secret key is not configured"
            };
        }

        if (string.IsNullOrWhiteSpace(countryCode) || string.IsNullOrWhiteSpace(taxId))
        {
            return new VatValidationResult
            {
                IsValid = false,
                ProviderName = Name,
                RawResponse = "Missing country code or tax ID"
            };
        }

        var normalizedCountry = countryCode.Trim().ToUpperInvariant();
        var normalizedTaxId = taxId.Trim().Replace(" ", string.Empty).Replace("-", string.Empty);

        var taxType = ResolveStripeTaxType(normalizedCountry);
        if (taxType is null)
        {
            return new VatValidationResult
            {
                IsValid = false,
                ProviderName = Name,
                RawResponse = $"Stripe VAT validation not supported for country '{normalizedCountry}'"
            };
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _stripeSettings.SecretKey);

        string? customerId = null;

        try
        {
            customerId = await CreateTemporaryCustomerAsync();
            var (isValid, status, responseJson) = await CreateAndVerifyTaxIdAsync(customerId, taxType, normalizedTaxId);

            return new VatValidationResult
            {
                IsValid = isValid,
                ProviderName = Name,
                RawResponse = $"status={status};response={responseJson}"
            };
        }
        catch (Exception ex)
        {
            return new VatValidationResult
            {
                IsValid = false,
                ProviderName = Name,
                RawResponse = $"Stripe validation error: {ex.Message}"
            };
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(customerId))
            {
                await DeleteTemporaryCustomerAsync(customerId);
            }
        }
    }

    private async Task<string> CreateTemporaryCustomerAsync()
    {
        var formData = new Dictionary<string, string>
        {
            ["name"] = "VAT Validation Temporary Customer",
            ["metadata[source]"] = "vat-validation"
        };

        using var response = await _httpClient.PostAsync(
            "/v1/customers",
            new FormUrlEncodedContent(formData));

        var body = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        using var json = JsonDocument.Parse(body);
        return json.RootElement.GetProperty("id").GetString() ?? throw new InvalidOperationException("Stripe customer ID missing");
    }

    private async Task<(bool IsValid, string Status, string ResponseJson)> CreateAndVerifyTaxIdAsync(string customerId, string taxType, string taxId)
    {
        var formData = new Dictionary<string, string>
        {
            ["type"] = taxType,
            ["value"] = taxId
        };

        using var response = await _httpClient.PostAsync(
            $"/v1/customers/{customerId}/tax_ids",
            new FormUrlEncodedContent(formData));

        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            return (false, "error", body);
        }

        using var json = JsonDocument.Parse(body);

        var status = "unknown";
        if (json.RootElement.TryGetProperty("verification", out var verification)
            && verification.TryGetProperty("status", out var statusValue)
            && statusValue.ValueKind == JsonValueKind.String)
        {
            status = statusValue.GetString() ?? "unknown";
        }

        var isValid = string.Equals(status, "verified", StringComparison.OrdinalIgnoreCase)
                      || string.Equals(status, "pending", StringComparison.OrdinalIgnoreCase);

        return (isValid, status, body);
    }

    private async Task DeleteTemporaryCustomerAsync(string customerId)
    {
        using var response = await _httpClient.DeleteAsync($"/v1/customers/{customerId}");
        _ = await response.Content.ReadAsStringAsync();
    }

    private static string? ResolveStripeTaxType(string countryCode)
    {
        if (EuCountries.Contains(countryCode))
        {
            return "eu_vat";
        }

        return countryCode switch
        {
            "GB" => "gb_vat",
            "AU" => "au_abn",
            "NZ" => "nz_gst",
            _ => null
        };
    }
}
