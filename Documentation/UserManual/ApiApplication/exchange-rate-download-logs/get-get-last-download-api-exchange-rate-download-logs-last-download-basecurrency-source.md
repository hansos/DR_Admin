# GET GetLastDownload

Gets the last successful download for a specific currency pair and source

## Endpoint

```
GET /api/exchange-rate-download-logs/last-download/{baseCurrency}/{source}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `baseCurrency` | Route | `string` |
| `source` | Route | `CurrencyRateSource` |
| `targetCurrency` | Query | `string?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `ExchangeRateDownloadLogDto` |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
