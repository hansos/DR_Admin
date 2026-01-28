# ExchangeRateLib

A .NET library for integrating with multiple exchange rate API providers.

## Features

- Support for multiple exchange rate providers
- Common interface for all providers
- Factory pattern for easy provider instantiation
- Comprehensive result models
- Full implementation for ExchangeRate.host and Frankfurter.app
- Placeholder implementations for future providers

## Supported Providers

### Fully Implemented
1. **ExchangeRate.host** (`exchangeratehost`)
   - Free and open exchange rate API
   - Supports historical rates, time series, and currency conversion
   
2. **Frankfurter.app** (`frankfurter`)
   - Free API based on European Central Bank data
   - Supports historical rates, time series, and currency conversion

### Placeholder Implementations (Coming Soon)
3. **Open Exchange Rates** (`openexchangerates`)
4. **CurrencyLayer** (`currencylayer`)
5. **Fixer.io** (`fixer`)
6. **XE** (`xe`)
7. **OANDA** (`oanda`)

## Usage

### Configuration

Add the following to your `appsettings.json`:

```json
{
  "ExchangeRate": {
    "Provider": "frankfurter",
    "Frankfurter": {
      "ApiUrl": "https://api.frankfurter.app",
      "UseHttps": true
    }
  }
}
```

Or for ExchangeRate.host:

```json
{
  "ExchangeRate": {
    "Provider": "exchangeratehost",
    "ExchangeRateHost": {
      "ApiUrl": "https://api.exchangerate.host",
      "ApiKey": "your-api-key-if-needed",
      "UseHttps": true
    }
  }
}
```

### Basic Usage

```csharp
using ExchangeRateLib.Factories;
using ExchangeRateLib.Infrastructure.Settings;

// Configure settings
var settings = new ExchangeRateSettings
{
    Provider = "frankfurter",
    Frankfurter = new FrankfurterSettings
    {
        ApiUrl = "https://api.frankfurter.app"
    }
};

// Create factory and provider
var factory = new ExchangeRateFactory(settings);
var provider = factory.CreateProvider();

// Get exchange rate
var result = await provider.GetExchangeRateAsync("USD", "EUR");
if (result.Success)
{
    Console.WriteLine($"1 {result.FromCurrency} = {result.Rate} {result.ToCurrency}");
}

// Convert currency
var conversion = await provider.ConvertCurrencyAsync(100, "USD", "EUR");
if (conversion.Success)
{
    Console.WriteLine($"{conversion.FromAmount} {conversion.FromCurrency} = {conversion.ToAmount} {conversion.ToCurrency}");
}

// Get historical rate
var historical = await provider.GetHistoricalRateAsync("USD", "EUR", new DateTime(2024, 1, 1));

// Get time series
var timeSeries = await provider.GetTimeSeriesAsync("USD", "EUR", 
    new DateTime(2024, 1, 1), 
    new DateTime(2024, 1, 31));

// Get supported currencies
var currencies = await provider.GetSupportedCurrenciesAsync();

// Get all latest rates for a base currency
var allRates = await provider.GetLatestRatesAsync("USD");
```

## Project Structure

```
ExchangeRateLib/
??? Interfaces/
?   ??? IExchangeRateProvider.cs
??? Implementations/
?   ??? BaseExchangeRateProvider.cs
?   ??? ExchangeRateHostProvider.cs
?   ??? FrankfurterProvider.cs
?   ??? OpenExchangeRatesProvider.cs (placeholder)
?   ??? CurrencyLayerProvider.cs (placeholder)
?   ??? FixerProvider.cs (placeholder)
?   ??? XeProvider.cs (placeholder)
?   ??? OandaProvider.cs (placeholder)
??? Models/
?   ??? ExchangeRateResult.cs
?   ??? ExchangeRatesResult.cs
?   ??? ConversionResult.cs
?   ??? TimeSeriesResult.cs
?   ??? SupportedCurrenciesResult.cs
??? Infrastructure/
?   ??? Settings/
?       ??? ExchangeRateSettings.cs
?       ??? ExchangeRateHostSettings.cs
?       ??? FrankfurterSettings.cs
?       ??? OpenExchangeRatesSettings.cs
?       ??? CurrencyLayerSettings.cs
?       ??? FixerSettings.cs
?       ??? XeSettings.cs
?       ??? OandaSettings.cs
??? Factories/
    ??? ExchangeRateFactory.cs
```

## API Methods

All providers implement the following interface:

- `GetExchangeRateAsync(fromCurrency, toCurrency)` - Get current exchange rate between two currencies
- `GetExchangeRatesAsync(baseCurrency, targetCurrencies)` - Get exchange rates for multiple target currencies
- `ConvertCurrencyAsync(amount, fromCurrency, toCurrency)` - Convert an amount between currencies
- `GetHistoricalRateAsync(fromCurrency, toCurrency, date)` - Get historical exchange rate for a specific date
- `GetTimeSeriesAsync(baseCurrency, targetCurrency, startDate, endDate)` - Get time series data
- `GetSupportedCurrenciesAsync()` - Get list of supported currencies
- `GetLatestRatesAsync(baseCurrency)` - Get latest rates for all currencies

## Error Handling

All methods return result objects with the following properties:
- `Success` - Indicates if the operation was successful
- `Message` - Human-readable message
- `ErrorCode` - Machine-readable error code (if applicable)
- `Errors` - List of error messages

Example:

```csharp
var result = await provider.GetExchangeRateAsync("USD", "EUR");
if (!result.Success)
{
    Console.WriteLine($"Error: {result.Message}");
    Console.WriteLine($"Error Code: {result.ErrorCode}");
}
```

## Notes

- ExchangeRate.host and Frankfurter.app are fully implemented and ready to use
- Other providers have placeholder implementations that return "NOT_IMPLEMENTED" errors
- All providers follow the same pattern for easy implementation
- The library uses modern .NET patterns including async/await and nullable reference types
