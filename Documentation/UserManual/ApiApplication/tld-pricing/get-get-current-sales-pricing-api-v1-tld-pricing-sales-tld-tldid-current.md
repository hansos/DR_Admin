# GET GetCurrentSalesPricing

Manages TLD sales pricing (temporal pricing for customer-facing prices)

## Endpoint

```
GET /api/v1/tld-pricing/sales/tld/{tldId}/current
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `tldId` | Route | `int` |
| `effectiveDate` | Query | `DateTime?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[TldSalesPricingDto](../dtos/tld-sales-pricing-dto.md)` |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)



