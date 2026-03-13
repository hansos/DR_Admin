# GET GetExchangeRate

Retrieves the current exchange rate between two currencies

## Endpoint

```
GET /api/v1/currencies/rates/exchange
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `from` | Query | `string` |
| `to` | Query | `string` |
| `date` | Query | `DateTime?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[CurrencyExchangeRateDto](../dtos/currency-exchange-rate-dto.md)` |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



