# GET GetRateById

Manages currency exchange rates and currency conversions

## Endpoint

```
GET /api/v1/currencies/rates/{id:int}
```

## Authorization

Requires authentication. Policy: **Currency.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [CurrencyExchangeRateDto](../dtos/currency-exchange-rate-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




