# GET GetLogById

Controller for managing exchange rate download logs

## Endpoint

```
GET /api/exchange-rate-download-logs/{id}
```

## Authorization

Requires authentication. Policy: **ExchangeRateDownloadLog.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [ExchangeRateDownloadLogDto](../dtos/exchange-rate-download-log-dto.md) |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




