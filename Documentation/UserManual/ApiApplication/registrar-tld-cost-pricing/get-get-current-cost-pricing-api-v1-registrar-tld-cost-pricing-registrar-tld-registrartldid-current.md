# GET GetCurrentCostPricing

Manages registrar TLD cost pricing (temporal pricing for registrar costs)

## Endpoint

```
GET /api/v1/registrar-tld-cost-pricing/registrar-tld/{registrarTldId}/current
```

## Authorization

Requires authentication. Policy: **Pricing.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarTldId` | Route | `int` |
| `effectiveDate` | Query | `DateTime?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[RegistrarTldCostPricingDto](../dtos/registrar-tld-cost-pricing-dto.md)` |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



