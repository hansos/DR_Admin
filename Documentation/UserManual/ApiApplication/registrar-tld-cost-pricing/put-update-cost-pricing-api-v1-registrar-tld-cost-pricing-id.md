# PUT UpdateCostPricing

Updates existing cost pricing (only future pricing if configured)

## Endpoint

```
PUT /api/v1/registrar-tld-cost-pricing/{id}
```

## Authorization

Requires authentication. Policy: **Pricing.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `[UpdateRegistrarTldCostPricingDto](../dtos/update-registrar-tld-cost-pricing-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[RegistrarTldCostPricingDto](../dtos/registrar-tld-cost-pricing-dto.md)` |
| 400 | Bad Request | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



