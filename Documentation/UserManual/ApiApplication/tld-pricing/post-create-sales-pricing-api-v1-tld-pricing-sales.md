# POST CreateSalesPricing

Creates new sales pricing for a TLD

## Endpoint

```
POST /api/v1/tld-pricing/sales
```

## Authorization

Requires authentication. Policy: **Pricing.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | [CreateTldSalesPricingDto](../dtos/create-tld-sales-pricing-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [TldSalesPricingDto](../dtos/tld-sales-pricing-dto.md) |

[Back to API Manual index](../index.md)




