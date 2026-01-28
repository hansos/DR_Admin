# Exchange Rate Download Logging System

## Overview

This system tracks all exchange rate download operations in the database, providing full audit trail and monitoring capabilities for currency rate updates.

## Database Schema

### ExchangeRateDownloadLogs Table

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| BaseCurrency | string | Base currency code (e.g., "EUR") |
| TargetCurrency | string? | Target currency code (null for bulk downloads) |
| Source | CurrencyRateSource | Provider/source of the rate |
| Success | bool | Whether the download succeeded |
| DownloadTimestamp | DateTime | When the download occurred |
| RatesDownloaded | int | Number of rates fetched |
| RatesAdded | int | Number of new rates added to DB |
| RatesUpdated | int | Number of rates updated in DB |
| ErrorMessage | string? | Error message if failed |
| ErrorCode | string? | Error code if failed |
| DurationMs | long | Download duration in milliseconds |
| Notes | string? | Additional notes |
| IsStartupDownload | bool | Whether triggered by app startup |
| IsScheduledDownload | bool | Whether scheduled/automatic |
| CreatedAt | DateTime | Record creation timestamp |
| UpdatedAt | DateTime | Record update timestamp |

## API Endpoints

### GET /api/exchange-rate-download-logs
Get all download logs (ordered by most recent first)

**Response**: `200 OK`
```json
[
  {
    "id": 1,
    "baseCurrency": "EUR",
    "targetCurrency": "USD",
    "source": 6,
    "sourceName": "ExchangeRateHost",
    "success": true,
    "downloadTimestamp": "2024-01-15T10:30:00Z",
    "ratesDownloaded": 1,
    "ratesAdded": 0,
    "ratesUpdated": 1,
    "errorMessage": null,
    "errorCode": null,
    "durationMs": 1250,
    "notes": "Total rates in batch: 168",
    "isStartupDownload": false,
    "isScheduledDownload": true,
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z"
  }
]
```

### GET /api/exchange-rate-download-logs/period?startDate={start}&endDate={end}
Get download logs for a specific time period

**Query Parameters:**
- `startDate` (DateTime, required): Start of period (ISO 8601)
- `endDate` (DateTime, required): End of period (ISO 8601)

**Example:**
```
GET /api/exchange-rate-download-logs/period?startDate=2024-01-01T00:00:00Z&endDate=2024-01-31T23:59:59Z
```

**Response**: `200 OK` (same format as Get All)

### GET /api/exchange-rate-download-logs/{id}
Get a specific download log by ID

**Response**: `200 OK` or `404 Not Found`

### GET /api/exchange-rate-download-logs/by-source/{source}
Get download logs by source/provider

**Path Parameters:**
- `source` (CurrencyRateSource enum): 6 = ExchangeRateHost, 7 = Frankfurter, etc.

**Example:**
```
GET /api/exchange-rate-download-logs/by-source/6
```

### GET /api/exchange-rate-download-logs/by-currency/{baseCurrency}?targetCurrency={target}
Get download logs by currency pair

**Path Parameters:**
- `baseCurrency` (string, required): Base currency code

**Query Parameters:**
- `targetCurrency` (string, optional): Target currency code

**Examples:**
```
GET /api/exchange-rate-download-logs/by-currency/EUR
GET /api/exchange-rate-download-logs/by-currency/EUR?targetCurrency=USD
```

### GET /api/exchange-rate-download-logs/summary?startDate={start}&endDate={end}
Get summary statistics for downloads

**Query Parameters:**
- `startDate` (DateTime, optional): Start of period
- `endDate` (DateTime, optional): End of period

**Response**: `200 OK`
```json
{
  "totalDownloads": 500,
  "successfulDownloads": 485,
  "failedDownloads": 15,
  "totalRatesDownloaded": 84000,
  "totalRatesAdded": 1200,
  "totalRatesUpdated": 3500,
  "lastSuccessfulDownload": "2024-01-15T10:30:00Z",
  "lastFailedDownload": "2024-01-14T15:22:00Z",
  "averageDurationMs": 1350,
  "successRate": 97.0
}
```

### GET /api/exchange-rate-download-logs/failed?startDate={start}&endDate={end}
Get only failed download logs

**Query Parameters:**
- `startDate` (DateTime, optional): Start of period
- `endDate` (DateTime, optional): End of period

### GET /api/exchange-rate-download-logs/last-download/{baseCurrency}/{source}?targetCurrency={target}
Get the last successful download for a currency pair and source

**Path Parameters:**
- `baseCurrency` (string, required): Base currency code
- `source` (CurrencyRateSource enum, required): Provider source

**Query Parameters:**
- `targetCurrency` (string, optional): Target currency code

**Example:**
```
GET /api/exchange-rate-download-logs/last-download/EUR/6?targetCurrency=USD
```

**Response**: `200 OK` or `404 Not Found`

### POST /api/exchange-rate-download-logs
Create a new download log entry (for manual logging)

**Request Body:**
```json
{
  "baseCurrency": "EUR",
  "targetCurrency": "USD",
  "source": 6,
  "success": true,
  "downloadTimestamp": "2024-01-15T10:30:00Z",
  "ratesDownloaded": 1,
  "ratesAdded": 0,
  "ratesUpdated": 1,
  "errorMessage": null,
  "errorCode": null,
  "durationMs": 1250,
  "notes": "Manual entry",
  "isStartupDownload": false,
  "isScheduledDownload": false
}
```

**Response**: `201 Created`

## Usage in Background Service

The `ExchangeRateUpdateService` automatically logs every download operation:

### Per-Currency Logging
For each currency downloaded, a log entry is created with:
- Individual currency pair (EUR/USD, EUR/GBP, etc.)
- Success/failure status
- Number of rates for that currency
- Whether it was added or updated

### Bulk Download Logging
A summary log is also created with:
- `TargetCurrency = null` (indicates bulk download)
- Total rates downloaded
- Total rates added across all currencies
- Total rates updated across all currencies

### Failure Logging
When downloads fail:
- Error message and code are captured
- Partial success is tracked (rates downloaded before failure)
- Each failed currency gets its own log entry

## Monitoring and Querying

### SQL Queries

#### Check Recent Downloads
```sql
SELECT 
    DownloadTimestamp,
    BaseCurrency,
    TargetCurrency,
    Source,
    Success,
    RatesDownloaded,
    RatesAdded,
    RatesUpdated,
    DurationMs
FROM ExchangeRateDownloadLogs
WHERE DownloadTimestamp >= DATETIME('now', '-1 day')
ORDER BY DownloadTimestamp DESC;
```

#### Success Rate by Provider
```sql
SELECT 
    Source,
    COUNT(*) as TotalDownloads,
    SUM(CASE WHEN Success = 1 THEN 1 ELSE 0 END) as SuccessfulDownloads,
    ROUND(CAST(SUM(CASE WHEN Success = 1 THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*) * 100, 2) as SuccessRate
FROM ExchangeRateDownloadLogs
GROUP BY Source;
```

#### Average Download Time by Currency
```sql
SELECT 
    TargetCurrency,
    COUNT(*) as Downloads,
    AVG(DurationMs) as AvgDurationMs,
    MIN(DurationMs) as MinDurationMs,
    MAX(DurationMs) as MaxDurationMs
FROM ExchangeRateDownloadLogs
WHERE Success = 1 AND TargetCurrency IS NOT NULL
GROUP BY TargetCurrency
ORDER BY AvgDurationMs DESC;
```

#### Failed Downloads with Errors
```sql
SELECT 
    DownloadTimestamp,
    BaseCurrency,
    TargetCurrency,
    Source,
    ErrorCode,
    ErrorMessage
FROM ExchangeRateDownloadLogs
WHERE Success = 0
ORDER BY DownloadTimestamp DESC
LIMIT 50;
```

#### Download Activity by Hour
```sql
SELECT 
    strftime('%Y-%m-%d %H:00', DownloadTimestamp) as Hour,
    COUNT(*) as Downloads,
    SUM(RatesDownloaded) as TotalRates
FROM ExchangeRateDownloadLogs
WHERE DownloadTimestamp >= DATETIME('now', '-7 days')
GROUP BY Hour
ORDER BY Hour DESC;
```

## Integration with Download Logic

### Check if Download is Needed

The service provides a method to check if a download is needed based on the last successful download:

```csharp
var isNeeded = await _logService.IsDownloadNeededAsync(
    baseCurrency: "EUR",
    targetCurrency: "USD",
    source: CurrencyRateSource.ExchangeRateHost,
    minTimeBetweenDownloads: TimeSpan.FromHours(1)
);

if (isNeeded)
{
    // Perform download
}
```

### Get Last Download

```csharp
var lastDownload = await _logService.GetLastDownloadAsync(
    baseCurrency: "EUR",
    targetCurrency: null, // null = bulk download
    source: CurrencyRateSource.ExchangeRateHost
);

if (lastDownload != null)
{
    Console.WriteLine($"Last download: {lastDownload.DownloadTimestamp}");
    Console.WriteLine($"Rates downloaded: {lastDownload.RatesDownloaded}");
}
```

## Dashboard Metrics

Use the summary endpoint to build dashboards:

```javascript
// Fetch summary for last 30 days
const endDate = new Date().toISOString();
const startDate = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString();

const response = await fetch(
  `/api/exchange-rate-download-logs/summary?startDate=${startDate}&endDate=${endDate}`
);

const summary = await response.json();

// Display metrics
console.log(`Success Rate: ${summary.successRate.toFixed(2)}%`);
console.log(`Total Downloads: ${summary.totalDownloads}`);
console.log(`Average Duration: ${summary.averageDurationMs}ms`);
```

## Troubleshooting

### High Failure Rate
1. Query failed logs: `GET /api/exchange-rate-download-logs/failed`
2. Check error messages and codes
3. Verify API credentials and connectivity
4. Check provider service status

### Slow Downloads
1. Query summary to see average duration
2. Check for specific currencies that are slow
3. Consider reducing `TargetCurrencies` list
4. Check network latency to provider

### Missing Downloads
1. Check if `UpdateOnStartup` is enabled
2. Verify `HoursBetweenUpdates` allows downloads
3. Check `MaxUpdatesPerDay` hasn't been exceeded
4. Review application logs for errors

## CurrencyRateSource Enum Values

| Value | Name | Description |
|-------|------|-------------|
| 0 | Manual | Manually entered |
| 1 | ECB | European Central Bank |
| 2 | OpenExchangeRates | Open Exchange Rates API |
| 3 | Fixer | Fixer.io API |
| 4 | CurrencyLayer | CurrencyLayer API |
| 5 | XE | XE.com API |
| 6 | ExchangeRateHost | ExchangeRate.host API |
| 7 | Frankfurter | Frankfurter.app API |
| 8 | OANDA | OANDA API |
| 99 | Other | Other/custom source |

## Database Migration

To create the database table, run:

```bash
# From DR_Admin project directory
dotnet ef migrations add AddExchangeRateDownloadLog
dotnet ef database update
```

## Performance Considerations

- **Index Recommendations**:
  - `DownloadTimestamp` (for period queries)
  - `Source` (for provider filtering)
  - `Success` (for failure queries)
  - Composite: `(BaseCurrency, TargetCurrency, Source, DownloadTimestamp)` (for last download queries)

- **Retention Policy**: Consider implementing data retention:
  ```sql
  -- Delete logs older than 1 year
  DELETE FROM ExchangeRateDownloadLogs 
  WHERE DownloadTimestamp < DATETIME('now', '-1 year');
  ```

- **Bulk Inserts**: The background service uses batching for efficiency

## Security

- All endpoints require authentication (`[Authorize]` attribute)
- Sensitive data (API keys) are never logged
- Error messages are sanitized before storage
