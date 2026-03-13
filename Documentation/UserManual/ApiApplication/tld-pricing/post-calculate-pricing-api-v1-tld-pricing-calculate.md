# POST CalculatePricing

Calculates final pricing for a TLD including discounts and promotions

## Endpoint

```
POST /api/v1/tld-pricing/calculate
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | `[CalculatePricingRequest](../dtos/calculate-pricing-request.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[CalculatePricingResponse](../dtos/calculate-pricing-response.md)` |

[Back to API Manual index](../index.md)



