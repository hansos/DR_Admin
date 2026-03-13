# POST ConvertCurrency

Converts an amount from one currency to another

## Endpoint

```
POST /api/v1/currencies/convert
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `convertDto` | Body | [ConvertCurrencyDto](../dtos/convert-currency-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [CurrencyConversionResultDto](../dtos/currency-conversion-result-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




