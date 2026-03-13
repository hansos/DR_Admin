# POST CreateVendorCost

Creates a new vendor cost

## Endpoint

```
POST /api/v1/vendor-costs
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateVendorCostDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `VendorCostDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
