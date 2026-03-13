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
| `request` | Body | `CalculatePricingRequest` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `CalculatePricingResponse` |

[Back to API Manual index](../index.md)
