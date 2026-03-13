# POST CreateCostPricing

Retrieves future scheduled cost pricing for a registrar-TLD

## Endpoint

```
POST /api/v1/registrar-tld-cost-pricing
```

## Authorization

Requires authentication. Policy: **Pricing.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateRegistrarTldCostPricingDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `RegistrarTldCostPricingDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
