# Exchange Rate Background Service

## Overview

The `ExchangeRateUpdateService` is a background service that automatically updates currency exchange rates in the database from configured external API providers. It runs continuously in the background and updates rates based on configurable schedules.

## Features

- ? **Automatic Updates**: Fetches latest exchange rates from configured provider
- ? **Startup Update**: Option to update rates when application starts
- ? **Configurable Schedule**: Control update frequency and daily limits
- ? **Multiple Providers**: Support for multiple exchange rate APIs
- ? **Rate History**: Maintains historical exchange rates by expiring old rates
- ? **Smart Updates**: Only creates new entries when rates actually change
- ? **Logging**: Comprehensive logging of all operations

## Configuration

Add the following configuration to your `appsettings.json` or `appsettings.Development.json`:

```json
{
  "ExchangeRate": {
    "Provider": "exchangeratehost",
    "ExchangeRateHost": {
      "ApiUrl": "https://api.exchangerate.host",
      "ApiKey": null,
      "UseHttps": true
    },
    "MaxUpdatesPerDay": 24,
    "HoursBetweenUpdates": 1,
    "UpdateOnStartup": true,
    "BaseCurrency": "EUR",
    "TargetCurrencies": []
  }
}
```

### Configuration Options

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `Provider` | string | - | Exchange rate provider to use (`exchangeratehost`, `frankfurter`, etc.) |
| `MaxUpdatesPerDay` | int | 24 | Maximum number of updates per day (0 = unlimited) |
| `HoursBetweenUpdates` | int | 1 | Minimum hours between each update |
| `UpdateOnStartup` | bool | true | Whether to fetch rates when application starts |
| `BaseCurrency` | string | "EUR" | Base currency for exchange rates (ISO 4217 code) |
| `TargetCurrencies` | array | [] | List of target currencies to fetch (empty = all available) |

### Provider-Specific Settings

Each provider has its own settings section. See `ExchangeRateLib` documentation for details.

## How It Works

### 1. Startup
When the application starts, if `UpdateOnStartup` is `true`, the service immediately fetches the latest exchange rates.

### 2. Periodic Updates
The service checks every 30 minutes whether it should perform an update based on:
- Time since last update (must be >= `HoursBetweenUpdates`)
- Number of updates today (must be < `MaxUpdatesPerDay`)

### 3. Rate Updates
When updating rates:
1. Fetches latest rates from configured provider
2. For each currency pair:
   - If rate exists and hasn't changed: No action
   - If rate exists and has changed:
     - Expires the old rate (sets `ExpiryDate` and `IsActive = false`)
     - Creates new rate entry with current timestamp
   - If rate doesn't exist:
     - Creates new rate entry

### 4. Database Storage
Exchange rates are stored in the `CurrencyExchangeRates` table with:
- `BaseCurrency`: The base currency (e.g., "EUR")
- `TargetCurrency`: The target currency (e.g., "USD", "GBP")
- `Rate`: The exchange rate value
- `EffectiveDate`: When this rate became effective
- `ExpiryDate`: When this rate was superseded (null if still active)
- `Source`: The provider that supplied this rate
- `IsActive`: Whether this is the current active rate
- `Markup`: Optional markup percentage (default: 0)

## Examples

### Example 1: Update Every Hour, Maximum 24 Times per Day
```json
{
  "ExchangeRate": {
    "Provider": "exchangeratehost",
    "MaxUpdatesPerDay": 24,
    "HoursBetweenUpdates": 1,
    "UpdateOnStartup": true
  }
}
```

### Example 2: Update Every 4 Hours, Unlimited Updates
```json
{
  "ExchangeRate": {
    "Provider": "frankfurter",
    "MaxUpdatesPerDay": 0,
    "HoursBetweenUpdates": 4,
    "UpdateOnStartup": true
  }
}
```

### Example 3: Update Specific Currencies Only
```json
{
  "ExchangeRate": {
    "Provider": "exchangeratehost",
    "BaseCurrency": "EUR",
    "TargetCurrencies": ["USD", "GBP", "JPY", "CHF", "SEK", "NOK", "DKK"],
    "HoursBetweenUpdates": 1,
    "UpdateOnStartup": true
  }
}
```

### Example 4: Multiple Updates per Day with Rate Limits
```json
{
  "ExchangeRate": {
    "Provider": "openexchangerates",
    "OpenExchangeRates": {
      "ApiUrl": "https://openexchangerates.org/api",
      "ApiKey": "your-api-key-here"
    },
    "MaxUpdatesPerDay": 5,
    "HoursBetweenUpdates": 4,
    "UpdateOnStartup": false
  }
}
```

## Logging

The service logs all important operations:

- **Information Level**:
  - Service start/stop
  - Update triggers and reasons
  - Update completion with statistics
  
- **Debug Level**:
  - Individual rate updates
  - Daily counter resets
  - Rate change details

- **Error Level**:
  - API failures
  - Database errors
  - Unexpected exceptions

Example log output:
```
[10:00:00 INF] Exchange Rate Update Service starting
[10:00:00 INF] UpdateOnStartup is enabled, updating exchange rates immediately
[10:00:00 INF] Starting exchange rate update from provider: exchangeratehost
[10:00:02 INF] Fetched 168 exchange rates from exchangeratehost
[10:00:03 INF] Exchange rate update completed: 45 added, 12 updated, 114 total changes saved
[11:00:01 INF] Triggering exchange rate update: Scheduled update (hours since last: 1.00, updates today: 1/24)
```

## Monitoring

### Check Last Update Time
Query the database to see when rates were last updated:

```sql
SELECT 
    Source,
    COUNT(*) as RateCount,
    MAX(EffectiveDate) as LastUpdate,
    COUNT(CASE WHEN IsActive = 1 THEN 1 END) as ActiveRates
FROM CurrencyExchangeRates
GROUP BY Source
ORDER BY LastUpdate DESC;
```

### View Active Rates
```sql
SELECT 
    BaseCurrency,
    TargetCurrency,
    Rate,
    EffectiveDate,
    Source
FROM CurrencyExchangeRates
WHERE IsActive = 1
ORDER BY BaseCurrency, TargetCurrency;
```

### Check Rate History
```sql
SELECT 
    BaseCurrency,
    TargetCurrency,
    Rate,
    EffectiveDate,
    ExpiryDate,
    Source,
    IsActive
FROM CurrencyExchangeRates
WHERE BaseCurrency = 'EUR' AND TargetCurrency = 'USD'
ORDER BY EffectiveDate DESC;
```

## Troubleshooting

### Rates Not Updating
1. Check application logs for errors
2. Verify provider API is accessible
3. Ensure API credentials are correct (if required)
4. Check `MaxUpdatesPerDay` hasn't been exceeded
5. Verify `HoursBetweenUpdates` allows updates

### Too Many API Calls
1. Increase `HoursBetweenUpdates`
2. Reduce `MaxUpdatesPerDay`
3. Set `UpdateOnStartup` to `false`

### Missing Currencies
1. Check provider supports those currencies
2. Verify `TargetCurrencies` list includes them
3. Check provider API limits

## Performance

- **API Calls**: Configurable via `MaxUpdatesPerDay` and `HoursBetweenUpdates`
- **Database Impact**: Only creates records when rates change
- **Memory**: Minimal - processes rates in batches
- **Background Thread**: Runs on separate thread pool, doesn't block main application

## Integration with Currency Service

The background service populates the `CurrencyExchangeRates` table. Your application can use the `ICurrencyService` to:

```csharp
// Get active exchange rate
var rate = await currencyService.GetActiveExchangeRateAsync("EUR", "USD");

// Convert currency
var converted = await currencyService.ConvertCurrencyAsync(100, "EUR", "USD");

// Get all active rates
var allRates = await currencyService.GetActiveRatesAsync();
```

## Migration Notes

If upgrading from a previous version without this background service:

1. Existing manual rates will not be affected
2. Automatic rates will be added with appropriate `Source` value
3. Multiple sources can coexist in the database
4. Each source maintains its own active/expired rates

## Security Considerations

1. **API Keys**: Store sensitive API keys in user secrets or environment variables, not in appsettings.json
2. **Rate Limiting**: Configure appropriate limits to avoid exceeding provider quotas
3. **Database Access**: Ensure background service has appropriate database permissions

## Future Enhancements

Potential future improvements:
- Webhook support for real-time rate updates
- Fallback providers if primary fails
- Rate comparison across multiple providers
- Alert notifications for significant rate changes
- Configurable markup rules per currency pair
