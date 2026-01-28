using ExchangeRateLib.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace ExchangeRateLib.Implementations
{
    public class ExchangeRateHostProvider : BaseExchangeRateProvider
    {
        private readonly string? _apiKey;

        public ExchangeRateHostProvider(string apiUrl, string? apiKey = null)
            : base(apiUrl)
        {
            _apiKey = apiKey;

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

                var endpoint = $"/convert?from={fromCurrency.ToUpper()}&to={toCurrency.ToUpper()}";
                
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateExchangeRateErrorResult(
                        $"ExchangeRate.host API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var rate = root.TryGetProperty("info", out var infoElement) &&
                               infoElement.TryGetProperty("rate", out var rateElement)
                        ? rateElement.GetDecimal()
                        : 0m;

                    var timestamp = root.TryGetProperty("date", out var dateElement)
                        ? DateTime.Parse(dateElement.GetString() ?? DateTime.UtcNow.ToString())
                        : DateTime.UtcNow;

                    return new ExchangeRateResult
                    {
                        Success = true,
                        Message = "Exchange rate retrieved successfully",
                        FromCurrency = fromCurrency.ToUpper(),
                        ToCurrency = toCurrency.ToUpper(),
                        Rate = rate,
                        Date = timestamp,
                        Timestamp = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.TryGetProperty("info", out var infoElement)
                        ? infoElement.GetString() ?? "Unknown error occurred"
                        : "Unknown error occurred";
                    return CreateExchangeRateErrorResult(errorMessage, "API_ERROR");
                }

                return CreateExchangeRateErrorResult("Invalid response format from ExchangeRate.host", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateExchangeRateErrorResult(
                    $"Network error while connecting to ExchangeRate.host: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateExchangeRateErrorResult(
                    $"Failed to parse ExchangeRate.host response: {ex.Message}",
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
                var endpoint = $"/latest?base={baseCurrency.ToUpper()}&symbols={symbols}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateExchangeRatesErrorResult(
                        $"ExchangeRate.host API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var rates = new Dictionary<string, decimal>();

                    if (root.TryGetProperty("rates", out var ratesElement))
                    {
                        foreach (var rate in ratesElement.EnumerateObject())
                        {
                            rates[rate.Name] = rate.Value.GetDecimal();
                        }
                    }

                    var timestamp = root.TryGetProperty("date", out var dateElement)
                        ? DateTime.Parse(dateElement.GetString() ?? DateTime.UtcNow.ToString())
                        : DateTime.UtcNow;

                    return new ExchangeRatesResult
                    {
                        Success = true,
                        Message = "Exchange rates retrieved successfully",
                        BaseCurrency = baseCurrency.ToUpper(),
                        Rates = rates,
                        Date = timestamp,
                        Timestamp = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.TryGetProperty("info", out var infoElement)
                        ? infoElement.GetString() ?? "Unknown error occurred"
                        : "Unknown error occurred";
                    return CreateExchangeRatesErrorResult(errorMessage, "API_ERROR");
                }

                return CreateExchangeRatesErrorResult("Invalid response format from ExchangeRate.host", "INVALID_RESPONSE");
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

                var endpoint = $"/convert?from={fromCurrency.ToUpper()}&to={toCurrency.ToUpper()}&amount={amount}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateConversionErrorResult(
                        $"ExchangeRate.host API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var result = root.TryGetProperty("result", out var resultElement)
                        ? resultElement.GetDecimal()
                        : 0m;

                    var rate = root.TryGetProperty("info", out var infoElement) &&
                               infoElement.TryGetProperty("rate", out var rateElement)
                        ? rateElement.GetDecimal()
                        : 0m;

                    var timestamp = root.TryGetProperty("date", out var dateElement)
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
                        Date = timestamp,
                        Timestamp = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.TryGetProperty("info", out var infoElement)
                        ? infoElement.GetString() ?? "Unknown error occurred"
                        : "Unknown error occurred";
                    return CreateConversionErrorResult(errorMessage, "API_ERROR");
                }

                return CreateConversionErrorResult("Invalid response format from ExchangeRate.host", "INVALID_RESPONSE");
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
                var endpoint = $"/{dateStr}?base={fromCurrency.ToUpper()}&symbols={toCurrency.ToUpper()}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateExchangeRateErrorResult(
                        $"ExchangeRate.host API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
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
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.TryGetProperty("info", out var infoElement)
                        ? infoElement.GetString() ?? "Unknown error occurred"
                        : "Unknown error occurred";
                    return CreateExchangeRateErrorResult(errorMessage, "API_ERROR");
                }

                return CreateExchangeRateErrorResult("Invalid response format from ExchangeRate.host", "INVALID_RESPONSE");
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
                var endpoint = $"/timeseries?start_date={startDateStr}&end_date={endDateStr}&base={baseCurrency.ToUpper()}&symbols={targetCurrency.ToUpper()}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateTimeSeriesErrorResult(
                        $"ExchangeRate.host API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
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
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.TryGetProperty("info", out var infoElement)
                        ? infoElement.GetString() ?? "Unknown error occurred"
                        : "Unknown error occurred";
                    return CreateTimeSeriesErrorResult(errorMessage, "API_ERROR");
                }

                return CreateTimeSeriesErrorResult("Invalid response format from ExchangeRate.host", "INVALID_RESPONSE");
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
                var endpoint = "/symbols";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateSupportedCurrenciesErrorResult(
                        $"ExchangeRate.host API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var currencies = new Dictionary<string, string>();

                    if (root.TryGetProperty("symbols", out var symbolsElement))
                    {
                        foreach (var symbol in symbolsElement.EnumerateObject())
                        {
                            currencies[symbol.Name] = symbol.Value.GetString() ?? symbol.Name;
                        }
                    }

                    return new SupportedCurrenciesResult
                    {
                        Success = true,
                        Message = "Supported currencies retrieved successfully",
                        Currencies = currencies
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.TryGetProperty("info", out var infoElement)
                        ? infoElement.GetString() ?? "Unknown error occurred"
                        : "Unknown error occurred";
                    return CreateSupportedCurrenciesErrorResult(errorMessage, "API_ERROR");
                }

                return CreateSupportedCurrenciesErrorResult("Invalid response format from ExchangeRate.host", "INVALID_RESPONSE");
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
                var endpoint = $"/latest?base={baseCurrency.ToUpper()}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateExchangeRatesErrorResult(
                        $"ExchangeRate.host API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var rates = new Dictionary<string, decimal>();

                    if (root.TryGetProperty("rates", out var ratesElement))
                    {
                        foreach (var rate in ratesElement.EnumerateObject())
                        {
                            rates[rate.Name] = rate.Value.GetDecimal();
                        }
                    }

                    var timestamp = root.TryGetProperty("date", out var dateElement)
                        ? DateTime.Parse(dateElement.GetString() ?? DateTime.UtcNow.ToString())
                        : DateTime.UtcNow;

                    return new ExchangeRatesResult
                    {
                        Success = true,
                        Message = "Latest exchange rates retrieved successfully",
                        BaseCurrency = baseCurrency.ToUpper(),
                        Rates = rates,
                        Date = timestamp,
                        Timestamp = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.TryGetProperty("info", out var infoElement)
                        ? infoElement.GetString() ?? "Unknown error occurred"
                        : "Unknown error occurred";
                    return CreateExchangeRatesErrorResult(errorMessage, "API_ERROR");
                }

                return CreateExchangeRatesErrorResult("Invalid response format from ExchangeRate.host", "INVALID_RESPONSE");
            }
            catch (Exception ex)
            {
                return CreateExchangeRatesErrorResult($"Error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }
    }
}
