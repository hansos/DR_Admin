# POST CreateLog

Gets failed exchange rate download logs

## Endpoint

```
POST /api/exchange-rate-download-logs
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | [CreateExchangeRateDownloadLogDto](../dtos/create-exchange-rate-download-log-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [ExchangeRateDownloadLogDto](../dtos/exchange-rate-download-log-dto.md) |
| 400 | Bad Request | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




