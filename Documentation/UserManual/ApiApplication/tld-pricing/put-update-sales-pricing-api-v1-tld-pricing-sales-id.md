# PUT UpdateSalesPricing

Updates existing sales pricing (only future pricing if configured)

## Endpoint

```
PUT /api/v1/tld-pricing/sales/{id}
```

## Authorization

Requires authentication. Policy: **Pricing.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateTldSalesPricingDto](../dtos/update-tld-sales-pricing-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [TldSalesPricingDto](../dtos/tld-sales-pricing-dto.md) |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)




