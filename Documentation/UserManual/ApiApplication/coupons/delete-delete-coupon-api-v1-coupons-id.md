# DELETE DeleteCoupon

Deletes a coupon (soft delete)

## Endpoint

```
DELETE /api/v1/coupons/{id}
```

## Authorization

Requires authentication. Policy: **Coupon.Delete**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
