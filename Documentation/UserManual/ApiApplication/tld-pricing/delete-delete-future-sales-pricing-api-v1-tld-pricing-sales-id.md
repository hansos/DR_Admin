# DELETE DeleteFutureSalesPricing

Deletes future sales pricing

## Endpoint

```
DELETE /api/v1/tld-pricing/sales/{id}
```

## Authorization

Requires authentication. Policy: **Pricing.Delete**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)
