# PUT UpdateVendorCost

Updates an existing vendor cost

## Endpoint

```
PUT /api/v1/vendor-costs/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateVendorCostDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `VendorCostDto` |
| 400 | Bad Request | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
