# POST CreateRate

Retrieves all exchange rates for a specific currency pair

## Endpoint

```
POST /api/v1/currencies/rates
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | [CreateCurrencyExchangeRateDto](../dtos/create-currency-exchange-rate-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [CurrencyExchangeRateDto](../dtos/currency-exchange-rate-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




