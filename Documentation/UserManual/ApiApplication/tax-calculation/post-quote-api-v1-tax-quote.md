# POST Quote

Provides quote and finalize tax endpoints for checkout and invoicing.

## Endpoint

```
POST /api/v1/tax/quote
```

## Authorization

Requires authentication. Policy: **TaxCalculation.Quote**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | [TaxQuoteRequestDto](../dtos/tax-quote-request-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [TaxQuoteResultDto](../dtos/tax-quote-result-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)




