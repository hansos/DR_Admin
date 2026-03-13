# PUT UpdateRate

Updates an existing currency exchange rate

## Endpoint

```
PUT /api/v1/currencies/rates/{id:int}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateCurrencyExchangeRateDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `CurrencyExchangeRateDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
