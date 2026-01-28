using ExchangeRateLib.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace ExchangeRateLib.Implementations
{
    public class FrankfurterProvider : BaseExchangeRateProvider
    {
        public FrankfurterProvider(string apiUrl)
            : base(apiUrl)
        {
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public override async Task<ExchangeRateResult> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fromCurrency))
                {
                    return CreateExchangeRateErrorResult("From currency is required", "INVALID_FROM_CURRENCY");
                }

                if (string.IsNullOrWhiteSpace(toCurrency))
                {
                    return CreateExchangeRateErrorResult("To currency is required", "INVALID_TO_CURRENCY");
                }

                var endpoint = $"/latest?from={fromCurrency.ToUpper()}&to={toCurrency.ToUpper()}";
                
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateExchangeRateErrorResult(
                        $"Frankfurter API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                var rate = 0m;
                if (root.TryGetProperty("rates", out var ratesElement) &&
                    ratesElement.TryGetProperty(toCurrency.ToUpper(), out var rateElement))
                {
                    rate = rateElement.GetDecimal();
                }

                var date = root.TryGetProperty("date", out var dateElement)
                    ? DateTime.Parse(dateElement.GetString() ?? DateTime.UtcNow.ToString())
                    : DateTime.UtcNow;

                return new ExchangeRateResult
                {
                    Success = true,
                    Message = "Exchange rate retrieved successfully",
                    FromCurrency = fromCurrency.ToUpper(),
                    ToCurrency = toCurrency.ToUpper(),
                    Rate = rate,
                    Date = date,
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (HttpRequestException ex)
            {
                return CreateExchangeRateErrorResult(
                    $"Network error while connecting to Frankfurter: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateExchangeRateErrorResult(
                    $"Failed to parse Frankfurter response: {ex.Message}",
                    "JSON_PARSE_ERROR"
                );
            }
            catch (Exception ex)
            {
                return CreateExchangeRateErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }

        public override async Task<ExchangeRatesResult> GetExchangeRatesAsync(string baseCurrency, List<string> targetCurrencies)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(baseCurrency))
                {
                    return CreateExchangeRatesErrorResult("Base currency is required", "INVALID_BASE_CURRENCY");
                }

                var symbols = string.Join(",", targetCurrencies.Select(c => c.ToUpper()));
                var endpoint = $"/latest?from={baseCurrency.ToUpper()}&to={symbols}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateExchangeRatesErrorResult(
                        $"Frankfurter API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                var rates = new Dictionary<string, decimal>();

                if (root.TryGetProperty("rates", out var ratesElement))
                {
                    foreach (var rate in ratesElement.EnumerateObject())
                    {
                        rates[rate.Name] = rate.Value.GetDecimal();
                    }
                }

                var date = root.TryGetProperty("date", out var dateElement)
                    ? DateTime.Parse(dateElement.GetString() ?? DateTime.UtcNow.ToString())
                    : DateTime.UtcNow;

                return new ExchangeRatesResult
                {
                    Success = true,
                    Message = "Exchange rates retrieved successfully",
                    BaseCurrency = baseCurrency.ToUpper(),
                    Rates = rates,
                    Date = date,
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return CreateExchangeRatesErrorResult($"Error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<ConversionResult> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            try
            {
                if (amount <= 0)
                {
                    return CreateConversionErrorResult("Amount must be greater than zero", "INVALID_AMOUNT");
                }

                var endpoint = $"/latest?amount={amount}&from={fromCurrency.ToUpper()}&to={toCurrency.ToUpper()}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateConversionErrorResult(
                        $"Frankfurter API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                var result = 0m;
                var rate = 0m;
                if (root.TryGetProperty("rates", out var ratesElement) &&
                    ratesElement.TryGetProperty(toCurrency.ToUpper(), out var rateElement))
                {
                    result = rateElement.GetDecimal();
                    rate = result / amount;
                }

                var date = root.TryGetProperty("date", out var dateElement)
                    ? DateTime.Parse(dateElement.GetString() ?? DateTime.UtcNow.ToString())
                    : DateTime.UtcNow;

                return new ConversionResult
                {
                    Success = true,
                    Message = "Currency conversion successful",
                    FromCurrency = fromCurrency.ToUpper(),
                    ToCurrency = toCurrency.ToUpper(),
                    FromAmount = amount,
                    ToAmount = result,
                    Rate = rate,
                    Date = date,
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return CreateConversionErrorResult($"Error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<ExchangeRateResult> GetHistoricalRateAsync(string fromCurrency, string toCurrency, DateTime date)
        {
            try
            {
                var dateStr = date.ToString("yyyy-MM-dd");
                var endpoint = $"/{dateStr}?from={fromCurrency.ToUpper()}&to={toCurrency.ToUpper()}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateExchangeRateErrorResult(
                        $"Frankfurter API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                var rate = 0m;
                if (root.TryGetProperty("rates", out var ratesElement) &&
                    ratesElement.TryGetProperty(toCurrency.ToUpper(), out var rateElement))
                {
                    rate = rateElement.GetDecimal();
                }

                return new ExchangeRateResult
                {
                    Success = true,
                    Message = "Historical exchange rate retrieved successfully",
                    FromCurrency = fromCurrency.ToUpper(),
                    ToCurrency = toCurrency.ToUpper(),
                    Rate = rate,
                    Date = date,
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return CreateExchangeRateErrorResult($"Error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<TimeSeriesResult> GetTimeSeriesAsync(string baseCurrency, string targetCurrency, DateTime startDate, DateTime endDate)
        {
            try
            {
                var startDateStr = startDate.ToString("yyyy-MM-dd");
                var endDateStr = endDate.ToString("yyyy-MM-dd");
                var endpoint = $"/{startDateStr}..{endDateStr}?from={baseCurrency.ToUpper()}&to={targetCurrency.ToUpper()}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateTimeSeriesErrorResult(
                        $"Frankfurter API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                var rates = new Dictionary<DateTime, decimal>();

                if (root.TryGetProperty("rates", out var ratesElement))
                {
                    foreach (var dateRate in ratesElement.EnumerateObject())
                    {
                        var date = DateTime.Parse(dateRate.Name);
                        if (dateRate.Value.TryGetProperty(targetCurrency.ToUpper(), out var rateElement))
                        {
                            rates[date] = rateElement.GetDecimal();
                        }
                    }
                }

                return new TimeSeriesResult
                {
                    Success = true,
                    Message = "Time series data retrieved successfully",
                    BaseCurrency = baseCurrency.ToUpper(),
                    TargetCurrency = targetCurrency.ToUpper(),
                    StartDate = startDate,
                    EndDate = endDate,
                    Rates = rates
                };
            }
            catch (Exception ex)
            {
                return CreateTimeSeriesErrorResult($"Error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<SupportedCurrenciesResult> GetSupportedCurrenciesAsync()
        {
            try
            {
                var endpoint = "/currencies";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateSupportedCurrenciesErrorResult(
                        $"Frankfurter API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                var currencies = new Dictionary<string, string>();

                foreach (var currency in root.EnumerateObject())
                {
                    currencies[currency.Name] = currency.Value.GetString() ?? currency.Name;
                }

                return new SupportedCurrenciesResult
                {
                    Success = true,
                    Message = "Supported currencies retrieved successfully",
                    Currencies = currencies
                };
            }
            catch (Exception ex)
            {
                return CreateSupportedCurrenciesErrorResult($"Error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<ExchangeRatesResult> GetLatestRatesAsync(string baseCurrency)
        {
            try
            {
                var endpoint = $"/latest?from={baseCurrency.ToUpper()}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateExchangeRatesErrorResult(
                        $"Frankfurter API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                var rates = new Dictionary<string, decimal>();

                if (root.TryGetProperty("rates", out var ratesElement))
                {
                    foreach (var rate in ratesElement.EnumerateObject())
                    {
                        rates[rate.Name] = rate.Value.GetDecimal();
                    }
                }

                var date = root.TryGetProperty("date", out var dateElement)
                    ? DateTime.Parse(dateElement.GetString() ?? DateTime.UtcNow.ToString())
                    : DateTime.UtcNow;

                return new ExchangeRatesResult
                {
                    Success = true,
                    Message = "Latest exchange rates retrieved successfully",
                    BaseCurrency = baseCurrency.ToUpper(),
                    Rates = rates,
                    Date = date,
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return CreateExchangeRatesErrorResult($"Error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }
    }
}
