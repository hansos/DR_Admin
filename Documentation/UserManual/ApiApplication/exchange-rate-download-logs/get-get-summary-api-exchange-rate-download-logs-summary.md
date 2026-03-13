# GET GetSummary

Gets exchange rate download logs by source/provider

## Endpoint

```
GET /api/exchange-rate-download-logs/summary
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `startDate` | Query | `DateTime?` |
| `endDate` | Query | `DateTime?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `ExchangeRateDownloadSummaryDto` |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
