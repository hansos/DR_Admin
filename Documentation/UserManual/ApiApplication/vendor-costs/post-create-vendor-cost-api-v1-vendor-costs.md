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
| `createDto` | Body | [CreateVendorCostDto](../dtos/create-vendor-cost-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [VendorCostDto](../dtos/vendor-cost-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




